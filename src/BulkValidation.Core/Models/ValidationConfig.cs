namespace BulkValidation.Core.Models;

public class ValidationConfig
{
    public string MessageTemplate { get; }
    public Type? ErrorType { get; }
    public ValidationConfig(string messageTemplate, Type? errorType = null)
    {
        MessageTemplate = messageTemplate;
        ErrorType = errorType;
    }
}