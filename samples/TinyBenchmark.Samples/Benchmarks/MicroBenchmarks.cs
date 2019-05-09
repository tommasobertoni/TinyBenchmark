using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TinyBenchmark.Attributes;

namespace TinyBenchmark.Samples
{
    public class MicroBenchmarks
    {
        [Param(200_000)]
        public int ItemsCount { get; set; }

        public string TargetItem { get; set; } = "abcdefghijklmnopqrstuvwxyz";

        private readonly HashSet<string> _hash = new HashSet<string>();
        private readonly List<string> _list = new List<string>();

        public void FindInHashInit()
        {
            for (int i = 0; i < this.ItemsCount - 1; i++)
                _hash.Add(Guid.NewGuid().ToString());

            _hash.Add(this.TargetItem);
        }

        public void FindInHashWarmup() => FindInHash();

        [Benchmark(Iterations = 10000, Name = "HashSet.Contains")]
        [InitWith(nameof(FindInHashInit))]
        [WarmupWith(nameof(FindInHashWarmup))]
        public void FindInHash() => _hash.Contains(TargetItem);

        public void FindInListInit()
        {
            for (int i = 0; i < this.ItemsCount - 1; i++)
                _list.Add(Guid.NewGuid().ToString());

            _list.Add(this.TargetItem);
        }

        public void FindInListWarmup() => FindInList();

        [Benchmark(Iterations = 5000, Name = "List.Contains", Baseline = true)]
        [InitWith(nameof(FindInListInit))]
        [WarmupWith(nameof(FindInListWarmup))]
        public void FindInList() => _list.Contains(TargetItem);
    }
}
