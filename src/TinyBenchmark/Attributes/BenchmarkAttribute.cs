using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TinyBenchmark.Attributes
{
    /// <summary>
    /// Identifies a benchmark method.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class BenchmarkAttribute : Attribute
    {
        /// <summary>
        /// The name of the benchmark that will be used in the reports: if not set, the name of the method will be used.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Defines how many iterations this benchmark will run, given the same parameters and arguments.
        /// This must be a positive value, and has a default of 1. Increasing the iterations may increase the accuracy of the results.
        /// </summary>
        public int Iterations { get; set; } = 1;

        /// <summary>
        /// Defines the order by which the benchmark should run against the other benchmarks contained in the same container.
        /// The lower the value, the earlier the benchmark will be invoked.
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        /// Defines if this benchmark will be used as a baseline for comparing the other benchmarks contained in the same container.
        /// Only one benchmark per container can be a baseline.
        /// </summary>
        public bool Baseline { get; set; }
    }
}
