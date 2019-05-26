using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TinyBenchmark
{
    public class BenchmarkReport : IReport
    {
        public string Name { get; }

        public DateTime StartedAtUtc { get; }

        public TimeSpan Duration { get; }

        public TimeSpan InitDuration { get; }

        public TimeSpan Warmup { get; }

        public TimeSpan AvgIterationDuration { get; }

        public bool IsBaseline { get; }

        public BaselineStats BaselineStats { get; internal set; }

        public IReadOnlyList<IterationReport> IterationReports { get; }

        public IReadOnlyList<Parameters> AppliedParameters { get; }

        public int SuccessfulIterations { get; }

        public AggregateException Exception { get; }

        public bool HasExceptions => this.Exception?.InnerExceptions?.Any() == true;

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
