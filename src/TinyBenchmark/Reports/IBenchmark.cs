using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TinyBenchmark
{
    public interface IBenchmark
    {
        void Accept(IExporter exporter);
    }
}
