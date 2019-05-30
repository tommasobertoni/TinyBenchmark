using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using TinyBenchmark.Attributes;

namespace TinyBenchmark.Analysis
{
    internal class ContainerReferenceConstructor
    {
        public bool UseConventions { get; }

        private readonly TypeInfoExtractor _tExtractor;

        internal ContainerReferenceConstructor(bool useConventions = true)
        {
            _tExtractor = new TypeInfoExtractor();
            this.UseConventions = useConventions;
        }

        internal ContainerReference CreateContainerReference(Type benchmarksContainerType)
        {
            var containerName = GetContainerName(benchmarksContainerType);
            var initContainerReference = TryCreateInitContainerReference(benchmarksContainerType);
            var parametersSetCollection = GetParametersSetCollection(benchmarksContainerType);

            return new ContainerReference(
                benchmarksContainerType,
                containerName,
                initContainerReference,
                parametersSetCollection);
        }

        internal InitReference TryCreateInitContainerReference(Type benchmarksContainerType)
        {
            var initsContainer = _tExtractor.GetMethodsWithAttribute<InitContainerAttribute>(benchmarksContainerType);

            if (initsContainer.Count > 1)
                // Only one initialization method is allowed for a container
                throw new InvalidOperationException(
                    $"Multiple {nameof(InitContainerAttribute)}s are not allowed in the same container. " +
                    $"Container: {benchmarksContainerType.Name}");

            var initContainerMethod = initsContainer.FirstOrDefault().method;

            if (initContainerMethod == null && this.UseConventions)
            {
                /*
                 * Conventions:
                 * - InitContainer
                 * - Init{class name}
                 */

                var defaultConventionName = "InitContainer";
                var namedConventionName = $"Init{benchmarksContainerType.Name}";

                var conventions = new[] { defaultConventionName, namedConventionName };

                var containerMethods = _tExtractor.GetMethods(benchmarksContainerType);

                var conventionBasedInitContainerMethods = containerMethods
                    .Where(m => conventions.Contains(m.Name, StringComparer.InvariantCultureIgnoreCase))
                    .ToList();

                if (conventionBasedInitContainerMethods.Count > 1)
                    // Only one initialization method is allowed for a container
                    throw new InvalidOperationException(
                        $"Multiple container initialization methods found using conventions. " +
                        $"Container: {benchmarksContainerType.Name}");

                initContainerMethod = conventionBasedInitContainerMethods.FirstOrDefault();
            }

            if (initContainerMethod?.GetParameters()?.Any() == true)
                // Initialization method must be parameterless
                throw new InvalidOperationException(
                    $"A container initialization method cannot have parameters." +
                    $"Container: {benchmarksContainerType.Name}");

            return initContainerMethod == null ? null : new InitReference(initContainerMethod);
        }

        internal string GetContainerName(Type benchmarksContainerType)
        {
            var benchmarksContainerAttribute = benchmarksContainerType
                .GetCustomAttributes(typeof(BenchmarksContainerAttribute), false)
                .FirstOrDefault() as BenchmarksContainerAttribute;

            var name = benchmarksContainerAttribute?.Name ?? benchmarksContainerType.Name;
            return name;
        }

        internal ParametersSetCollection GetParametersSetCollection(Type benchmarksContainerType)
        {
            var parametersSetCollection = new ParametersSetCollection();

            var parameterProperties = _tExtractor.GetPropertiesWithAttribute<ParamAttribute>(benchmarksContainerType);

            foreach (var (property, attribute) in parameterProperties)
            {
                // Verify property is writable.
                if (!property.CanWrite)
                    throw new InvalidOperationException(
                        $"Property {property.DeclaringType.Name}.{property.Name} must be writable " +
                        $"for the {nameof(ParamAttribute)} to work.");

                // Verify parameters type.
                foreach (var paramValue in attribute.Values)
                {
                    if (paramValue == null)
                    {
                        if (property.PropertyType.IsValueType)
                            throw new InvalidOperationException(
                                $"Cannot assign null to the property {property.DeclaringType.Name}.{property.Name} " +
                                $"of type {property.PropertyType.Name}.");
                    }
                    else if (!property.PropertyType.IsAssignableFrom(paramValue.GetType()))
                        throw new InvalidOperationException(
                            $"Cannot associate a value of type {paramValue.GetType().Name} to the property " +
                            $"{property.DeclaringType.Name}.{property.Name} of type {property.PropertyType.Name}.");
                }

                var propertyParametersCollection = new PropertyWithParametersCollection(property, attribute);
                parametersSetCollection.Add(propertyParametersCollection);
            }

            return parametersSetCollection;
        }
    }
}
