using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TinyBenchmark
{
    public class BenchmarksContainerReport
    {
        public string Name { get; }

        public Type BenchmarkContainerType { get; }

        public DateTime StartedAtUtc { get; }

        public TimeSpan Duration { get; }

        public IReadOnlyList<BenchmarkReport> Reports { get; }

        public AggregateException Exception { get; set; }

        public bool HasExceptions => this.Exception?.InnerExceptions?.Any() == true;

        public BenchmarksContainerReport(
            string name,
            Type benchmarkContainerType,
            DateTime startedAtUtc,
            TimeSpan duration,
            IEnumerable<BenchmarkReport> reports,
            AggregateException exception = null)
        {
            this.Name = name;
            this.BenchmarkContainerType = benchmarkContainerType;
            this.StartedAtUtc = startedAtUtc.ToUniversalTime();
            this.Duration = duration;
            this.Reports = reports?.ToList().AsReadOnly();
            this.Exception = exception;
        }
    }
}
