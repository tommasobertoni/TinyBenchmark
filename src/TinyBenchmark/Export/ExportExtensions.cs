using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TinyBenchmark
{
    public static class ExportExtensions
    {
        public static IExporter ExportWith<TExporter>(this IBenchmark container)
            where TExporter : IExporter, new()
        {
            var exporter = new TExporter();
            container.ExportWith(exporter);
            return exporter;
        }

        public static void ExportWith<TExporter>(this IBenchmark container, TExporter exporter)
            where TExporter : IExporter => container.Accept(exporter);

        public static string ExportAsText(this IBenchmark container, bool includeIterations = true)
        {
            var exporter = new TextExporter { IncludeIterations = includeIterations };
            container.Accept(exporter);
            var text = exporter.GetText();
            return text;
        }
    }
}
