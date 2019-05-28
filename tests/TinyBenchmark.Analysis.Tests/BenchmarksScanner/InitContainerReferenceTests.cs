using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using TinyBenchmark.Analysis;
using TinyBenchmark.Attributes;
using TinyBenchmark.Tests;

namespace Analysis
{
    /// <summary>
    /// InitContainer can be identified with:
    /// - attribute: InitContainer
    /// - name: InitContainer
    /// - name: Init{class name}
    /// </summary>
    [TestFixture]
    public class InitContainerReferenceTests
    {
        [Test]
        public void ContainerWithoutInitIsAllowed()
        {
            var scanner = new BenchmarksScanner(Shared.SilentOutput);
            Assert.That(() => scanner.Scan<ContainerWithoutInit>(), Throws.Nothing);
            Assert.That(scanner.Container.InitContainer, Is.Null);
        }

        [Test]
        public void InitContainerAttributeIsFound()
        {
            var scanner = new BenchmarksScanner(Shared.SilentOutput);
            Assert.That(() => scanner.Scan<ContainerWithInitAttribute>(), Throws.Nothing);
            Assert.That(scanner.Container.InitContainer, Is.Not.Null);
            Assert.That(scanner.Container.InitContainer.Method, Is.Not.Null);
            Assert.That(scanner.Container.InitContainer.Method.Name,
                Is.EqualTo(nameof(ContainerWithInitAttribute.TheContainerInitializationMethod)));
        }

        [Test]
        public void DefaultConventionIsFound()
        {
            var scanner = new BenchmarksScanner(Shared.SilentOutput);
            Assert.That(() => scanner.Scan<ContainerWithDefaultInitConvention>(), Throws.Nothing);
            Assert.That(scanner.Container.InitContainer, Is.Not.Null);
            Assert.That(scanner.Container.InitContainer.Method, Is.Not.Null);
            Assert.That(scanner.Container.InitContainer.Method.Name,
                Is.EqualTo(nameof(ContainerWithDefaultInitConvention.InitContainer)));
        }

        [Test]
        public void NamedConventionIsFound()
        {
            var scanner = new BenchmarksScanner(Shared.SilentOutput);
            Assert.That(() => scanner.Scan<ContainerWithNamedInitConvention>(), Throws.Nothing);
            Assert.That(scanner.Container.InitContainer, Is.Not.Null);
            Assert.That(scanner.Container.InitContainer.Method, Is.Not.Null);
            Assert.That(scanner.Container.InitContainer.Method.Name,
                Is.EqualTo(nameof(ContainerWithNamedInitConvention.InitContainerWithNamedInitConvention)));
        }

        [Test]
        public void InitContainerCannotHaveParameters()
        {
            var scanner = new BenchmarksScanner(Shared.SilentOutput);

            Assert.That(() => scanner.Scan<ContainerWithInitAttributeWithParameters>(), Throws.Exception);
            Assert.That(scanner.Container?.InitContainer, Is.Null);

            Assert.That(() => scanner.Scan<ContainerWithDefaultInitConventionWithParameters>(), Throws.Exception);
            Assert.That(scanner.Container?.InitContainer, Is.Null);

            Assert.That(() => scanner.Scan<ContainerWithNamedInitConventionWithParameters>(), Throws.Exception);
            Assert.That(scanner.Container?.InitContainer, Is.Null);
        }

        [Test]
        public void InitContainerAttributeHasPriorityOverConventions()
        {
            var scanner = new BenchmarksScanner(Shared.SilentOutput);
            Assert.That(() => scanner.Scan<ContainerWithBothAttributeAndConventions>(), Throws.Nothing);
            Assert.That(scanner.Container.InitContainer, Is.Not.Null);
            Assert.That(scanner.Container.InitContainer.Method, Is.Not.Null);
            Assert.That(scanner.Container.InitContainer.Method.Name,
                Is.EqualTo(nameof(ContainerWithBothAttributeAndConventions.TheContainerInitializationMethod)));
        }

        [Test]
        public void OnlyOneInitContainerIsAllowed()
        {
            var scanner = new BenchmarksScanner(Shared.SilentOutput);
            Assert.That(() => scanner.Scan<ContainerWithMultipleAttributes>(), Throws.Exception);
            Assert.That(() => scanner.Scan<ContainerWithMultipleConventions>(), Throws.Exception);
        }
    }

    #region Test types

    internal class ContainerWithoutInit { }

    internal class ContainerWithInitAttribute
    {
        [InitContainer]
        public void TheContainerInitializationMethod() { }
    }

    internal class ContainerWithDefaultInitConvention
    {
        public void InitContainer() { }
    }

    internal class ContainerWithNamedInitConvention
    {
        public void InitContainerWithNamedInitConvention() { }
    }

    #region Init method with parameters

    internal class ContainerWithInitAttributeWithParameters
    {
        [InitContainer]
        public void TheContainerInitializationMethod(int num) { }
    }

    internal class ContainerWithDefaultInitConventionWithParameters
    {
        public void InitContainer(int num) { }
    }

    internal class ContainerWithNamedInitConventionWithParameters
    {
        public void InitContainerWithNamedInitConventionWithParameters(int num) { }
    }

    #endregion

    #region Multiple init methods

    internal class ContainerWithBothAttributeAndConventions
    {
        [InitContainer]
        public void TheContainerInitializationMethod() { }

        public void InitContainer() { }

        public void InitContainerWithBothAttributeAndConventions() { }
    }

    internal class ContainerWithMultipleAttributes
    {
        [InitContainer]
        public void TheContainerInitializationMethod() { }

        [InitContainer]
        public void AnotherContainerInitializationMethod() { }
    }

    internal class ContainerWithMultipleConventions
    {
        public void InitContainer() { }

        public void InitContainerWithMultipleConventions() { }
    }

    #endregion

    #endregion
}
