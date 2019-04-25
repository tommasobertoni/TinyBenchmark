using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TinyBenchmark.Analysis;

namespace TinyBenchmark
{
    public class BenchmarkRunner
    {
        private readonly BenchmarksAnalyzer _analyzer;

        public BenchmarkRunner()
        {
            _analyzer = new BenchmarksAnalyzer();
        }

        public IRunnableBenchmarks InSequence(Action<SequentialBenchmarksConfiguration> sequenceConfiguration)
        {
            var config = new SequentialBenchmarksConfiguration(this);
            sequenceConfiguration(config);
            return config;
        }

        public BenchmarksContainerReport Run<TBenchmarksContainer>() where TBenchmarksContainer : new() =>
            this.Run(typeof(TBenchmarksContainer));

        public BenchmarksContainerReport Run(Type benchmarksContainerType, string withName = null)
        {
            var benchmarks = _analyzer.FindAllbenchmarks(benchmarksContainerType);

            var containerReport = new BenchmarksContainerReport
            {
                Name = withName,
                StartedAtUtc = DateTime.UtcNow,
                Reports = new List<BenchmarkReport>(),
                BenchmarkContainerType = benchmarksContainerType,
            };

            var sw = System.Diagnostics.Stopwatch.StartNew();

            foreach (var bref in benchmarks)
            {
                var report = new BenchmarkReport
                {
                    StartedAtUtc = DateTime.UtcNow,
                    Name = bref.Name,
                };

                RunIterations(bref, benchmarksContainerType, report);

                containerReport.Reports.Add(report);
            }

            sw.Stop();

            containerReport.Elapsed = sw.Elapsed;

            return containerReport;
        }

        private void RunIterations(
            BenchmarkReference bref,
            Type benchmarksContainerType,
            BenchmarkReport report)
        {
            var ticksOfRuns = new List<long>();
            var exceptions = new List<Exception>();

            for (int i = 0; i < bref.Iterations; i++)
            {
                GC.Collect();

                try
                {
                    var container = PrepareWarmContainer(bref, benchmarksContainerType, report);

                    var runSW = System.Diagnostics.Stopwatch.StartNew();
                    bref.Executable.Invoke(container, null);
                    runSW.Stop();

                    ticksOfRuns.Add(runSW.ElapsedTicks);
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                }
            }

            if (exceptions.Any())
                report.Exception = new AggregateException(exceptions);

            var avgTicks = ticksOfRuns.Sum(x => x) / ticksOfRuns.Count;
            report.Elapsed = TimeSpan.FromTicks(avgTicks);
        }

        private object PrepareWarmContainer(
            BenchmarkReference bref,
            Type benchmarksContainerType,
            BenchmarkReport report)
        {            
            var constructorWithParameters = benchmarksContainerType.GetConstructors().FirstOrDefault(c =>
            {
                var constructorParameters = c.GetParameters();
                if (constructorParameters.Any())
                {
                    return
                        constructorParameters.Length == 1 &&
                        constructorParameters.First().ParameterType == typeof(IBenchmarkOutput);
                }

                return false;
            });

            var warmupSW = System.Diagnostics.Stopwatch.StartNew();

            var container = constructorWithParameters == null
                ? Activator.CreateInstance(benchmarksContainerType)
                : constructorWithParameters.Invoke(new[] { new BenchmarkOutput(this) });

            foreach (var warmup in bref.Warmups)
                warmup.Invoke(container, null);

            return container;
        }
    }

    internal class BenchmarkOutput : IBenchmarkOutput
    {
        private readonly BenchmarkRunner _runner;
        private readonly StringBuilder _sb;

        public BenchmarkOutput(BenchmarkRunner runner)
        {
            _runner = runner;
            _sb = new StringBuilder();
        }

        public void WriteLine(string text)
        {
            _sb.AppendLine(text);
            Console.WriteLine(text); // TODO: remove this
        }
    }
}
