using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TinyBenchmark
{
    /// <summary>
    /// The report of a benchmarks container.
    /// </summary>
    public class BenchmarksContainerReport : IReport
    {
        /// <summary>
        /// The name of the container.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The type of the container.
        /// </summary>
        public Type BenchmarkContainerType { get; }

        /// <summary>
        /// The UTC time of when the container benchmark started.
        /// </summary>
        public DateTime StartedAtUtc { get; }

        /// <summary>
        /// The total duration of the benchmark of this container.
        /// </summary>
        public TimeSpan Duration { get; }

        /// <summary>
        /// The reports of the benchmarks included in this container.
        /// </summary>
        public IReadOnlyList<BenchmarkReport> Reports { get; }

        /// <summary>
        /// The exceptions thrown during the execution of this container.
        /// </summary>
        public AggregateException Exception { get; set; }

        /// <summary>
        /// Indicates whether or not this container had any exceptions during the benchmark.
        /// </summary>
        public bool HasExceptions => this.Exception?.InnerExceptions?.Any() == true;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">The name of the container.</param>
        /// <param name="benchmarkContainerType">The type of the container.</param>
        /// <param name="startedAtUtc">The UTC time of when the benchmark started.</param>
        /// <param name="duration">The total duration of the benchmark of this container.</param>
        /// <param name="reports">The reports of the benchmarks included in this container.</param>
        /// <param name="exception">The exceptions thrown during the execution of this container.</param>
        protected internal BenchmarksContainerReport(
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

        void IReport.Accept(IExporter exporter) => exporter.Visit(this);
    }
}
