using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TinyBenchmark
{
    public class Argument
    {
        public string VariableName { get; }

        public object Value { get; }

        internal Argument(string variableName, object value)
        {
            this.VariableName = variableName;
            this.Value = value;
        }
    }
}
