using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TinyBenchmark.Attributes;

namespace TinyBenchmark.Samples
{
    public class MiscBenchmarks
    {
        private readonly IBenchmarkOutput _output;
        private static int _iterationCount = 0;

        public MiscBenchmarks(IBenchmarkOutput output)
        {
            _output = output;
        }

        [Benchmark(Iterations = 5)]
        public void OddFailingBenchmark()
        {
            _iterationCount++;

            if (_iterationCount % 2 == 1)
                throw new InvalidOperationException($"Iteration {_iterationCount} is odd.");

            System.Threading.Thread.Sleep(300);
        }

        [Benchmark(Iterations = 3)]
        public void AlwaysFailingBenchmark() => throw new Exception("Cannot run this benchmark");
    }
}
