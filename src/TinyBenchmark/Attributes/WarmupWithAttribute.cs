using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TinyBenchmark.Attributes
{
    /// <summary>
    /// Identifies a warm-up method to be executed before the benchmark.
    /// This attribute is meant to be applied to a benchmark method.
    /// The time the warm-up method takes to executed won't be mixed with
    /// the actual benchmark's time, and it will be stored in a dedicated variable.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public class WarmupWithAttribute : Attribute
    {
        /// <summary>
        /// The name of the warm-up method for this benchmark.
        /// </summary>
        public string MethodName { get; }

        /// <summary>
        /// Defines the order by which the wamu-up method should run against the other warm-up methods for this benchmark.
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        /// Identifies a warm-up method to be executed before the benchmark.
        /// </summary>
        /// <param name="methodName">The warm-up method's name.</param>
        public WarmupWithAttribute(string methodName)
        {
            this.MethodName = methodName;
        }
    }
}
