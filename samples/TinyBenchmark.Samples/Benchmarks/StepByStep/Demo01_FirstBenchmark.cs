using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TinyBenchmark.Attributes;

namespace TinyBenchmark.Samples.Demo01_FirstBenchmark
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
        [Benchmark]
        public void TestMe()
        {
            string token = "test";
            string msg = string.Empty;
            for (int i = 0; i < 10_000; i++)
                msg += token;
        }
    }
}
