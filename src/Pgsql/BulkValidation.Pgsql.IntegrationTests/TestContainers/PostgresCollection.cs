namespace BulkValidation.Pgsql.IntegrationTests.TestContainers;

[CollectionDefinition("Postgres collection")]
public class PostgresCollection : ICollectionFixture<PostgresContainerFixture>
{
}