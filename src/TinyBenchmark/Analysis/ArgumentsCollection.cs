using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TinyBenchmark.Analysis
{
    internal class BenchmarkArguments
    {
        private readonly Dictionary<string, object> _arguments = new Dictionary<string, object>();

        public BenchmarkArguments()
        {
        }

        public void Add(string variableName, object value)
        {
            _arguments[variableName] = value;
        }

        public object[] AsMethodParameters() => _arguments?.Values?.ToArray();

        internal List<Argument> ToArgumentsModel()
        {
            return _arguments.Select(x => new Argument
            {
                VariableName = x.Key,
                Value = x.Value,
            }).ToList();
        }
    }
}
