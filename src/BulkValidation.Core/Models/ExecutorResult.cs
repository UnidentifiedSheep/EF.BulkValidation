namespace BulkValidation.Core.Models;

public record ExecutorResult<TValue>(string ColumnName, TValue Value);