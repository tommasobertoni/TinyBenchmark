using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TinyBenchmark.Attributes;

namespace TinyBenchmark.Samples
{
    [BenchmarksContainer(Name = "Linq benchmarks")]
    public class LinqBenchmarks
    {
        private const string FindMe = "FindMe";
        private readonly List<string> _dataSet = new List<string>();
        private readonly IBenchmarkOutput _output;

        [Param(10_000_000)]
        public int N { get; set; }

        public LinqBenchmarks(IBenchmarkOutput output)
        {
            _output = output;
            Enumerable.Range(1, N).ToList().ForEach(x => _dataSet.Add(x.ToString()));
            _dataSet.Insert(_dataSet.Count / 2, FindMe);
        }

        // First

        [Warmup(ForBenchmark = "Using Linq.First", Order = 1)]
        public void WarmupForLinqFirst()
        {
            _output.WriteLine($"Invoked {nameof(WarmupForLinqFirst)}");
        }

        public void AnotherWarmupForLinqFirst()
        {
            _output.WriteLine($"Invoked {nameof(AnotherWarmupForLinqFirst)}");
        }

        [WarmupWith(nameof(AnotherWarmupForLinqFirst), Order = 2)]
        [Benchmark(Name = "Using Linq.First", Iterations = 5)]
        public void First()
        {
            _output.WriteLine($"Invoked {nameof(First)}");
            var foundItem = _dataSet.First(x => x == FindMe);
        }

        // Single

        [Warmup(ForBenchmark = "Using Linq.Single", Order = 2)]
        public void WarmupForLinqSingle()
        {
            _output.WriteLine($"Invoked {nameof(WarmupForLinqSingle)}");
        }

        public void AnotherWarmupForLinqSingle()
        {
            _output.WriteLine($"Invoked {nameof(AnotherWarmupForLinqSingle)}");
        }

        [WarmupWith(nameof(AnotherWarmupForLinqSingle), Order = 1)]
        [Benchmark(Name = "Using Linq.Single", Iterations = 5)]
        public void Single()
        {
            _output.WriteLine($"Invoked {nameof(Single)}");
            var foundItem = _dataSet.Single(x => x == FindMe);
        }
    }
}
