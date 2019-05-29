using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using TinyBenchmark.Attributes;

namespace TinyBenchmark.Analysis
{
    internal class WarmupReferenceConstructor
    {
        public bool UseConventions { get; }

        private readonly TypeInfoExtractor _tExtractor;
        private readonly BenchmarkOutput _output;

        internal WarmupReferenceConstructor(BenchmarkOutput output, bool useConventions = true)
        {
            _tExtractor = new TypeInfoExtractor();
            _output = output;
            this.UseConventions = useConventions;
        }

        internal IReadOnlyList<WarmupReference> TryCreateWarmupReferences(MethodInfo method)
        {
            var warmupReferences = new List<WarmupReference>();

            var warmupAttributes = method.GetCustomAttributes<WarmupWithAttribute>();

            if (warmupAttributes?.Any() == true)
            {
                var containerType = method.DeclaringType;

                var warmupReferencesFromAttributes = warmupAttributes
                    .Select(a => TryCreateWarmupReference(containerType, a))
                    .Where(wref => wref != null);

                warmupReferences.AddRange(warmupReferencesFromAttributes);
            }

            if (this.UseConventions)
            {
                /*
                 * Conventions:
                 * - {method name}Warmup
                 * - {method name}_Warmup
                 * - WarmupFor{method name}
                 * - WarmupFor_{method name}
                 */

                var methodName = method.Name;
                var methodWarmup = $"{methodName}Warmup";
                var method_Warmup = $"{methodName}_Warmup";
                var warmupForMethod = $"WarmupFor{methodName}";
                var warmupFor_Method = $"WarmupFor_{methodName}";

                var conventions = new[] { methodWarmup, method_Warmup, warmupForMethod, warmupFor_Method };

                var containerType = method.DeclaringType;
                var containerMethods = _tExtractor.GetMethods(containerType);

                var conventionBasedWarmupMethods = containerMethods
                    .Where(m => conventions.Contains(m.Name, StringComparer.InvariantCultureIgnoreCase))
                    .ToList();

                foreach (var conventionWarmupMethod in conventionBasedWarmupMethods)
                {
                    if (warmupReferences.Any(existingWRef => existingWRef.Method == conventionWarmupMethod))
                        // Warmup method already found using the attribute
                        continue;

                    var conventionWRef = CreateWarmupReference(conventionWarmupMethod);
                    warmupReferences.Add(conventionWRef);
                }
            }

            return warmupReferences.Any() ? warmupReferences.AsReadOnly() : null; // Return null when empty.
        }

        internal WarmupReference TryCreateWarmupReference(Type benchmarksContainerType, WarmupWithAttribute attr)
        {
            var warmupMethod = _tExtractor.GetMethods(benchmarksContainerType).FirstOrDefault(m => m.Name == attr.MethodName);

            if (warmupMethod == null)
            {
                _output.WriteLine(OutputLevel.Normal, $"Couldn't find warmup method {benchmarksContainerType.FullName}.{warmupMethod?.Name}");
                return null;
            }
            else
            {
                return CreateWarmupReference(warmupMethod, attr.Order);
            }
        }

        internal WarmupReference CreateWarmupReference(MethodInfo warmupMethod, int order = 0)
        {
            var containerType = warmupMethod?.DeclaringType;

            if (warmupMethod.GetParameters().Any())
            {
                throw new InvalidOperationException(
                    $"Warmup method {containerType.FullName}.{warmupMethod.Name} cannot have parameters.");
            }
            else
            {
                return new WarmupReference(warmupMethod.Name, order, warmupMethod);
            }
        }
    }
}
