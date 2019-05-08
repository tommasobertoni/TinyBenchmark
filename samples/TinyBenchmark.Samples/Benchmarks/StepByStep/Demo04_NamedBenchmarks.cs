using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TinyBenchmark.Attributes;

namespace TinyBenchmark.Samples.Demo04_NamedBenchmarks
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
        [Benchmark(Name = "String concatenation")]
        public void StringConcatenation()
        {
            string msg = string.Empty;
            for (int i = 0; i < 50_000; i++)
                msg += "test";
        }
    }
}
