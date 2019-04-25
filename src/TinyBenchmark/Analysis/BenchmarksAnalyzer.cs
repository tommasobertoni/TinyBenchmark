using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using TinyBenchmark.Attributes;

namespace TinyBenchmark.Analysis
{
    internal class BenchmarksAnalyzer
    {
        public List<BenchmarkReference> FindAllbenchmarks<TBenchmarksContainer>(TBenchmarksContainer container) =>
            this.FindAllbenchmarks(container.GetType());

        public List<BenchmarkReference> FindAllbenchmarks<TBenchmarksContainer>() =>
            this.FindAllbenchmarks(typeof(TBenchmarksContainer));

        public List<BenchmarkReference> FindAllbenchmarks(Type benchmarksContainerType)
        {
            var benchmarkMethods = benchmarksContainerType
                .GetMethods(BindingFlags.Instance | BindingFlags.Public)
                .Select(m => (method: m, attribute: m.GetCustomAttribute<BenchmarkAttribute>()))
                .Where(x => x.attribute != null)
                .ToList();

            var warmupMethods = benchmarksContainerType
                .GetMethods(BindingFlags.Instance | BindingFlags.Public)
                .Select(m => (method: m, attribute: m.GetCustomAttribute<WarmupAttribute>()))
                .Where(x => x.attribute != null)
                .ToList();

            var references = benchmarkMethods.Select(CreateReference).ToList();
            return references;

            // Local functions

            BenchmarkReference CreateReference((MethodInfo executable, BenchmarkAttribute attribute) discoveredBenchmark)
            {
                var (executable, attribute) = discoveredBenchmark;
                var benchmarkName = attribute.Name ?? executable.Name;

                var warmups = new HashSet<(MethodInfo warmup, int order)>();
                var warmupsInContainerTargetingTheBenchmark = warmupMethods.Where(x => x.attribute.ForBenchmark == benchmarkName).ToList();
                warmupsInContainerTargetingTheBenchmark.ForEach(x => warmups.Add((x.method, x.attribute.Order)));

                var directlyTargetedWarmupAttributes = executable.GetCustomAttributes<WarmupWithAttribute>();
                if (directlyTargetedWarmupAttributes.Any())
                {
                    foreach (var attr in directlyTargetedWarmupAttributes)
                    {
                        var warmupMethod = benchmarksContainerType.GetMethod(attr.MethodName,
                            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);

                        warmups.Add((warmupMethod, attr.Order));
                    }
                }

                var orderedWarmups = warmups.OrderBy(x => x.order).Select(x => x.warmup).ToList();

                return new BenchmarkReference
                {
                    Executable = executable,
                    Name = benchmarkName,
                    Iterations = Math.Max(1, attribute.Iterations),
                    Warmups = orderedWarmups,
                };
            }
        }
    }
}
