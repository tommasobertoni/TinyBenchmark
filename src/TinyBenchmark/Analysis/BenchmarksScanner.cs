using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using TinyBenchmark.Attributes;

namespace TinyBenchmark.Analysis
{
    /// <summary>
    /// Scans a type and extracts benchmarks information.
    /// </summary>
    internal class BenchmarksScanner
    {
        private static readonly BindingFlags _Flags = BindingFlags.Public | BindingFlags.Instance;

        private static readonly ConcurrentDictionary<Type, MethodInfo[]> _methodsCache =
            new ConcurrentDictionary<Type, MethodInfo[]>();

        private static readonly ConcurrentDictionary<Type, PropertyInfo[]> _propertiesCache =
            new ConcurrentDictionary<Type, PropertyInfo[]>();

        /// <summary>
        /// true when the scanner is allowed to create references using conventions, even when
        /// there are no attributes; otherwise, false.
        /// </summary>
        public bool UseConventions { get; }

        /// <summary>
        /// The benchmarks container reference.
        /// </summary>
        public ContainerReference Container { get; private set; }

        /// <summary>
        /// The benchmarks init reference, if available.
        /// </summary>
        public InitReference Init { get; private set; }

        /// <summary>
        /// The benchmark references, if any.
        /// </summary>
        public IReadOnlyList<BenchmarkReference> Benchmarks { get; private set; }

        private readonly BenchmarkOutput _output;

        public BenchmarksScanner(BenchmarkOutput output, bool useConventions = true)
        {
            _output = output;
            UseConventions = useConventions;
        }

        public void Scan<TBenchmarksContainer>() => this.Scan(typeof(TBenchmarksContainer));

        public void Scan(Type benchmarksContainerType)
        {
            _output.WriteLine(OutputLevel.Minimal, $"Scanning type {benchmarksContainerType.FullName}");
            _output.IndentLevel++;

            // Parameters

            var parametersSetCollection = GetParametersSetCollection(benchmarksContainerType);

            // Container

            this.Container = GetContainerReference(benchmarksContainerType, parametersSetCollection);
            _output.WriteLine(OutputLevel.Normal, $"Found container {this.Container.Name}");

            if (this.Container.InitContainer != null)
                _output.WriteLine(OutputLevel.Verbose, $"Found init container named {this.Container.InitContainer.Method.Name}");

            // Init

            this.Init = TryGetInitReference(benchmarksContainerType);

            if (this.Init != null)
                _output.WriteLine(OutputLevel.Verbose, $"Found init named {this.Init.Method.Name}");

            // Benchmarks

            this.Benchmarks = GetBenchmarkReferences(benchmarksContainerType);

            if (_output.IsShown(OutputLevel.Normal))
            {
                var baselineBenchmark = this.Benchmarks.FirstOrDefault(br => br.IsBaseline);
                if (baselineBenchmark == null)
                    _output.WriteLine(OutputLevel.Normal, $"Found {this.Benchmarks.Count} benchmarks");
                else
                    _output.WriteLine(OutputLevel.Normal, $"Found {this.Benchmarks.Count} benchmarks, baseline: {baselineBenchmark.Name}");
            }

            _output.IndentLevel--;
        }

        #region Parameters

        private ParametersSetCollection GetParametersSetCollection(Type benchmarksContainerType)
        {
            var parametersSetCollection = new ParametersSetCollection();

            var parameterProperties = GetPropertiesWithAttribute<ParamAttribute>(benchmarksContainerType);

            foreach (var (property, attribute) in parameterProperties)
            {
                // Verify property is writable.
                if (!property.CanWrite)
                    throw new InvalidOperationException($"Property {property.DeclaringType.Name}.{property.Name} must be writable for the {nameof(ParamAttribute)} to work.");

                // Verify parameters type.
                foreach (var paramValue in attribute.Values)
                {
                    if (paramValue == null && property.PropertyType.IsValueType)
                        throw new InvalidOperationException($"Cannot assign null to the property {property.DeclaringType.Name}.{property.Name} of type {property.PropertyType.Name}.");

                    if (!property.PropertyType.IsAssignableFrom(paramValue.GetType()))
                        throw new InvalidOperationException($"Cannot associate a value of type {paramValue.GetType().Name} to the property {property.DeclaringType.Name}.{property.Name} of type {property.PropertyType.Name}.");
                }

                var propertyParametersCollection = new PropertyWithParametersCollection(property, attribute);
                parametersSetCollection.Add(propertyParametersCollection);
            }

            return parametersSetCollection;
        }

        #endregion

        #region Container

        private ContainerReference GetContainerReference(Type benchmarksContainerType, ParametersSetCollection parametersSetCollection)
        {
            var initContainerMethod = TryGetInitContainerMethod(benchmarksContainerType);
            var initContainerReference = initContainerMethod == null ? null : new InitReference(initContainerMethod);

            var benchmarksContainerAttribute = benchmarksContainerType
                .GetCustomAttributes(typeof(BenchmarksContainerAttribute), false)
                .FirstOrDefault() as BenchmarksContainerAttribute;

            var name = benchmarksContainerAttribute?.Name ?? benchmarksContainerType.Name;
            return new ContainerReference(benchmarksContainerType, name, initContainerReference, parametersSetCollection);
        }

