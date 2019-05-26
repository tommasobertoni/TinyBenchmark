using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TinyBenchmark.Attributes
{
    /// <summary>
    /// Defines the benchmark arguments when the method accepts input values.
    /// The type and the number of arguments must be coherent with what the method expects.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public class ArgumentsAttribute : Attribute
    {
        internal IReadOnlyList<object> Arguments { get; }

        /// <summary>
        /// Defines the collection of arguments.
        /// </summary>
        /// <param name="arguments">The method's arguments.</param>
        public ArgumentsAttribute(params object[] arguments)
            : this(arguments?.AsEnumerable())
        {
        }

        internal ArgumentsAttribute(IEnumerable<object> arguments)
        {
            if (arguments?.Any() != true)
                throw new ArgumentException($"There must be some arguments.");

            this.Arguments = arguments.ToList();
        }
    }
}
