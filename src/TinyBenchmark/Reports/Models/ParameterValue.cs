using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TinyBenchmark
{
    public class ParameterValue
    {
        public string PropertyName { get; }

        public object Value { get; }

        internal ParameterValue(string propertyName, object value)
        {
            this.PropertyName = propertyName;
            this.Value = value;
        }
    }
}
