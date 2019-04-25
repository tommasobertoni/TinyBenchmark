using System;
using System.Collections;
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

            var initMethods = benchmarksContainerType
                .GetMethods(BindingFlags.Instance | BindingFlags.Public)
                .Select(m => (method: m, attribute: m.GetCustomAttribute<InitAttribute>()))
                .Where(x => x.attribute != null)
                .ToList();

            var references = benchmarkMethods.OrderBy(x => x.attribute.Order).Select(CreateReference).ToList();
            return references;

            // Local functions

            BenchmarkReference CreateReference((MethodInfo executable, BenchmarkAttribute attribute) discoveredBenchmark)
            {
                var (executable, attribute) = discoveredBenchmark;
                var benchmarkName = attribute.Name ?? executable.Name;

                if (initMethods.Count > 1)
                    throw new InvalidOperationException($"Found multiple methods decorated with {nameof(InitAttribute)}: only one per container is allowed.");

                var init = initMethods.FirstOrDefault().method;

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

                var parametersSetCollection = new ParametersSetCollection(benchmarksContainerType);

                return new BenchmarkReference
                {
                    Init = init,
                    Executable = executable,
                    Name = benchmarkName,
                    Iterations = Math.Max(1, attribute.Iterations),
                    Warmups = orderedWarmups,
                    ParametersSetCollection = parametersSetCollection,
                };
            }
        }

        //private List<ParametersSet> CreateParametersSets(List<(PropertyInfo property, ParamAttribute attribute)> discovederParameterProperties)
        //{
        //    var parametersSets = new List<ParametersSet>();

        //    if (!discovederParameterProperties.Any())
        //        return parametersSets;

        //    var firstDiscoveredParameterProperty = discovederParameterProperties[0];
        //    foreach (var v in firstDiscoveredParameterProperty.attribute.Values)
        //    {
        //        var parametersSubSets = JoinWithTheOthers(firstDiscoveredParameterProperty.property, v, startingFromIndex: 1);
        //        parametersSets.AddRange(parametersSubSets);
        //    }

        //    return parametersSets;

        //    // Local functions

        //    List<ParametersSet> JoinWithTheOthers(PropertyInfo property, object value, int startingFromIndex)
        //    {
        //        var localParametersSets = new List<ParametersSet>();

        //        if (discovederParameterProperties.Count > startingFromIndex)
        //        {
        //            var nextParameterProperties = discovederParameterProperties[startingFromIndex];
        //            foreach (var v in nextParameterProperties.attribute.Values)
        //            {
        //                var parametersSubSets = JoinWithTheOthers(nextParameterProperties.property, v, startingFromIndex + 1);
        //                localParametersSets.AddRange(parametersSubSets);
        //            }
        //        }
        //        else
        //        {
        //            localParametersSets.Add(new ParametersSet { [property] = value });
        //        }

        //        return localParametersSets;
        //    }
        //}
    }
}
