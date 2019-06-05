using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using TinyBenchmark.Attributes;

namespace TinyBenchmark.Analysis
{
    internal class BenchmarkReferenceConstructor
    {
        public bool UseConventions { get; }

        private readonly TypeInfoExtractor _tExtractor;
        private readonly BenchmarkOutput _output;

        internal BenchmarkReferenceConstructor(BenchmarkOutput output, bool useConventions = true)
        {
            _tExtractor = new TypeInfoExtractor();
            _output = output;
            this.UseConventions = useConventions;
        }

        internal IReadOnlyList<BenchmarkReference> TryCreateBenchmarkReferences(Type benchmarksContainerType)
        {
            var benchmarks = _tExtractor.GetMethodsWithAttribute<BenchmarkAttribute>(benchmarksContainerType);
            var benchmarkReferences = benchmarks?.Select(x => CreateBenchmarkReference(x.method, x.attribute));
            return benchmarkReferences?.OrderBy(b => b.Order)?.ToList()?.AsReadOnly();
        }

        internal BenchmarkReference CreateBenchmarkReference(MethodInfo method, BenchmarkAttribute attribute)
        {
            var initWithReference = TryCreateInitWithReference(method);

            var warmupRefCtor = new WarmupReferenceConstructor(_output, this.UseConventions);
            var warmupCollection = warmupRefCtor.TryCreateWarmupReferences(method);
            var orderedWarmupCollection = warmupCollection?.OrderBy(w => w.Order);

            var argumentsRefCtor = new ArgumentsReferenceConstructor();
            var argumentsCollection = argumentsRefCtor.TryCreateArgumentsReferences(method);

            var iterations = attribute.Iterations > 0
                ? attribute.Iterations
                : throw new InvalidOperationException($"{nameof(attribute.Iterations)} must be a positive number.");

            var reference = new BenchmarkReference(
                attribute.Name ?? method.Name,
                attribute.Order,
                initWithReference,
                orderedWarmupCollection,
                argumentsCollection,
                method,
                iterations,
                attribute.Baseline);

            return reference;
        }

        internal InitReference TryCreateInitWithReference(MethodInfo method)
        {
            var initWithAttributes = method.GetCustomAttributes<InitWithAttribute>()?.ToList();

            if (initWithAttributes?.Count > 1)
                throw new InvalidOperationException($"Multiple {nameof(InitWithAttribute)} are not allowed in the same container.");

            var attr = initWithAttributes.FirstOrDefault();

            var containerType = method.DeclaringType;
            var initWithMethod = null as MethodInfo;

            if (attr != null)
            {
                initWithMethod = _tExtractor.GetMethods(containerType).FirstOrDefault(m => m.Name == attr.MethodName);

                if (initWithMethod == null)
                {
                    _output.WriteLine(OutputLevel.Normal, $"Couldn't find init method {containerType.FullName}.{attr.MethodName}");
                }
                else if (initWithMethod.GetParameters().Any())
                {
                    throw new InvalidOperationException(
                        $"Init method {containerType.FullName}.{initWithMethod.Name} cannot have parameters.");
                }
            }

            if (initWithMethod == null && this.UseConventions)
            {
                /*
                 * Conventions:
                 * - {method name}Init
                 * - {method name}_Init
                 * - Init{method name}
                 * - Init_{method name}
                 */

                var methodName = method.Name;
                var methodInit = $"{methodName}Init";
                var method_Init = $"{methodName}_Init";
                var initMethod = $"Init{methodName}";
                var init_Method = $"Init_{methodName}";

                var conventions = new[] { methodInit, method_Init, initMethod, init_Method };

                var containerMethods = _tExtractor.GetMethods(containerType);

                var conventionBasedInitMethods = containerMethods
                    .Where(m => conventions.Contains(m.Name, StringComparer.InvariantCultureIgnoreCase))
                    .ToList();

                if (conventionBasedInitMethods.Count > 1)
                    // Only one initialization method is allowed for a container
                    throw new InvalidOperationException(
                        $"Multiple container initialization methods found using conventions. " +
                        $"Container: {containerType.Name}");

                initWithMethod = conventionBasedInitMethods.FirstOrDefault();
            }

            return initWithMethod == null ? null : new InitReference(initWithMethod);
        }
    }
}
