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

        [Param(1_000_000, 10_000_000, 20_000_000)]
        public int ListSize { get; set; }

        [Init]
        public void Init()
        {
            Enumerable.Range(1, ListSize).ToList().ForEach(x => _dataSet.Add(x.ToString()));
            _dataSet.Insert(_dataSet.Count / 2, FindMe);
        }

        // First

        public void WarmupForLinqFirst() { }
        public void AnotherWarmupForLinqFirst() { }

        [WarmupWith(nameof(WarmupForLinqFirst), Order = 1)]
        [WarmupWith(nameof(AnotherWarmupForLinqFirst), Order = 2)]
        [Benchmark(Name = "Using Linq.First", Iterations = 2)]
        public void First()
        {
            var foundItem = _dataSet.First(x => x == FindMe);
        }

        // Single

        public void WarmupForLinqSingle() { }
        public void AnotherWarmupForLinqSingle() { }

        [WarmupWith(nameof(AnotherWarmupForLinqSingle), Order = 1)]
        [WarmupWith(nameof(WarmupForLinqSingle), Order = 2)]
        [Benchmark(Name = "Using Linq.Single", Iterations = 2)]
        public void Single()
        {
            var foundItem = _dataSet.Single(x => x == FindMe);
        }
    }
}
