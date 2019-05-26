using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TinyBenchmark
{
    /// <summary>
    /// A property value used for a benchmark.
    /// </summary>
    public class ParameterValue
    {
        /// <summary>
        /// The container's property name.
        /// </summary>
        public string PropertyName { get; }

        /// <summary>
        /// The value of this parameter.
        /// </summary>
        public object Value { get; }

        internal ParameterValue(string propertyName, object value)
        {
            this.PropertyName = propertyName;
            this.Value = value;
        }
    }
}
