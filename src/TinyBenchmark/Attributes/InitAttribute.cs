using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TinyBenchmark.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class InitAttribute : Attribute
    {
    }
}
