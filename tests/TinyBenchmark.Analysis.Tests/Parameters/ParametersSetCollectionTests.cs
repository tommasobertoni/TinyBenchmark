using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TinyBenchmark.Analysis;

namespace Analysis.Parameters
{
    [TestFixture]
    public class ParametersSetCollectionTests
    {
        [Test]
        public void CollectionCanBeEmpty()
        {
            var collection = new ParametersSetCollection();
            Assert.That(collection.ParametrizedPropertiesCount, Is.EqualTo(0));

            var parametersSets = collection.ToList();
            Assert.That(parametersSets, Is.Not.Null);
            Assert.That(parametersSets, Is.Empty);
        }
    }
}
