using System.Linq.Expressions;
using BulkValidation.Core.Enums;
using BulkValidation.Core.Interfaces;
using BulkValidation.Core.Models;
using BulkValidation.Core.Rules;
using BulkValidation.Pgsql.SqlBuilders;
using BulkValidation.Pgsql.UnitTests.TestClasses;
using Microsoft.EntityFrameworkCore;
using Moq;
using NpgsqlTypes;
// ReSharper disable BitwiseOperatorOnEnumWithoutFlags

namespace BulkValidation.Pgsql.UnitTests.Tests;

public class PgsqlExistenceRuleSqlBuilderTests
{
    private readonly Mock<IMetadataResolver<DbContext>> _metadataResolverMock;

    public PgsqlExistenceRuleSqlBuilderTests()
    {
        _metadataResolverMock = new Mock<IMetadataResolver<DbContext>>();
        _metadataResolverMock.Setup(m => m.GetColumnName(It.IsAny<Type>(), It.IsAny<LambdaExpression>()))
            .Returns("Id");
        _metadataResolverMock.Setup(m => m.GetTableName(It.IsAny<Type>()))
            .Returns("DummyTable");
        _metadataResolverMock.Setup(m => m.GetSchemaName(It.IsAny<Type>()))
            .Returns("public");
        
    }
    
    private PgsqlExistenceRuleSqlBuilder<DbContext> Builder => new(_metadataResolverMock.Object, new SharedCounter());

    [Fact]
    public void BuildSql_KeyValue_UsesMetadataResolver()
    {
        var guid = Guid.NewGuid();
        var kv = new KeyValue<Dummy, Guid>(x => x.Id, guid);
        var rule = new ExistenceRule<Dummy>(kv);
        
        var sqlCommand = Builder.BuildSql(rule);
        
        Assert.Contains("EXISTS", sqlCommand.Sql);
        Assert.Contains("@p1", sqlCommand.Sql);
        Assert.Single(sqlCommand.Parameters);
        Assert.Equal(guid, sqlCommand.Parameters[0].Value);
        Assert.Equal(NpgsqlDbType.Uuid, sqlCommand.Parameters[0].NpgsqlDbType);
    }
    
    [Fact]
    public void BuildSql_TupleKeyValue_UsesMetadataResolver()
    {
        var guid = Guid.NewGuid();
        var kv = new TupleKeyValue<Dummy, (Guid, string)>(x => x.Id, x => x.Name);
        kv.WithValue((guid, "test"));
        
        var rule = new ExistenceRule<Dummy>(kv);
        
        var sqlCommand = Builder.BuildSql(rule);
        
        Assert.Contains("EXISTS", sqlCommand.Sql);
        Assert.Contains("@p1", sqlCommand.Sql);
        Assert.Contains("@p2", sqlCommand.Sql);
        Assert.Equal(2, sqlCommand.Parameters.Count);
        
        Assert.Equal(guid, sqlCommand.Parameters[0].Value);
        Assert.Equal(NpgsqlDbType.Uuid, sqlCommand.Parameters[0].NpgsqlDbType);
        
        Assert.Equal("test", sqlCommand.Parameters[1].Value);
        Assert.Equal(NpgsqlDbType.Text, sqlCommand.Parameters[1].NpgsqlDbType);
    }
    
    [Fact]
    public void BuildSql_KeyValuesAny_UsesMetadataResolver()
    {
        var guid = Guid.NewGuid();
        var otherGuid = Guid.NewGuid();
        var kv = new KeyValues<Dummy, Guid>(x => x.Id);
        kv.WithValue(guid, otherGuid);
        var rule = new ExistenceRule<Dummy>(kv, Quantifier.Any);
        
        var sqlCommand = Builder.BuildSql(rule);
        
        Assert.Contains("@p1", sqlCommand.Sql);
        Assert.Single(sqlCommand.Parameters);
        
        Assert.Equal(new[] { guid, otherGuid }, sqlCommand.Parameters[0].Value);
        Assert.Equal(NpgsqlDbType.Uuid | NpgsqlDbType.Array, sqlCommand.Parameters[0].NpgsqlDbType);
    }
    
    [Fact]
    public void BuildSql_KeyValuesAll_UsesMetadataResolver()
    {
        var guid = Guid.NewGuid();
        var otherGuid = Guid.NewGuid();
        var kv = new KeyValues<Dummy, Guid>(x => x.Id);
        kv.WithValue(guid, otherGuid);
        var rule = new ExistenceRule<Dummy>(kv, Quantifier.All);
        
        var sqlCommand = Builder.BuildSql(rule);
        
        Assert.Contains("@p1", sqlCommand.Sql);
        Assert.Single(sqlCommand.Parameters);
        
        Assert.Equal(new[] { guid, otherGuid }, sqlCommand.Parameters[0].Value);
        Assert.Equal(NpgsqlDbType.Uuid | NpgsqlDbType.Array, sqlCommand.Parameters[0].NpgsqlDbType);
    }
}