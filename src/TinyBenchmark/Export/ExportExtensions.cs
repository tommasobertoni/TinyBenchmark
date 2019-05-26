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
        /// Create an export of a report with the given exporter type.
        /// </summary>
        /// <typeparam name="TExporter">The type of the exporter to use.</typeparam>
        /// <param name="report">The report.</param>
        /// <returns>The exported instance used on the report.</returns>
        public static IExporter ExportWith<TExporter>(this IReport report)
            where TExporter : IExporter, new()
        {
            var exporter = new TExporter();
            report.ExportWith(exporter);
            return exporter;
        }

        /// <summary>
        /// Create an export of a report with the given exporter instance.
        /// </summary>
        /// <typeparam name="TExporter">The type of the exporter.</typeparam>
        /// <param name="report">The report.</param>
        /// <param name="exporter">The exporter instance to use.</param>
        public static void ExportWith<TExporter>(this IReport report, TExporter exporter)
            where TExporter : IExporter => report.Accept(exporter);

        /// <summary>
        /// Create an export of a report with a <see cref="TextExporter"/>.
        /// </summary>
        /// <param name="report"></param>
        /// <param name="includeIterations"></param>
        /// <returns>The resulting text export.</returns>
        public static string ExportAsText(this IReport report, bool includeIterations = true)
        {
            var exporter = new TextExporter { IncludeIterations = includeIterations };
            report.Accept(exporter);
            var text = exporter.GetText();
            return text;
        }
    }
}
