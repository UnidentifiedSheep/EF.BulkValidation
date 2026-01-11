namespace BulkValidation.Core.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public class ValidateTupleAttribute : Attribute
{
    public readonly string Association;
    public ValidateTupleAttribute(string association)
    {
        if (string.IsNullOrWhiteSpace(association)) throw new ArgumentNullException(nameof(association));
        Association = association;
    }
}