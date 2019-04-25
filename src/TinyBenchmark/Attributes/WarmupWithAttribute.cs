using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TinyBenchmark.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public class WarmupWithAttribute : Attribute
    {
        public string MethodName { get; }

        public int Order { get; set; }

        public WarmupWithAttribute(string methodName)
        {
            this.MethodName = methodName;
        }
    }
}
