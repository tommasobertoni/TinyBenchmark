using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
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

        [Test]
        public void WarmupOrderIsFound()
        {
            var refConstructor = new WarmupReferenceConstructor(Shared.SilentOutput, useConventions: true);
            var m = typeof(ContainerWithOrderedWarmups).GetMethod(nameof(ContainerWithOrderedWarmups.Method));
            var warmupRefs = refConstructor.TryCreateWarmupReferences(m);
            Assert.That(warmupRefs, Is.Not.Null);
            Assert.That(warmupRefs, Has.Count.EqualTo(2));

            var firstWarmup = warmupRefs.OrderBy(w => w.Order).First();
            Assert.That(firstWarmup, Is.Not.Null);
            Assert.That(firstWarmup.Method?.Name, Is.EqualTo(nameof(ContainerWithOrderedWarmups.ReadyMethod)));

            var secondWarmup = warmupRefs.OrderBy(w => w.Order).Skip(1).First();
            Assert.That(secondWarmup, Is.Not.Null);
            Assert.That(secondWarmup.Method?.Name, Is.EqualTo(nameof(ContainerWithOrderedWarmups.SecondReadyMethod)));
        }

        [Test]
        public void WarmupConventionsAreFound()
        {
            /*
             * Conventions:
             * - {method name}Warmup
             * - {method name}_Warmup
             * - WarmupFor{method name}
             * - WarmupFor_{method name}
             */

            var refConstructor = new WarmupReferenceConstructor(Shared.SilentOutput, useConventions: true);
            var m = typeof(ContainerWithConventions).GetMethod(nameof(ContainerWithConventions.Method));
            var warmupRefs = refConstructor.TryCreateWarmupReferences(m);
            Assert.That(warmupRefs, Is.Not.Null);
            Assert.That(warmupRefs, Has.Count.EqualTo(5));

            var attributeWarmupRef = warmupRefs[0];
            Assert.That(attributeWarmupRef, Is.Not.Null);
            Assert.That(attributeWarmupRef.Method?.Name, Is.EqualTo(nameof(ContainerWithConventions.ReadyMethod)));

            var methodWarmupConvRef = warmupRefs[1];
            Assert.That(methodWarmupConvRef, Is.Not.Null);
            Assert.That(methodWarmupConvRef.Method?.Name, Is.EqualTo(nameof(ContainerWithConventions.MethodWarmup)));

            var method_WarmupConvRef = warmupRefs[2];
            Assert.That(method_WarmupConvRef, Is.Not.Null);
            Assert.That(method_WarmupConvRef.Method?.Name, Is.EqualTo(nameof(ContainerWithConventions.Method_Warmup)));

            var warmupForMethodConvRef = warmupRefs[3];
            Assert.That(warmupForMethodConvRef, Is.Not.Null);
            Assert.That(warmupForMethodConvRef.Method?.Name, Is.EqualTo(nameof(ContainerWithConventions.WarmupForMethod)));

            var warmupFor_MethodConvRef = warmupRefs[4];
            Assert.That(warmupFor_MethodConvRef, Is.Not.Null);
            Assert.That(warmupFor_MethodConvRef.Method?.Name, Is.EqualTo(nameof(ContainerWithConventions.WarmupFor_Method)));
        }

        [Test]
        public void WarmupConventionsAreNotFoundBySetup()
        {
            var refConstructor = new WarmupReferenceConstructor(Shared.SilentOutput, useConventions: false);
            var m = typeof(ContainerWithConventions).GetMethod(nameof(ContainerWithConventions.Method));
            var warmupRefs = refConstructor.TryCreateWarmupReferences(m);
            Assert.That(warmupRefs, Is.Not.Null);
            Assert.That(warmupRefs, Has.Count.EqualTo(1));

            var attributeWarmupRef = warmupRefs[0];
            Assert.That(attributeWarmupRef, Is.Not.Null);
            Assert.That(attributeWarmupRef.Method?.Name, Is.EqualTo(nameof(ContainerWithConventions.ReadyMethod)));
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

    internal class ContainerWithOrderedWarmups
    {
        public void ReadyMethod() { }

        public void SecondReadyMethod() { }

        [WarmupWith(nameof(SecondReadyMethod), Order = 99)]
        [WarmupWith(nameof(ReadyMethod), Order = 1)]
        public void Method() { }
    }

    internal class ContainerWithConventions
    {
        public void ReadyMethod() { }

        [WarmupWith(nameof(ReadyMethod))]
        public void Method() { }

        // Conventions

        public void MethodWarmup() { }

        public void Method_Warmup() { }

        public void WarmupForMethod() { }

        public void WarmupFor_Method() { }
    }

    #endregion
}
