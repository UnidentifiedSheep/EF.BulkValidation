namespace BulkValidation.Core.Models;

public record ValidationConfig(string MessageTemplate, Type? ErrorType = null);