using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TinyBenchmark.Analysis
{
    internal class BenchmarksPlanner
    {
        private readonly BenchmarkOutput _output;
        private readonly BenchmarksScanner _scanner;

        public BenchmarksPlanner(BenchmarkOutput output, BenchmarksScanner scanner)
        {
            _output = output;
            _scanner = scanner;
        }

        public ContainerExecutionPlan CreateExecutionPlan<TBenchmarksContainer>() => this.CreateExecutionPlan(typeof(TBenchmarksContainer));

        public ContainerExecutionPlan CreateExecutionPlan(Type benchmarksContainerType)
        {
            _scanner.Scan(benchmarksContainerType);

            var containerExecutionPlan = new ContainerExecutionPlan(_output, _scanner.Container);
            var container = containerExecutionPlan.Container;

            foreach (var benchmark in _scanner.Benchmarks)
            {
                if (container.ParametersSetCollection?.Any() != true)
                    AddPlansWithParameters(benchmark, null);
                else
                    foreach (var parametersSet in container.ParametersSetCollection)
                        AddPlansWithParameters(benchmark, parametersSet);
            }

            return containerExecutionPlan;

            // Local functions

            void AddPlansWithParameters(BenchmarkReference benchmark, ParametersSet parametersSet)
            {
                if (benchmark.ArgumentsCollection?.Any() != true)
                    AddPlanWithParametersAndArguments(benchmark, parametersSet, null);
                else
                    foreach (var arguments in benchmark.ArgumentsCollection)
                        AddPlanWithParametersAndArguments(benchmark, parametersSet, arguments);
            }

            void AddPlanWithParametersAndArguments(
                BenchmarkReference benchmark,
                ParametersSet parametersSet,
                ArgumentsReference arguments)
            {
                var benchmarkPlan = new BenchmarkPlan(
                    _output,
                    _scanner.Init,
                    benchmark.WarmupCollection,
                    parametersSet,
                    arguments,
                    benchmark,
                    benchmark.Iterations);

                containerExecutionPlan.Add(benchmarkPlan);
            }
        }
    }
}
