using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TinyBenchmark
{
    internal interface IOutputBuffer
    {
        void AppendLine(string message);

        void AppendLine(OutputLevel outputLevel, string message);

        void Write();
    }
}
