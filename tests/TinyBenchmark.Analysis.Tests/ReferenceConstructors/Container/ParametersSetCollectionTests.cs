using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TinyBenchmark.Analysis;
using TinyBenchmark.Attributes;

namespace Analysis.ReferenceConstructors.Container
{
    [TestFixture]
    public class ParametersSetCollectionTests
    {
        [Test]
        public void ContainerWithoutParamsIsAllowed()
        {
            var refConstructor = new ContainerReferenceConstructor(useConventions: true);
            var paramsSetCollection = refConstructor.GetParametersSetCollection(typeof(ContainerWithoutInit));
            Assert.That(paramsSetCollection, Is.Not.Null);
            Assert.That(paramsSetCollection, Is.Empty);
        }

        [Test]
        public void OneParamValueOnAPropertyIsFound()
        {
            var refConstructor = new ContainerReferenceConstructor(useConventions: true);
            var paramsSetCollection = refConstructor.GetParametersSetCollection(typeof(ContainerWithOneParam));
            Assert.That(paramsSetCollection, Is.Not.Null);
            Assert.That(paramsSetCollection, Is.Not.Empty);

            var parametersSetsList = paramsSetCollection.ToList();
            Assert.That(parametersSetsList, Has.Count.EqualTo(1));

            var parameterSet = parametersSetsList.FirstOrDefault()?.ToList();
            Assert.That(parameterSet, Is.Not.Null);
            Assert.That(parameterSet, Has.Count.EqualTo(1));
            Assert.That(parameterSet[0].Key, Is.EqualTo(nameof(ContainerWithOneParam.Count)));
            Assert.That(parameterSet[0].Value, Is.EqualTo(1));
        }

        [Test]
        public void ManyParamValuesOnAPropertyAreFound()
        {
            var refConstructor = new ContainerReferenceConstructor(useConventions: true);
            var paramsSetCollection = refConstructor.GetParametersSetCollection(typeof(ContainerWithManyParams));
            Assert.That(paramsSetCollection, Is.Not.Null);
            Assert.That(paramsSetCollection, Is.Not.Empty);

            var parametersSetsList = paramsSetCollection.ToList();
            Assert.That(parametersSetsList, Has.Count.EqualTo(4));

            var expectedParameters = new[] { 1, 10, 100, 1000 };

            for (int i = 0; i < expectedParameters.Length; i++)
            {
                var expected = expectedParameters[i];
                var parametersSet = parametersSetsList[i]?.ToList();
                Assert.That(parametersSet, Is.Not.Null);
                Assert.That(parametersSet, Has.Count.EqualTo(1));
                Assert.That(parametersSet[0].Key, Is.EqualTo(nameof(ContainerWithManyParams.Count)));
                Assert.That(parametersSet[0].Value, Is.EqualTo(expected));
            }
        }

        [Test]
        public void ManyParamValuesOnManyPropertiesAreFound()
        {
            var refConstructor = new ContainerReferenceConstructor(useConventions: true);
            var paramsSetCollection = refConstructor.GetParametersSetCollection(typeof(ContainerWithManyPropertiesManyParams));
            Assert.That(paramsSetCollection, Is.Not.Null);
            Assert.That(paramsSetCollection, Is.Not.Empty);

            var parametersSetsList = paramsSetCollection.ToList();
            // Prop1.Values.Count x Prop2.Values.Count
            Assert.That(parametersSetsList, Has.Count.EqualTo(12));
            
            var expectedParameters = new[]
            {
                (count: 1, prefix: "foo"),
                (count: 1, prefix: "bar"),
                (count: 1, prefix: "baz"),
                (count: 10, prefix: "foo"),
                (count: 10, prefix: "bar"),
                (count: 10, prefix: "baz"),
                (count: 100, prefix: "foo"),
                (count: 100, prefix: "bar"),
                (count: 100, prefix: "baz"),
                (count: 1000, prefix: "foo"),
                (count: 1000, prefix: "bar"),
                (count: 1000, prefix: "baz"),
            };

            for (int i = 0; i < expectedParameters.Length; i++)
            {
                var (expectedCount, expectedPrefix) = expectedParameters[i];
                var parametersSet = parametersSetsList[i]?.ToList();
                Assert.That(parametersSet, Is.Not.Null);
                Assert.That(parametersSet, Has.Count.EqualTo(2));
                Assert.That(parametersSet[0].Key, Is.EqualTo(nameof(ContainerWithManyPropertiesManyParams.Count)));
                Assert.That(parametersSet[0].Value, Is.EqualTo(expectedCount));
                Assert.That(parametersSet[1].Key, Is.EqualTo(nameof(ContainerWithManyPropertiesManyParams.Prefix)));
                Assert.That(parametersSet[1].Value, Is.EqualTo(expectedPrefix));
            }
        }

