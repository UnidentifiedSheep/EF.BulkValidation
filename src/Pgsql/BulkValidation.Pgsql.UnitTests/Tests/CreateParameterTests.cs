using BulkValidation.Pgsql.UnitTests.TestClasses;
using BulkValidation.Pgsql.UnitTests.TestEnums;
using NpgsqlTypes;
// ReSharper disable BitwiseOperatorOnEnumWithoutFlags

namespace BulkValidation.Pgsql.UnitTests.Tests;

public class CreateParameterTests
{
    [Fact]
    public void CreateParameter_Int_SetsIntegerType()
    {
        var builder = new TestPgsqlRuleSqlBuilder();

        var param = builder.CreateParameterPublic("p", 5, typeof(int));

        Assert.Equal("p", param.ParameterName);
        Assert.Equal(5, param.Value);
        Assert.Equal(NpgsqlDbType.Integer, param.NpgsqlDbType);
    }
    
    [Fact]
    public void CreateParameter_NullValue_UsesDBNull()
    {
        var builder = new TestPgsqlRuleSqlBuilder();

        var param = builder.CreateParameterPublic("p", null, typeof(int?));

        Assert.Equal(DBNull.Value, param.Value);
    }
    
    [Fact]
    public void CreateParameter_IntArray_SetsArrayType()
    {
        var builder = new TestPgsqlRuleSqlBuilder();

        var param = builder.CreateArrayParameterPublic("p", [1, 2, 3], typeof(int));

        Assert.Equal(NpgsqlDbType.Array | NpgsqlDbType.Integer, param.NpgsqlDbType);

        var array = Assert.IsType<int[]>(param.Value);
        Assert.Equal(new[] { 1, 2, 3 }, array);
    }
    
    [Fact]
    public void CreateParameter_ValueTypeArray_WithNull_Throws()
    {
        var builder = new TestPgsqlRuleSqlBuilder();

        var ex = Assert.Throws<InvalidOperationException>(() =>
            builder.CreateArrayParameterPublic("p", [1, null], typeof(int))
        );

        Assert.Contains("Null values are not allowed for value type array", ex.Message);
    }

    [Fact]
    public void CreateParameter_EnumArray_UsesUnderlyingType()
    {
        var builder = new TestPgsqlRuleSqlBuilder();

        var param = builder.CreateArrayParameterPublic("p", [TestEnum.A, TestEnum.B], typeof(TestEnum));

        Assert.Equal(NpgsqlDbType.Array | NpgsqlDbType.Integer, param.NpgsqlDbType);
    }

    [Theory]
    [InlineData(typeof(int), 1, NpgsqlDbType.Integer)]
    [InlineData(typeof(long), 1L, NpgsqlDbType.Bigint)]
    [InlineData(typeof(string), "test", NpgsqlDbType.Text)]
    [InlineData(typeof(Guid), "00000000-0000-0000-0000-000000000001", NpgsqlDbType.Uuid)]
    public void CreateParameter_MapsTypesCorrectly(Type type, object value, NpgsqlDbType expected)
    {
        var builder = new TestPgsqlRuleSqlBuilder();

        if (type == typeof(Guid) && value is string s)
            value = Guid.Parse(s);

        var param = builder.CreateParameterPublic("p", value, type);

        Assert.Equal(expected, param.NpgsqlDbType);
    }
}