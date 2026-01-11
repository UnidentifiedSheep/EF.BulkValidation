using System.Linq.Expressions;

namespace BulkValidation.Core.Interfaces;

public interface IMetadataResolver<in TContext>
{
    string GetColumnName(Type entityType, LambdaExpression keySelector);
    string GetTableName(Type entityType);
    string? GetSchemaName(Type entityType);
}