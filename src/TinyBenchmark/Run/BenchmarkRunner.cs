using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using TinyBenchmark.Analysis;
using TinyBenchmark.Attributes;

namespace TinyBenchmark
{
    public class BenchmarkRunner
    {
        public OutputLevel MaxOutputLevel { get; }

        public BenchmarkRunner(OutputLevel? maxOutputLevel = null)
        {
            this.MaxOutputLevel = maxOutputLevel ?? OutputLevel.Normal;
        }

        public BenchmarksContainerReport Run<TBenchmarksContainer>() => this.Run(typeof(TBenchmarksContainer));

        public BenchmarksContainerReport Run(Type benchmarksContainerType)
        {
            var output = new BenchmarkOutput(this.MaxOutputLevel);

            var scanner = new BenchmarksScanner(new BenchmarkOutput(this.MaxOutputLevel));
            var planner = new BenchmarksPlanner(new BenchmarkOutput(this.MaxOutputLevel), scanner);

            var executionPlan = planner.CreateExecutionPlan(benchmarksContainerType);

            output.WriteLine(OutputLevel.Normal, string.Empty);

            var containerReport = executionPlan.Run();

            return containerReport;
        }
    }
}
