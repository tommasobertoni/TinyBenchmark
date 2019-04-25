using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TinyBenchmark
{
    public class SequentialBenchmarksConfiguration : IRunnableBenchmarks
    {
        private readonly BenchmarkRunner _runner;
        private readonly HashSet<Type> _benchmarkContainerTypes = new HashSet<Type>();

        internal SequentialBenchmarksConfiguration(BenchmarkRunner runner)
        {
            _runner = runner;
        }

        public SequentialBenchmarksConfiguration Append<TBenchmarksContainer>() => Append(typeof(TBenchmarksContainer));

        public SequentialBenchmarksConfiguration Append(Type benchmarksContainerType)
        {
            _benchmarkContainerTypes.Add(benchmarksContainerType);
            return this;
        }

        BenchmarksCollectionReport IRunnableBenchmarks.Run()
        {
            var report = new BenchmarksCollectionReport { StartedAtUtc = DateTime.UtcNow };

            var sw = System.Diagnostics.Stopwatch.StartNew();

            report.ContainerReports = _benchmarkContainerTypes.Select(t => _runner.Run(t)).ToList();

            sw.Stop();
            report.Elapsed = sw.Elapsed;

            return report;
        }
    }
}
