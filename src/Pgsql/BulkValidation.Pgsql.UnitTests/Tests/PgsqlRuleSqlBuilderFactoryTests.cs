using BulkValidation.Core.Rules;
using BulkValidation.Pgsql.Factories;
using BulkValidation.Pgsql.SqlBuilders;
using BulkValidation.Pgsql.UnitTests.TestClasses;
using Microsoft.EntityFrameworkCore;

namespace BulkValidation.Pgsql.UnitTests.Tests;

public class PgsqlRuleSqlBuilderFactoryTests
{
    [Fact]
    public void GetBuilder_ReturnsRegisteredBuilder()
    {
        var builder = new TestPgsqlRuleSqlBuilder(); 
        var factory = new PgsqlRuleSqlBuilderFactory([builder]);

        var result = factory.GetBuilder(typeof(TestRule));

        Assert.Same(builder, result);
    }
    
    [Fact]
    public void GetBuilder_ThrowsForUnknownRule()
    {
        var builder = new TestPgsqlRuleSqlBuilder();
        var factory = new PgsqlRuleSqlBuilderFactory([builder]);

        Assert.Throws<NotSupportedException>(() => factory.GetBuilder(typeof(Dummy)));
    }

    [Fact]
    public void GetBuilder_ReturnsCorrectBuilder_WhenMultipleRegistered()
    {
        var builder1 = new TestPgsqlRuleSqlBuilder();
        var builder2 = new PgsqlExistenceRuleSqlBuilder<DbContext>(null!, null!);
        var factory = new PgsqlRuleSqlBuilderFactory([builder1, builder2]);

        Assert.Same(builder1, factory.GetBuilder(typeof(TestRule)));
        Assert.Same(builder2, factory.GetBuilder(typeof(ExistenceRule)));
    }

}