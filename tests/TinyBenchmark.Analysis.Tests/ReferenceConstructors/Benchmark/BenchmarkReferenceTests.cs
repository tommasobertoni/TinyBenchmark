using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TinyBenchmark.Analysis;
using TinyBenchmark.Attributes;
using TinyBenchmark.Tests;

namespace Analysis.ReferenceConstructors.Benchmark
{
    [TestFixture]
    public class BenchmarkReferenceTests
    {
        [Test]
        public void EmptyContainerIsAllowed()
        {
            var refConstructor = new BenchmarkReferenceConstructor(Shared.SilentOutput, useConventions: true);
            var benchmarkRefs = refConstructor.TryCreateBenchmarkReferences(typeof(EmptyContainer));
            Assert.That(benchmarkRefs, Is.Not.Null);
            Assert.That(benchmarkRefs, Is.Empty);
        }

        [Test]
        public void ContainerWithoutInitIsAllowed()
        {
            var refConstructor = new BenchmarkReferenceConstructor(Shared.SilentOutput, useConventions: true);
            var m = typeof(BaseContainer).GetMethod(nameof(BaseContainer.Method));

            Assert.That(() => refConstructor.TryCreateInitWithReference(m), Throws.Nothing);
            var initRef = refConstructor.TryCreateInitWithReference(m);
            Assert.That(initRef, Is.Null);

            var benchmarkRefs = refConstructor.TryCreateBenchmarkReferences(typeof(BaseContainer));
            Assert.That(benchmarkRefs, Is.Not.Null);
            Assert.That(benchmarkRefs, Has.Count.EqualTo(1));

            var benchmarkRef = benchmarkRefs[0];
            Assert.That(benchmarkRef, Is.Not.Null);
            Assert.That(benchmarkRef.Method?.Name, Is.EqualTo(nameof(BaseContainer.Method)));
            Assert.That(benchmarkRef.Name, Is.EqualTo(nameof(BaseContainer.Method)));
            Assert.That(benchmarkRef.IsBaseline, Is.False);
            Assert.That(benchmarkRef.Iterations, Is.EqualTo(1));
            Assert.That(benchmarkRef.ArgumentsCollection, Is.Null);
            Assert.That(benchmarkRef.InitWithReference, Is.Null);
            Assert.That(benchmarkRef.WarmupCollection, Is.Null);
        }

        [Test]
        public void MultipleBenchmarksAreFound()
        {
            var refConstructor = new BenchmarkReferenceConstructor(Shared.SilentOutput, useConventions: true);
            var benchmarkRefs = refConstructor.TryCreateBenchmarkReferences(typeof(ContainerWithMultipleBenchmarks));
            Assert.That(benchmarkRefs, Is.Not.Null);
            Assert.That(benchmarkRefs, Has.Count.EqualTo(2));

            var benchmark = benchmarkRefs[0];
            Assert.That(benchmark, Is.Not.Null);
            Assert.That(benchmark.Method?.Name, Is.EqualTo(nameof(ContainerWithMultipleBenchmarks.Method)));

            var anotherBenchmark = benchmarkRefs[1];
            Assert.That(anotherBenchmark, Is.Not.Null);
            Assert.That(anotherBenchmark.Method?.Name, Is.EqualTo(nameof(ContainerWithMultipleBenchmarks.AnotherMethod)));
        }

        [Test]
        public void BenchmarkInitIsFound()
        {
            var refConstructor = new BenchmarkReferenceConstructor(Shared.SilentOutput, useConventions: true);
            var m = typeof(ContainerWithBenchmarkInit).GetMethod(nameof(ContainerWithBenchmarkInit.Method));
            var initRef = refConstructor.TryCreateInitWithReference(m);
            Assert.That(initRef, Is.Not.Null);
            Assert.That(initRef.Method?.Name, Is.EqualTo(nameof(ContainerWithBenchmarkInit.BenchmarkInit)));
        }

        [Test]
        public void InitAttributeHasPriorityOverConventions()
        {
            var refConstructor = new BenchmarkReferenceConstructor(Shared.SilentOutput, useConventions: true);
            var m = typeof(ContainerWithAttributeAndInitConvention).GetMethod(nameof(ContainerWithAttributeAndInitConvention.Method));
            var initRef = refConstructor.TryCreateInitWithReference(m);
            Assert.That(initRef, Is.Not.Null);
            Assert.That(initRef.Method?.Name, Is.EqualTo(nameof(ContainerWithAttributeAndInitConvention.BenchmarkInit)));
        }

