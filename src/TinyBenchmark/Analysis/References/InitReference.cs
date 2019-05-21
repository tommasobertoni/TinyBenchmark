using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace TinyBenchmark.Analysis
{
    internal class InitReference
    {
        public MethodInfo Method { get; }

        public InitReference(
            MethodInfo method)
        {
            this.Method = method;
        }
    }
}
