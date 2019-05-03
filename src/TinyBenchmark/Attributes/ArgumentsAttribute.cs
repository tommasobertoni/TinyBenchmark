using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TinyBenchmark.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public class ArgumentsAttribute : Attribute
    {
        internal IReadOnlyList<object> Arguments { get; }

        public ArgumentsAttribute(object argument)
            : this(new[] { argument })
        {
        }

        public ArgumentsAttribute(object argument, params object[] otherArguments)
            : this(new[] { argument }.Concat(otherArguments))
        {
        }

        internal ArgumentsAttribute(IEnumerable<object> arguments)
        {
            if (arguments?.Any() != true)
                throw new ArgumentException($"There must be some arguments.");

            this.Arguments = arguments.ToList();
        }
    }
}
