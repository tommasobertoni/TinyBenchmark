using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using TinyBenchmark.Attributes;

namespace TinyBenchmark.Samples
{
    [BenchmarksContainer(Name = "Collections increase capacity benchmarks")]
    public class CollectionsBenchmarks
    {
        [Param(10_000, 1_000_000, 10_000_000)]
        public int ItemsCount { get; set; }

        // List

        private List<int> _list;

        public void ListWarmup() => _list = new List<int>();

        [WarmupWith(nameof(ListWarmup))]
        [Benchmark(Order = 1, Baseline = true)]
        public void List()
        {
            for (int i = 0; i < this.ItemsCount; i++)
                _list.Add(i);
        }

        // LinkedList

        private LinkedList<int> _linked;

        public void LinkedListWarmup() => _linked = new LinkedList<int>();

        [WarmupWith(nameof(LinkedListWarmup))]
        [Benchmark(Order = 5, Iterations = 2)]
        public void LinkedList()
        {
            for (int i = 0; i < this.ItemsCount; i++)
                _linked.AddLast(i);
        }

        // Queue

        private Queue<int> _queue;

        public void QueueWarmup() => _queue = new Queue<int>();

        [WarmupWith(nameof(QueueWarmup))]
        [Benchmark(Order = 4)]
        public void Queue()
        {
            for (int i = 0; i < this.ItemsCount; i++)
                _queue.Enqueue(i);
        }

        // Stack

        private Stack<int> _stack;

        public void StackWarmup() => _stack = new Stack<int>();

        [WarmupWith(nameof(StackWarmup))]
        [Benchmark(Order = 3, Iterations = 3)]
        public void Stack()
        {
            for (int i = 0; i < this.ItemsCount; i++)
                _stack.Push(i);
        }

        // HashSet

        private HashSet<int> _hashSet;
        private readonly IBenchmarkOutput _output;

        public void HashSetWarmup() => _hashSet = new HashSet<int>();

        [Benchmark(Order = 2, Iterations = 2)]
        [WarmupWith(nameof(HashSetWarmup))]
        public void HashSet()
        {
            for (int i = 0; i < this.ItemsCount; i++)
                _hashSet.Add(i);
        }
    }
}
