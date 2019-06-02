using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using TinyBenchmark.Analysis;

namespace Analysis.References
{
    [TestFixture]
    public class ContainerReferenceTests
    {
        [Test]
        public void ContainerReferenceMustHaveTypeAndName()
        {
            Assert.That(() => new ContainerReference(typeof(object), "Container", null, null), Throws.Nothing);
            Assert.That(() => new ContainerReference(typeof(object), null, null, null), Throws.Exception);
            Assert.That(() => new ContainerReference(null, "Container", null, null), Throws.Exception);
        }
    }
}
