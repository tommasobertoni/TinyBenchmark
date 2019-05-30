using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using TinyBenchmark.Analysis;
using TinyBenchmark.Attributes;

namespace Analysis.ReferenceConstructors.Container
{
    [TestFixture]
    public class ContainerReferenceTests
    {
        [Test]
        public void EmptyContainerIsAllowed()
        {
            var refConstructor = new ContainerReferenceConstructor(useConventions: true);
            var containerRef = refConstructor.CreateContainerReference(typeof(EmptyContainer));
            Assert.That(containerRef, Is.Not.Null);
            Assert.That(containerRef.ContainerType, Is.EqualTo(typeof(EmptyContainer)));
            Assert.That(containerRef.Name, Is.EqualTo(nameof(EmptyContainer)));
            Assert.That(containerRef.InitContainer, Is.Null);
            Assert.That(containerRef.ParametersSetCollection, Is.Empty);
        }

        [Test]
        public void ContainerReferencesAreFound()
        {
            var refConstructor = new ContainerReferenceConstructor(useConventions: true);
            var containerRef = refConstructor.CreateContainerReference(typeof(ContainerWithReferences));
            Assert.That(containerRef, Is.Not.Null);
            Assert.That(containerRef.ContainerType, Is.EqualTo(typeof(ContainerWithReferences)));
            Assert.That(containerRef.Name, Is.EqualTo(nameof(ContainerWithReferences)));
            Assert.That(containerRef.InitContainer, Is.Not.Null);
            Assert.That(containerRef.InitContainer.Method.Name, Is.EqualTo(nameof(ContainerWithReferences.InitContainer)));
            Assert.That(containerRef.ParametersSetCollection, Is.Not.Null);
            Assert.That(containerRef.ParametersSetCollection.ParametrizedPropertiesCount, Is.EqualTo(1));
        }

        [Test]
        public void NamedContainerReferencesAreFound()
        {
            var refConstructor = new ContainerReferenceConstructor(useConventions: true);
            var containerRef = refConstructor.CreateContainerReference(typeof(NamedContainerWithReferences));
            Assert.That(containerRef, Is.Not.Null);
            Assert.That(containerRef.ContainerType, Is.EqualTo(typeof(NamedContainerWithReferences)));
            Assert.That(containerRef.Name, Is.EqualTo("Check my name"));
            Assert.That(containerRef.InitContainer, Is.Not.Null);
            Assert.That(containerRef.InitContainer.Method.Name, Is.EqualTo(nameof(NamedContainerWithReferences.InitMe)));
            Assert.That(containerRef.ParametersSetCollection, Is.Not.Null);
            Assert.That(containerRef.ParametersSetCollection.ParametrizedPropertiesCount, Is.EqualTo(2));
        }
    }

    #region Test types

    internal class EmptyContainer { }

    internal class ContainerWithReferences
    {
        [Param(1)]
        public int Count { get; set; }

        public void InitContainer() { }
    }

    [BenchmarksContainer(Name = "Check my name")]
    internal class NamedContainerWithReferences
    {
        [Param(1)]
        public int Count { get; set; }

        [Param("Mr.")]
        public string Prefix { get; set; }

        [InitContainer]
        public void InitMe() { }
    }

    #endregion
}
