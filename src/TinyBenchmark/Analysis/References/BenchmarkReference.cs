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

        public InitReference InitWithReference { get; }

        public IReadOnlyList<WarmupReference> WarmupCollection { get; }

        public IReadOnlyList<ArgumentsReference> ArgumentsCollection { get; }

        public MethodInfo Method { get; }

        public int Iterations { get; }

        public bool IsBaseline { get; }

        public BenchmarkReference(
            string name,
            InitReference initWithReference,
            IEnumerable<WarmupReference> warmupCollection,
            IEnumerable<ArgumentsReference> argumentsCollection,
            MethodInfo method,
            int iterations,
            bool isBaseline = false)
        {
            this.Name = name ?? throw new ArgumentNullException(nameof(name));
            this.InitWithReference = initWithReference;
            this.WarmupCollection = warmupCollection?.ToList().AsReadOnly();
            this.ArgumentsCollection = argumentsCollection?.ToList().AsReadOnly();
            this.Method = method ?? throw new ArgumentNullException(nameof(method));

            if (iterations < 1)
                throw new ArgumentOutOfRangeException($"{nameof(iterations)} must have a positive value.");

            this.Iterations = iterations;
            this.IsBaseline = isBaseline;
        }
    }
}
