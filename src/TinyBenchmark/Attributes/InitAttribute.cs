using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TinyBenchmark.Attributes
{
    /// <summary>
    /// Identifies an initialization method to be executed before each benchmark in the container.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class InitAttribute : Attribute
    {
    }
}
