using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TinyBenchmark.Attributes;

namespace TinyBenchmark.Samples
{
    public class MiscBenchmarks
    {
        private static int _iterationCount = 0;

        private readonly IBenchmarkOutput _output;

        public MiscBenchmarks(IBenchmarkOutput output)
        {
            _output = output;
        }

        [Benchmark(Iterations = 2)]
        [Arguments("test", 1)]
        [Arguments("another", 2)]
        public void WithArguments(string text, int n)
        {
            _output.WriteLine($"{text}-{n}");
        }

        [Benchmark(Iterations = 5)]
        public void OddFailingBenchmark()
        {
            _iterationCount++;

            if (_iterationCount % 2 == 1)
                throw new InvalidOperationException($"Iteration {_iterationCount} is odd.");

            System.Threading.Thread.Sleep(300);
        }

        public void InitAlwaysFailingBenchmark() => _output.WriteLine("Initializing method...");

        [Benchmark(Iterations = 3)]
        [InitWith(nameof(InitAlwaysFailingBenchmark))]
        public void AlwaysFailingBenchmark() => throw new Exception("Cannot run this benchmark");
    }
}
