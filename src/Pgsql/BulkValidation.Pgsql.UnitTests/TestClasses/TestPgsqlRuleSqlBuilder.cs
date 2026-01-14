using BulkValidation.Core.Models;
using BulkValidation.Pgsql.Abstractions;
using Npgsql;

namespace BulkValidation.Pgsql.UnitTests.TestClasses;

internal sealed class TestPgsqlRuleSqlBuilder : PgsqlRuleSqlBuilderBase<TestRule>
{
    protected override SqlCommand<NpgsqlParameter> BuildSql(TestRule rule)
    {
        throw new NotImplementedException();
    }
    
    public NpgsqlParameter CreateParameterPublic(string name, object? value, Type type)
        => CreateParameter(name, value, type);

    public NpgsqlParameter CreateArrayParameterPublic(string name, object?[] values, Type type)
        => CreateParameter(name, values, type);
}