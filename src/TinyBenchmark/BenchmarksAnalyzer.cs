using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using TinyBenchmark.Attributes;

namespace TinyBenchmark
{
    public class BenchmarkReference
    {
        public string Name { get; internal set; }

        public MethodInfo Executable { get; internal set; }

        public int Iterations { get; internal set; }
    }

    public class BenchmarksAnalyzer
    {
        public List<BenchmarkReference> FindAllbenchmarks<TBenchmarksContainer>(TBenchmarksContainer container)
        {
            var benchmarkMethods = container.GetType()
                .GetMethods(BindingFlags.Instance | BindingFlags.Public)
                .Select(m => (method: m, attribute: m.GetCustomAttribute<BenchmarkAttribute>()))
                .Where(x => x.attribute != null)
                .ToList();

            var references = benchmarkMethods.Select(x => new BenchmarkReference
            {
                Executable = x.method,
                Name = x.attribute.Name,
                Iterations = Math.Max(1, x.attribute.Iterations),
            }).ToList();

            return references;
        }
    }
}
