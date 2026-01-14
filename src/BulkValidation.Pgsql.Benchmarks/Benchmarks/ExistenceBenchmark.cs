using BenchmarkDotNet.Attributes;
using BulkValidation.Core.Models;
using BulkValidation.Core.Plan;
using BulkValidation.Pgsql.Benchmarks.Contexts;
using BulkValidation.Pgsql.DbValidators;
using BulkValidation.Pgsql.Executors;
using BulkValidation.Pgsql.Factories;
using BulkValidation.Pgsql.MetadataResolvers;
using BulkValidation.Pgsql.SqlBuilders;
using Microsoft.EntityFrameworkCore;

namespace BulkValidation.Pgsql.Benchmarks.Benchmarks;

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

        // заранее готовим случайные данные
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
        
        await _dbValidator.Validate(plan);
    }
}
