using System;
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

                var benchmarkReportsWithSameParameters = new List<BenchmarkReport>();
                var plansGroupedByParameters = _benchmarkPlans.ToLookup(p => p.ParametersSet);

                foreach (var group in plansGroupedByParameters)
                {
                    var plans = group.ToList();

                    var aPlan = plans.First();

                    if (aPlan.ParametersSet != null)
                    {
                        var parametersStrings = aPlan.ParametersSet.Select(p => $"{p.Key}={p.Value}");
                        _output.WriteLine(OutputLevel.Verbose, $"With parameters {string.Join(", ", parametersStrings)}");
                    }
                    else
                    {
                        _output.WriteLine(OutputLevel.Verbose, $"Without parameters");
                    }

                    _output.IndentLevel++;

                    benchmarkReportsWithSameParameters.Clear();

                    foreach (var benchmarkPlan in plans)
                    {
                        #region Output

                        if (benchmarkPlan.Iterations <= 1)
                            _output.WriteLine(OutputLevel.Normal, $"Benchmark: {benchmarkPlan.Benchmark.Name}");
                        else
                            _output.WriteLine(OutputLevel.Normal, $"Benchmark: {benchmarkPlan.Benchmark.Name}, iterations: {benchmarkPlan.Iterations}");

                        // The output is shared between the execution plan and the benchmark plan.
                        // Therefore an increase in the indent level before running the benchmark plan
                        // will result in its output to be properly formatted.
                        _output.IndentLevel++;

                        #endregion

                        var report = benchmarkPlan.Run(() => CreateNew(this.Container.ContainerType));
                        benchmarkReportsWithSameParameters.Add(report);

                        _output.IndentLevel--;
                        progress?.IncreaseProcessedItems();
                    }

                    // The baseline is evaluated on every group of benchmarks with the same parameters.
                    EvaluateBaseline(benchmarkReportsWithSameParameters);

                    benchmarkReports.AddRange(benchmarkReportsWithSameParameters);

                    _output.IndentLevel--;
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

            return containerReport;
        }

        private void EvaluateBaseline(List<BenchmarkReport> reports)
        {
            var baselineReport = reports.FirstOrDefault(r => r.IsBaseline);
            if (baselineReport == null) return;

            var avgBaselineDurationTicks = baselineReport.AvgIterationDuration.Ticks;
            EvaluateBaselineOnIterations(baselineReport.IterationReports);

            foreach (var report in reports.Where(r => r != baselineReport))
            {
                var avgDurationTicks = report.AvgIterationDuration.Ticks;
                report.BaselineRatio = avgDurationTicks * 1.0m / avgBaselineDurationTicks;
                EvaluateBaselineOnIterations(report.IterationReports);
            }

            // Local functions

            void EvaluateBaselineOnIterations(IReadOnlyList<IterationReport> iterationReports)
            {
                foreach (var ir in iterationReports)
                {
                    var durationTicks = ir.Duration.Ticks;
                    ir.BaselineRatio = durationTicks * 1.0m / avgBaselineDurationTicks;
                }
            }
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
