using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TinyBenchmark
{
    public static class ExportExtensions
    {
        public static string ExportAsJson(this IReport container, bool formatted = true)
        {
            var exporter = new JsonExporter
            {
                Formatted = formatted
            };

            container.Accept(exporter);
            var json = exporter.GetJson();
            return json;
        }

        //public static string ExportAsMarkdown(this IBenchmark container)
        //{
        //    var exporter = new MarkdownExporter();
        //    container.Accept(exporter);
        //    var markdown = exporter.GetMarkdown();
        //    return markdown;
        //}
    }
}