        [Test]
        public void BenchmarkNamesAreFound()
        {
            var refConstructor = new BenchmarkReferenceConstructor(Shared.SilentOutput, useConventions: true);
            var benchmarkRefs = refConstructor.TryCreateBenchmarkReferences(typeof(ContainerWithNamedOrderedBenchmarks));
            Assert.That(benchmarkRefs, Is.Not.Null);
            Assert.That(benchmarkRefs, Has.Count.EqualTo(2));

            var firstBenchmark = benchmarkRefs.FirstOrDefault(b => b.Name == "First");
            Assert.That(firstBenchmark, Is.Not.Null);
            Assert.That(firstBenchmark.Method?.Name, Is.EqualTo(nameof(ContainerWithNamedOrderedBenchmarks.AnotherMethod)));

            var secondBenchmark = benchmarkRefs.FirstOrDefault(b => b.Name == "Second");
            Assert.That(secondBenchmark.Method?.Name, Is.EqualTo(nameof(ContainerWithNamedOrderedBenchmarks.Method)));
        }

        [Test]
        public void BenchmarksAreOrdered()
        {
            var refConstructor = new BenchmarkReferenceConstructor(Shared.SilentOutput, useConventions: true);
            var benchmarkRefs = refConstructor.TryCreateBenchmarkReferences(typeof(ContainerWithNamedOrderedBenchmarks));
            Assert.That(benchmarkRefs, Is.Not.Null);
            Assert.That(benchmarkRefs, Has.Count.EqualTo(2));

            var firstBenchmark = benchmarkRefs[0];
            Assert.That(firstBenchmark, Is.Not.Null);
            Assert.That(firstBenchmark.Order, Is.EqualTo(1));
            Assert.That(firstBenchmark.Method?.Name, Is.EqualTo(nameof(ContainerWithNamedOrderedBenchmarks.AnotherMethod)));

            var secondBenchmark = benchmarkRefs[1];
            Assert.That(secondBenchmark, Is.Not.Null);
            Assert.That(secondBenchmark.Order, Is.EqualTo(99));
            Assert.That(secondBenchmark.Method?.Name, Is.EqualTo(nameof(ContainerWithNamedOrderedBenchmarks.Method)));
        }

        [Test]
        public void BaselineBenchmarkIsFound()
        {
            var refConstructor = new BenchmarkReferenceConstructor(Shared.SilentOutput, useConventions: true);
            var benchmarkRefs = refConstructor.TryCreateBenchmarkReferences(typeof(ContainerWithBaseline));
            Assert.That(benchmarkRefs, Is.Not.Null);
            Assert.That(benchmarkRefs, Has.Count.EqualTo(2));

            var firstBenchmark = benchmarkRefs[0];
            Assert.That(firstBenchmark, Is.Not.Null);
            Assert.That(firstBenchmark.IsBaseline, Is.False);

            var secondBenchmark = benchmarkRefs[1];
            Assert.That(secondBenchmark, Is.Not.Null);
            Assert.That(secondBenchmark.IsBaseline, Is.True);
        }

        [Test]
        public void IterationsAreFound()
        {
            var refConstructor = new BenchmarkReferenceConstructor(Shared.SilentOutput, useConventions: true);
            var benchmarkRefs = refConstructor.TryCreateBenchmarkReferences(typeof(ContainerWithNamedOrderedBenchmarks));
            Assert.That(benchmarkRefs, Is.Not.Null);
            Assert.That(benchmarkRefs, Has.Count.EqualTo(2));

            var firstBenchmark = benchmarkRefs[0];
            Assert.That(firstBenchmark, Is.Not.Null);
            Assert.That(firstBenchmark.Iterations, Is.EqualTo(10));

            var secondBenchmark = benchmarkRefs[1];
            Assert.That(secondBenchmark, Is.Not.Null);
            Assert.That(secondBenchmark.Iterations, Is.EqualTo(20));
        }

        [Test]
        public void IterationsMustBePositive()
        {
            var refConstructor = new BenchmarkReferenceConstructor(Shared.SilentOutput, useConventions: true);
            Assert.That(() => refConstructor.TryCreateBenchmarkReferences(typeof(ContainerWithNoIterations)), Throws.Exception);
            Assert.That(() => refConstructor.TryCreateBenchmarkReferences(typeof(ContainerWithNegativeIterations)), Throws.Exception);
        }

