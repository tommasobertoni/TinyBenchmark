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
