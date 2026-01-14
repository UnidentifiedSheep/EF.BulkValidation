using BulkValidation.Core.Abstractions;
using Npgsql;
using NpgsqlTypes;
// ReSharper disable BitwiseOperatorOnEnumWithoutFlags

namespace BulkValidation.Pgsql.Abstractions;

public abstract class PgsqlRuleSqlBuilderBase<TRule> : RuleSqlBuilderBase<TRule, NpgsqlParameter> where TRule : BaseRule
{
    protected override NpgsqlParameter CreateParameter(string name, object?[] keys, Type keyType)
    {
        var elementType = Nullable.GetUnderlyingType(keyType) ?? keyType;

        var typedArray = ConvertToTypedArray(keys, elementType);

        var param = new NpgsqlParameter
        {
            ParameterName = name,
            Value = typedArray,
            NpgsqlDbType = NpgsqlDbType.Array | ResolveDbTypeFromValues(keys, elementType)
        };

        return param;
    }
    
    protected override NpgsqlParameter CreateParameter(string name, object? key, Type keyType)
    {
        var param = new NpgsqlParameter
        {
            ParameterName = name,
            Value = key ?? DBNull.Value
        };

        keyType = Nullable.GetUnderlyingType(keyType) ?? keyType;

        param.NpgsqlDbType = ResolveDbType(key, keyType);

        return param;
    }
    
    private static NpgsqlDbType ResolveDbTypeFromValues(object?[] values, Type elementType)
    {
        if (elementType == typeof(DateTime))
        {
            if (values.OfType<DateTime>().Any(v => v.Kind == DateTimeKind.Utc))
                return NpgsqlDbType.TimestampTz;

            return NpgsqlDbType.Timestamp;
        }

        return ResolveDbType(null, elementType);
    }

    protected Array ConvertToTypedArray(object?[] keys, Type elementType)
    {
        if (keys.Any(k => k is null) && elementType.IsValueType)
            throw new InvalidOperationException( 
                $"Null values are not allowed for value type array '{elementType.Name}'.");

        if (elementType.IsEnum)
            elementType = Enum.GetUnderlyingType(elementType);

        var arr = Array.CreateInstance(elementType, keys.Length);

        for (int i = 0; i < keys.Length; i++)
            arr.SetValue(keys[i], i);

        return arr;
    }
    
    private static NpgsqlDbType ResolveDbType(object? value, Type type)
    {
        if (type.IsEnum)
            return NpgsqlDbType.Integer;

        if (type == typeof(Guid))
            return NpgsqlDbType.Uuid;

        if (type == typeof(string))
            return NpgsqlDbType.Text;

        if (type == typeof(bool))
            return NpgsqlDbType.Boolean;

        if (type == typeof(byte) || type == typeof(short))
            return NpgsqlDbType.Smallint;

        if (type == typeof(int))
            return NpgsqlDbType.Integer;

        if (type == typeof(long))
            return NpgsqlDbType.Bigint;

        if (type == typeof(float))
            return NpgsqlDbType.Real;

        if (type == typeof(double))
            return NpgsqlDbType.Double;

        if (type == typeof(decimal))
            return NpgsqlDbType.Numeric;

        if (type == typeof(byte[]))
            return NpgsqlDbType.Bytea;

        if (type == typeof(DateTime))
            return value is DateTime { Kind: DateTimeKind.Utc } ? NpgsqlDbType.TimestampTz : NpgsqlDbType.Timestamp;
        

        if (type == typeof(DateTimeOffset))
            return NpgsqlDbType.TimestampTz;

        if (type == typeof(TimeSpan))
            return NpgsqlDbType.Interval;

        if (type == typeof(object))
            return NpgsqlDbType.Jsonb;

        return NpgsqlDbType.Text;
    }
}