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

        [Param(10_000_000)]
        public int ListSize { get; set; }

        [Init]
        public void Init()
        {
            Enumerable.Range(1, ListSize).ToList().ForEach(x => _dataSet.Add(x.ToString()));
            _dataSet.Insert(_dataSet.Count / 2, FindMe);
        }

        // First

        [Warmup(ForBenchmark = "Using Linq.First", Order = 1)]
        public void WarmupForLinqFirst() { }

        public void AnotherWarmupForLinqFirst() { }

        [WarmupWith(nameof(AnotherWarmupForLinqFirst), Order = 2)]
        [Benchmark(Name = "Using Linq.First", Iterations = 2)]
        public void First()
        {
            var foundItem = _dataSet.First(x => x == FindMe);
        }

        // Single

        [Warmup(ForBenchmark = "Using Linq.Single", Order = 2)]
        public void WarmupForLinqSingle() { }

        public void AnotherWarmupForLinqSingle() { }

        [WarmupWith(nameof(AnotherWarmupForLinqSingle), Order = 1)]
        [Benchmark(Name = "Using Linq.Single", Iterations = 2)]
        public void Single()
        {
            var foundItem = _dataSet.Single(x => x == FindMe);
        }
    }
}
