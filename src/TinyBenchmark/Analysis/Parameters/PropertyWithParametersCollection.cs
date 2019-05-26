using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using TinyBenchmark.Attributes;

namespace TinyBenchmark.Analysis
{
    /// <summary>
    /// Identifies a property decorated with with a <see cref="ParamAttribute"/>.
    /// It can return an enumerator of the param values defined in the attribute.
    /// </summary>
    internal class PropertyWithParametersCollection : IEnumerable<object>
    {
        public PropertyInfo Property { get; }

        public ParamAttribute Attribute { get; }

        public PropertyWithParametersCollection(PropertyInfo property, ParamAttribute attribute)
        {
            this.Property = property;
            this.Attribute = attribute;
        }

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        public IEnumerator<object> GetEnumerator() => this.Attribute.Values.GetEnumerator();
    }
}
