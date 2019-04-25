using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TinyBenchmark
{
    public class BenchmarksCollectionReport
    {
        public DateTime StartedAtUtc { get; internal set; }

        public TimeSpan Elapsed { get; internal set; }

        public List<BenchmarksContainerReport> ContainerReports { get; internal set; }
    }
}
