using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TinyBenchmark.Analysis;

namespace Analysis.Parameters
{
    [TestFixture]
    public class ParametersSetTests
    {
        [Test]
        public void ParametersForDifferentPropertiesCanBeAdded()
        {
            var type = typeof(ContainerWithProperties);
            var countProp = type.GetProperty(nameof(ContainerWithProperties.Count));
            var prefixProp = type.GetProperty(nameof(ContainerWithProperties.Prefix));
            var itemProp = type.GetProperty(nameof(ContainerWithProperties.Item));

            var pset = new ParametersSet();

            Assert.That(() => pset.Add(countProp, 1), Throws.Nothing);
            Assert.That(() => pset.Add(prefixProp, "Mr."), Throws.Nothing);
            Assert.That(() => pset.Add(itemProp, new object()), Throws.Nothing);

            Assert.That(() => pset.Add(countProp, 10), Throws.Exception);
        }

        [Test]
        public void NullParametersCanBeAdded()
        {
            var type = typeof(ContainerWithProperties);
            var prefixProp = type.GetProperty(nameof(ContainerWithProperties.Prefix));
            var itemProp = type.GetProperty(nameof(ContainerWithProperties.Item));

            var pset = new ParametersSet();

            Assert.That(() => pset.Add(prefixProp, null), Throws.Nothing);
            Assert.That(() => pset.Add(itemProp, null), Throws.Nothing);
        }

        [Test]
        public void PropertyMustBeDefined()
        {
            var pset = new ParametersSet();
            Assert.That(() => pset.Add(null, 1), Throws.Exception);
        }

        [Test]
        public void ValueMustBeCompatibleWithPropertyType()
        {
            var type = typeof(ContainerWithProperties);
            var countProp = type.GetProperty(nameof(ContainerWithProperties.Count));
            var prefixProp = type.GetProperty(nameof(ContainerWithProperties.Prefix));
            var itemProp = type.GetProperty(nameof(ContainerWithProperties.Item));

            {
                var pset = new ParametersSet();
                Assert.That(() => pset.Add(countProp, 1), Throws.Nothing);
                Assert.That(() => pset.Add(prefixProp, "Mr."), Throws.Nothing);
                Assert.That(() => pset.Add(itemProp, new object()), Throws.Nothing);
            }

            {
                var pset = new ParametersSet();
                Assert.That(() => pset.Add(prefixProp, string.Empty), Throws.Nothing);
                Assert.That(() => pset.Add(itemProp, null), Throws.Nothing);
            }

            {
                var pset = new ParametersSet();
                Assert.That(() => pset.Add(prefixProp, null), Throws.Nothing);
                Assert.That(() => pset.Add(itemProp, "Mr."), Throws.Nothing);
            }

            {
                var pset = new ParametersSet();
                Assert.That(() => pset.Add(countProp, "1"), Throws.Exception);
                Assert.That(() => pset.Add(countProp, null), Throws.Exception);
            }
        }

        [Test]
        public void MultipleValuesOnAPropertyAreNotAllowed()
        {
            var type = typeof(ContainerWithProperties);
            var countProp = type.GetProperty(nameof(ContainerWithProperties.Count));
            var prefixProp = type.GetProperty(nameof(ContainerWithProperties.Prefix));
            var itemProp = type.GetProperty(nameof(ContainerWithProperties.Item));

            var pset = new ParametersSet
            {
                { countProp, 1 },
                { prefixProp, "Mr." },
            };

            Assert.That(() => pset.Add(countProp, 10), Throws.Exception);
            Assert.That(() => pset[countProp] = 100, Throws.Exception);
            Assert.That(() => pset.Add(prefixProp, null), Throws.Exception);
            Assert.That(() => pset[prefixProp] = "Mr.", Throws.Exception);

            Assert.That(() => pset.Add(itemProp, new object()), Throws.Nothing);
            Assert.That(() => pset.Add(itemProp, null), Throws.Exception);
            Assert.That(() => pset.Add(itemProp, new object()), Throws.Exception);
            Assert.That(() => pset[itemProp] = new object(), Throws.Exception);
        }

