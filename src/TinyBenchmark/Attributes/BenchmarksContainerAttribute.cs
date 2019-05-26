using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TinyBenchmark.Attributes
{
    /// <summary>
    /// Defines additional information about the class that contains the benchmarks.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class BenchmarksContainerAttribute : Attribute
    {
        /// <summary>
        /// The name of the benchmarks container that will be used in the reports: if not set, the class name will be used.
        /// </summary>
        public string Name { get; set; }
    }
}