        private MethodInfo TryGetInitContainerMethod(Type benchmarksContainerType)
        {
            var containerName = benchmarksContainerType.Name;
            var initsContainer = GetMethodsWithAttribute<InitContainerAttribute>(benchmarksContainerType);

            if (initsContainer.Count > 1)
                throw new InvalidOperationException($"Multiple {nameof(InitContainerAttribute)}s are not allowed in the same container. Container: {containerName}");

            var initContainerMethodInfo = initsContainer.FirstOrDefault().method;

            if (initContainerMethodInfo == null && this.UseConventions)
            {
                /*
                 * Conventions:
                 * - InitContainer
                 * - Init{class name}
                 */

                var defaultConventionName = "InitContainer";
                var namedConventionName = $"Init{benchmarksContainerType.Name}";

                var containerMethods = GetMethods(benchmarksContainerType);

                var conventionBasedInitContainerMethods = containerMethods
                    .Where(m =>
                        m.Name.Equals(defaultConventionName, StringComparison.InvariantCultureIgnoreCase) ||
                        m.Name.Equals(namedConventionName, StringComparison.InvariantCultureIgnoreCase))
                    .ToList();

                if (conventionBasedInitContainerMethods.Count > 1)
                    throw new InvalidOperationException($"Multiple container initialization methods found using conventions. Container: {containerName}");

                initContainerMethodInfo = conventionBasedInitContainerMethods.FirstOrDefault();
            }

            if (initContainerMethodInfo?.GetParameters()?.Any() == true)
                throw new InvalidOperationException($"A container initialization method cannot have parameters. Container: {containerName}");

            return initContainerMethodInfo; // Can be null
        }

        #endregion

        #region Init

        private InitReference TryGetInitReference(Type benchmarksContainerType)
        {
            var inits = GetMethodsWithAttribute<InitAttribute>(benchmarksContainerType);

            if (inits.Count > 1)
                throw new InvalidOperationException($"Found multiple methods decorated with {nameof(InitAttribute)}: only one per container is allowed.");

            var initMethod = inits.FirstOrDefault().method;

            if (initMethod != null && initMethod.GetParameters().Any())
            {
                throw new InvalidOperationException($"Init method {benchmarksContainerType.FullName}.{initMethod.Name} cannot have parameters.");
            }

            return initMethod == null ? null : new InitReference(initMethod);
        }

        #endregion

        #region Benchmarks

        private List<BenchmarkReference> GetBenchmarkReferences(Type benchmarksContainerType)
        {
            var benchmarks = GetMethodsWithAttribute<BenchmarkAttribute>(benchmarksContainerType);

            var benchmarkReferences = benchmarks.Select(CreateBenchmarkReference).ToList();

            return benchmarkReferences;

            // Local functions

            BenchmarkReference CreateBenchmarkReference((MethodInfo method, BenchmarkAttribute attribute) benchmark)
            {
                var (method, attribute) = benchmark;

                var initWithReference = TryGetInitWithReference(method);

                var warmupCollection = CreateWarmupCollection(method);
                var orderedWarmupCollection = warmupCollection.OrderBy(w => w.Order);

                var argumentsCollection = CreateArgumentsCollection(method);

                var iterations = attribute.Iterations > 0
                    ? attribute.Iterations
                    : throw new InvalidOperationException($"{nameof(attribute.Iterations)} must be a positive number.");

                var reference = new BenchmarkReference(
                    attribute.Name ?? method.Name,
                    initWithReference,
                    orderedWarmupCollection,
                    argumentsCollection,
                    method,
                    iterations,
                    attribute.Baseline);

                return reference;
            }
        }

        private InitReference TryGetInitWithReference(MethodInfo method)
        {
            var initWithAttributes = method.GetCustomAttributes<InitWithAttribute>()?.ToList();

            if (initWithAttributes?.Count > 1)
                throw new InvalidOperationException($"Multiple {nameof(InitContainerAttribute)} not allowed in the same container.");

            var attr = initWithAttributes.FirstOrDefault();
            if (attr == null) return null;

            var containerType = method.DeclaringType;

            var initWithMethod = GetMethods(containerType).FirstOrDefault(m => m.Name == attr.MethodName);

            if (initWithMethod == null)
            {
                _output.WriteLine(OutputLevel.Normal, $"Couldn't find init method {containerType.FullName}.{attr.MethodName}");
            }
            else if (initWithMethod.GetParameters().Any())
            {
                throw new InvalidOperationException($"Init method {containerType.FullName}.{initWithMethod.Name} cannot have parameters.");
            }

            return new InitReference(initWithMethod);
        }

