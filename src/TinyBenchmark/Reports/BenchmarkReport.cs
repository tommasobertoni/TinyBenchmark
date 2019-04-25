using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TinyBenchmark
{
    public class BenchmarkReport
    {
        public DateTime StartedAtUtc { get; internal set; }

        public TimeSpan Warmup { get; internal set; }

        public TimeSpan Elapsed { get; internal set; }

        public string Name { get; internal set; }

        public bool Failed => this.Exception != null;

        public AggregateException Exception { get; set; }
    }
}
