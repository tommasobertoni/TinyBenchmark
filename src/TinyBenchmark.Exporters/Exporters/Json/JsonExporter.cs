using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TinyBenchmark
{
    /// <summary>
    /// Creates a text export of a report.
    /// </summary>
    public class JsonExporter : IExporter
    {
        private static readonly IContractResolver _ExcludePropertiesResolver;

        static JsonExporter()
        {
            var excludePropertiesResolver = new ExcludePropertiesResolver();
            excludePropertiesResolver.Register(new ParametersPropertiesDefinition());
            _ExcludePropertiesResolver = excludePropertiesResolver;
        }

        /// <summary>
        /// Indicates if the resulting json should be formatted; the default is true.
        /// </summary>
        public bool Formatted { get; set; } = true;

        /// <summary>
        /// Indicates if the properties with a null value should be excluded from the json; the default is true.
        /// </summary>
        public bool IgnoreNullValues { get; set; } = true;

        private string _json;

        void IExporter.Visit(BenchmarksContainerReport report) => _json = ToJson(report);

        void IExporter.Visit(BenchmarkReport report) => _json = ToJson(report);

        void IExporter.Visit(IterationReport report) => _json = ToJson(report);

        /// <summary>
        /// Creates the json export.
        /// </summary>
        /// <returns>The json export.</returns>
        public string GetJson() => _json;

        #region Helpers

        /// <summary>
        /// Serializes the object to json.
        /// </summary>
        /// <param name="report">The report to serialize.</param>
        /// <returns>The json export.</returns>
        protected virtual string ToJson(object report) => JsonConvert.SerializeObject(report, new JsonSerializerSettings
        {
            Formatting = this.Formatted ? Formatting.Indented : Formatting.None,
            NullValueHandling = this.IgnoreNullValues ? NullValueHandling.Ignore : NullValueHandling.Include,
            ContractResolver = _ExcludePropertiesResolver
        });

        #endregion
    }
}