        private IEnumerable<ArgumentsReference> CreateArgumentsCollection(MethodInfo method)
        {
            var argumentsAttributes = method.GetCustomAttributes<ArgumentsAttribute>();

            if (argumentsAttributes != null)
            {
                var expectedMethodArguments = method.GetParameters().ToList();

                foreach (var argumentsAttribute in argumentsAttributes)
                {
                    var args = argumentsAttribute.Arguments;

                    // Verify that arguments are allowed.
                    if (expectedMethodArguments?.Any() != true && args.Any())
                        throw ArgumentsNotAccepted(args);

                    // Verify arguments count.
                    if (expectedMethodArguments.Count != args.Count)
                        throw ArgumentsCountDoNotMatch(args, expected: expectedMethodArguments.Count, actual: args.Count);

                    var argumentsReference = new ArgumentsReference();

                    // Verify argument type.
                    for (int i = 0; i < expectedMethodArguments.Count; i++)
                    {
                        var arg = args[i];
                        var methodArgument = expectedMethodArguments[i];

                        if (arg == null)
                            throw ArgumentNullCannotBeAssignedToValueType(args, methodArgument);

                        if (!methodArgument.ParameterType.IsAssignableFrom(arg.GetType()))
                            throw ArgumentTypesDoNotMatch(args, expected: methodArgument.GetType(), actual: arg.GetType());

                        argumentsReference.Add(methodArgument.Name, arg);
                    }

                    yield return argumentsReference;
                }
            }

            // Local functions

            string ToString(IEnumerable<object> args) => string.Join(", ", args.Select(x => x.ToString()));

            string GetExceptionPrefix(IEnumerable<object> args) =>
                $"Mismatching arguments on method {method.DeclaringType.Name}.{method.Name}, arguments: {ToString(args)}";

            InvalidOperationException ArgumentsNotAccepted(IEnumerable<object> args) => new InvalidOperationException(
                $"{GetExceptionPrefix(args)}: the method doesn't accept arguments.");

            InvalidOperationException ArgumentsCountDoNotMatch(IEnumerable<object> args, int expected, int actual) => new InvalidOperationException(
                $"{GetExceptionPrefix(args)}: expected {expected} arguments but {actual} arguments were provided.");

            InvalidOperationException ArgumentNullCannotBeAssignedToValueType(IEnumerable<object> args, ParameterInfo argumentInfo) => new InvalidOperationException(
                $"{GetExceptionPrefix(args)}: cannot assign null to the argument {argumentInfo.Name} of type {argumentInfo.ParameterType.Name}.");

            InvalidOperationException ArgumentTypesDoNotMatch(IEnumerable<object> args, Type expected, Type actual) => new InvalidOperationException(
                $"{GetExceptionPrefix(args)}: expected {expected.Name} but {actual.Name} was provided.");
        }

        private IEnumerable<WarmupReference> CreateWarmupCollection(MethodInfo method)
        {
            var warmupAttributes = method.GetCustomAttributes<WarmupWithAttribute>();

            if (warmupAttributes.Any())
            {
                var containerType = method.DeclaringType;

                foreach (var attr in warmupAttributes)
                {
                    var warmupMethod = GetMethods(containerType).FirstOrDefault(m => m.Name == attr.MethodName);

                    if (warmupMethod == null)
                    {
                        _output.WriteLine(OutputLevel.Normal, $"Couldn't find warmup method {containerType.FullName}.{attr.MethodName}");
                    }
                    else if (warmupMethod.GetParameters().Any())
                    {
                        throw new InvalidOperationException($"Warmup method {containerType.FullName}.{warmupMethod.Name} cannot have parameters.");
                    }
                    else
                    {
                        yield return new WarmupReference(attr.MethodName, attr.Order, warmupMethod);
                    }
                }
            }
        }

        #endregion

        #region Helpers

        private IReadOnlyList<MethodInfo> GetMethods(Type type)
        {
            if (_methodsCache.ContainsKey(type))
                return _methodsCache[type];

            var methods = type.GetMethods(_Flags);
            _methodsCache[type] = methods;
            return methods;
        }

        private IReadOnlyList<(MethodInfo method, TAttribute attribute)> GetMethodsWithAttribute<TAttribute>(Type type)
            where TAttribute : Attribute
        {
            var methods = GetMethods(type);
            return methods
                .Select(m => (method: m, attribute: m.GetCustomAttribute<TAttribute>()))
                .Where(x => x.attribute != null)
                .ToList();
        }

        private IReadOnlyList<PropertyInfo> GetProperties(Type type)
        {
            if (_propertiesCache.ContainsKey(type))
                return _propertiesCache[type];

            var properties = type.GetProperties(_Flags);
            _propertiesCache[type] = properties;
            return properties;
        }

        private IReadOnlyList<(PropertyInfo property, TAttribute attribute)> GetPropertiesWithAttribute<TAttribute>(Type type)
            where TAttribute : Attribute
        {
            var properties = GetProperties(type);
            return properties
                .Select(p => (property: p, attribute: p.GetCustomAttribute<TAttribute>()))
                .Where(x => x.attribute != null)
                .ToList();
        }

        #endregion
    }
}
