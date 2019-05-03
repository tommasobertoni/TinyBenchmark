using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TinyBenchmark
{
    internal class PreDefinedBenchmarkOutput : IBenchmarkOutput
    {
        private readonly OutputLevel _preDefinedOutputLevel;
        private readonly BenchmarkOutput _output;

        public PreDefinedBenchmarkOutput(OutputLevel preDefinedOutputLevel, BenchmarkOutput output)
        {
            _preDefinedOutputLevel = preDefinedOutputLevel;
            _output = output;
        }

        public void WriteLine(string message) => _output.WriteLine(_preDefinedOutputLevel, message);
    }
}
