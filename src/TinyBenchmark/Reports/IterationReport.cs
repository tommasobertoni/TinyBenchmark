using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TinyBenchmark
{
    /// <summary>
    /// The report of a single iteration of a benchmark.
    /// </summary>
    public class IterationReport : IReport
    {
        /// <summary>
        /// The number of the iteration.
        /// </summary>
        public int IterationNumber { get; }

        /// <summary>
        /// The parameters that were applied to the container during this iteration.
        /// </summary>
        public Parameters Parameters { get; }

        /// <summary>
        /// The arguments that were used during this iteration.
        /// </summary>
        public IReadOnlyList<Argument> Arguments { get; }

        /// <summary>
        /// The UTC time of when this iteration started.
        /// </summary>
        public DateTime StartedAtUtc { get; }

        /// <summary>
        /// The duration of this iteration.
        /// </summary>
        public TimeSpan Duration { get; }

        /// <summary>
        /// The comparison data against the baseline benchmark's results.
        /// It's null when there was no baseline benchmark.
        /// </summary>
        public BaselineStats BaselineStats { get; internal set; }

        /// <summary>
        /// The exceptions thrown during the execution of this benchmark.
        /// </summary>
        public Exception Exception { get; }

        /// <summary>
        /// Whether or not this iteration failed.
        /// </summary>
        public bool Failed => this.Exception != null;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="iterationNumber">The number of the iteration.</param>
        /// <param name="parameters">The parameters that were applied to the container during this iteration.</param>
        /// <param name="arguments">The arguments that were used during this iteration.</param>
        /// <param name="startedAtUtc">The UTC time of when this iteration started.</param>
        /// <param name="duration">The duration of this iteration.</param>
        /// <param name="exception">The exceptions thrown during the execution of this benchmark.</param>
        protected internal IterationReport(
            int iterationNumber,
            Parameters parameters,
            IEnumerable<Argument> arguments,
            DateTime startedAtUtc,
            TimeSpan duration,
            Exception exception = null)
            : this(iterationNumber, parameters, arguments, startedAtUtc, duration, null, exception)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="iterationNumber">The number of the iteration.</param>
        /// <param name="parameters">The parameters that were applied to the container during this iteration.</param>
        /// <param name="arguments">The arguments that were used during this iteration.</param>
        /// <param name="startedAtUtc">The UTC time of when this iteration started.</param>
        /// <param name="duration">The duration of this iteration.</param>
        /// <param name="baselineStats">The comparison data against the baseline benchmark's results.</param>
        /// <param name="exception">The exceptions thrown during the execution of this benchmark.</param>
        protected internal IterationReport(
            int iterationNumber,
            Parameters parameters,
            IEnumerable<Argument> arguments,
            DateTime startedAtUtc,
            TimeSpan duration,
            BaselineStats baselineStats,
            Exception exception = null)
        {
            this.IterationNumber = iterationNumber;
            this.Parameters = parameters;
            this.Arguments = arguments?.ToList().AsReadOnly();
            this.StartedAtUtc = startedAtUtc.ToUniversalTime();
            this.Duration = duration;
            this.BaselineStats = baselineStats;
            this.Exception = exception;
        }

        void IReport.Accept(IExporter exporter) => exporter.Visit(this);
    }
}
