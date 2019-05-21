using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TinyBenchmark
{
    internal interface IPropertiesDefinition
    {
        Type TargetType { get; }

        IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization);
    }

    internal class ExcludePropertiesResolver : DefaultContractResolver
    {
        private readonly Dictionary<Type, IPropertiesDefinition> _propertiesDefinitions = new Dictionary<Type, IPropertiesDefinition>();

        public void Register(IPropertiesDefinition definition) => _propertiesDefinitions[definition.TargetType] = definition;

        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            return _propertiesDefinitions.TryGetValue(type, out var definition)
                ? definition.CreateProperties(type, memberSerialization)
                : base.CreateProperties(type, memberSerialization);
        }
    }

    internal class ParametersPropertiesDefinition : DefaultContractResolver, IPropertiesDefinition
    {
        public Type TargetType => typeof(Parameters);

        IList<JsonProperty> IPropertiesDefinition.CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            IList<JsonProperty> props = base.CreateProperties(type, memberSerialization);
            return props.Where(p => p.PropertyName != nameof(Parameters.Hash)).ToList();
        }
    }
}
