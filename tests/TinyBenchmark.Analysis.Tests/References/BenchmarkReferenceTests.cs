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
            Assert.That(() => new BenchmarkReference("Benchmark", 0, null, null, null, m, 1), Throws.Nothing);
            Assert.That(() => new BenchmarkReference("Benchmark", 0, null, null, null, m, 1, true), Throws.Nothing);
            Assert.That(() => new BenchmarkReference("Benchmark", 0, null, null, null, m, 1, false), Throws.Nothing);
            Assert.That(() => new BenchmarkReference("Benchmark", 0, null, null, null, null, 1), Throws.Exception);
            Assert.That(() => new BenchmarkReference(null, 0, null, null, null, m, 1), Throws.Exception);
        }

        [Test]
        public void IterationsMustBePositive()
        {
            var m = typeof(BenchmarkReferenceTests).GetMethod(nameof(BenchmarkReferenceTests.BenchmarkReferenceMustHaveNameAndMethod));
            Assert.That(() => new BenchmarkReference("Benchmark", 0, null, null, null, m, 1), Throws.Nothing);
            Assert.That(() => new BenchmarkReference("Benchmark", 0, null, null, null, m, 0), Throws.Exception);
            Assert.That(() => new BenchmarkReference("Benchmark", 0, null, null, null, m, -1), Throws.Exception);
        }

        [Test]
        public void NoValidationOnOrder()
        {
            var m = typeof(BenchmarkReferenceTests).GetMethod(nameof(BenchmarkReferenceTests.BenchmarkReferenceMustHaveNameAndMethod));
            Assert.That(() => new BenchmarkReference("Benchmark", 0, null, null, null, m, 1), Throws.Nothing);
            Assert.That(() => new BenchmarkReference("Benchmark", 1, null, null, null, m, 1), Throws.Nothing);
            Assert.That(() => new BenchmarkReference("Benchmark", 99, null, null, null, m, 1), Throws.Nothing);
            Assert.That(() => new BenchmarkReference("Benchmark", int.MaxValue, null, null, null, m, 1), Throws.Nothing);
            Assert.That(() => new BenchmarkReference("Benchmark", -1, null, null, null, m, 1), Throws.Nothing);
            Assert.That(() => new BenchmarkReference("Benchmark", int.MinValue, null, null, null, m, 1), Throws.Nothing);
        }
    }
}
