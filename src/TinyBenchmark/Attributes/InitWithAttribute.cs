using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TinyBenchmark.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class InitWithAttribute : Attribute
    {
        public string MethodName { get; }

        public InitWithAttribute(string methodName)
        {
            this.MethodName = methodName;
        }
    }
}
