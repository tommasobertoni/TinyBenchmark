using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TinyBenchmark.Analysis;
using TinyBenchmark.Attributes;

namespace Analysis.Parameters
{
    [TestFixture]
    public class PropertyWithParametersCollectionTests
    {
        [Test]
        public void CollectionCanBeEmpy()
        {
            var type = typeof(EmptyContainer);
            var prop = type.GetProperty(nameof(EmptyContainer.Count));
            var attr = prop.GetCustomAttributes(typeof(ParamAttribute), false).FirstOrDefault();
            Assert.That(prop, Is.Not.Null);
            Assert.That(attr, Is.Null);
        }

        [Test]
        public void CollectionCanBeEnumerated()
        {
            var type = typeof(Container);
            var prop = type.GetProperty(nameof(Container.Count));
            var attr = prop.GetCustomAttributes(typeof(ParamAttribute), false).FirstOrDefault() as ParamAttribute;
            Assert.That(prop, Is.Not.Null);
            Assert.That(attr, Is.Not.Null);
            CollectionAssert.AreEqual(attr.Values, new[] { 1, 10, 100, 1000 });
            var propWithParamCollection = new PropertyWithParametersCollection(prop, attr);
            var values = propWithParamCollection.ToList();
            Assert.That(values, Is.Not.Null);
            CollectionAssert.AreEqual(values, new[] { 1, 10, 100, 1000 });
        }

        [Test]
        public void ConstructorArguments()
        {
            var type = typeof(Container);
            var prop = type.GetProperty(nameof(Container.Count));
            var attr = prop.GetCustomAttributes(typeof(ParamAttribute), false).FirstOrDefault() as ParamAttribute;
            Assert.That(() => new PropertyWithParametersCollection(prop, attr), Throws.Nothing);
            Assert.That(() => new PropertyWithParametersCollection(prop, null), Throws.Exception);
            Assert.That(() => new PropertyWithParametersCollection(null, attr), Throws.Exception);
            Assert.That(() => new PropertyWithParametersCollection(null, null), Throws.Exception);
        }
    }

    internal class EmptyContainer
    {
        public int Count { get; set; }
    }

    internal class Container
    {
        [Param(1, 10, 100, 1000)]
        public int Count { get; set; }
    }
}
