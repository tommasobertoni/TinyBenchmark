﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinyBenchmark.Analysis
{
    internal class ContainerExecutionPlan
    {
        public ContainerReference Container { get; }

        public IReadOnlyList<BenchmarkPlan> BenchmarkPlans => _benchmarkPlans.AsReadOnly();

        private readonly BenchmarkOutput _output;
        private readonly PreDefinedBenchmarkOutput _preDefinedBenchmarkOutput;

        private readonly List<BenchmarkPlan> _benchmarkPlans = new List<BenchmarkPlan>();

        public ContainerExecutionPlan(BenchmarkOutput output, ContainerReference container)
        {
            _output = output;
            _preDefinedBenchmarkOutput = new PreDefinedBenchmarkOutput(OutputLevel.Verbose, _output);

            this.Container = container;
        }

        public void Add(BenchmarkPlan benchmarkPlan) => _benchmarkPlans.Add(benchmarkPlan);

        public BenchmarksContainerReport Run()
        {
            #region Output

            var outputContainerName = _output.IsShown(OutputLevel.Verbose)
                ? $"{this.Container.Name} (type {this.Container.ContainerType.FullName})"
                : this.Container.Name;

            _output.WriteLine(OutputLevel.Minimal, $"Running container \"{outputContainerName}\", total planned benchmarks: {this.BenchmarkPlans.Count}");
            _output.WriteLine(OutputLevel.Verbose, string.Empty);

            #endregion

            AggregateException exception = null;
            var benchmarkReports = new List<BenchmarkReport>(_benchmarkPlans.Count);
            var startedAtUtc = DateTime.UtcNow;
            var durationSW = Stopwatch.StartNew();

            if (this.BenchmarkPlans.Count == 0)
            {
                durationSW.Stop();
                _output.IndentLevel++;
                _output.WriteLine(OutputLevel.Minimal, $"No benchmarks found in container {outputContainerName}");
                _output.IndentLevel--;

                return new BenchmarksContainerReport(
                    this.Container.Name,
                    this.Container.ContainerType,
                    startedAtUtc,
                    durationSW.Elapsed,
                    benchmarkReports,
                    exception: exception);
            }

            try
            {
                #region Output

                // Use progress only when level is < Normal
                var progress = _output.IsShown(OutputLevel.Normal)
                    ? default : _output.ProgressFor(OutputLevel.Minimal, totalItems: this.BenchmarkPlans.Count);

                _output.IndentLevel++;

                #endregion

                foreach (var benchmarkPlan in _benchmarkPlans)
                {
                    #region Output

                    _output.WriteLine(OutputLevel.Verbose, string.Empty);
                    _output.WriteLine(OutputLevel.Normal, $"Benchmark: {benchmarkPlan.Benchmark.Name}");
                    _output.WriteLine(OutputLevel.Verbose, string.Empty);

                    // The output is shared between the execution plan and the benchmark plan.
                    // Therefore an increase in the indent level before running the benchmark plan
                    // will result in its output to be properly formatted.
                    _output.IndentLevel++;

                    #endregion

                    var report = benchmarkPlan.Run(() => CreateNew(this.Container.ContainerType));
                    benchmarkReports.Add(report);

                    #region Output

                    _output.IndentLevel--;

                    var warmupOutput = _output.IsShown(OutputLevel.Verbose)
                        ? $", avg warmup {report.AvgIterationWarmup}"
                        : string.Empty;

                    _output.WriteLine(OutputLevel.Verbose, string.Empty);
                    _output.WriteLine(OutputLevel.Normal,
                        $"=> elapsed: {report.Duration}, avg duration: {report.AvgIterationDuration} {warmupOutput}".TrimEnd());

                    _output.WriteLine(OutputLevel.Normal, string.Empty);

                    progress?.IncreaseProcessedItems();

                    #endregion
                }

                _output.IndentLevel--;
            }
            catch (Exception ex)
            {
                exception = new AggregateException(ex);
            }
            finally
            {
                durationSW.Stop();
            }

            var containerReport = new BenchmarksContainerReport(
                this.Container.Name,
                this.Container.ContainerType,
                startedAtUtc,
                durationSW.Elapsed,
                benchmarkReports,
                exception: exception);

            #region Output

            _output.WriteLine(OutputLevel.Normal, string.Empty);
            _output.WriteLine(OutputLevel.Normal, $"=> elapsed: {containerReport.Duration}");

            #endregion

            return containerReport;
        }

        #region Helpers

        private object CreateNew(Type benchmarksContainerType)
        {
            // TODO: allow a dynamic list of allowed constructor parameters.
            
            // TODO: should the constructor be searched for during analysis?
            //       BenchmarksScanner => ContainerConstructorReference + List<> Dependencies

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

            var container = constructorWithParameters == null
                ? Activator.CreateInstance(benchmarksContainerType)
                : constructorWithParameters.Invoke(new[] { _preDefinedBenchmarkOutput });

            return container;
        }

        #endregion
    }
}