using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TinyBenchmark.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public class WarmupAttribute : Attribute
    {
        public string ForBenchmark { get; set; }

        public int Order { get; set; }
    }
}
