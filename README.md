# TinyBenchmark

[![Nuget](https://img.shields.io/nuget/v/TinyBenchmark.svg?logo=nuget)](https://www.nuget.org/packages/TinyBenchmark)
[![GitHub](https://img.shields.io/github/license/tommasobertoni/TinyBenchmark.svg)](https://github.com/tommasobertoni/TinyBenchmark/blob/master/LICENSE)
[![Samples](https://img.shields.io/badge/samples-3-brightgreen.svg)](https://github.com/tommasobertoni/TinyBenchmark/tree/master/samples/TinyBenchmark.Samples)

_Define benchmarks with ease!_

TinyBenchmark provides a simple set of APIs that enable you to test the time of execution of
different methods and comparison between each other.

_**Jump to**_

- _**Usage**_
  - [Run your first benchmark](#run-your-first-benchmark)
  - [Define benchmark arguments](#define-benchmark-arguments)
  - [Average the results over multiple iterations](#average-the-results-over-multiple-iterations)
  - [Named benchmarks](#named-benchmarks)
  - [Benchmarks comparison](#benchmarks-comparison)

<br />

#### Run your first benchmark

```csharp
class Demo
{
    public static void Main(string[] args)
    {
        var runner = new BenchmarkRunner();
        var report = runner.Run<BenchmarksContainer>();

        // Explore the data in the report!
        Console.WriteLine($"Total duration: {report.Duration}");
    }
}

class BenchmarksContainer
{
    [Benchmark]
    public void TestMe()
    {
        string token = "test";
        string msg = string.Empty;
        for (int i = 0; i < 10_000; i++)
            msg += token;
    }
}
```
<br />

#### Define benchmark arguments
```csharp
class BenchmarksContainer
{
    [Benchmark]
    [Arguments(10_000)]
    [Arguments(100_000)]
    /// <summary>
    /// This benchmark will be executed once for each [Arguments],
    /// with the "times" variable assigned to the value specified the attribute.
    /// </summary>
    public void TestMe(int times)
    {
        string token = "test";
        string msg = string.Empty;
        for (int i = 0; i < times; i++)
            msg += token;
    }
}
```
<br />

#### Average the results over multiple iterations
```csharp
class Demo
{
    public static void Main(string[] args)
    {
        var runner = new BenchmarkRunner();
        var report = runner.Run<BenchmarksContainer>();

        // Explore the report!
        Console.WriteLine($"Total duration: {report.Duration}");

        var benchmarkReport = report.Reports.First();
        Console.WriteLine($"Benchmark: {benchmarkReport.Name}");
        Console.WriteLine($"  average iteration duration: {benchmarkReport.AvgIterationDuration}");
    }
}

class BenchmarksContainer
{
    [Benchmark(Iterations = 3)]
    [Arguments(10_000)]
    [Arguments(100_000)]
    /// <summary>
    /// This benchmark will be executed once for each [Arguments]
    /// and once for each iteration, for a total of 6 runs.
    /// </summary>
    public void TestMe(int times)
    {
        string token = "test";
        string msg = string.Empty;
        for (int i = 0; i < times; i++)
            msg += token;
    }
}
```
<br />

#### Named benchmarks
```csharp
class BenchmarksContainer
{
    [Benchmark(Name = "String concatenation")]
    public void StringConcatenation()
    {
        string msg = string.Empty;
        for (int i = 0; i < 50_000; i++)
            msg += "test";
    }
}
```
<br />

#### Benchmarks comparison
```csharp
public class BasicBenchmarks
{
    private readonly string _token = "test";
    private readonly int _tokensCount = 50_000;

    [Benchmark(Name = "String concatenation")]
    public void StringConcatenation()
    {
        string msg = string.Empty;
        for (int i = 0; i < _tokensCount; i++)
            msg += _token;
    }

    [Benchmark(Name = "Concatenation using StringBuilder")]
    public void StringBuilder()
    {
        var sb = new StringBuilder();
        for (int i = 0; i < _tokensCount; i++)
            sb.Append(_token);

        var msg = sb.ToString();
    }
}
```
