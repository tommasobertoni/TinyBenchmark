using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TinyBenchmark
{
    /// <summary>
    /// The report of a benchmark.
    /// </summary>
    public class BenchmarkReport : IReport
    {
        /// <summary>
        /// The name of this benchmark.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The UTC time of when this benchmark started.
        /// </summary>
        public DateTime StartedAtUtc { get; }

        /// <summary>
        /// The total duration of this benchmark.
        /// </summary>
        public TimeSpan Duration { get; }

        /// <summary>
        /// The duration of the initialization.
        /// </summary>
        public TimeSpan InitDuration { get; }

        /// <summary>
        /// The duration of the warm-up.
        /// </summary>
        public TimeSpan Warmup { get; }

        /// <summary>
        /// The average duration of all the iterations of this benchmark.
        /// </summary>
        public TimeSpan AvgIterationDuration { get; }

        /// <summary>
        /// Indicates whether or not this benchmark was marked as a baseline for comparing the other benchmarks in the same container.
        /// </summary>
        public bool IsBaseline { get; }

        /// <summary>
        /// The comparison data against the baseline benchmark's results.
        /// It's null when there was no baseline benchmark or when this is the baseline benchmark.
        /// </summary>
        public BaselineStats BaselineStats { get; internal set; }

        /// <summary>
        /// The reports of the iterations of this benchmark.
        /// </summary>
        public IReadOnlyList<IterationReport> IterationReports { get; }

        /// <summary>
        /// The parameters that were applied to the container right before running this benchmark.
        /// </summary>
        public IReadOnlyList<Parameters> AppliedParameters { get; }

        /// <summary>
        /// The number of successful iterations.
        /// </summary>
        public int SuccessfulIterations { get; }

        /// <summary>
        /// The exceptions thrown during the execution of this benchmark.
        /// </summary>
        public AggregateException Exception { get; }

        /// <summary>
        /// Indicates whether or not this benchmark had any exceptions.
        /// </summary>
        public bool HasExceptions => this.Exception?.InnerExceptions?.Any() == true;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">The name of this benchmark.</param>
        /// <param name="startedAtUtc">The UTC time of when this benchmark started.</param>
        /// <param name="duration">The total duration of this benchmark.</param>
        /// <param name="initDuration">The duration of the initialization for this benchmark.</param>
        /// <param name="warmup">The duration of the warm-up for this benchmark.</param>
        /// <param name="isBaseline">Indicates whether or not this benchmark was marked as a baseline for comparing the other benchmarks in the same container.</param>
        /// <param name="iterationReports">The reports of the iterations of this benchmark.</param>
        /// <param name="exception">The exceptions thrown during the execution of this benchmark.</param>
        protected internal BenchmarkReport(
            string name,
            DateTime startedAtUtc,
            TimeSpan duration,
            TimeSpan initDuration,
            TimeSpan warmup,
            bool isBaseline,
            IEnumerable<IterationReport> iterationReports,
            AggregateException exception = null)
            : this(name, startedAtUtc, duration, initDuration, warmup, isBaseline, null, iterationReports, exception)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">The name of this benchmark.</param>
        /// <param name="startedAtUtc">The UTC time of when this benchmark started.</param>
        /// <param name="duration">The total duration of this benchmark.</param>
        /// <param name="initDuration">The duration of the initialization for this benchmark.</param>
        /// <param name="warmup">The duration of the warm-up for this benchmark.</param>
        /// <param name="isBaseline">Indicates whether or not this benchmark was marked as a baseline for comparing the other benchmarks in the same container.</param>
        /// <param name="baselineStats">The comparison data against the baseline benchmark's results.</param>
        /// <param name="iterationReports">The reports of the iterations of this benchmark.</param>
        /// <param name="exception">The exceptions thrown during the execution of this benchmark.</param>
        protected internal BenchmarkReport(
            string name,
            DateTime startedAtUtc,
            TimeSpan duration,
            TimeSpan initDuration,
            TimeSpan warmup,
            bool isBaseline,
            BaselineStats baselineStats,
            IEnumerable<IterationReport> iterationReports,
            AggregateException exception = null)
        {
            this.Name = name;
            this.StartedAtUtc = startedAtUtc.ToUniversalTime();
            this.Duration = duration;
            this.InitDuration = initDuration;
            this.Warmup = warmup;

            this.IsBaseline = isBaseline;
            this.BaselineStats = baselineStats;

            this.IterationReports = iterationReports?.ToList().AsReadOnly();
            this.AppliedParameters = this.IterationReports?.Select(ir => ir.Parameters).Distinct().ToList();
            this.Exception = exception;

            this.SuccessfulIterations = this.IterationReports?.Count(ir => ir.Failed == false) ?? 0;
            this.AvgIterationDuration = iterationReports == null ? default : Average(ir => ir.Duration);

            // Local functions

            TimeSpan Average(Func<IterationReport, TimeSpan> timeProjection)
            {
                if (this.SuccessfulIterations <= 0) return default;
                var sumOfTicks = iterationReports.Sum(t => timeProjection(t).Ticks);
                var avgTicks = sumOfTicks / this.SuccessfulIterations;
                var avgTime = TimeSpan.FromTicks(avgTicks);
                return avgTime;
            }
        }

        void IReport.Accept(IExporter exporter) => exporter.Visit(this);
    }
}