        [Test]
        public void ArgumentsAreFound()
        {
            var refConstructor = new BenchmarkReferenceConstructor(Shared.SilentOutput, useConventions: true);
            var benchmarkRefs = refConstructor.TryCreateBenchmarkReferences(typeof(ContainerWithArguments));
            Assert.That(benchmarkRefs, Is.Not.Null);
            Assert.That(benchmarkRefs, Has.Count.EqualTo(1));

            var benchmark = benchmarkRefs[0];
            Assert.That(benchmark.ArgumentsCollection, Is.Not.Null);
            Assert.That(benchmark.ArgumentsCollection, Has.Count.EqualTo(3));

            var expectedArguments = new[] { 1, 10, 100 };

            for (int i = 0; i < expectedArguments.Length; i++)
            {
                var expectedArgument = expectedArguments[i];
                var arguments = benchmark.ArgumentsCollection[i].ToList();
                Assert.That(arguments, Is.Not.Null);
                Assert.That(arguments, Has.Count.EqualTo(1));
                var actualArgument = arguments[0];
                Assert.That(actualArgument.Key, Is.EqualTo("count"));
                Assert.That(actualArgument.Value, Is.EqualTo(expectedArgument));
            }
        }

        [Test]
        public void InitConventionsAreFound()
        {
            /*
             * Conventions:
             * - {method name}Init
             * - {method name}_Init
             * - Init{method name}
             * - Init_{method name}
             */

            var refConstructor = new BenchmarkReferenceConstructor(Shared.SilentOutput, useConventions: true);

            var typesWithInitConvention = new[]
            {
                (type: typeof(ContainerWithInitConvention1), conventionName: nameof(ContainerWithInitConvention1.MethodInit)),
                (type: typeof(ContainerWithInitConvention2), conventionName: nameof(ContainerWithInitConvention2.Method_Init)),
                (type: typeof(ContainerWithInitConvention3), conventionName: nameof(ContainerWithInitConvention3.InitMethod)),
                (type: typeof(ContainerWithInitConvention4), conventionName: nameof(ContainerWithInitConvention4.Init_Method)),
            };

            foreach (var (type, conventionName) in typesWithInitConvention)
            {
                var m = type.GetMethod("Method");
                var initRef = refConstructor.TryCreateInitWithReference(m);
                Assert.That(initRef, Is.Not.Null);
                Assert.That(initRef.Method?.Name, Is.EqualTo(conventionName));
            }
        }

        [Test]
        public void InitConventionsAreFoundBySetup()
        {
            var refConstructor = new BenchmarkReferenceConstructor(Shared.SilentOutput, useConventions: false);

            var typesWithInitConvention = new[]
            {
                typeof(ContainerWithInitConvention1),
                typeof(ContainerWithInitConvention2),
                typeof(ContainerWithInitConvention3),
                typeof(ContainerWithInitConvention4),
            };

            foreach (var type in typesWithInitConvention)
            {
                var m = type.GetMethod("Method");
                var initRef = refConstructor.TryCreateInitWithReference(m);
                Assert.That(initRef, Is.Null);
            }
        }

        [Test]
        public void MultipleInitConventionsAreNotAllowed()
        {
            var refConstructor = new BenchmarkReferenceConstructor(Shared.SilentOutput, useConventions: true);
            var m = typeof(ContainerWithMultipleInitConventions).GetMethod(nameof(ContainerWithMultipleInitConventions.Method));
            Assert.That(() => refConstructor.TryCreateInitWithReference(m), Throws.Exception);
        }

        [Test]
        public void BenchmarkWarmupsAreFound()
        {
            {
                // Without conventions
                var refConstructor = new BenchmarkReferenceConstructor(Shared.SilentOutput, useConventions: false);
                var benchmarkRefs = refConstructor.TryCreateBenchmarkReferences(typeof(ContainerWithWarmups));
                Assert.That(benchmarkRefs, Is.Not.Null);
                Assert.That(benchmarkRefs, Has.Count.EqualTo(1));

                var benchmark = benchmarkRefs[0];
                Assert.That(benchmark, Is.Not.Null);
                Assert.That(benchmark.WarmupCollection, Is.Not.Null);
                Assert.That(benchmark.WarmupCollection, Has.Count.EqualTo(1));
                Assert.That(benchmark.WarmupCollection[0].Method?.Name, Is.EqualTo(nameof(ContainerWithWarmups.ReadyMethod)));
            }

            {
                // With conventions
                var refConstructor = new BenchmarkReferenceConstructor(Shared.SilentOutput, useConventions: true);
                var benchmarkRefs = refConstructor.TryCreateBenchmarkReferences(typeof(ContainerWithWarmups));
                Assert.That(benchmarkRefs, Is.Not.Null);
                Assert.That(benchmarkRefs, Has.Count.EqualTo(1));

                var benchmark = benchmarkRefs[0];
                Assert.That(benchmark, Is.Not.Null);
                Assert.That(benchmark.WarmupCollection, Is.Not.Null);
                Assert.That(benchmark.WarmupCollection, Has.Count.EqualTo(5));
                Assert.That(benchmark.WarmupCollection[0].Method?.Name, Is.EqualTo(nameof(ContainerWithWarmups.ReadyMethod)));
                Assert.That(benchmark.WarmupCollection[1].Method?.Name, Is.EqualTo(nameof(ContainerWithWarmups.MethodWarmup)));
                Assert.That(benchmark.WarmupCollection[2].Method?.Name, Is.EqualTo(nameof(ContainerWithWarmups.Method_Warmup)));
                Assert.That(benchmark.WarmupCollection[3].Method?.Name, Is.EqualTo(nameof(ContainerWithWarmups.WarmupForMethod)));
                Assert.That(benchmark.WarmupCollection[4].Method?.Name, Is.EqualTo(nameof(ContainerWithWarmups.WarmupFor_Method)));
            }
        }

