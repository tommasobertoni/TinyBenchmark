using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace TinyBenchmark.Analysis
{
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

        public BenchmarkPlan(
            BenchmarkOutput output,
            InitReference init,
            IEnumerable<WarmupReference> warmups,
            ParametersSet parametersSet,
            ArgumentsReference arguments,
            BenchmarkReference benchmark,
            int iterations,
            bool isBaseline = false)
        {
            _output = output;

            this.Init = init;
            this.Warmups = warmups?.ToList().AsReadOnly();
            this.ParametersSet = parametersSet;
            this.Arguments = arguments;
            this.Benchmark = benchmark;
            this.Iterations = iterations > 0 ? iterations : throw new ArgumentException("Iterations must be positive");
            this.IsBaseline = isBaseline;
        }

        internal BenchmarkReport Run<TBenchmarksContainer>(
            Func<TBenchmarksContainer> benchmarksContainerFactory,
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

            var startedAtUtc = DateTime.UtcNow;
            var durationSW = Stopwatch.StartNew();

            try
            {
                // Create

                initSW = Stopwatch.StartNew();

                var benchmarksContainer = CreateContainer(benchmarksContainerFactory);

                initSW.Stop();

                // Warmup

                warmupSW = Stopwatch.StartNew();

                WarmupContainer(benchmarksContainer);

                warmupSW.Stop();

                // Run

                var methodParameters = this.Arguments?.AsMethodParameters();
                var executable = this.Benchmark.Executable;

                GC.Collect();

                for (int iterationNumber = 0; iterationNumber < this.Iterations; iterationNumber++)
                {
                    var iterationReport = RunIteration(benchmarksContainer, executable, methodParameters, iterationNumber);
                    iterationReports.Add(iterationReport);

                    progress?.IncreaseProcessedItems();

                    #region Output

                    if (iterationReport.Failed)
                    {
                        var failedOutput = _output.IsShown(OutputLevel.Verbose)
                            ? $"[FAILED] => {LimitTo(iterationReport.Exception.Message, 200)}"
                            : $"[FAILED]";

                        _output.WriteLine(failedOutput);
                    }

                    #endregion
                }
            }
            catch (Exception ex)
            {
                initSW.Stop();
                warmupSW.Stop();
                exception = new AggregateException(ex);

                if (!_output.IsShown(OutputLevel.Silent) && !_output.IsShown(OutputLevel.Minimal))
                    _output.WriteLine(OutputLevel.ErrorsOnly, $"[Error] {ex.Message}");
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

        protected virtual IterationReport RunIteration<TBenchmarksContainer>(
            TBenchmarksContainer benchmarksContainer,
            MethodInfo executable,
            object[] methodParameters,
            int iterationNumber)
        {
            Exception exception = null;

            _output.IndentLevel++;

            var startedAtUtc = DateTime.UtcNow;
            Stopwatch durationSW = Stopwatch.StartNew();

            try
            {
                executable.Invoke(benchmarksContainer, methodParameters);
                durationSW.Stop();
            }
            catch (TargetInvocationException ex)
            {
                exception = ex.InnerException;

                if (!_output.IsShown(OutputLevel.Silent) && !_output.IsShown(OutputLevel.Minimal))
                    _output.WriteLine(OutputLevel.ErrorsOnly, $"[Error] {ex.InnerException.Message}");
            }
            catch (Exception ex)
            {
                exception = ex;

                if (!_output.IsShown(OutputLevel.Silent) && !_output.IsShown(OutputLevel.Minimal))
                    _output.WriteLine(OutputLevel.ErrorsOnly, $"[Error] {ex.Message}");
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

        private object CreateContainer<TBenchmarksContainer>(Func<TBenchmarksContainer> benchmarksContainerFactory)
        {
            var benchmarksContainer = benchmarksContainerFactory();

            this.ParametersSet?.ApplyTo(benchmarksContainer);

            this.Init?.Executable.Invoke(benchmarksContainer, null);

            return benchmarksContainer;
        }

        protected virtual object WarmupContainer<TBenchmarksContainer>(TBenchmarksContainer benchmarksContainer)
        {
            this.Benchmark?.InitWithReference?.Executable.Invoke(benchmarksContainer, null);

            if (this.Warmups.Any())
            {
                foreach (var warmup in this.Warmups)
                {
                    warmup.Executable.Invoke(benchmarksContainer, null);
                }
            }

            return benchmarksContainer;
        }

        #region Helpers

        private string LimitTo(string message, int maxLength)
        {
            if (maxLength < 0) throw new ArgumentException("Max length must be non-negative.");

            int messageLength = message?.Length ?? 0;

            if (messageLength <= maxLength)
                return message;

            var croppedMessage = message.Substring(0, maxLength);
            return $"{croppedMessage}...";
        }

        #endregion
    }
}
