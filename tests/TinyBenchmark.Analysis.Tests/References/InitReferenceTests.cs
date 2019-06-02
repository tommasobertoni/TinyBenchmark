using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using TinyBenchmark.Analysis;

namespace Analysis.References
{
    [TestFixture]
    public class InitReferenceTests
    {
        [Test]
        public void InitMethodMustBeDefined()
        {
            Assert.That(() => new InitReference(
                typeof(InitReferenceTests).GetMethod(nameof(InitReferenceTests.InitMethodMustBeDefined))),
                Throws.Nothing);

            Assert.That(() => new InitReference(null), Throws.Exception);
        }
    }
}
