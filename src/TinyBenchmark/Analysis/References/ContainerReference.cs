using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace TinyBenchmark.Analysis
{
    internal class ContainerReference
    {
        public Type ContainerType { get; }

        public string Name { get; }

        public InitReference InitContainer { get; }

        public ParametersSetCollection ParametersSetCollection { get; }

        public ContainerReference(
            Type benchmarksContainerType,
            string name,
            InitReference initContainer,
            ParametersSetCollection parametersSetCollection)
        {
            this.ContainerType = benchmarksContainerType ?? throw new ArgumentNullException(nameof(benchmarksContainerType));
            this.Name = name ?? throw new ArgumentNullException(nameof(name));
            this.InitContainer = initContainer;
            this.ParametersSetCollection = parametersSetCollection;
        }
    }
}
