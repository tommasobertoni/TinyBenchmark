using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TinyBenchmark.Analysis
{
    /// <summary>
    /// Identifies a set of property-value pair to be applied to a benchmarks container.
    /// </summary>
    internal class ParametersSet : IEnumerable<KeyValuePair<string, object>>, IEquatable<ParametersSet>
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

        private IEnumerable<KeyValuePair<string, object>> AsEnumerable()
        {
            foreach (var pair in _valuesMap)
                yield return new KeyValuePair<string, object>(pair.Key, pair.Value.value);
        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator() => this.AsEnumerable().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        public override string ToString()
        {
            var toStrings = this._valuesMap.Select(x => $"{x.Key}:{x.Value.value}");
            var toString = string.Join(", ", toStrings);
            return toString;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;

                foreach (var v in this._valuesMap)
                {
                    var vHash = 17;
                    vHash = vHash * 23 + v.Key?.GetHashCode() ?? 0;
                    vHash = vHash * 23 + v.Value.value?.GetHashCode() ?? 0;
                    hash = hash * 23 + vHash;
                }

                return hash;
            }
        }

        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;

            return Equals(obj as ParametersSet);
        }

        public bool Equals(ParametersSet other)
        {
            if (other is null) return false;

            if (this._valuesMap.Count != other._valuesMap.Count) return false;

            foreach (var key in this._valuesMap.Keys)
            {
                if (!other._valuesMap.ContainsKey(key)) return false;

                var (prop, value) = this._valuesMap[key];
                var (otherProp, otherValue) = other._valuesMap[key];

                if (value == null && otherValue == null) continue;
                if (value == null && otherValue != null) return false;
                if (value != null && otherValue == null) return false;
                if (!value.Equals(otherValue)) return false;
            }

            return true;
        }
    }
}
