using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TinyBenchmark
{
    public class JsonExporter : IExporter
    {
        public bool Formatted { get; set; } = true;

        private string _json;

        public void Visit(BenchmarksContainerReport report) => _json = ToJson(report);

        public void Visit(BenchmarkReport report) => _json = ToJson(report);

        public void Visit(IterationReport report) => _json = ToJson(report);

        public string GetJson() => _json;

        #region Helpers

        protected virtual string ToJson(object report) => JsonConvert.SerializeObject(report, this.Formatted ? Formatting.Indented : Formatting.None);

        #endregion
    }
}
