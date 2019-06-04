using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TinyBenchmark.Analysis;
using TinyBenchmark.Attributes;

namespace Analysis.ReferenceConstructors.Arguments
{
    [TestFixture]
    public class ArgumentsReferencesTests
    {
        [Test]
        public void MethodWithoutArgumentsIsAllowed()
        {
            var m = typeof(ContainerWithoutArguments).GetMethod(nameof(ContainerWithoutArguments.Method));
            var refConstructor = new ArgumentsReferenceConstructor();
            var argumentsRefs = refConstructor.TryCreateArgumentsReferences(m);
            Assert.That(argumentsRefs, Is.Null);
        }

        [Test]
        public void ReferenceIsCreated()
        {
            var refConstructor = new ArgumentsReferenceConstructor();
            var m = typeof(ContainerWithArguments).GetMethod(nameof(ContainerWithArguments.Method));
            var argumentsRef = refConstructor.CreateArgumentsReference(m, new ArgumentsAttribute(1000, "Mr."));
            Assert.That(argumentsRef, Is.Not.Null);
        }

        [Test]
        public void ReferencesAreCreated()
        {
            var refConstructor = new ArgumentsReferenceConstructor();
            var m = typeof(ContainerWithArguments).GetMethod(nameof(ContainerWithArguments.Method));
            var argumentsRefs = refConstructor.TryCreateArgumentsReferences(m);
            Assert.That(argumentsRefs, Is.Not.Null);
            Assert.That(argumentsRefs, Has.Count.EqualTo(4));

            var expectedArguments = new[]
            {
                (1, "Mr."),
                (10, null),
                (100, null),
                (1000, "Mr."),
            };

            for (int i = 0; i < expectedArguments.Length; i++)
            {
                var (count, prefix) = expectedArguments[i];
                var argumentsRef = argumentsRefs[i];
                var arguments = argumentsRef.ToList();
                Assert.That(arguments, Has.Count.EqualTo(2));
                Assert.That(arguments[0].Key, Is.EqualTo("count"));
                Assert.That(arguments[0].Value, Is.EqualTo(count));
                Assert.That(arguments[1].Key, Is.EqualTo("prefix"));
                Assert.That(arguments[1].Value, Is.EqualTo(prefix));
            }
        }

        [Test]
        public void ArgumentsMustBeOfCompatibleType()
        {
            var refConstructor = new ArgumentsReferenceConstructor();

            {
                var m = typeof(ContainerWithWrongArgumentTypes).GetMethod(nameof(ContainerWithWrongArgumentTypes.Method));
                Assert.That(() => refConstructor.TryCreateArgumentsReferences(m), Throws.Exception);
            }

            {
                var m = typeof(ContainerWithCompatibleArgumentTypes).GetMethod(nameof(ContainerWithCompatibleArgumentTypes.Method));
                Assert.That(() => refConstructor.TryCreateArgumentsReferences(m), Throws.Nothing);
                var argumentsRefs = refConstructor.TryCreateArgumentsReferences(m);
                Assert.That(argumentsRefs, Is.Not.Null);
                Assert.That(argumentsRefs, Has.Count.EqualTo(1));
                var arguments = argumentsRefs[0].ToList();
                Assert.That(arguments, Has.Count.EqualTo(2));
                Assert.That(arguments[0].Key, Is.EqualTo("id"));
                Assert.That(arguments[0].Value, Is.EqualTo(1000));
                Assert.That(arguments[1].Key, Is.EqualTo("item"));
                Assert.That(arguments[1].Value, Is.EqualTo("Mr."));
            }
        }

        [Test]
        public void ArgumentsCountMustBeAsExpected()
        {
            var refConstructor = new ArgumentsReferenceConstructor();
            var m = typeof(ContainerWithWrongArgumentsCount).GetMethod(nameof(ContainerWithWrongArgumentsCount.Method));
            Assert.That(() => refConstructor.TryCreateArgumentsReferences(m), Throws.Exception);
        }

        [Test]
        public void NullArgumentsAreAllowed()
        {
            var refConstructor = new ArgumentsReferenceConstructor();
            var m = typeof(ContainerWithNullArguments).GetMethod(nameof(ContainerWithNullArguments.Method));
            Assert.That(() => refConstructor.TryCreateArgumentsReferences(m), Throws.Nothing);
            var argumentsRefs = refConstructor.TryCreateArgumentsReferences(m);
            Assert.That(argumentsRefs, Is.Not.Null);
            Assert.That(argumentsRefs, Has.Count.EqualTo(2));

            var argumentsRef = argumentsRefs[0];
            var arguments = argumentsRef.ToList();
            Assert.That(arguments[0].Key, Is.EqualTo("count"));
            Assert.That(arguments[0].Value, Is.EqualTo(1));
            Assert.That(arguments[1].Key, Is.EqualTo("prefix"));
            Assert.That(arguments[1].Value, Is.EqualTo("Mr."));

            var nullArgumentsRef = argumentsRefs[1];
            var nullArguments = nullArgumentsRef.ToList();
            Assert.That(nullArguments[0].Key, Is.EqualTo("count"));
            Assert.That(nullArguments[0].Value, Is.EqualTo(null));
            Assert.That(nullArguments[1].Key, Is.EqualTo("prefix"));
            Assert.That(nullArguments[1].Value, Is.EqualTo(null));
        }
    }

    #region Test types

    internal class ContainerWithoutArguments
    {
        public void Method(int count, string prefix) { }
    }

    internal class ContainerWithArguments
    {
        [Arguments(1, "Mr.")]
        [Arguments(10, null)]
        [Arguments(100, null)]
        [Arguments(1000, "Mr.")]
        public void Method(int count, string prefix) { }
    }

    internal class ContainerWithWrongArgumentTypes
    {
        [Arguments(10, 100)]
        public void Method(int count, DateTime date) { }
    }

    internal class ContainerWithCompatibleArgumentTypes
    {
        [Arguments(1000, "Mr.")]
        public void Method(long id, object item) { }
    }

    internal class ContainerWithWrongArgumentsCount
    {
        [Arguments(10, "Mr.", null)]
        public void Method(int count, string prefix) { }
    }

    internal class ContainerWithNullArguments
    {
        [Arguments(1, "Mr.")]
        [Arguments(null, null)]
        public void Method(int? count, string prefix) { }
    }

    #endregion
}
