using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TinyBenchmark
{
    public class Parameters : IEquatable<Parameters>
    {
        public IReadOnlyList<ParameterValue> Values { get; }

        public int Hash { get; }

        internal Parameters(IEnumerable<ParameterValue> values)
        {
            this.Values = values?.ToList().AsReadOnly();
            this.Hash = this.GetHashCode();
        }

        public override string ToString()
        {
            var toStrings = this.Values.Select(x => $"{x.PropertyName}:{x.Value}");
            var toString = string.Join(", ", toStrings);
            return toString;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;

                if (this.Values != null)
                {
                    foreach (var v in this.Values)
                    {
                        var vHash = 17;
                        vHash = vHash * 23 + v.PropertyName?.GetHashCode() ?? 0;
                        vHash = vHash * 23 + v.Value?.GetHashCode() ?? 0;
                        hash = hash * 23 + vHash;
                    }
                }

                return hash;
            }
        }

        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;

            return Equals(obj as Parameters);
        }

        public bool Equals(Parameters other)
        {
            if (other is null) return false;

            if (other.Values == null && this.Values == null) return true; // Both without values.
            if (other.Values == null || this.Values == null) return false; // One, but not both, is without values.

            if (other.Values.Count != this.Values.Count) return false; // Different values count.

            for (int i = 0; i < this.Values.Count; i++)
            {
                var value = this.Values[i];
                var otherValue = other.Values[i];

                if (value.PropertyName != otherValue.PropertyName) return false;
                if (value.Value == null && otherValue.Value != null) return false;
                if (value.Value != null && !value.Value.Equals(otherValue.Value)) return false;
            }

            return true;
        }

        public static bool operator ==(Parameters prms1, Parameters prms2)
        {
            if (prms1 is null && prms2 is null) return true;
            if (prms1 is null && !(prms2 is null)) return false;
            return prms1.Equals(prms2);
        }

        public static bool operator !=(Parameters prms1, Parameters prms2) => !(prms1 == prms2);
    }
}
