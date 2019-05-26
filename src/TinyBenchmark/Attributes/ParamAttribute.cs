using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TinyBenchmark.Attributes
{
    /// <summary>
    /// Defines all the possible values of a property that will be used by all the benchmarks.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class ParamAttribute : Attribute
    {
        internal IReadOnlyList<object> Values { get; }

        /// <summary>
        /// Defines all the possible values of a property.
        /// </summary>
        /// <param name="values">The property values that will be used by the benchmarks in the same container.</param>
        public ParamAttribute(params object[] values)
            : this(values?.AsEnumerable())
        {
        }

        internal ParamAttribute(IEnumerable<object> values)
        {
            if (values?.Any() != true)
                throw new ArgumentException($"There must be some values.");

            this.Values = values.ToList();
        }
    }
}
