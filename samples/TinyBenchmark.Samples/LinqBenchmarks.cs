using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TinyBenchmark.Attributes;

namespace TinyBenchmark.Samples
{
    public class LinqBenchmarks
    {
        private const string FindMe = "FindMe";
        private readonly List<string> _dataSet = new List<string>();

        public LinqBenchmarks()
        {
            Enumerable.Range(1, 1_000_000).ToList().ForEach(x => _dataSet.Add(x.ToString()));
            _dataSet.Insert(_dataSet.Count / 2, FindMe);
        }

        [Benchmark(Name = "Using Linq.First", Iterations = 5)]
        public void First()
        {
            var foundItem = _dataSet.First(x => x == FindMe);
        }

        [Benchmark(Name = "Using Linq.Single", Iterations = 5)]
        public void Single()
        {
            var foundItem = _dataSet.Single(x => x == FindMe);
        }
    }
}
