using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using TinyBenchmark.Attributes;

namespace TinyBenchmark.Analysis
{
    /// <summary>
    /// Scans a type and extracts benchmarks information.
    /// </summary>
    internal class BenchmarksScanner
    {
        /// <summary>
        /// true when the scanner is allowed to create references using conventions, even when
        /// there are no attributes; otherwise, false.
        /// </summary>
        public bool UseConventions { get; }

        /// <summary>
        /// The benchmarks container reference.
        /// </summary>
        public ContainerReference Container { get; private set; }

        /// <summary>
        /// The benchmarks init reference, if available.
        /// </summary>
        public InitReference Init { get; private set; }

        /// <summary>
        /// The benchmark references, if any.
        /// </summary>
        public IReadOnlyList<BenchmarkReference> Benchmarks { get; private set; }

        private readonly BenchmarkOutput _output;

        public BenchmarksScanner(BenchmarkOutput output, bool useConventions = true)
        {
            _output = output;
            this.UseConventions = useConventions;
        }

        public void Scan<TBenchmarksContainer>() => this.Scan(typeof(TBenchmarksContainer));

        public void Scan(Type benchmarksContainerType)
        {
            _output.WriteLine(OutputLevel.Minimal, $"Scanning type {benchmarksContainerType.FullName}");
            _output.IndentLevel++;

            var containerRefCtor = new ContainerReferenceConstructor(this.UseConventions);
            this.Container = containerRefCtor.CreateContainerReference(benchmarksContainerType);
            _output.WriteLine(OutputLevel.Normal, $"Found container {this.Container.Name}");

            if (this.Container.InitContainer != null)
                _output.WriteLine(OutputLevel.Verbose, $"Found init container named {this.Container.InitContainer.Method.Name}");

            // Init

            var initRefCtor = new InitReferenceConstructor(this.UseConventions);
            this.Init = initRefCtor.TryCreateInitReference(benchmarksContainerType);

            if (this.Init != null)
                _output.WriteLine(OutputLevel.Verbose, $"Found init named {this.Init.Method.Name}");

            // Benchmarks

            var benchmarkRefCtor = new BenchmarkReferenceConstructor(_output, this.UseConventions);
            this.Benchmarks = benchmarkRefCtor.TryCreateBenchmarkReferences(benchmarksContainerType);

            if (this.Benchmarks == null)
            {
                _output.WriteLine(OutputLevel.Normal, $"No benchmarks found in container {benchmarksContainerType.Name}");
            }
            else if (_output.IsShown(OutputLevel.Normal))
            {
                var baselineBenchmark = this.Benchmarks.FirstOrDefault(br => br.IsBaseline);
                if (baselineBenchmark == null)
                    _output.WriteLine(OutputLevel.Normal, $"Found {this.Benchmarks.Count} benchmarks");
                else
                    _output.WriteLine(OutputLevel.Normal, $"Found {this.Benchmarks.Count} benchmarks, baseline: {baselineBenchmark.Name}");
            }

            _output.IndentLevel--;
        }
    }
}
