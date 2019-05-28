using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TinyBenchmark.Analysis
{
    internal class ArgumentsReference : IEnumerable<KeyValuePair<string, object>>
    {
        private readonly Dictionary<string, object> _arguments = new Dictionary<string, object>();

        public ArgumentsReference()
        {
        }

        public void Add(string variableName, object value)
        {
            if (string.IsNullOrWhiteSpace(variableName))
                throw new ArgumentNullException(nameof(variableName));

            if (_arguments.ContainsKey(variableName))
                throw new InvalidOperationException($"A value for the variable {variableName} already exists in this reference.");

            _arguments[variableName] = value;
        }

        public object[] AsMethodParameters() => _arguments.Values?.ToArray() ?? Array.Empty<object>();

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator() => _arguments.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    }
}
