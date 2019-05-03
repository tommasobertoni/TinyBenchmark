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

            var runner = new BenchmarkRunner(OutputLevel.Verbose);

            var hashVsListReport = RunAndPrint<HashBenchmarks>();
            //var linqReport = RunAndPrint<LinqBenchmarks>();
            //var collectionsReport = RunAndPrint<CollectionsBenchmarks>();
            //var miscReport = RunAndPrint<MiscBenchmarks>();
            //var noBenchmarksReport = RunAndPrint<NoBenchmarks>();

            if (System.Diagnostics.Debugger.IsAttached)
            {
                Console.Write($"{Environment.NewLine}Press ENTER to exit...");
                Console.ReadLine();
            }

            // Local functions

            BenchmarksContainerReport RunAndPrint<TBenchmarksContainer>()
            {
                Console.Write($"{Environment.NewLine}{Environment.NewLine}");
                Console.WriteLine($"{typeof(TBenchmarksContainer).Name}");
                Console.WriteLine($"{separator}{Environment.NewLine}");
                var report = runner.Run<TBenchmarksContainer>();
                return report;
            }
        }
    }
}
