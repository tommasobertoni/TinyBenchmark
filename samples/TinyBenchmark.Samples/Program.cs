using System;
using System.Linq;
using System.Text;

namespace TinyBenchmark.Samples
{
    class Program
    {
        static void Main(string[] args)
        {
            var sequenceReport = new BenchmarkRunner()
                .InSequence(config => config
                    .Append<LinqBenchmarks>()
                    .Append<LinqBenchmarks>())
                .Run();

            Console.WriteLine($"{Environment.NewLine}[Sequence]{Environment.NewLine}");
            PrettyPrint(sequenceReport);

            Console.Write($"{Environment.NewLine}Press ENTER to exit...");
            Console.ReadLine();

            // Local functions

            void PrettyPrint(BenchmarksCollectionReport preport)
            {
                const string separator = "------------------------------";

                Console.WriteLine($"Executed {preport.ContainerReports.Sum(x => x.Reports.Count)} benchmarks in parallel, in {preport.ContainerReports.Count} containers,");
                Console.WriteLine($"execution completed in {preport.Elapsed}{Environment.NewLine}");

                var formatterContainers = preport.ContainerReports.Select(StringifyContainer);
                var combined = string.Join($"{Environment.NewLine}{separator}{Environment.NewLine}", formatterContainers);

                Console.WriteLine(separator);
                Console.WriteLine(combined);
                Console.WriteLine(separator);
            }

            string StringifyContainer(BenchmarksContainerReport conainerReport)
            {
                var sb = new StringBuilder();

                if (conainerReport.Name != null)
                    sb.AppendLine($"Container: {conainerReport.Name} ({conainerReport.BenchmarkContainerType.FullName})");
                else
                    sb.AppendLine($"Container: {conainerReport.BenchmarkContainerType.FullName}");

                sb.AppendLine();
                sb.AppendLine($"Total benchmarks container duration:");
                sb.AppendLine($"- Started: {conainerReport.StartedAtUtc}");
                sb.AppendLine($"- Elapsed: {conainerReport.Elapsed}");

                if (conainerReport.Reports?.Any() == true)
                {
                    sb.AppendLine();
                    var formattedReports = conainerReport.Reports.Select(Stringify);
                    var combined = string.Join($"{Environment.NewLine}", formattedReports);
                    sb.AppendLine(combined);
                }

                return sb.ToString();
            }

            string Stringify(BenchmarkReport report)
            {
                var sb = new StringBuilder();

                if (report.Name != null)
                    sb.AppendLine($"  {report.Name}");

                sb.AppendLine($"  - Started: {report.StartedAtUtc}");
                sb.AppendLine($"  - Elapsed: {report.Elapsed}");

                if (report.Failed)
                {
                    sb.AppendLine();
                    sb.AppendLine($"  Exception: {report.Exception}");
                }

                return sb.ToString();
            }
        }
    }
}
