using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TinyBenchmark.Attributes;

namespace TinyBenchmark.Samples.Demo02_BenchmarkArguments
{
    class Program
    {
        public static void Main(string[] args)
        {
            var runner = new BenchmarkRunner();
            var report = runner.Run<BenchmarksContainer>();

            // Explore the data in the report!
            Console.WriteLine($"Total duration: {report.Duration}");
        }
    }

    class BenchmarksContainer
    {
        /// <summary>
        /// This benchmark will be executed once for each [Arguments],
        /// with the "times" variable assigned to the value specified the attribute.
        /// </summary>
        [Benchmark]
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
