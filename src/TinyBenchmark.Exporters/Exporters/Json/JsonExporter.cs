using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TinyBenchmark
{
    public class JsonExporter : IExporter
    {
        private static readonly IContractResolver _ExcludePropertiesResolver;

        static JsonExporter()
        {
            var excludePropertiesResolver = new ExcludePropertiesResolver();
            excludePropertiesResolver.Register(new ParametersPropertiesDefinition());
            _ExcludePropertiesResolver = excludePropertiesResolver;
        }

        public bool Formatted { get; set; } = true;

        public bool IgnoreNullValues { get; set; } = true;

        private string _json;

        public void Visit(BenchmarksContainerReport report) => _json = ToJson(report);

        public void Visit(BenchmarkReport report) => _json = ToJson(report);

        public void Visit(IterationReport report) => _json = ToJson(report);

        public string GetJson() => _json;

        #region Helpers

        protected virtual string ToJson(object report) => JsonConvert.SerializeObject(report, new JsonSerializerSettings
        {
            Formatting = this.Formatted ? Formatting.Indented : Formatting.None,
            NullValueHandling = this.IgnoreNullValues ? NullValueHandling.Ignore : NullValueHandling.Include,
            ContractResolver = _ExcludePropertiesResolver
        });

        #endregion
    }
}
