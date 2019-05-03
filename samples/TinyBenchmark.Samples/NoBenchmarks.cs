using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TinyBenchmark.Attributes;

namespace TinyBenchmark.Samples
{
    [BenchmarksContainer(Name = "Container without benchmarks")]
    public class NoBenchmarks
    {
        private readonly IBenchmarkOutput _output;

        [Param(1, 2, 3)]
        public int P1 { get; set; }

        public NoBenchmarks(IBenchmarkOutput output)
        {
            _output = output;
        }

        [InitContainer]
        public void InitContainer()
        {
            _output.WriteLine($"I shouldn't be invoked!");
        }

        [Init]
        public void Init()
        {
            _output.WriteLine($"I shouldn't be invoked!");
        }

        public void AWarmup() { }

        [Arguments("abc")]
        [WarmupWith(nameof(AWarmup))]
        public void NotABenchmark(string text)
        {
            _output.WriteLine($"I shouldn't be running! {text}");
        }
    }
}
