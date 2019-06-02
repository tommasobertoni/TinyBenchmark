using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using TinyBenchmark.Analysis;

namespace Analysis.References
{
    [TestFixture]
    public class WarmupReferenceTests
    {
        [Test]
        public void WarmupReferenceMustHaveNameAndMethod()
        {
            var m = typeof(WarmupReferenceTests).GetMethod(nameof(WarmupReferenceTests.WarmupReferenceMustHaveNameAndMethod));
            Assert.That(() => new WarmupReference("Warmup", 0, m), Throws.Nothing);
            Assert.That(() => new WarmupReference("Warmup", 0, null), Throws.Exception);
            Assert.That(() => new WarmupReference(null, 0, m), Throws.Exception);
        }

        [Test]
        public void OrderCanBeNegative()
        {
            var m = typeof(WarmupReferenceTests).GetMethod(nameof(WarmupReferenceTests.WarmupReferenceMustHaveNameAndMethod));
            Assert.That(() => new WarmupReference("Warmup", 1, m), Throws.Nothing);
            Assert.That(() => new WarmupReference("Warmup", 0, m), Throws.Nothing);
            Assert.That(() => new WarmupReference("Warmup", -1, m), Throws.Nothing);
            Assert.That(() => new WarmupReference("Warmup", int.MinValue, m), Throws.Nothing);
        }
    }
}
