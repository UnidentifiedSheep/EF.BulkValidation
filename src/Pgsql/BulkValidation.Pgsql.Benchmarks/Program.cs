using BenchmarkDotNet.Running;
using BulkValidation.Pgsql.Benchmarks.Benchmarks;
using BulkValidation.Pgsql.Benchmarks.Contexts;
using Microsoft.EntityFrameworkCore;

BenchmarkRunner.Run<ExistenceBenchmark>();