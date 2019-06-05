using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using TinyBenchmark.Analysis;
using TinyBenchmark.Attributes;
using TinyBenchmark.Tests;

namespace Analysis.ReferenceConstructors.Warmup
{
    [TestFixture]
    public class WarmupReferenceTests
    {
        [Test]
        public void MethodWithoutWarmupIsAllowed()
        {
            var refConstructor = new WarmupReferenceConstructor(Shared.SilentOutput, useConventions: true);
            var m = typeof(ContainerWithoutWarmups).GetMethod(nameof(ContainerWithoutWarmups.Method));
            var warmupRefs = refConstructor.TryCreateWarmupReferences(m);
            Assert.That(warmupRefs, Is.Null);
        }

        [Test]
        public void WarmupAttributeIsFound()
        {
            var refConstructor = new WarmupReferenceConstructor(Shared.SilentOutput, useConventions: true);
            var m = typeof(ContainerWithWarmup).GetMethod(nameof(ContainerWithWarmup.Method));
            var warmupRefs = refConstructor.TryCreateWarmupReferences(m);
            Assert.That(warmupRefs, Is.Not.Null);
            Assert.That(warmupRefs, Has.Count.EqualTo(1));

            var warmupRef = warmupRefs[0];
            Assert.That(warmupRef, Is.Not.Null);
            Assert.That(warmupRef.Method, Is.Not.Null);
            Assert.That(warmupRef.Method.Name, Is.EqualTo(nameof(ContainerWithWarmup.ReadyMethod)));
            Assert.That(warmupRef.Name, Is.EqualTo(nameof(ContainerWithWarmup.ReadyMethod)));
            Assert.That(warmupRef.Order, Is.EqualTo(0));
        }
    }

    #region Test types

    internal class ContainerWithoutWarmups
    {
        public void Method() { }
    }

    internal class ContainerWithWarmup
    {
        public void ReadyMethod() { }

        [WarmupWith(nameof(ReadyMethod))]
        public void Method() { }
    }

    #endregion
}
