using System;
using System.Collections.Generic;
using System.Linq;

namespace TinyBenchmark
{
    public class BenchmarksContainerReport
    {
        public string Name { get; set; }

        public Type BenchmarkContainerType { get; internal set; }

        public DateTime StartedAtUtc { get; internal set; }

        public TimeSpan Elapsed { get; internal set; }

        public List<BenchmarkReport> Reports { get; internal set; }
    }

    public class BenchmarkReport
    {
        public DateTime StartedAtUtc { get; internal set; }

        public TimeSpan Elapsed { get; internal set; }

        public string Name { get; internal set; }

        public bool Failed => this.Exception != null;

        public Exception Exception { get; set; }
    }

    public class BenchmarksCollectionReport
    {
        public DateTime StartedAtUtc { get; internal set; }

        public TimeSpan Elapsed { get; internal set; }

        public List<BenchmarksContainerReport> ContainerReports { get; internal set; }
    }

    public class BenchmarkRunner
    {
        private readonly BenchmarksAnalyzer _analyzer;

        public BenchmarkRunner()
        {
            _analyzer = new BenchmarksAnalyzer();
        }

        public IRunnableBenchmarks InSequence(Action<SequentialBenchmarkRunnersConfiguration> sequenceConfiguration)
        {
            var config = new SequentialBenchmarkRunnersConfiguration(this);
            sequenceConfiguration(config);
            return config;
        }

        public BenchmarksContainerReport Run<TBenchmarksContainer>() where TBenchmarksContainer : new() => this.Run(Activator.CreateInstance<TBenchmarksContainer>());

        public BenchmarksContainerReport Run<TBenchmarksContainer>(TBenchmarksContainer container, string withName = null)
        {
            var benchmarks = _analyzer.FindAllbenchmarks(container);

            var containerReport = new BenchmarksContainerReport
            {
                Name = withName,
                StartedAtUtc = DateTime.UtcNow,
                Reports = new List<BenchmarkReport>(),
                BenchmarkContainerType = container.GetType(),
            };

            var sw = System.Diagnostics.Stopwatch.StartNew();

            foreach (var bref in benchmarks)
            {
                var report = new BenchmarkReport
                {
                    StartedAtUtc = DateTime.UtcNow,
                    Name = bref.Name,
                };

                RunIterations(bref, container, report);

                containerReport.Reports.Add(report);
            }

            sw.Stop();

            containerReport.Elapsed = sw.Elapsed;

            return containerReport;
        }

        private void RunIterations<TBenchmarksContainer>(BenchmarkReference bref, TBenchmarksContainer container, BenchmarkReport report)
        {
            var ticksOfRuns = new List<long>();

            for (int i = 0; i < bref.Iterations; i++)
            {
                var sw = System.Diagnostics.Stopwatch.StartNew();

                try
                {
                    bref.Executable.Invoke(container, null);
                    sw.Stop();
                }
                catch (Exception ex)
                {
                    sw.Stop();
                    report.Exception = ex;
                }

                ticksOfRuns.Add(sw.ElapsedTicks);
            }

            var avgTicks = ticksOfRuns.Sum(x => x) / bref.Iterations;
            report.Elapsed = TimeSpan.FromTicks(avgTicks);
        }
    }

    public interface IRunnableBenchmarks
    {
        BenchmarksCollectionReport Run();
    }

    public class SequentialBenchmarkRunnersConfiguration : IRunnableBenchmarks
    {
        private readonly BenchmarkRunner _runner;
        private readonly List<object> _benchmarkInstances = new List<object>();

        internal SequentialBenchmarkRunnersConfiguration(BenchmarkRunner runner)
        {
            _runner = runner;
        }

        public SequentialBenchmarkRunnersConfiguration Append<TBenchmarksContainer>() where TBenchmarksContainer : new()
        {
            var instance = Activator.CreateInstance<TBenchmarksContainer>();
            _benchmarkInstances.Add(instance);
            return this;
        }

        BenchmarksCollectionReport IRunnableBenchmarks.Run()
        {
            var report = new BenchmarksCollectionReport { StartedAtUtc = DateTime.UtcNow };

            var sw = System.Diagnostics.Stopwatch.StartNew();
            report.ContainerReports = _benchmarkInstances.Select(x => _runner.Run(x)).ToList();
            sw.Stop();

            report.Elapsed = sw.Elapsed;

            return report;
        }
    }
}
