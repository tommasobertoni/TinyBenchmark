using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using TinyBenchmark.Analysis;
using TinyBenchmark.Attributes;

namespace Analysis.ReferenceConstructors.Init
{
    [TestFixture]
    public class InitReferenceTests
    {
        [Test]
        public void ContainerWithoutInitIsAllowed()
        {
            var refConstructor = new InitReferenceConstructor(useConventions: true);
            var initReference = refConstructor.TryCreateInitReference(typeof(EmptyContainer));
            Assert.That(initReference, Is.Null);
        }

        [Test]
        public void InitReferenceIsFound()
        {
            var refConstructor = new InitReferenceConstructor(useConventions: true);
            var initReference = refConstructor.TryCreateInitReference(typeof(ContainerWithInit));
            Assert.That(initReference, Is.Not.Null);
            Assert.That(initReference.Method, Is.Not.Null);
            Assert.That(initReference.Method.Name, Is.EqualTo(nameof(ContainerWithInit.Setup)));
        }

        [Test]
        public void InitReferenceIsFoundUsingConventions()
        {
            var refConstructor = new InitReferenceConstructor(useConventions: true);
            var initReference = refConstructor.TryCreateInitReference(typeof(ContainerWithInitConvention));
            Assert.That(initReference, Is.Not.Null);
            Assert.That(initReference.Method, Is.Not.Null);
            Assert.That(initReference.Method.Name, Is.EqualTo(nameof(ContainerWithInitConvention.Init)));
        }

        [Test]
        public void InitAttributeHasPriority()
        {
            var refConstructor = new InitReferenceConstructor(useConventions: true);
            var initReference = refConstructor.TryCreateInitReference(typeof(ContainerWithInitAndConvention));
            Assert.That(initReference, Is.Not.Null);
            Assert.That(initReference.Method, Is.Not.Null);
            Assert.That(initReference.Method.Name, Is.EqualTo(nameof(ContainerWithInitAndConvention.Setup)));
        }

        [Test]
        public void MultipleInitsAreNotAllowed()
        {
            var refConstructor = new InitReferenceConstructor(useConventions: true);
            Assert.That(() => refConstructor.TryCreateInitReference(typeof(ContainerWithMultipleInits)), Throws.Exception);
        }

        [Test]
        public void ConventionsAreNotFoundBySetup()
        {
            var refConstructor = new InitReferenceConstructor(useConventions: false);
            var initReference = refConstructor.TryCreateInitReference(typeof(ContainerWithInitConvention));
            Assert.That(initReference, Is.Null);
        }
    }

    #region Test types

    internal class EmptyContainer { }

    internal class ContainerWithInit
    {
        [Init]
        public void Setup() { }
    }

    internal class ContainerWithInitConvention
    {
        public void Init() { }
    }

    internal class ContainerWithInitAndConvention
    {
        [Init]
        public void Setup() { }

        public void Init() { }
    }

    internal class ContainerWithMultipleInits
    {
        [Init]
        public void Setup() { }

        [Init]
        public void Startup() { }
    }

    #endregion
}
