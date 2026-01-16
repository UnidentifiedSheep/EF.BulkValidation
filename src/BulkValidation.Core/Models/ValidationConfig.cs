namespace BulkValidation.Core.Models;

public class ValidationConfig
{
    public string MessageTemplate { get; set; } = string.Empty;
    public Type? ErrorType { get; set; }
}