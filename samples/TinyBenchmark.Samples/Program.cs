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
                    .Append<CollectionsBenchmarks>()
                    .Append<MiscBenchmarks>())
                .Run();

            PrettyPrint(sequenceReport);

            Console.Write($"{Environment.NewLine}Press ENTER to exit...");
            Console.ReadLine();

            // Local functions

            void PrettyPrint(BenchmarksCollectionReport preport)
            {
                const string separator = "------------------------------";

                Console.WriteLine($"Executed {preport.ContainerReports.Sum(x => x.Reports.Count)} benchmarks, in {preport.ContainerReports.Count} containers,");
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
                {
                    if (report.IterationReports.Count == 1)
                        sb.AppendLine($"  {report.Name}");
                    else
                    {
                        if (report.IterationReports.Count == report.SuccessfulIterations)
                            sb.AppendLine($"  {report.Name}: {report.SuccessfulIterations} iterations");
                        else
                        {
                            var successPercentage = report.SuccessfulIterations * 100.0 / report.IterationReports.Count;
                            sb.AppendLine($"  {report.Name}: {report.SuccessfulIterations} successful iterations of {report.IterationReports.Count} total iterations ({successPercentage}%)");
                        }
                    }
                }

                sb.AppendLine($"  - Started: {report.StartedAtUtc}");
                sb.AppendLine($"  - Warmup:  {report.Warmup}");
                sb.AppendLine($"  - Elapsed: {report.Elapsed}");

                var successfulIterationReports = report.IterationReports.Where(ir => ir.Failed == false).ToList();
                if (successfulIterationReports.Count > 1)
                {
                    var allElapsedTimes = successfulIterationReports.Select(ir => ir.Elapsed);
                    sb.AppendLine($"  - Elapsed of each iteration:");
                    foreach (var t in allElapsedTimes)
                        sb.AppendLine($"             {t}");
                }

                var failedIterationReports = report.IterationReports.Where(ir => ir.Failed).ToList();
                if (failedIterationReports.Any())
                {
                    sb.AppendLine($"  - Failed iterations: {failedIterationReports.Count}");
                    foreach (var ir in failedIterationReports)
                        sb.AppendLine($"    {ir.Exception?.Message}");
                }

                return sb.ToString();
            }
        }
    }
}
