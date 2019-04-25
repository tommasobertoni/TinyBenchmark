using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TinyBenchmark
{
    public class IterationReport
    {
        public DateTime StartedAtUtc { get; internal set; }

        public TimeSpan Warmup { get; internal set; }

        public TimeSpan Elapsed { get; internal set; }

        public bool Failed => this.Exception != null;

        public Exception Exception { get; set; }
    }
}
