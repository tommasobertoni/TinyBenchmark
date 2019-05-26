using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TinyBenchmark
{
    /// <summary>
    /// An argument used for a benchmark.
    /// </summary>
    public class Argument
    {
        /// <summary>
        /// The method's variable name.
        /// </summary>
        public string VariableName { get; }

        /// <summary>
        /// The value of this argument.
        /// </summary>
        public object Value { get; }

        internal Argument(string variableName, object value)
        {
            this.VariableName = variableName;
            this.Value = value;
        }
    }
}