        [Test]
        public void FullTest()
        {
            var refConstructor = new BenchmarkReferenceConstructor(Shared.SilentOutput, useConventions: true);
            var benchmarkRefs = refConstructor.TryCreateBenchmarkReferences(typeof(FullContainer));
            Assert.That(benchmarkRefs, Is.Not.Null);
            Assert.That(benchmarkRefs, Has.Count.EqualTo(2));

            var baselineBenchmark = benchmarkRefs[0];
            Assert.That(baselineBenchmark, Is.Not.Null);
            Assert.That(baselineBenchmark.Name, Is.EqualTo("Baseline method"));
            Assert.That(baselineBenchmark.Method?.Name, Is.EqualTo(nameof(FullContainer.SecondMethod)));
            Assert.That(baselineBenchmark.Order, Is.EqualTo(-99));
            Assert.That(baselineBenchmark.IsBaseline, Is.True);
            Assert.That(baselineBenchmark.Iterations, Is.EqualTo(1));
            Assert.That(baselineBenchmark.ArgumentsCollection, Is.Null);
            Assert.That(baselineBenchmark.InitWithReference, Is.Null);
            Assert.That(baselineBenchmark.WarmupCollection, Is.Not.Null);
            Assert.That(baselineBenchmark.WarmupCollection, Has.Count.EqualTo(2));
            Assert.That(baselineBenchmark.WarmupCollection[0].Method?.Name, Is.EqualTo(nameof(FullContainer.SharedWarmup)));
            Assert.That(baselineBenchmark.WarmupCollection[0].Order, Is.EqualTo(0));
            Assert.That(baselineBenchmark.WarmupCollection[1].Method?.Name, Is.EqualTo(nameof(FullContainer.WarmupFor_SecondMethod)));
            Assert.That(baselineBenchmark.WarmupCollection[1].Order, Is.EqualTo(0));

            var otherBenchmark = benchmarkRefs[1];
            Assert.That(otherBenchmark, Is.Not.Null);
            Assert.That(otherBenchmark.Name, Is.EqualTo("A method"));
            Assert.That(otherBenchmark.Method?.Name, Is.EqualTo(nameof(FullContainer.Method)));
            Assert.That(otherBenchmark.Order, Is.EqualTo(1));
            Assert.That(otherBenchmark.IsBaseline, Is.False);
            Assert.That(otherBenchmark.Iterations, Is.EqualTo(3));

            Assert.That(otherBenchmark.ArgumentsCollection, Is.Not.Null);
            Assert.That(otherBenchmark.ArgumentsCollection, Has.Count.EqualTo(3));
            
            var expectedArguments = new[]
            {
                (count: 1 as int?, prefix: "Mr."),
                (count: null, prefix: "Mr."),
                (count: null, prefix: null),
            };

            for (int i = 0; i < expectedArguments.Length; i++)
            {
                var (count, prefix) = expectedArguments[i];
                var arguments = otherBenchmark.ArgumentsCollection[i].ToList();
                Assert.That(arguments, Has.Count.EqualTo(2));
                Assert.That(arguments[0].Key, Is.EqualTo("count"));
                Assert.That(arguments[0].Value, Is.EqualTo(count));
                Assert.That(arguments[1].Key, Is.EqualTo("prefix"));
                Assert.That(arguments[1].Value, Is.EqualTo(prefix));
            }

            Assert.That(otherBenchmark.InitWithReference, Is.Not.Null);
            Assert.That(otherBenchmark.InitWithReference.Method?.Name, Is.EqualTo(nameof(FullContainer.Method_Init)));

            Assert.That(otherBenchmark.WarmupCollection, Is.Not.Null);
            Assert.That(otherBenchmark.WarmupCollection, Has.Count.EqualTo(3));
            Assert.That(otherBenchmark.WarmupCollection[0].Method?.Name, Is.EqualTo(nameof(FullContainer.MethodFirstWarmup)));
            Assert.That(otherBenchmark.WarmupCollection[0].Order, Is.EqualTo(1));
            Assert.That(otherBenchmark.WarmupCollection[1].Method?.Name, Is.EqualTo(nameof(FullContainer.MethodSecondWarmup)));
            Assert.That(otherBenchmark.WarmupCollection[1].Order, Is.EqualTo(2));
            Assert.That(otherBenchmark.WarmupCollection[2].Method?.Name, Is.EqualTo(nameof(FullContainer.SharedWarmup)));
            Assert.That(otherBenchmark.WarmupCollection[2].Order, Is.EqualTo(99));
        }
    }

