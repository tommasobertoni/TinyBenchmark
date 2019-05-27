using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using TinyBenchmark.Analysis;
using TinyBenchmark.Attributes;
using TinyBenchmark.Run;

namespace TinyBenchmark
{
    /// <summary>
    /// Executes benchmarks and reports.
    /// </summary>
    public class BenchmarkRunner
    {
        /// <summary>
        /// The max output level for this runner.
        /// </summary>
        public OutputLevel MaxOutputLevel { get; }

        /// <summary>
        /// Creates a new instance, with an optional max output level.
        /// </summary>
        /// <param name="maxOutputLevel">The max output level for this runner; default is Normal.</param>
        public BenchmarkRunner(OutputLevel? maxOutputLevel = null)
        {
            this.MaxOutputLevel = maxOutputLevel ?? OutputLevel.Normal;
        }

        /// <summary>
        /// Runs a benchmarks container.
        /// </summary>
        /// <typeparam name="TBenchmarksContainer">The benchmarks container type.</typeparam>
        /// <returns>The benchmarks container report.</returns>
        public BenchmarksContainerReport Run<TBenchmarksContainer>() => this.Run(typeof(TBenchmarksContainer));

        /// <summary>
        /// Runs a benchmarks container container.
        /// </summary>
        /// <param name="benchmarksContainerType">The benchmarks container type.</param>
        /// <returns>The benchmarks container report.</returns>
        public BenchmarksContainerReport Run(Type benchmarksContainerType)
        {
            var scanner = new BenchmarksScanner(new BenchmarkOutput(this.MaxOutputLevel));
            var planner = new BenchmarksPlanner(new BenchmarkOutput(this.MaxOutputLevel), scanner);

            var executionPlan = planner.CreateExecutionPlan(benchmarksContainerType);

            var containerReport = executionPlan.Run();

            return containerReport;
        }
    }
}
