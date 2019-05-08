using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TinyBenchmark.Attributes;

namespace TinyBenchmark.Samples.Demo03_MultipleIterations
{
    class Program
    {
        public static void Main(string[] args)
        {
            var runner = new BenchmarkRunner();
            var report = runner.Run<BenchmarksContainer>();

            var benchmarkReport = report.Reports.First();
            Console.WriteLine($"Benchmark: {benchmarkReport.Name}");
            Console.WriteLine($"  average iteration duration: {benchmarkReport.AvgIterationDuration}");
        }
    }

    class BenchmarksContainer
    {
        /// <summary>
        /// This benchmark will be executed once for each [Arguments]
        /// and once for each iteration, for a total of 6 runs.
        /// </summary>
        [Benchmark(Iterations = 3)]
        [Arguments(10_000)]
        [Arguments(100_000)]
        public void TestMe(int times)
        {
            string token = "test";
            string msg = string.Empty;
            for (int i = 0; i < times; i++)
                msg += token;
        }
    }
}