    #region Test types

    internal class EmptyContainer { }

    internal class BaseContainer
    {
        [Benchmark]
        public void Method() { }
    }

    internal class ContainerWithMultipleBenchmarks
    {
        [Benchmark]
        public void Method() { }

        [Benchmark]
        public void AnotherMethod() { }

        public void ANonBenchmarkMethod() { }
    }

    internal class ContainerWithNamedOrderedBenchmarks
    {
        [Benchmark(Order = 99, Name = "Second", Iterations = 20)]
        public void Method() { }

        [Benchmark(Order = 1, Name = "First", Iterations = 10)]
        public void AnotherMethod() { }
    }

    internal class ContainerWithBaseline
    {
        [Benchmark]
        public void Method() { }

        [Benchmark(Baseline = true)]
        public void AnotherMethod() { }
    }

    #region Iterations

    internal class ContainerWithNoIterations
    {
        [Benchmark(Iterations = 0)]
        public void Method() { }
    }

    internal class ContainerWithNegativeIterations
    {
        [Benchmark(Iterations = -1)]
        public void Method() { }
    }

    #endregion

    #region Arguments

    internal class ContainerWithArguments
    {
        [Benchmark]
        [Arguments(1)]
        [Arguments(10)]
        [Arguments(100)]
        public void Method(int count) { }
    }

    #endregion

    #region Warmup

    internal class ContainerWithWarmups
    {
        public void ReadyMethod() { }

        [WarmupWith(nameof(ReadyMethod))]
        [Benchmark]
        public void Method() { }

        // Conventions

        public void MethodWarmup() { }

        public void Method_Warmup() { }

        public void WarmupForMethod() { }

        public void WarmupFor_Method() { }
    }

    #endregion

    #region Init

    internal class ContainerWithBenchmarkInit
    {
        public void BenchmarkInit() { }

        [Benchmark]
        [InitWith(nameof(BenchmarkInit))]
        public void Method() { }
    }

    internal class ContainerWithAttributeAndInitConvention
    {
        public void BenchmarkInit() { }

        public void MethodInit() { }

        [Benchmark]
        [InitWith(nameof(BenchmarkInit))]
        public void Method() { }
    }

    #region Init conventions

    internal class ContainerWithInitConvention1
    {
        public void Method() { }

        public void MethodInit() { }
    }

    internal class ContainerWithInitConvention2
    {
        public void Method() { }

        public void Method_Init() { }
    }

    internal class ContainerWithInitConvention3
    {
        public void Method() { }

        public void InitMethod() { }
    }

    internal class ContainerWithInitConvention4
    {
        public void Method() { }

        public void Init_Method() { }
    }

    internal class ContainerWithMultipleInitConventions
    {
        public void Method() { }

        public void MethodInit() { }

        public void Method_Init() { }

        public void InitMethod() { }

        public void Init_Method() { }
    }

    #endregion

    #endregion

    internal class FullContainer
    {
        public void Method_Init() { }

        public void SharedWarmup() { }

        public void MethodFirstWarmup() { }

        public void MethodSecondWarmup() { }

        [Benchmark(Name = "A method", Iterations = 3, Order = 1)]
        [WarmupWith(nameof(MethodSecondWarmup), Order = 2)]
        [WarmupWith(nameof(MethodFirstWarmup), Order = 1)]
        [WarmupWith(nameof(SharedWarmup), Order = 99)]
        [Arguments(1, "Mr.")]
        [Arguments(null, "Mr.")]
        [Arguments(null, null)]
        public void Method(int? count, string prefix) { }

        public void WarmupFor_SecondMethod() { }

        [WarmupWith(nameof(SharedWarmup))]
        [Benchmark(Name = "Baseline method", Baseline = true, Order = -99)]
        public void SecondMethod() { }
    }

    #endregion
}
