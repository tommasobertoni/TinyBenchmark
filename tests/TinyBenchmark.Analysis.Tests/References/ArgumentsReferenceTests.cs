using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using TinyBenchmark.Analysis;

namespace Analysis.References
{
    [TestFixture]
    public class ArgumentsReferenceTests
    {
        [Test]
        public void ArgumentsCanBeAddedWithDifferentVariableNames()
        {
            var reference = new ArgumentsReference();

            for (int i = 0; i < 10; i++)
                Assert.That(() => reference.Add($"variable{i}", i), Throws.Nothing);

            Assert.That(() => reference.Add("variable0", 42), Throws.Exception);
        }

        [Test]
        public void NullArgumentsCanBeAdded()
        {
            var reference = new ArgumentsReference
            {
                { "num", 42 },
                { "str", "foo" }
            };

            Assert.That(() => reference.Add("optional", null), Throws.Nothing);
        }

        [Test]
        public void VariableNameMustBeDefined()
        {
            var reference = new ArgumentsReference
            {
                { "num", 42 },
                { "str", "foo" }
            };

            Assert.That(() => reference.Add(null, 42), Throws.Exception);
            Assert.That(() => reference.Add(string.Empty, 42), Throws.Exception);
            Assert.That(() => reference.Add("   ", 42), Throws.Exception);
        }

        [Test]
        public void ReferenceCanBeTransformedIntoMethodParameters()
        {
            var reference = new ArgumentsReference();

            var emptyMethodParameters = reference.AsMethodParameters();

            Assert.That(emptyMethodParameters, Is.Not.Null);
            Assert.That(emptyMethodParameters, Is.Empty);

            reference.Add("num", 42);
            reference.Add("str", "foo");
            reference.Add("optional", null);

            var methodParameters = reference.AsMethodParameters();

            Assert.That(methodParameters, Is.Not.Null);
            Assert.That(methodParameters, Is.Not.Empty);
            Assert.That(methodParameters, Has.Length.EqualTo(3));
            Assert.That(methodParameters[0], Is.EqualTo(42));
            Assert.That(methodParameters[1], Is.EqualTo("foo"));
            Assert.That(methodParameters[2], Is.EqualTo(null));
        }

        [Test]
        public void ReferenceCanBeIterated()
        {
            var reference = new ArgumentsReference();

            // Empty IEnumerator

            {
                IEnumerable enumerableReference = reference;
                var enumerator = enumerableReference.GetEnumerator();
                Assert.That(enumerator, Is.Not.Null);
                Assert.That(enumerator.MoveNext(), Is.False);
            }

            // Empty IEnumerator<T>

            {
                IEnumerator<KeyValuePair<string, object>> enumerator = reference.GetEnumerator();
                Assert.That(enumerator, Is.Not.Null);
                Assert.That(enumerator.MoveNext(), Is.False);
            }

            // Add some arguments

            reference.Add("num", 42);
            reference.Add("str", "foo");
            reference.Add("optional", null);

            // Non empty IEnumerable

            {
                IEnumerable enumerableReference = reference;
                var enumerator = enumerableReference.GetEnumerator();

                int itemsCount = 0;

                while (enumerator.MoveNext())
                {
                    var pair = (KeyValuePair<string, object>)enumerator.Current;

                    switch (itemsCount)
                    {
                        case 0:
                            Assert.That(pair.Key, Is.EqualTo("num"));
                            Assert.That(pair.Value, Is.EqualTo(42));
                            break;

                        case 1:
                            Assert.That(pair.Key, Is.EqualTo("str"));
                            Assert.That(pair.Value, Is.EqualTo("foo"));
                            break;

                        case 2:
                            Assert.That(pair.Key, Is.EqualTo("optional"));
                            Assert.That(pair.Value, Is.EqualTo(null));
                            break;
                    }

                    itemsCount++;
                }

                Assert.That(itemsCount, Is.EqualTo(3));
            }

            // Non empty IEnumerable<T>

            var argumentsList = reference.ToList();
            Assert.That(argumentsList, Is.Not.Empty);
            Assert.That(argumentsList, Has.Count.EqualTo(3));
            Assert.That(argumentsList[0].Key, Is.EqualTo("num"));
            Assert.That(argumentsList[0].Value, Is.EqualTo(42));
            Assert.That(argumentsList[1].Key, Is.EqualTo("str"));
            Assert.That(argumentsList[1].Value, Is.EqualTo("foo"));
            Assert.That(argumentsList[2].Key, Is.EqualTo("optional"));
            Assert.That(argumentsList[2].Value, Is.EqualTo(null));
        }
    }
}
