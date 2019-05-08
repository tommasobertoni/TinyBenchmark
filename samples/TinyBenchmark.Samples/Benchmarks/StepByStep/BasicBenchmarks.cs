using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyBenchmark;
using TinyBenchmark.Attributes;

namespace StepByStepSamples.BasicBenchmarks
{
    class Program
    {
        public static void Main()
        {
            var runner = new BenchmarkRunner();
            var report = runner.Run<BasicBenchmarks>();

            var stringConcatenationBenchmarkReport = report.Reports[0];
            Console.WriteLine($"Benchmark: {stringConcatenationBenchmarkReport.Name}");
            Console.WriteLine($"  average iteration duration: {stringConcatenationBenchmarkReport.AvgIterationDuration}");

            var stringBuilderBenchmarkReport = report.Reports[1];
            Console.WriteLine($"Benchmark: {stringBuilderBenchmarkReport.Name}");
            Console.WriteLine($"  average iteration duration: {stringBuilderBenchmarkReport.AvgIterationDuration}");

            var efficiency = Math.Round(stringConcatenationBenchmarkReport.AvgIterationDuration / stringBuilderBenchmarkReport.AvgIterationDuration, 1);
            Console.WriteLine($"{stringBuilderBenchmarkReport.Name} is {efficiency} times faster than {stringConcatenationBenchmarkReport.Name}!");
        }
    }

    public class BasicBenchmarks
    {
        private readonly string _token = "test";
        private readonly int _tokensCount = 50_000;

        [Benchmark(Name = "String concatenation")]
        public void StringConcatenation()
        {
            string msg = string.Empty;
            for (int i = 0; i < _tokensCount; i++)
                msg += _token;
        }

        [Benchmark(Name = "Concatenation using StringBuilder")]
        public void StringBuilder()
        {
            var sb = new StringBuilder();
            for (int i = 0; i < _tokensCount; i++)
                sb.Append(_token);

            var msg = sb.ToString();
        }
    }
}
