using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace TinyBenchmark.Analysis
{
    internal class InitReference
    {
        public MethodInfo Executable { get; }

        public InitReference(
            MethodInfo executable)
        {
            this.Executable = executable;
        }
    }
}
