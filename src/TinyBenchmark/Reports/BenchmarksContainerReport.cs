using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TinyBenchmark
{
    public class BenchmarksContainerReport
    {
        public string Name { get; set; }

        public Type BenchmarkContainerType { get; internal set; }

        public DateTime StartedAtUtc { get; internal set; }

        public TimeSpan Elapsed { get; internal set; }

        public List<BenchmarkReport> Reports { get; internal set; }
    }
}
