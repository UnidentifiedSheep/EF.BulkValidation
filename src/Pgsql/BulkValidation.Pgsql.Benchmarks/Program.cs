using BenchmarkDotNet.Running;
using BulkValidation.Pgsql.Benchmarks.Benchmarks;

BenchmarkRunner.Run<ExistenceBenchmark>();