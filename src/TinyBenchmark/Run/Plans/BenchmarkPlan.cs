using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using TinyBenchmark.Analysis;

namespace TinyBenchmark.Run
{
    /// <summary>
    /// Identifies an execution plan for a benchmark method.
    /// </summary>
    internal class BenchmarkPlan
    {
        public InitReference Init { get; }

        public IReadOnlyList<WarmupReference> Warmups { get; }

        public ParametersSet ParametersSet { get; }

        public BenchmarkReference Benchmark { get; }

        public ArgumentsReference Arguments { get; }

        public int Iterations { get; }

        public bool IsBaseline { get; }

        private readonly BenchmarkOutput _output;
        private readonly ExecutableBuilder _executableBuilder;

        public BenchmarkPlan(
            BenchmarkOutput output,
            ExecutableBuilder executableBuilder,
            InitReference init,
            IEnumerable<WarmupReference> warmups,
            ParametersSet parametersSet,
            ArgumentsReference arguments,
            BenchmarkReference benchmark,
            int iterations,
            bool isBaseline = false)
        {
            _output = output;
            _executableBuilder = executableBuilder;

            this.Init = init;
            this.Warmups = warmups?.ToList().AsReadOnly();
            this.ParametersSet = parametersSet;
            this.Arguments = arguments;
            this.Benchmark = benchmark;
            this.Iterations = iterations > 0 ? iterations : throw new ArgumentException("Iterations must be positive");
            this.IsBaseline = isBaseline;
        }

        public BenchmarkReport Run<TBenchmarksContainer>(
            TBenchmarksContainer benchmarksContainer,
            ProgressWriter progress)
        {
            #region Output

            if (this.Arguments?.Any() == true && _output.IsShown(OutputLevel.Verbose))
            {
                var argumentsStrings = this.Arguments.Select(a => $"{a.Key}={a.Value}");
                _output.WriteLine(OutputLevel.Verbose, $"with arguments {string.Join(", ", argumentsStrings)}");
            }

            _output.IndentLevel++;

            #endregion

            var iterationReports = new List<IterationReport>(this.Iterations);

            AggregateException exception = null;
            Stopwatch initSW = null;
            Stopwatch warmupSW = null;

            var methodParameters = this.Arguments?.AsMethodParameters();

            var executable = _executableBuilder
                .Using(benchmarksContainer)
                .With(this.Init)
                .With(this.Benchmark?.InitWithReference)
                .With(this.Warmups)
                .For(this.Benchmark.Method, methodParameters)
                .Create();

            var startedAtUtc = DateTime.UtcNow;
            var durationSW = Stopwatch.StartNew();

            try
            {
                // Init

                initSW = Stopwatch.StartNew();
                this.ParametersSet?.ApplyTo(benchmarksContainer);
                executable.ExecuteInits();
                initSW.Stop();

                // Warmup

                warmupSW = Stopwatch.StartNew();
                executable.ExecuteWarmups();
                warmupSW.Stop();

                // Run

                for (int iterationNumber = 0; iterationNumber < this.Iterations; iterationNumber++)
                {
                    GC.Collect();

                    var iterationReport = RunIteration(executable, iterationNumber);
                    iterationReports.Add(iterationReport);

                    progress?.IncreaseProcessedItems();
                }
            }
            catch (Exception ex)
            {
                initSW.Stop();
                warmupSW.Stop();
                exception = new AggregateException(ex);
                _output.WriteLine(OutputLevel.ErrorsOnly, $"[Error] {ex.Message.LimitTo(200)}");
            }
            finally
            {
                durationSW.Stop();
            }

            var iterationExceptions = iterationReports.Where(ir => ir.Failed).Select(ir => ir.Exception).ToList();
            if (iterationExceptions.Any())
            {
                exception = exception == null
                    ? new AggregateException(iterationExceptions)
                    : new AggregateException(exception.InnerExceptions.Concat(iterationExceptions));
            }

            _output.IndentLevel--;

            return new BenchmarkReport(
                this.Benchmark.Name,
                startedAtUtc,
                durationSW.Elapsed,
                initSW.Elapsed,
                warmupSW.Elapsed,
                this.IsBaseline,
                null,
                iterationReports,
                exception);
        }

        protected virtual IterationReport RunIteration(Executable executable, int iterationNumber)
        {
            Exception exception = null;

            _output.IndentLevel++;

            var startedAtUtc = DateTime.UtcNow;
            Stopwatch durationSW = Stopwatch.StartNew();

            try
            {
                executable.ExecuteBenchmark();
                durationSW.Stop();
            }
            catch (TargetInvocationException ex)
            {
                exception = ex.InnerException;
                _output.WriteLine(OutputLevel.ErrorsOnly, $"[Error] {ex.InnerException.Message.LimitTo(200)}");
            }
            catch (Exception ex)
            {
                exception = ex;
                _output.WriteLine(OutputLevel.ErrorsOnly, $"[Error] {ex.Message.LimitTo(200)}");
            }
            finally
            {
                _output.IndentLevel--;
                durationSW.Stop();
            }

            var parametersModels = this.ParametersSet == null
                ? null :
                new Parameters(this.ParametersSet.Select(p => new ParameterValue(p.Key, p.Value)));

            var argumentModels = this.Arguments?.Select(a => new Argument(a.Key, a.Value));

            return new IterationReport(
                iterationNumber,
                parametersModels,
                argumentModels,
                startedAtUtc,
                durationSW.Elapsed,
                exception);
        }
    }
}
