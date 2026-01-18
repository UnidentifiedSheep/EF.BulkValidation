using BulkValidation.Pgsql.Benchmarks.Contexts;
using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;

namespace BulkValidation.Pgsql.IntegrationTests.TestContainers;

public class PostgresContainerFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgresqlContainer = new PostgreSqlBuilder()
        .WithImage("postgres:latest")
        .Build();

    public string ConnectionString => _postgresqlContainer.GetConnectionString();

    public async Task InitializeAsync()
    {
        await _postgresqlContainer.StartAsync();
        var connectionOptions = new DbContextOptionsBuilder<DContext>().UseNpgsql(ConnectionString).Options;
        await using var context = new DContext(connectionOptions);
        await context.Database.EnsureCreatedAsync();
        Console.WriteLine("✅ PostgreSQL container started, database ensured created.");
    }

    public async Task DisposeAsync()
    {
        await _postgresqlContainer.DisposeAsync().AsTask();
    }
}