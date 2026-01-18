using System.Text;
using BulkValidation.Core.Abstractions;
using BulkValidation.Core.Enums;
using BulkValidation.Core.Interfaces;
using BulkValidation.Core.Models;
using BulkValidation.Core.Rules;
using BulkValidation.Pgsql.Abstractions;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace BulkValidation.Pgsql.SqlBuilders;

public class PgsqlNotExistenceRuleSqlBuilder<TContext>(IMetadataResolver<TContext> metadataResolver, SharedCounter sharedCounter)
    : PgsqlRuleSqlBuilderBase<NotExistenceRule> where TContext : DbContext
{
    protected override SqlCommand<NpgsqlParameter> BuildSql(NotExistenceRule rule)
    {
        var kvp = rule.RuleKeyValue;

        return kvp.KeyValueType switch
        {
            KeyValueType.Single => BuildForSingle(kvp),
            KeyValueType.Tuple => BuildForTuple(kvp),
            KeyValueType.MultipleKeys => BuildForMultipleKeys(kvp, rule.Quantifier),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    private SqlCommand<NpgsqlParameter> BuildForSingle(RuleKeyValuePairBase kvp)
    {
        var keySelector = (IHasKeySelector)kvp;
        var value = (IHasValue)kvp;

        var fieldName = metadataResolver.GetColumnName(kvp.EntityType, keySelector.KeySelector);
        var schema = metadataResolver.GetSchemaName(kvp.EntityType);
        var tableName = metadataResolver.GetTableName(kvp.EntityType);
        var sIndex = sharedCounter.GetNextInt();
        var paramName = $"@p{sIndex}";
        var columnName = GetReturnColumnName(tableName, fieldName, sIndex);
        
        var sql = $"""
                   EXISTS (
                    SELECT 1
                    FROM "{schema}"."{tableName}"
                    WHERE "{fieldName}" = {paramName}
                    LIMIT 1
                   ) AS {columnName}
                   """;
        
        return new SqlCommand<NpgsqlParameter>(sql, columnName, CreateParameter(paramName, value.Value, keySelector.KeyType));
    }

    private SqlCommand<NpgsqlParameter> BuildForTuple(RuleKeyValuePairBase kvp)
    {
        var keySelectors = (IHasKeySelectors)kvp;
        var values = (IHasValues)kvp;

        var schema = metadataResolver.GetSchemaName(kvp.EntityType);
        var tableName = metadataResolver.GetTableName(kvp.EntityType);

        //(field1, field2, field3, ...)
        StringBuilder fieldsString = new StringBuilder("(");
        //(param1, param2, param3, ...)
        StringBuilder paramsString = new StringBuilder("(");
        NpgsqlParameter[] parameters = new NpgsqlParameter[keySelectors.KeySelectors.Count];
        
        for (int i = 0; i < keySelectors.KeySelectors.Count; i++)
        {
            var keySelector = keySelectors.KeySelectors[i];
            var (value, type) = values.Values[i];
            var paramName = $"@p{sharedCounter.GetNextInt()}";
            var fieldName = metadataResolver.GetColumnName(kvp.EntityType, keySelector);
            
            fieldsString.Append($"{fieldName}, ");
            paramsString.Append($"{paramName}, ");
            parameters[i] = CreateParameter(paramName, value, type);
        }

        fieldsString.Length -= 2; //remove last ", "
        paramsString.Length -= 2; //remove last ", "

        fieldsString.Append(')');
        paramsString.Append(')');
        
        var columnName = GetReturnColumnName(tableName, "manyFields", sharedCounter.GetNextInt());
        
        var sql = $"""
                   EXISTS (
                    SELECT 1
                    FROM "{schema}"."{tableName}"
                    WHERE {fieldsString} = {paramsString}
                    LIMIT 1
                   ) AS {columnName}
                   """;
        return new SqlCommand<NpgsqlParameter>(sql, columnName, parameters);
    }

    private SqlCommand<NpgsqlParameter> BuildForMultipleKeys(RuleKeyValuePairBase kvp, Quantifier quantifier)
    {
        var keySelector = (IHasKeySelector)kvp;
        var values = (IHasValues)kvp;
        
        var fieldName = metadataResolver.GetColumnName(kvp.EntityType, keySelector.KeySelector);
        var schema = metadataResolver.GetSchemaName(kvp.EntityType);
        var tableName = metadataResolver.GetTableName(kvp.EntityType);

        int sIndex = sharedCounter.GetNextInt();
        var paramName = $"@p{sIndex}";
        var columnName = GetReturnColumnName(tableName, fieldName, sharedCounter.GetNextInt());
        
        string sql = quantifier switch
        {
            Quantifier.Any => $"""
                               EXISTS (
                                   SELECT 1
                                   FROM "{schema}"."{tableName}"
                                   WHERE "{fieldName}" = ANY({paramName})
                                   LIMIT 1
                               ) AS {columnName}
                               """,

            Quantifier.All => $"""
                               (
                                   SELECT COUNT(DISTINCT t."{fieldName}")
                                   FROM "{schema}"."{tableName}" t
                                   WHERE t."{fieldName}" = ANY({paramName})
                               ) = cardinality({paramName}) AS {columnName}
                               """,

            _ => throw new NotSupportedException($"Quantifier {quantifier} is not supported")
        };

        var objs = values.Values.Select(x => x.Value).Distinct().ToArray();
        
        return new SqlCommand<NpgsqlParameter>(sql, columnName, CreateParameter(paramName, objs, keySelector.KeyType));
    }
}