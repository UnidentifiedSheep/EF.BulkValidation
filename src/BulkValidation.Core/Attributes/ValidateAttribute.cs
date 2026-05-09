using BulkValidation.Core.Enums;

namespace BulkValidation.Core.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public class ValidateAttribute(
    string fieldName = "",
    MappingMode mapping = MappingMode.Default)
    : Attribute
{
    public string FieldName { get; } = fieldName;
    public MappingMode Mapping { get; } = mapping;
}