        [Test]
        public void ParametersSetsEquality()
        {
            var type = typeof(ContainerWithProperties);
            var countProp = type.GetProperty(nameof(ContainerWithProperties.Count));
            var prefixProp = type.GetProperty(nameof(ContainerWithProperties.Prefix));
            var itemProp = type.GetProperty(nameof(ContainerWithProperties.Item));

            var aSet = new ParametersSet();
            var anotherSet = new ParametersSet();

            Assert.That(aSet, Is.EqualTo(anotherSet));
            Assert.That(aSet.GetHashCode(), Is.EqualTo(anotherSet.GetHashCode()));

            aSet[countProp] = 1;
            Assert.That(aSet, Is.Not.EqualTo(anotherSet));
            Assert.That(aSet.GetHashCode(), Is.Not.EqualTo(anotherSet.GetHashCode()));

            anotherSet[countProp] = 1;
            Assert.That(aSet, Is.EqualTo(anotherSet));
            Assert.That(aSet.GetHashCode(), Is.EqualTo(anotherSet.GetHashCode()));

            aSet[prefixProp] = null;
            Assert.That(aSet, Is.Not.EqualTo(anotherSet));
            Assert.That(aSet.GetHashCode(), Is.Not.EqualTo(anotherSet.GetHashCode()));

            anotherSet[prefixProp] = null;
            Assert.That(aSet, Is.EqualTo(anotherSet));
            Assert.That(aSet.GetHashCode(), Is.EqualTo(anotherSet.GetHashCode()));

            aSet[itemProp] = new object();
            Assert.That(aSet, Is.Not.EqualTo(anotherSet));
            Assert.That(aSet.GetHashCode(), Is.Not.EqualTo(anotherSet.GetHashCode()));

            anotherSet[itemProp] = null;
            Assert.That(aSet, Is.Not.EqualTo(anotherSet));
            Assert.That(aSet.GetHashCode(), Is.Not.EqualTo(anotherSet.GetHashCode()));
        }

        [Test]
        public void ParametersSetsCanBeIterated()
        {
            var type = typeof(ContainerWithProperties);
            var countProp = type.GetProperty(nameof(ContainerWithProperties.Count));
            var prefixProp = type.GetProperty(nameof(ContainerWithProperties.Prefix));
            var itemProp = type.GetProperty(nameof(ContainerWithProperties.Item));

            var pset = new ParametersSet();

            {
                var parametersList = pset.ToList();
                Assert.That(parametersList, Is.Not.Null);
                Assert.That(parametersList, Is.Empty);
            }

            pset[countProp] = 1;
            pset[prefixProp] = "Mr.";
            pset[itemProp] = new DateTime(1990, 01, 01);

            {
                var parametersList = pset.ToList();
                Assert.That(parametersList, Is.Not.Null);
                Assert.That(parametersList, Has.Count.EqualTo(3));
                Assert.That(parametersList[0].Key, Is.EqualTo(nameof(ContainerWithProperties.Count)));
                Assert.That(parametersList[0].Value, Is.EqualTo(1));
                Assert.That(parametersList[1].Key, Is.EqualTo(nameof(ContainerWithProperties.Prefix)));
                Assert.That(parametersList[1].Value, Is.EqualTo("Mr."));
                Assert.That(parametersList[2].Key, Is.EqualTo(nameof(ContainerWithProperties.Item)));
                Assert.That(parametersList[2].Value, Is.EqualTo(new DateTime(1990, 01, 01)));
            }
        }

        [Test]
        public void ParametersSetCanBeAppliedToContainer()
        {

        }
    }

    #region Test types

    internal class ContainerWithProperties
    {
        public int Count { get; set; }

        public string Prefix { get; set; }

        public object Item { get; set; }
    }

    #endregion
}
