using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TinyBenchmark.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class ParamAttribute : Attribute
    {
        internal List<object> Values { get; }

        public ParamAttribute(object value)
            : this(new[] { value })
        {
        }

        public ParamAttribute(object value, params object[] otherValues)
            : this(new[] { value }.Concat(otherValues))
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
