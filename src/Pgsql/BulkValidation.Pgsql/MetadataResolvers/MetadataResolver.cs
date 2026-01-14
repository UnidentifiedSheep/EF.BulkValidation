using System.Collections.Concurrent;
using System.Linq.Expressions;
using BulkValidation.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
// ReSharper disable StaticMemberInGenericType

namespace BulkValidation.Pgsql.MetadataResolvers;

public class MetadataResolver<TContext>(TContext context) : IMetadataResolver<TContext> where TContext : DbContext
{
    private static readonly ConcurrentDictionary<(Type, string), string> ColumnNameCache = new();
    private static readonly ConcurrentDictionary<Type, string> TableNameCache = new();
    private static readonly ConcurrentDictionary<Type, string?> SchemaNameCache = new();
    
    public string GetColumnName(Type entityType, LambdaExpression keySelector)
    {
        var memberExpression = keySelector.Body as MemberExpression
                               ?? (keySelector.Body as UnaryExpression)?.Operand as MemberExpression
                               ?? throw new InvalidOperationException("KeySelector does not refer to a property.");

        var propertyName = memberExpression.Member.Name;

        return ColumnNameCache.GetOrAdd((entityType, propertyName), _ =>
        {
            var efEntity = context.Model.FindEntityType(entityType) ?? 
                           throw new InvalidOperationException($"Entity type '{entityType.Name}' was not found in the EF Core model.");

            var property = efEntity.FindProperty(propertyName) ?? 
                           throw new InvalidOperationException($"Property '{propertyName}' was not found on entity '{entityType.Name}'.");

            return property.GetColumnName() ?? throw new InvalidOperationException("Failed to resolve the database column name.");
        });
    }

    public string GetTableName(Type entityType)
    {
        return TableNameCache
            .GetOrAdd(entityType, _ => context.Model.FindEntityType(entityType)?.GetTableName()
                                       ?? throw new ArgumentNullException(
                                           $"Entity {entityType.Name} is not mapped to a table in the EF Core model."));
    }

    public string? GetSchemaName(Type entityType)
    {
        return SchemaNameCache.GetOrAdd(entityType, _ =>
        {
            var schema = context.Model.FindEntityType(entityType)?.GetSchema();
            return string.IsNullOrWhiteSpace(schema) ? "public" : schema;
        });
    }
}