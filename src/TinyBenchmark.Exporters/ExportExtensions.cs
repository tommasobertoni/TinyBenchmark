using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TinyBenchmark
{
    /// <summary>
    /// A set of extensions for reports and exporters.
    /// </summary>
    public static class ExportExtensions
    {
        /// <summary>
        /// Create an export of a report with a <see cref="JsonExporter"/>.
        /// </summary>
        /// <param name="report">The report.</param>
        /// <param name="formatted">true if the resulting json should be formatted; the default is true.</param>
        /// <param name="ignoreNullValues">true if the properties with a null value should be excluded from the json; the default is true.</param>
        /// <returns>The resulting json export.</returns>
        public static string ExportAsJson(
            this IReport report,
            bool formatted = true,
            bool ignoreNullValues = true)
        {
            var exporter = new JsonExporter
            {
                Formatted = formatted,
                IgnoreNullValues = ignoreNullValues,
            };

            report.Accept(exporter);
            var json = exporter.GetJson();
            return json;
        }
    }
}
