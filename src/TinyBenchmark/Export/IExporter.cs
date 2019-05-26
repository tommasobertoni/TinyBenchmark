using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TinyBenchmark
{
    /// <summary>
    /// Defines a report exporter.
    /// </summary>
    public interface IExporter
    {
        /// <summary>
        /// Visits a benchmarks container report to create an export.
        /// </summary>
        /// <param name="report">The benchmarks container report.</param>
        void Visit(BenchmarksContainerReport report);

        /// <summary>
        /// Visits a benchmark report to create an export.
        /// </summary>
        /// <param name="report">The benchmark report.</param>
        void Visit(BenchmarkReport report);

        /// <summary>
        /// Visits a benchmark iteration report to create an export.
        /// </summary>
        /// <param name="report">The benchmark iteration report.</param>
        void Visit(IterationReport report);
    }
}
