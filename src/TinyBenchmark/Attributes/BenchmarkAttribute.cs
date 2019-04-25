using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TinyBenchmark.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class BenchmarkAttribute : Attribute
    {
        public string Name { get; set; }

        public int Iterations { get; set; }

        public int Order { get; set; }
    }
}
