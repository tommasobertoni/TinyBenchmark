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

            if (System.Diagnostics.Debugger.IsAttached)
            {
                Console.Write($"{Environment.NewLine}Press ENTER to exit...");
                Console.ReadLine();
            }

            // Local functions

            void PrettyPrint(BenchmarksCollectionReport preport)
            {
                const string separator = "------------------------------";

                Console.WriteLine($"Executed {preport.ContainerReports.Sum(x => x.Reports.Count)} benchmarks");
                Console.WriteLine($"with a total of {preport.ContainerReports.Sum(x => x.Reports.Sum(r => r.IterationReports.Count))} iterations");
                Console.WriteLine($"in {preport.ContainerReports.Count} containers");
                Console.WriteLine($"execution completed in {preport.Elapsed}{Environment.NewLine}");

                var formatterContainers = preport.ContainerReports.Select(cr => StringifyContainer(cr, groupByParameters: true));
                var combined = string.Join($"{Environment.NewLine}{separator}{Environment.NewLine}", formatterContainers);

                Console.WriteLine(separator);
                Console.WriteLine(combined);
                Console.WriteLine(separator);
            }

            string StringifyContainer(BenchmarksContainerReport containerReport, bool groupByParameters)
            {
                var sb = new StringBuilder();

                if (containerReport.Name != null)
                    sb.AppendLine($"Container: {containerReport.Name} ({containerReport.BenchmarkContainerType.FullName})");
                else
                    sb.AppendLine($"Container: {containerReport.BenchmarkContainerType.FullName}");

                sb.AppendLine();
                sb.AppendLine($"Total benchmarks container duration:");
                sb.AppendLine($"- Started: {containerReport.StartedAtUtc.ToLocalTime()}");
                sb.AppendLine($"- Elapsed: {containerReport.Elapsed}");

                if (containerReport.Reports?.Any() == true)
                {
                    sb.AppendLine();

                    if (groupByParameters)
                    {
                        var formattedReportNames = containerReport.Reports.Select(StringifyName);
                        var combinedNames = string.Join($"", formattedReportNames);
                        sb.AppendLine($"- Benchmarks:");
                        sb.AppendLine(combinedNames);

                        var allParametersSets = containerReport.Reports
                            .SelectMany(r => r.IterationReports.Select(ir => ir.Parameters))
                            .ToHashSet();

                        foreach (var p in allParametersSets)
                        {
                            if (p == null)
                                sb.AppendLine($"  Benchmarks without parameters {Environment.NewLine}");
                            else
                                sb.AppendLine($"  Benchmarks with parameters: {p}{Environment.NewLine}");

                            foreach (var report in containerReport.Reports)
                            {
                                var iterationsWithParameters = report.IterationReports.Where(ir => ir.Parameters == p);
                                var successfulIterations = iterationsWithParameters.Where(ir => ir.Failed == false);
                                var failedIterations = iterationsWithParameters.Where(ir => ir.Failed);

                                sb.AppendLine($"    Iterations of benchmark: {report.Name}");

                                foreach (var ir in successfulIterations)
                                {
                                    sb.AppendLine($"    - Elapsed: {ir.Elapsed}");

                                    //sb.AppendLine($"        Started: {ir.StartedAtUtc.ToLocalTime()}");

                                    //if (report.Warmup > TimeSpan.FromMilliseconds(1))
                                    //    sb.AppendLine($"        Warmup:  {ir.Warmup}");
                                }

                                if (failedIterations.Any())
                                    sb.AppendLine($"    - Failed iterations: {failedIterations.Count()}");

                                sb.AppendLine();
                            }
                        }
                    }
                    else
                    {
                        var formattedReports = containerReport.Reports.Select(Stringify);
                        var combined = string.Join($"{Environment.NewLine}", formattedReports);
                        sb.AppendLine(combined);
                    }
                }

                return sb.ToString();
            }

            string StringifyName(BenchmarkReport report)
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

                return sb.ToString();
            }

            string Stringify(BenchmarkReport report)
            {
                var sb = new StringBuilder();
                sb.Append(StringifyName(report));
                sb.AppendLine($"  - Started: {report.StartedAtUtc.ToLocalTime()}");

                if (report.Warmup > TimeSpan.FromMilliseconds(1))
                    sb.AppendLine($"  - Warmup:  {report.Warmup}");

                sb.AppendLine($"  - Elapsed: {report.Elapsed}");

                var successfulIterationReports = report.IterationReports.Where(ir => ir.Failed == false).ToList();
                if (successfulIterationReports.Count > 1)
                {
                    sb.AppendLine($"  - Elapsed of each iteration:");
                    foreach (var ir in successfulIterationReports)
                    {
                        if (ir.Parameters.Values?.Any() == true)
                        {
                            var parametersStrings = ir.Parameters.Values.Select(p => $"{p.PropertyName}:{p.Value}");
                            sb.AppendLine($"             {ir.Elapsed} with parameters: {string.Join(", ", parametersStrings)}");
                        }
                        else
                        {
                            sb.AppendLine($"             {ir.Elapsed}");
                        }
                    }
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
