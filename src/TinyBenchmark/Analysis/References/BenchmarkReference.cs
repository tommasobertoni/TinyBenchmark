using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace TinyBenchmark.Analysis
{
    internal class BenchmarkReference
    {
        public string Name { get; }

        public IReadOnlyList<WarmupReference> WarmupCollection { get; }

        public IReadOnlyList<ArgumentsReference> ArgumentsCollection { get; }

        public MethodInfo Executable { get; }

        public int Iterations { get; }

        public bool IsBaseline { get; }

        public BenchmarkReference(
            string name,
            IEnumerable<WarmupReference> warmupCollection,
            IEnumerable<ArgumentsReference> argumentsCollection,
            MethodInfo executable,
            int iterations,
            bool isBaseline = false)
        {
            this.Name = name;
            this.WarmupCollection = warmupCollection?.ToList().AsReadOnly();
            this.ArgumentsCollection = argumentsCollection?.ToList().AsReadOnly();
            this.Executable = executable;
            this.Iterations = iterations;
            this.IsBaseline = isBaseline;
        }
    }
}
