using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TinyBenchmark.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class ParamAttribute : Attribute
    {
        private IEnumerable<object> _values;

        public ParamAttribute(params object[] values)
            : this(values.AsEnumerable())
        {
        }

        public ParamAttribute(IEnumerable<object> values)
        {
            _values = values;
        }
    }
}
