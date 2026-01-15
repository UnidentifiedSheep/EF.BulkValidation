## Benchmarks

This document shows the performance of EF.BulkValidation with PostgreSQL (Npgsql).
The goal is to show how batch validation reduces the number of database queries and memory usage compared to the usual approach.

## Test Scenario

We check the existence of records for the `RandomDatum` entity in two ways:

1. **NormalRequest** - the usual approach with `.AnyAsync()` in a loop
2. **BatchExistenceRequest** - batch validation via `ValidationPlan` + `PgsqlDbValidator`

### Environment:
* .NET 10.0
* EF Core 10
* PostgreSQL 18
* BenchmarkDotNet 0.15.8
* Local database with `RandomData` table

### Test data:

* 3 random numbers for `First` and `Second` columns
* 1 random `Guid` for the `Guid` column

## Results
|  Method |  Average time |  Error |  StdDev |  Gen0 | Memory   |
|---|---|---|---|---|----------|
| NormalRequest  | 3,456.0 μs |  66.97 μs |  79.72 μs |  - | 60.5 KB  |
| BatchExistenceRequest | 598.5 μs  |  11.77 μs |  20.31 μs | 1.9531  | 21.51 KB |


## Conclusions:

* Batch checking with `BatchExistenceRequest` is ~5.7 times faster than the usual approach.
* Memory usage is reduced by more than 2.5 times.
* The number of SQL queries is reduced to one per batch, which solves the N+1 problem.

### Benchmark code

```csharp
[MemoryDiagnoser]
public class ExistenceBenchmark
{
    private DContext _context = null!;
    private PgsqlDbValidator<DContext> _dbValidator = null!;
    private int[] _randomInts = null!;
    private Guid[] _randomGuids = null!;

    [GlobalSetup]
    public void Setup()
    {
        _context = new DContext();
        var sh = new SharedCounter();
        var md = new MetadataResolver<DContext>(_context);
        var factory = new PgsqlRuleSqlBuilderFactory(new[] {
            new PgsqlExistenceRuleSqlBuilder<DContext>(md, sh)
        });
        var comb = new PgsqlCombinedSqlBuilder();
        var executor = new PgsqlSqlExecutor<DContext>(_context, comb);
        _dbValidator = new PgsqlDbValidator<DContext>(factory, executor);

        var rnd = new Random();
        _randomInts = Enumerable.Range(0, 3).Select(_ => rnd.Next(1, 10_000_000)).ToArray();
        _randomGuids = Enumerable.Range(0, 1).Select(_ => Guid.NewGuid()).ToArray();
    }

    [Benchmark]
    public async Task NormalRequest()
    {
        foreach (var value in _randomInts)
            _ = await _context.RandomData.AsNoTracking().AnyAsync(x => x.First == value);

        foreach (var value in _randomInts)
            _ = await _context.RandomData.AsNoTracking().AnyAsync(x => x.Second == value);

        foreach (var value in _randomGuids)
            _ = await _context.RandomData.AsNoTracking().AnyAsync(x => x.Guid == value);
    }

    [Benchmark]
    public async Task BatchExistenceRequest()
    {
        var plan = new ValidationPlan();
        foreach (var value in _randomInts)
            plan.ValidateRandomDatumExistsFirst(value);

        foreach (var value in _randomInts)
            plan.ValidateRandomDatumExistsSecond(value);

        foreach (var value in _randomGuids)
            plan.ValidateRandomDatumExistsGuid(value);

        await _dbValidator.Validate(plan, false);
    }
}
```