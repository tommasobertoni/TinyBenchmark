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

            //var rb = new ReflectionBenchmarks();
            //rb.Manual();

            var runner = new BenchmarkRunner(OutputLevel.Minimal);

            //var collectionsBenchmarksReport = RunAndPrint<CollectionsBenchmarks>();
            //var hashBenchmarksReport = RunAndPrint<HashBenchmarks>();
            //var linqBenchmarksReport = RunAndPrint<LinqBenchmarks>();
            var benchmarksReport = RunAndPrint<CollectionsBenchmarks>();
            //var miscBenchmarksReport = RunAndPrint<MiscBenchmarks>();
            //var noBenchmarksReport = RunAndPrint<NoBenchmarks>();

            var text = benchmarksReport.ExportAsText(includeIterations: false);
            Console.WriteLine(text);

            var json = benchmarksReport.ExportAsJson(formatted: true);

            SaveToFileAndOpenInExplorer(json);

            //Console.WriteLine();
            //Console.WriteLine("Json:");

            //var json = hashBenchmarksReport.ExportAsJson();
            //Console.WriteLine(json);

            //Demo01_FirstBenchmark.Program.Main(args);
            //Demo02_BenchmarkArguments.Program.Main(args);
            //Demo03_MultipleIterations.Program.Main(args);
            //Demo04_NamedBenchmarks.Program.Main(args);
            //Demo05_BenchmarksComparison.Program.Main(args);
            //Demo06_BenchmarksOutput.Program.Main(args);

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
