using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using TinyBenchmark.Analysis;

namespace Analysis.References
{
    [TestFixture]
    public class BenchmarkReferenceTests
    {
        [Test]
        public void BenchmarkReferenceMustHaveNameAndMethod()
        {
            var m = typeof(BenchmarkReferenceTests).GetMethod(nameof(BenchmarkReferenceTests.BenchmarkReferenceMustHaveNameAndMethod));
            Assert.That(() => new BenchmarkReference("Benchmark", null, null, null, m, 1), Throws.Nothing);
            Assert.That(() => new BenchmarkReference("Benchmark", null, null, null, m, 1, true), Throws.Nothing);
            Assert.That(() => new BenchmarkReference("Benchmark", null, null, null, m, 1, false), Throws.Nothing);
            Assert.That(() => new BenchmarkReference("Benchmark", null, null, null, null, 1), Throws.Exception);
            Assert.That(() => new BenchmarkReference(null, null, null, null, m, 1), Throws.Exception);
        }

        [Test]
        public void IterationsMustBePositive()
        {
            var m = typeof(BenchmarkReferenceTests).GetMethod(nameof(BenchmarkReferenceTests.BenchmarkReferenceMustHaveNameAndMethod));
            Assert.That(() => new BenchmarkReference("Benchmark", null, null, null, m, 1), Throws.Nothing);
            Assert.That(() => new BenchmarkReference("Benchmark", null, null, null, m, 0), Throws.Exception);
            Assert.That(() => new BenchmarkReference("Benchmark", null, null, null, m, -1), Throws.Exception);
        }
    }
}
