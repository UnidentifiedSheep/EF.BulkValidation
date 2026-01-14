namespace BulkValidation.SourceGeneration.Models;

public class ArgumentMetadata
{
    public string Type { get; private set; }
    public string Name { get; private set; }
    public string? DefaultValue { get; private set; }
    
    public ArgumentMetadata(string type, string name, string? defaultValue)
    {
        Type = type;
        Name = name;
        DefaultValue = defaultValue;
    }
}