namespace BulkValidation.Core.Models;

public sealed record TypedObject
{
    public object? Value { get; }
    public Type Type { get; }

    public TypedObject(object? Value, Type Type)
    {
        this.Value = Value;
        this.Type = Type;
    }
    public void Deconstruct(out object? value, out Type type)
    {
        value = Value;
        type = Type;
    }
}