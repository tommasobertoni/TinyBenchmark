using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TinyBenchmark.Attributes;

namespace TinyBenchmark.Samples.Demo06_BenchmarksOutput
{
    class Program
    {
        public static void Main(string[] args)
        {
            var runner = new BenchmarkRunner(maxOutputLevel: OutputLevel.Verbose);
            var report = runner.Run<BenchmarksContainer>();

            var benchmarkReport = report.Reports.First();
            Console.WriteLine($"Benchmark: {benchmarkReport.Name}");
            Console.WriteLine($"  average iteration duration: {benchmarkReport.AvgIterationDuration}");
        }
    }

    class BenchmarksContainer
    {
        private readonly IBenchmarkOutput _output;

        public BenchmarksContainer(IBenchmarkOutput output)
        {
            _output = output;
        }

        [Benchmark(Name = "String concatenation")]
        public void StringConcatenation()
        {
            int times = 50_000;
            string token = "test";

            _output.WriteLine($"Concatenating {times} times the string \"{token}\"");

            string msg = string.Empty;
            for (int i = 0; i < times; i++)
                msg += token;

            _output.WriteLine("Terminated");
        }
    }
}
