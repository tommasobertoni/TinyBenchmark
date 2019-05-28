using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TinyBenchmark.Tests
{
    internal static class Shared
    {
        public static readonly BenchmarkOutput SilentOutput = new BenchmarkOutput(OutputLevel.Silent);
    }
}
