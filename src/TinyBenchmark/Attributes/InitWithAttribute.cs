using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TinyBenchmark.Attributes
{
    /// <summary>
    /// Identifies an initialization method to be executed before the benchmark.
    /// This attribute is meant to be applied to a benchmark method.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class InitWithAttribute : Attribute
    {
        /// <summary>
        /// An initialization method's name.
        /// </summary>
        public string MethodName { get; }

        /// <summary>
        /// Identifies an initialization method.
        /// </summary>
        /// <param name="methodName">The initialization method's name.</param>
        public InitWithAttribute(string methodName)
        {
            this.MethodName = methodName;
        }
    }
}
