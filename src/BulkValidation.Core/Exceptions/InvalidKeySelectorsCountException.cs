namespace BulkValidation.Core.Exceptions;

public class InvalidKeySelectorsCountException : Exception
{
    public InvalidKeySelectorsCountException() : base("Invalid key selectors count.") { }
}