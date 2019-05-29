using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TinyBenchmark.Attributes;

namespace TinyBenchmark.Analysis
{
    internal class InitReferenceConstructor
    {
        public bool UseConventions { get; }

        private readonly TypeInfoExtractor _tExtractor;

        internal InitReferenceConstructor(bool useConventions = true)
        {
            _tExtractor = new TypeInfoExtractor();
            this.UseConventions = useConventions;
        }

        internal InitReference TryCreateInitReference(Type benchmarksContainerType)
        {
            var inits = _tExtractor.GetMethodsWithAttribute<InitAttribute>(benchmarksContainerType);

            if (inits.Count > 1)
                throw new InvalidOperationException(
                    // Only one initialization method is allowed for a container
                    $"Found multiple methods decorated with {nameof(InitAttribute)}: only one per container is allowed.");

            var initMethod = inits.FirstOrDefault().method;

            if (initMethod == null && this.UseConventions)
            {
                /*
                 * Conventions:
                 * - Init
                 */

                var defaultConventionName = "Init";

                var containerMethods = _tExtractor.GetMethods(benchmarksContainerType);

                var conventionBasedInitMethods = containerMethods
                    .Where(m => m.Name.Equals(defaultConventionName, StringComparison.InvariantCultureIgnoreCase))
                    .ToList();

                if (conventionBasedInitMethods.Count > 1)
                    // Only one initialization method is allowed for a container
                    throw new InvalidOperationException(
                        $"Multiple initialization methods found using conventions. " +
                        $"Container: {benchmarksContainerType.Name}");

                initMethod = conventionBasedInitMethods.FirstOrDefault();
            }

            if (initMethod?.GetParameters()?.Any() == true)
                // Initialization method must be parameterless
                throw new InvalidOperationException(
                    $"Init method {benchmarksContainerType.FullName}.{initMethod.Name} cannot have parameters.");

            return initMethod == null ? null : new InitReference(initMethod);
        }
    }
}
