using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using TinyBenchmark.Analysis;
using TinyBenchmark.Attributes;

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
            var benchmarksContainerAttribute = benchmarksContainerType
                .GetCustomAttributes(typeof(BenchmarksContainerAttribute), false)
                .FirstOrDefault() as BenchmarksContainerAttribute;

            var containerReport = new BenchmarksContainerReport
            {
                Name = benchmarksContainerAttribute?.Name ?? withName,
                StartedAtUtc = DateTime.UtcNow,
                Reports = new List<BenchmarkReport>(),
                BenchmarkContainerType = benchmarksContainerType,
            };

            var sw = System.Diagnostics.Stopwatch.StartNew();

            foreach (var bref in benchmarks)
            {
                var report = RunIterations(bref, benchmarksContainerType);
                containerReport.Reports.Add(report);
            }

            sw.Stop();

            containerReport.Elapsed = sw.Elapsed;

            return containerReport;
        }

        private BenchmarkReport RunIterations(
            BenchmarkReference bref,
            Type benchmarksContainerType)
        {
            var report = new BenchmarkReport
            {
                StartedAtUtc = DateTime.UtcNow,
                Name = bref.Name,
                SuccessfulIterations = 0,
                IterationReports = new List<IterationReport>(),
            };

            if (bref.ParametersSetCollection?.Any() == true)
            {
                foreach (var parameterSet in bref.ParametersSetCollection)
                    RunBenchmarkIterationsWithParameterSet(parameterSet);
            }
            else
            {
                RunBenchmarkIterationsWithParameterSet(null);
            }

            var failedIterationReports = report.IterationReports.Where(ir => ir.Failed).ToList();
            if (failedIterationReports.Any())
                report.Exception = new AggregateException(failedIterationReports.Select(ir => ir.Exception));

            var successfulIterationReports = report.IterationReports.Where(ir => ir.Failed == false).ToList();
            report.SuccessfulIterations = successfulIterationReports.Count;
            if (successfulIterationReports.Any())
            {
                var avgElapsedTicks = successfulIterationReports.Sum(ir => ir.Elapsed.Ticks) / successfulIterationReports.Count;
                report.Elapsed = TimeSpan.FromTicks(avgElapsedTicks);

                var avgWarmupTicks = successfulIterationReports.Sum(ir => ir.Warmup.Ticks) / successfulIterationReports.Count;
                report.Warmup = TimeSpan.FromTicks(avgWarmupTicks);
            }

            return report;

            // Local functions

            void RunBenchmarkIterationsWithParameterSet(ParametersSet parameterSet)
            {
                for (int i = 0; i < bref.Iterations; i++)
                {
                    GC.Collect();
                    var iterationReport = RunIteration(parameterSet);
                    report.IterationReports.Add(iterationReport);
                }
            }

            IterationReport RunIteration(ParametersSet parametersSet)
            {
                var iterationReport = new IterationReport
                {
                    StartedAtUtc = DateTime.UtcNow,
                    Parameters = parametersSet?.ToParameterValues(),
                };

                try
                {
                    var container = PrepareWarmContainer(bref, benchmarksContainerType, parametersSet, iterationReport);

                    var runSW = System.Diagnostics.Stopwatch.StartNew();
                    bref.Executable.Invoke(container, null);
                    runSW.Stop();

                    iterationReport.Elapsed = runSW.Elapsed;
                }
                catch (TargetInvocationException ex)
                {
                    iterationReport.Exception = ex.InnerException;
                }
                catch (Exception ex)
                {
                    iterationReport.Exception = ex;
                }

                return iterationReport;
            }
        }

        private object PrepareWarmContainer(
            BenchmarkReference bref,
            Type benchmarksContainerType,
            ParametersSet parametersSet,
            IterationReport iterationReport)
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

            // Create

            var container = constructorWithParameters == null
                ? Activator.CreateInstance(benchmarksContainerType)
                : constructorWithParameters.Invoke(new[] { new BenchmarkOutput(this) });

            // Init

            parametersSet?.ApplyTo(container);
            bref.Init?.Invoke(container, null);

            // Warm up

            foreach (var warmup in bref.Warmups)
                warmup.Invoke(container, null);

            warmupSW.Stop();

            iterationReport.Warmup = warmupSW.Elapsed;

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