        [Test]
        public void ParamsWithCompatibleTypeAreFound()
        {
            var refConstructor = new ContainerReferenceConstructor(useConventions: true);
            var paramsSetCollection = refConstructor.GetParametersSetCollection(typeof(ContainerWithCompatibleParamType));
            Assert.That(paramsSetCollection, Is.Not.Null);
            Assert.That(paramsSetCollection, Is.Not.Empty);

            var parametersSetsList = paramsSetCollection.ToList();
            Assert.That(parametersSetsList, Has.Count.EqualTo(3));

            var expectedParameters = new[] { "foo", "bar", "baz" };

            for (int i = 0; i < expectedParameters.Length; i++)
            {
                var expected = expectedParameters[i];
                var parametersSet = parametersSetsList[i]?.ToList();
                Assert.That(parametersSet, Is.Not.Null);
                Assert.That(parametersSet, Has.Count.EqualTo(1));
                Assert.That(parametersSet[0].Key, Is.EqualTo(nameof(ContainerWithCompatibleParamType.Item)));
                Assert.That(parametersSet[0].Value, Is.EqualTo(expected));
            }
        }

        [Test]
        public void ParamsMustBeOfCompatibleType()
        {
            var refConstructor = new ContainerReferenceConstructor(useConventions: true);
            Assert.That(() => refConstructor.GetParametersSetCollection(typeof(ContainerWithWrongType)), Throws.Exception);
        }

        [Test]
        public void ParamsWithConvertibleTypesAreFound()
        {
            var refConstructor = new ContainerReferenceConstructor(useConventions: true);
            var paramsSetCollection = refConstructor.GetParametersSetCollection(typeof(ContainerWithConvertibleTypes));
            Assert.That(paramsSetCollection, Is.Not.Null);
            Assert.That(paramsSetCollection, Is.Not.Empty);

            var parametersSetsList = paramsSetCollection.ToList();
            Assert.That(parametersSetsList, Has.Count.EqualTo(6));

            var expectedParameters = new[] { 1m, 10m, 100.1m, 100.2m, 1000.5m, default(decimal?) };

            for (int i = 0; i < expectedParameters.Length; i++)
            {
                var expected = expectedParameters[i];
                var parametersSet = parametersSetsList[i]?.ToList();
                Assert.That(parametersSet, Is.Not.Null);
                Assert.That(parametersSet, Has.Count.EqualTo(1));
                Assert.That(parametersSet[0].Key, Is.EqualTo(nameof(ContainerWithConvertibleTypes.Score)));
                Assert.That(parametersSet[0].Value, Is.EqualTo(expected));
            }
        }

        [Test]
        public void PropertoesMustBeWritable()
        {
            var refConstructor = new ContainerReferenceConstructor(useConventions: true);
            Assert.That(() => refConstructor.GetParametersSetCollection(typeof(ContainerWithReadonlyProperty)), Throws.Exception);
        }

