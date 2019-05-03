﻿using System;
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

        private readonly BenchmarkOutput _output;

        public BenchmarkPlan(
            BenchmarkOutput output,
            InitReference init,
            IEnumerable<WarmupReference> warmups,
            ParametersSet parametersSet,
            ArgumentsReference arguments,
            BenchmarkReference benchmark,
            int iterations)
        {
            _output = output;

            this.Init = init;
            this.Warmups = warmups?.ToList().AsReadOnly();
            this.ParametersSet = parametersSet;
            this.Arguments = arguments;
            this.Benchmark = benchmark;
            this.Iterations = iterations > 0 ? iterations : throw new ArgumentException("Iterations must be positive");
        }

        internal BenchmarkReport Run<TBenchmarksContainer>(Func<TBenchmarksContainer> benchmarksContainerFactory)
        {
            #region Output

            if (this.ParametersSet != null)
            {
                var parametersStrings = this.ParametersSet.Select(p => $"{p.Key}={p.Value}");
                _output.WriteLine(OutputLevel.Normal, $"with parameters {string.Join(", ", parametersStrings)}");
            }

            if (this.Arguments?.Any() == true && _output.IsShown(OutputLevel.Verbose))
            {
                var argumentsStrings = this.Arguments.Select(a => $"{a.Key}={a.Value}");
                _output.WriteLine(OutputLevel.Verbose, $"with arguments {string.Join(", ", argumentsStrings)}");
            }

            _output.WriteLine(OutputLevel.Verbose, string.Empty);

            _output.IndentLevel++;

            #endregion

            var iterationReports = new List<IterationReport>(this.Iterations);

            AggregateException exception = null;

            var startedAtUtc = DateTime.UtcNow;
            var durationSW = Stopwatch.StartNew();

            try
            {
                for (int iterationNumber = 0; iterationNumber < this.Iterations; iterationNumber++)
                {
                    var iterationReport = RunIteration(benchmarksContainerFactory, iterationNumber);
                    iterationReports.Add(iterationReport);

                    #region Output

                    var prefix = $"[Iteration {iterationNumber}]";

                    if (_output.IsShown(OutputLevel.Verbose))
                    {
                        var failedOutput = iterationReport.Failed
                            ? $"[FAILED] => {LimitTo(iterationReport.Exception.Message, 200)}"
                            : string.Empty;

                        _output.WriteLine(OutputLevel.Verbose,
                            $"{prefix} elapsed: {iterationReport.Duration}, warmup: {iterationReport.Warmup} {failedOutput}".TrimEnd());
                    }
                    else
                    {
                        var failedOutput = iterationReport.Failed ? $"[FAILED]" : string.Empty;

                        _output.WriteLine(OutputLevel.Normal,
                            $"{prefix} elapsed: {iterationReport.Duration} {failedOutput}".TrimEnd());
                    }

                    if (iterationNumber < this.Iterations - 1) // Not for the last iteration
                        _output.WriteLine(OutputLevel.Verbose, string.Empty);

                    #endregion
                }
            }
            catch (Exception ex)
            {
                exception = new AggregateException(ex);
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
                iterationReports,
                exception);
        }

        protected virtual IterationReport RunIteration<TBenchmarksContainer>(
            Func<TBenchmarksContainer> benchmarksContainerFactory,
            int iterationNumber)
        {
            GC.Collect();

            Exception exception = null;

            Stopwatch warmupSW = null;
            Stopwatch durationSW = null;
            var startedAtUtc = DateTime.UtcNow;

            try
            {
                warmupSW = Stopwatch.StartNew();

                var benchmarksContainer = PrepareWarmContainer(benchmarksContainerFactory);

                warmupSW.Stop();


                durationSW = Stopwatch.StartNew();

                #region Output

                _output.WriteLine(OutputLevel.Normal, "Output:");
                _output.IndentLevel++;

                #endregion

                var methodParameters = this.Arguments?.AsMethodParameters();
                this.Benchmark.Executable.Invoke(benchmarksContainer, methodParameters);

                durationSW.Stop();
            }
            catch (TargetInvocationException ex)
            {
                exception = ex.InnerException;
            }
            catch (Exception ex)
            {
                exception = ex;
            }
            finally
            {
                _output.IndentLevel--;
                warmupSW.Stop();
                durationSW.Stop();
            }

            var argumentModels = this.Arguments?.Select(a => new Argument(a.Key, a.Value));

            return new IterationReport(
                iterationNumber,
                null,
                argumentModels,
                startedAtUtc,
                warmupSW.Elapsed,
                durationSW.Elapsed,
                exception);
        }

        protected virtual object PrepareWarmContainer<TBenchmarksContainer>(Func<TBenchmarksContainer> benchmarksContainerFactory)
        {
            var benchmarksContainer = benchmarksContainerFactory();

            this.ParametersSet?.ApplyTo(benchmarksContainer);

            this.Init?.Executable.Invoke(benchmarksContainer, null);
            this.Benchmark?.InitWithReference?.Executable.Invoke(benchmarksContainer, null);

            if (this.Warmups.Any())
            {
                foreach (var warmup in this.Warmups)
                {
                    #region Output

                    if (_output.IsShown(OutputLevel.Verbose))
                        _output.WriteLine(OutputLevel.Verbose, $"- warmup {warmup.Name}, order {warmup.Order}");
                    else
                        _output.WriteLine(OutputLevel.Verbose, $"- warmup {warmup.Name}");

                    #endregion

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