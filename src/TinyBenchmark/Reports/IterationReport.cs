using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TinyBenchmark
{
    public class IterationReport : IBenchmark
    {
        public int IterationNumber { get; }

        public Parameters Parameters { get; }

        public IReadOnlyList<Argument> Arguments { get; }

        public DateTime StartedAtUtc { get; }

        public TimeSpan Warmup { get; }

        public TimeSpan Duration { get; }

        public decimal? BaselineRatio { get; internal set; }

        public Exception Exception { get; }

        public bool Failed => this.Exception != null;

        protected internal IterationReport(
            int iterationNumber,
            Parameters parameters,
            IEnumerable<Argument> arguments,
            DateTime startedAtUtc,
            TimeSpan warmup,
            TimeSpan duration,
            Exception exception = null)
        {
            this.IterationNumber = iterationNumber;
            this.Parameters = parameters;
            this.Arguments = arguments?.ToList().AsReadOnly();
            this.StartedAtUtc = startedAtUtc.ToUniversalTime();
            this.Warmup = warmup;
            this.Duration = duration;
            this.Exception = exception;
        }

        void IBenchmark.Accept(IExporter exporter) => exporter.Visit(this);
    }
}
