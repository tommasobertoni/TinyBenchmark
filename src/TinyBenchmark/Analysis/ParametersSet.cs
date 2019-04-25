using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TinyBenchmark.Analysis
{
    public class ParametersSet
    {
        public object this[PropertyInfo property]
        {
            set => this.Add(property, value);
        }

        private readonly Dictionary<string, (PropertyInfo property, object value)> _valuesMap =
            new Dictionary<string, (PropertyInfo property, object value)>();

        public void Add(PropertyInfo property, object value)
        {
            if (value != null && !property.PropertyType.IsAssignableFrom(value.GetType()))
                throw new ArgumentException($"Cannot assign a value of type {value.GetType().Name} to a property of type {property.PropertyType.Name}, property {property.Name}, value {value}");

            if (_valuesMap.ContainsKey(property.Name))
                throw new ArgumentException($"Cannot add another property value in this parameters set to the property {property.Name}");
            
            _valuesMap.Add(property.Name, (property, value));
        }

        public void ApplyTo<TBenchmarkContainer>(TBenchmarkContainer container)
        {
            foreach (var (property, value) in _valuesMap.Values)
                property.SetValue(container, value);
        }

        public List<ParameterValue> ToParameterValues()
        {
            return _valuesMap.Values.Select(x => new ParameterValue
            {
                PropertyName = x.property.Name,
                Value = x.value
            }).ToList();
        }

        public override string ToString()
        {
            var toStrings = _valuesMap.Values.Select(x => $"{x.property.Name}:{x.value}");
            var toString = string.Join(", ", toStrings);
            return toString;
        }
    }
}
