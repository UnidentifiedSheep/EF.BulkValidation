```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26200.6899/25H2/2025Update/HudsonValley2)
12th Gen Intel Core i5-12500H 2.50GHz, 1 CPU, 16 logical and 12 physical cores
.NET SDK 10.0.100
  [Host]     : .NET 10.0.0 (10.0.0, 10.0.25.52411), X64 RyuJIT x86-64-v3
  DefaultJob : .NET 10.0.0 (10.0.0, 10.0.25.52411), X64 RyuJIT x86-64-v3


```
| Method                | Mean     | Error    | StdDev   | Gen0    | Gen1   | Allocated |
|---------------------- |---------:|---------:|---------:|--------:|-------:|----------:|
| BatchExistenceRequest | 85.29 μs | 1.705 μs | 3.202 μs | 25.1465 | 5.1270 | 232.15 KB |
