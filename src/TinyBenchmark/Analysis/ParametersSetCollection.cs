using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using TinyBenchmark.Attributes;

namespace TinyBenchmark.Analysis
{
    internal class ParametersSetCollection : IEnumerable<ParametersSet>
    {
        private readonly List<PropertyWithParametersCollection> _propertiesWithParameters = new List<PropertyWithParametersCollection>();

        public ParametersSetCollection(Type benchmarksContainerType)
        {
            var parameterProperties = benchmarksContainerType
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Select(p => (property: p, attribute: p.GetCustomAttribute<ParamAttribute>()))
                .Where(x => x.attribute != null)
                .ToList();

            foreach (var (property, attribute) in parameterProperties)
            {
                var propertyParametersCollection = new PropertyWithParametersCollection(property, attribute);
                var parametersEnumerator = propertyParametersCollection.GetEnumerator();
                _propertiesWithParameters.Add(propertyParametersCollection);
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        public IEnumerator<ParametersSet> GetEnumerator() => new ParametersSetsEnumerator(this);

        private class ParametersSetsEnumerator : IEnumerator<ParametersSet>
        {
            private readonly ParametersSetCollection _parametersSetCollection;

            private readonly List<(PropertyWithParametersCollection collection, IEnumerator<object> enumerator)> _propertiesParametersEnumerations =
            new List<(PropertyWithParametersCollection, IEnumerator<object>)>();
            
            private int _targetCollectionIndex;

            public ParametersSetsEnumerator(ParametersSetCollection parametersSetCollection)
            {
                _parametersSetCollection = parametersSetCollection;
                Init(_parametersSetCollection);
            }

            private void Init(ParametersSetCollection parametersSetCollection)
            {
                if (parametersSetCollection._propertiesWithParameters?.Any() == true)
                {
                    foreach (var propertyWithParameters in parametersSetCollection._propertiesWithParameters)
                    {
                        var enumerator = propertyWithParameters.GetEnumerator();
                        _propertiesParametersEnumerations.Add((propertyWithParameters, enumerator));
                    }

                    _targetCollectionIndex = -1;
                }
            }

            object IEnumerator.Current => this.Current;

            public ParametersSet Current
            {
                get
                {
                    if (_propertiesParametersEnumerations.Any() == false)
                        throw new IndexOutOfRangeException();

                    var parametersSet = new ParametersSet();

                    foreach (var (collection, enumerator) in _propertiesParametersEnumerations)
                        parametersSet.Add(collection.Property, enumerator.Current);

                    return parametersSet;
                }
            }

            public bool MoveNext()
            {
                if (_propertiesParametersEnumerations.Any() == false)
                    return false;

                if (_targetCollectionIndex == -1)
                {
                    bool anyMovedNext = false;

                    // This is the first access to the collection (or after an enumerator reset).
                    // Move next all the enumerators.
                    foreach (var (collection, enumerator) in _propertiesParametersEnumerations)
                    {
                        bool movedNext = enumerator.MoveNext();
                        anyMovedNext |= movedNext;
                    }

                    _targetCollectionIndex = _propertiesParametersEnumerations.Count - 1;

                    return anyMovedNext;
                }

                if (_propertiesParametersEnumerations[_targetCollectionIndex].enumerator.MoveNext())
                    return true;

                // Current enumerator doesn't have any more items.

                for (var previousCollectionIndex = _targetCollectionIndex - 1; previousCollectionIndex >= 0; previousCollectionIndex--)
                {
                    var (_, targetEnumerator) = _propertiesParametersEnumerations[previousCollectionIndex];
                    
                    if (targetEnumerator.MoveNext())
                    {
                        // The target enumerator has more items, therefore all the succeding enumerators gets reset
                        // and the targets is moved to the last one.

                        for (int i = previousCollectionIndex + 1; i < _propertiesParametersEnumerations.Count; i++)
                        {
                            var (_, succedingEnumerator) = _propertiesParametersEnumerations[i];
                            succedingEnumerator.Reset();
                            succedingEnumerator.MoveNext(); // Move to the next item.
                        }

                        _targetCollectionIndex = _propertiesParametersEnumerations.Count - 1; // Target the last available collection (that has just been reset).
                        return true;
                    }
                }

                // The index is targeting the head collection, therefore all the combinations have been exhausted.
                return false;
            }

            public void Reset()
            {
                _propertiesParametersEnumerations.Clear();
                Init(_parametersSetCollection);
            }

            public void Dispose() => _propertiesParametersEnumerations.Clear();
        }
    }
}
