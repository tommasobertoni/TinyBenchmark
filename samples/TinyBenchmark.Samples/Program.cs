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

            //var collectionsBenchmarksReport = RunAndPrint<CollectionsBenchmarks>();
            var hashBenchmarksReport = RunAndPrint<HashBenchmarks>();
            //var linqBenchmarksReport = RunAndPrint<LinqBenchmarks>();
            //var miscBenchmarksReport = RunAndPrint<MiscBenchmarks>();
            //var noBenchmarksReport = RunAndPrint<NoBenchmarks>();

            Console.WriteLine();
            Console.WriteLine("Test:");

            var text = hashBenchmarksReport.ExportAsText();
            Console.WriteLine(text);

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
    }
}
