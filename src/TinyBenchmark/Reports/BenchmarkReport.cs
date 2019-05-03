using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TinyBenchmark
{
    public class BenchmarkReport
    {
        public string Name { get; }

        public DateTime StartedAtUtc { get; }

        public TimeSpan Duration { get; }

        public TimeSpan AvgIterationWarmup { get; }

        public TimeSpan AvgIterationDuration { get; }

        public IReadOnlyList<IterationReport> IterationReports { get; }

        public int SuccessfulIterations { get; }

        public AggregateException Exception { get; }

        public bool HasExceptions => this.Exception?.InnerExceptions?.Any() == true;

        internal BenchmarkReport(
            string name,
            DateTime startedAtUtc,
            TimeSpan duration,
            IEnumerable<IterationReport> iterationReports,
            AggregateException exception = null)
        {
            this.Name = name;
            this.StartedAtUtc = startedAtUtc.ToUniversalTime();
            this.Duration = duration;
            this.IterationReports = iterationReports?.ToList().AsReadOnly();
            this.Exception = exception;

            this.SuccessfulIterations = this.IterationReports?.Count(ir => ir.Failed == false) ?? 0;
            this.AvgIterationWarmup = iterationReports == null ? default : Average(ir => ir.Warmup);
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
    }
}
