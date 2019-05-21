using System;
using System.Linq;
using System.Text;

namespace TinyBenchmark.Samples
{
    class Program
    {
        static void Main(string[] args)
        {
            const string separator = "------------------------------------------------------------";

            var runner = new BenchmarkRunner(OutputLevel.Minimal);

            var benchmarksReport = RunAndPrint<CollectionsBenchmarks>();

            var text = benchmarksReport.ExportAsText(includeIterations: false);
            Console.WriteLine(text);

            var json = benchmarksReport.ExportAsJson(formatted: true);
            SaveToFileAndOpenInExplorer(json);

            if (System.Diagnostics.Debugger.IsAttached)
            {
                Console.Write($"{Environment.NewLine}Press ENTER to exit...");
                Console.ReadLine();
            }

            // Local functions

            BenchmarksContainerReport RunAndPrint<TBenchmarksContainer>()
            {
                Console.WriteLine($"{typeof(TBenchmarksContainer).Name}");
                Console.WriteLine($"{separator}{Environment.NewLine}");
                var report = runner.Run<TBenchmarksContainer>();
                return report;
            }
        }

        private static void SaveToFileAndOpenInExplorer(string json)
        {
            var jsonFileFolderPath = System.IO.Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                "TinyBenchmark",
                "json_tests");

            var jsonFilePath = System.IO.Path.Combine(jsonFileFolderPath, "benchmarksReport.json");

            if (!System.IO.Directory.Exists(jsonFileFolderPath))
                System.IO.Directory.CreateDirectory(jsonFileFolderPath);

            System.IO.File.WriteAllText(jsonFilePath, json);

            System.Diagnostics.Process.Start("explorer.exe", jsonFileFolderPath);
        }
    }
}
