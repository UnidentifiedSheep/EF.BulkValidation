namespace BulkValidation.Core.Models;

public class ExecutorResult<TValue>
{
    public TValue Value { get; }
    public string ColumnName { get; }
    public ExecutorResult(string columnName, TValue value)
    {
        ColumnName = columnName;
        Value = value;
    }
}