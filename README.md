# ConcurrentThreadsBenchmark
Benchmark of concurrent threads in C#

## Running solution

Build a Release output and run
```
dotnet C:\your-path\ConcurrentThreadsBenchmark\bin\Release\netcoreapp2.2\ConcurrentThreadsBenchmark.dll
```
wait results and then check output folder

```
C:\your-path\ConcurrentThreadsBenchmark\bin\Release\netcoreapp2.2\BenchmarkDotNet.Artifacts\results
```

## First run 1000 records, 8 threads

``` ini

BenchmarkDotNet=v0.11.3, OS=Windows 10.0.17763.253 (1809/October2018Update/Redstone5)
Intel Core i7-3632QM CPU 2.20GHz (Ivy Bridge), 1 CPU, 8 logical and 4 physical cores
.NET Core SDK=2.2.200-preview-009648
  [Host] : .NET Core 2.2.0 (CoreCLR 4.6.27110.04, CoreFX 4.6.27110.04), 64bit RyuJIT
  Core   : .NET Core 2.2.0 (CoreCLR 4.6.27110.04, CoreFX 4.6.27110.04), 64bit RyuJIT

Job=Core  Runtime=Core  

```
|                 Method |     Mean |    Error |    StdDev |   Median | Rank | Gen 0/1k Op | Gen 1/1k Op | Gen 2/1k Op | Allocated Memory/Op |
|----------------------- |---------:|---------:|----------:|---------:|-----:|------------:|------------:|------------:|--------------------:|
|     RunConcurrentQueue | 232.8 ms | 6.075 ms | 17.912 ms | 225.0 ms |    1 |           - |           - |           - |            36.06 KB |
|   RunWithSemaphoreSlim | 235.1 ms | 4.630 ms |  7.208 ms | 234.5 ms |    1 |           - |           - |           - |             2.71 KB |
| RunWithParallelForeach | 237.5 ms | 4.631 ms |  7.479 ms | 237.6 ms |    1 |           - |           - |           - |            36.74 KB |
|           RunWithPolly | 232.8 ms | 4.627 ms |  5.851 ms | 231.4 ms |    1 |           - |           - |           - |           719.79 KB |

`RunWithSemaphoreSlim` consumes less memory which is interesting if you intend to save resources. Third party libraryPolly spends much more memory per operation.

## Second run 500 records, 8 threads

``` ini

BenchmarkDotNet=v0.11.3, OS=Windows 10.0.17763.253 (1809/October2018Update/Redstone5)
Intel Core i7-3632QM CPU 2.20GHz (Ivy Bridge), 1 CPU, 8 logical and 4 physical cores
.NET Core SDK=2.2.200-preview-009648
  [Host] : .NET Core 2.2.0 (CoreCLR 4.6.27110.04, CoreFX 4.6.27110.04), 64bit RyuJIT
  Core   : .NET Core 2.2.0 (CoreCLR 4.6.27110.04, CoreFX 4.6.27110.04), 64bit RyuJIT

Job=Core  Runtime=Core  

```
|                 Method |     Mean |    Error |   StdDev |   Median | Rank | Gen 0/1k Op | Gen 1/1k Op | Gen 2/1k Op | Allocated Memory/Op |
|----------------------- |---------:|---------:|---------:|---------:|-----:|------------:|------------:|------------:|--------------------:|
|     RunConcurrentQueue | 113.1 ms | 2.213 ms | 3.312 ms | 113.1 ms |    1 |           - |           - |           - |            19.63 KB |
|   RunWithSemaphoreSlim | 122.2 ms | 2.302 ms | 2.463 ms | 122.1 ms |    2 |           - |           - |           - |             2.66 KB |
| RunWithParallelForeach | 121.3 ms | 2.376 ms | 4.463 ms | 121.9 ms |    2 |           - |           - |           - |            21.19 KB |
|           RunWithPolly | 119.1 ms | 2.349 ms | 3.368 ms | 120.9 ms |    2 |           - |           - |           - |           360.24 KB |

`RunConcurrentQueue` is the fastest for processing a list of 500 items. But `RunWithSemaphoreSlim` takes less memory. `RunWithParallelForeach` takes almost the same time used by `RunWithSemaphoreSlim`.