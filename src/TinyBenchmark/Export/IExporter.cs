using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TinyBenchmark
{
    public interface IExporter
    {
        void Visit(BenchmarksContainerReport report);

        void Visit(BenchmarkReport report);

        void Visit(IterationReport report);
    }
}
