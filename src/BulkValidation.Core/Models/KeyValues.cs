using System.Linq.Expressions;
using BulkValidation.Core.Abstractions;
using BulkValidation.Core.Enums;
using BulkValidation.Core.Interfaces;

namespace BulkValidation.Core.Models;

public sealed class KeyValues<TEntity, TKey> : RuleKeyValuePairBase<TEntity>, IHasKeySelector, IHasValues
{
    private bool _newValueSet;
    public Expression<Func<TEntity, TKey>> KeySelector { get; }
    public IReadOnlyList<TKey?> Values { get; private set; }
    public Type KeyType { get; }

    LambdaExpression IHasKeySelector.KeySelector => KeySelector;
    IReadOnlyList<TypedObject> IHasValues.Values
    {
        get
        {
            if (_newValueSet)
                field = GetTypedObjects();
            if (Values.Count == 0)
                throw new InvalidOperationException("Values are not set. Use WithValue() method to set values.");
            return field ??= GetTypedObjects();
        }
    }

    public KeyValues(Expression<Func<TEntity, TKey>> keySelector, params TKey[] values)
    {
        KeySelector = keySelector;
        Values = values.ToArray();
        KeyValueType = KeyValueType.MultipleKeys;
        KeyType = typeof(TKey);
        _newValueSet = true;
    }
    
    public KeyValues(Expression<Func<TEntity, TKey>> keySelector)
    {
        KeySelector = keySelector;
        Values = [];
        KeyValueType = KeyValueType.MultipleKeys;
        KeyType = typeof(TKey);
    }

    private TypedObject[] GetTypedObjects()
    {
        _newValueSet = false;
        return Values.Select(v => new TypedObject(v, KeyType)).ToArray();
    }

    public void WithValue(params TKey[] values)
    {
        if (values.Length == 0)
            throw new ArgumentException("Value cannot be empty for multiple keys.");
        Values = values;
    }
    
    public override void WithValue(object? value)
    {
        Values = value switch
        {
            null => throw new ArgumentException("Value cannot be null for multiple keys."),
            TKey?[] array => array,
            IReadOnlyList<TKey?> list => list,
            IEnumerable<TKey?> enumerable => [..enumerable],
            _ => throw new ArgumentException(
                $"Value type '{value.GetType()}' does not match expected type 'IEnumerable<{typeof(TKey)}>'.")
        };
        _newValueSet = true;
    }
}
