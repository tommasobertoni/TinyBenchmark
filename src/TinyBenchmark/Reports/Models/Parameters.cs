using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TinyBenchmark
{
    /// <summary>
    /// Parameters used for a benchmark.
    /// </summary>
    public class Parameters : IEquatable<Parameters>
    {
        /// <summary>
        /// The parameters and their values.
        /// </summary>
        public IReadOnlyList<ParameterValue> Values { get; }

        /// <summary>
        /// The hash code of this parameters collection.
        /// </summary>
        public int Hash { get; }

        internal Parameters(IEnumerable<ParameterValue> values)
        {
            this.Values = values?.ToList().AsReadOnly();
            this.Hash = this.GetHashCode();
        }

        /// <summary>
        /// Returns a string that represents this parameters collection.
        /// </summary>
        /// <returns>A string that represents this parameters collection.</returns>
        public override string ToString()
        {
            var toStrings = this.Values.Select(x => $"{x.PropertyName}:{x.Value}");
            var toString = string.Join(", ", toStrings);
            return toString;
        }

        /// <summary>
        /// Calculates the hash code for this parameters collection.
        /// </summary>
        /// <returns>A hash code for this parameters collection.</returns>
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

        /// <summary>
        /// Determines whether the specified object is equal to this parameters collection.
        /// </summary>
        /// <param name="obj">The object to compare with this parameters collection.</param>
        /// <returns>true if the specified object is equal to this parameters collection; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;

            return (this as IEquatable<Parameters>).Equals(obj as Parameters);
        }

        bool IEquatable<Parameters>.Equals(Parameters other)
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

        /// <summary>
        /// Determines whether the first parameters collection is equal to the second one.
        /// </summary>
        /// <param name="prms1">The first parameters collection.</param>
        /// <param name="prms2">The second parameters collection.</param>
        /// <returns>true when the two parameters collections are equal; otherwise, false.</returns>
        public static bool operator ==(Parameters prms1, Parameters prms2)
        {
            if (prms1 is null && prms2 is null) return true;
            if (prms1 is null && !(prms2 is null)) return false;
            return prms1.Equals(prms2);
        }

        /// <summary>
        /// Determines whether the first parameters collection is not equal to the second one.
        /// </summary>
        /// <param name="prms1">The first parameters collection.</param>
        /// <param name="prms2">The second parameters collection.</param>
        /// <returns>true when the two parameters collections are different; otherwise, false.</returns>
        public static bool operator !=(Parameters prms1, Parameters prms2) => !(prms1 == prms2);
    }
}
