using System.Linq.Expressions;
using BulkValidation.Core.Abstractions;
using BulkValidation.Core.Enums;
using BulkValidation.Core.Interfaces;

namespace BulkValidation.Core.Models;

public sealed class KeyValue<TEntity, TKey> : RuleKeyValuePairBase<TEntity>, IHasKeySelector, IHasValue
{
    public Expression<Func<TEntity, TKey>> KeySelector { get; }
    public TKey? Value { get; private set; }
    public Type KeyType { get; }
    
    LambdaExpression IHasKeySelector.KeySelector => KeySelector;
    object? IHasValue.Value => Value;
    
    public KeyValue(Expression<Func<TEntity, TKey>> keySelector, TKey value)
    {
        (KeySelector, Value) = (keySelector, value);
        KeyValueType = KeyValueType.Single;
        KeyType = typeof(TKey);
    }
    
    public KeyValue(Expression<Func<TEntity, TKey>> keySelector)
    {
        KeySelector = keySelector;
        KeyValueType = KeyValueType.Single;
        KeyType = typeof(TKey);
    }

    public void WithValue(TKey value) => Value = value;

    // Для полиморфизма
    public override void WithValue(object? value)
    {
        if (value is not TKey typed)
            throw new ArgumentException($"Expected {typeof(TKey)}, got {value?.GetType()}");
        Value = typed;
    }
}