        [Test]
        public void NullParamValuesAreAllowed()
        {
            var refConstructor = new ContainerReferenceConstructor(useConventions: true);
            var paramsSetCollection = refConstructor.GetParametersSetCollection(typeof(ContainerWithNullParam));
            Assert.That(paramsSetCollection, Is.Not.Null);
            Assert.That(paramsSetCollection, Is.Not.Empty);

            var parametersSetsList = paramsSetCollection.ToList();
            Assert.That(parametersSetsList, Has.Count.EqualTo(3));

            var expectedParameters = new[] { "Mr.", "", null };

            for (int i = 0; i < expectedParameters.Length; i++)
            {
                var expected = expectedParameters[i];
                var parametersSet = parametersSetsList[i]?.ToList();
                Assert.That(parametersSet, Is.Not.Null);
                Assert.That(parametersSet, Has.Count.EqualTo(1));
                Assert.That(parametersSet[0].Key, Is.EqualTo(nameof(ContainerWithNullParam.Prefix)));
                Assert.That(parametersSet[0].Value, Is.EqualTo(expected));
            }
        }

        [Test]
        public void WrongDefaultParametersAreNotAllowed()
        {
            var refConstructor = new ContainerReferenceConstructor(useConventions: true);
            Assert.That(() => refConstructor.GetParametersSetCollection(typeof(ContainerWithWrongDefaultType)), Throws.Exception);
        }

        [Test]
        public void ParametersSetsCanBeAppliedToTheContainer()
        {
            var refConstructor = new ContainerReferenceConstructor(useConventions: true);
            var paramsSetCollection = refConstructor.GetParametersSetCollection(typeof(FullContainer));
            Assert.That(paramsSetCollection, Is.Not.Null);
            Assert.That(paramsSetCollection, Is.Not.Empty);

            foreach (var parametersSet in paramsSetCollection)
            {
                var container = new FullContainer();
                Assert.That(container.Count, Is.EqualTo(default(int)));
                Assert.That(container.Prefix, Is.EqualTo(default(string)));

                Assert.That(() => parametersSet.ApplyTo(container), Throws.Nothing);

                var parameters = parametersSet.ToList();
                Assert.That(parameters, Is.Not.Null);
                Assert.That(parameters, Has.Count.EqualTo(4));

                Assert.That(parameters[0].Key, Is.EqualTo(nameof(container.Count)));
                Assert.That(parameters[0].Value, Is.EqualTo(container.Count));
                Assert.That(parameters[1].Key, Is.EqualTo(nameof(container.Prefix)));
                Assert.That(parameters[1].Value, Is.EqualTo(container.Prefix));
                Assert.That(parameters[2].Key, Is.EqualTo(nameof(container.Id)));
                Assert.That(parameters[2].Value, Is.EqualTo(container.Id));
            }
        }
    }

    #region Test types

    internal class ContainerWithoutParams
    {
        public int Count { get; set; }
    }

    internal class ContainerWithOneParam
    {
        [Param(1)]
        public int Count { get; set; }
    }

    internal class ContainerWithManyParams
    {
        [Param(1, 10, 100, 1000)]
        public int Count { get; set; }
    }

    internal class ContainerWithManyPropertiesManyParams
    {
        [Param(1, 10, 100, 1000)]
        public int Count { get; set; }

        [Param("foo", "bar", "baz")]
        public string Prefix { get; set; }
    }

    internal class ContainerWithCompatibleParamType
    {
        [Param("foo", "bar", "baz")]
        public object Item { get; set; }
    }

    internal class ContainerWithWrongType
    {
        [Param(null, 'a', "abcd")]
        public int Count { get; set; }
    }

    internal class ContainerWithConvertibleTypes
    {
        [Param(1, 10L, 100.1D, 100.2F, "1000.5", null)]
        public decimal? Score { get; set; }
    }

    internal class ContainerWithReadonlyProperty
    {
        [Param(1, 10, 100, 1000)]
        public int Count { get; }
    }

    internal class ContainerWithNullParam
    {
        [Param("Mr.", "", null)]
        public string Prefix { get; set; }
    }

    internal class ContainerWithWrongDefaultType
    {
        [Param(null)]
        public int Count { get; set; }
    }

    internal class FullContainer
    {
        [Param(1, 10, 100, 1000)]
        public int Count { get; set; }

        [Param("foo", "bar", "baz")]
        public string Prefix { get; set; }

        [Param(10L, null)]
        public long? Id { get; set; }

        [Param(1, 10L, 100.1D, 100.2F, "1000.5", null)]
        public decimal? Score { get; set; }
    }

    #endregion
}
