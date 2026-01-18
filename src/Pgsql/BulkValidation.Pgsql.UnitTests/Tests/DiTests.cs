using BulkValidation.Base.Interfaces;
using BulkValidation.Core.Abstractions;
using BulkValidation.Core.Interfaces;
using BulkValidation.Core.Models;
using BulkValidation.Pgsql.Extensions;
using BulkValidation.Pgsql.UnitTests.TestClasses;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace BulkValidation.Pgsql.UnitTests.Tests;

public class DiTests
{
    [Fact]
    public void AllServices_ShouldResolveSuccessfully()
    {
        var services = new ServiceCollection();
        services.AddScoped<TestDbContext>(_ => new TestDbContext());

        services.AddPgsqlDbValidators<TestDbContext>();

        var provider = services.BuildServiceProvider();

        var counter = provider.GetService<SharedCounter>();
        Assert.NotNull(counter);

        var resolver = provider.GetService<IMetadataResolver<TestDbContext>>();
        Assert.NotNull(resolver);

        var executor = provider.GetService<ISqlExecutor<NpgsqlParameter>>();
        Assert.NotNull(executor);

        var validator = provider.GetService<IDbValidator<TestDbContext, NpgsqlParameter>>();
        Assert.NotNull(validator);

        var factory = provider.GetService<IRuleSqlBuilderFactory<NpgsqlParameter>>();
        Assert.NotNull(factory);

        var combined = provider.GetService<CombinedSqlBuilderBase<NpgsqlParameter>>();
        Assert.NotNull(combined);

        var builders = provider.GetServices<RuleSqlBuilderBase<NpgsqlParameter>>();
        Assert.NotEmpty(builders);
    }

    [Fact]
    public void AllBuilders_ShouldBeUnique()
    {
        var services = new ServiceCollection();
        services.AddScoped<TestDbContext>(_ => new TestDbContext());
        services.AddPgsqlDbValidators<TestDbContext>();
        var provider = services.BuildServiceProvider();

        var builders = provider.GetServices<RuleSqlBuilderBase<NpgsqlParameter>>();
        var builderTypes = new HashSet<Type>();
        foreach (var b in builders)
        {
            Assert.DoesNotContain(b.GetType(), builderTypes);
            builderTypes.Add(b.GetType());
        }
    }
}