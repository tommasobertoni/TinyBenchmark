using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace TinyBenchmark.Analysis
{
    internal class BenchmarkReference
    {
        public string Name { get; internal set; }

        public MethodInfo Init { get; set; }

        public MethodInfo Executable { get; internal set; }

        public List<BenchmarkArguments> ArgumentsCollection { get; set; }

        public int Iterations { get; internal set; }

        public List<MethodInfo> Warmups { get; set; }

        public ParametersSetCollection ParametersSetCollection { get; set; }
    }
}
