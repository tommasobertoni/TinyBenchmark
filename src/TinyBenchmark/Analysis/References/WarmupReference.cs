using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace TinyBenchmark.Analysis
{
    internal class WarmupReference
    {
        public string Name { get; }

        public int Order { get; }

        public MethodInfo Executable { get; }

        public WarmupReference(
            string name,
            int order,
            MethodInfo executable)
        {
            this.Name = name;
            this.Order = order;
            this.Executable = executable;
        }
    }
}
