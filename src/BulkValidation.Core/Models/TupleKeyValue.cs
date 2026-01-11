using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using BulkValidation.Core.Abstractions;
using BulkValidation.Core.Enums;
using BulkValidation.Core.Interfaces;

namespace BulkValidation.Core.Models;

public sealed class TupleKeyValue<TEntity, TTuple> : RuleKeyValuePairBase<TEntity>, IHasKeySelectors, IHasValues
{
    private bool _newValueSet;

    public IReadOnlyList<Expression<Func<TEntity, object>>> KeySelectors { get; }
    public IReadOnlyList<object?> Values { get; private set; } = [];

    IReadOnlyList<LambdaExpression> IHasKeySelectors.KeySelectors => KeySelectors;

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

    public TupleKeyValue(Expression<Func<TEntity, object>>[] keySelectors, params object?[] values)
    {
        if (keySelectors.Length != values.Length)
            throw new ArgumentException("Key selectors count must be equal to values count.");

        ValidateKeyValueTypes(keySelectors, values);

        KeySelectors = keySelectors;
        Values = values.ToArray();
        KeyValueType = KeyValueType.Tuple;
        _newValueSet = true;
    }
    
    public TupleKeyValue(Expression<Func<TEntity, object>>[] keySelectors, TTuple value)
    {
        KeySelectors = keySelectors;
        KeyValueType = KeyValueType.Tuple;
        WithValue(value);
    }

    public TupleKeyValue(params Expression<Func<TEntity, object>>[] keySelectors)
    {
        KeySelectors = keySelectors;
        KeyValueType = KeyValueType.Tuple;
    }
    

    private void ValidateKeyValueTypes(Expression<Func<TEntity, object>>[] keySelectors, object?[] values)
    {
        for (int i = 0; i < keySelectors.Length; i++)
        {
            var selector = keySelectors[i];
            var value = values[i];

            var selectorType = GetSelectorReturnType(selector);

            if (value is null)
            {
                if (selectorType.IsValueType && Nullable.GetUnderlyingType(selectorType) == null)
                    throw new ArgumentException($"Null is not allowed for non-nullable selector '{selector}'.");
                continue;
            }

            var valueType = value.GetType();
            if (!selectorType.IsAssignableFrom(valueType))
                throw new ArgumentException($"Value type '{valueType}' does not match selector type '{selectorType}' " + 
                                            $"for selector '{selector}'.");
        }
    }

    private TypedObject[] GetTypedObjects()
    {
        var typedObjects = new TypedObject[KeySelectors.Count];
        for (int i = 0; i < KeySelectors.Count; i++)
        {
            var selector = KeySelectors[i];
            var value = Values[i];

            var selectorType = GetSelectorReturnType(selector);
            typedObjects[i] = new TypedObject(value, selectorType);
        }

        _newValueSet = false;
        return typedObjects;
    }

    public void WithValue(TTuple values)
    {
        Values = values switch
        {
            ITuple tuple => Enumerable.Range(0, tuple.Length).Select(i => tuple[i]).ToArray(),
            _ => throw new ArgumentException($"Expected ValueTuple type, got {values?.GetType()}")
        };

        if (Values.Count != KeySelectors.Count)
            throw new ArgumentException($"KeySelectors count ({KeySelectors.Count}) does not match tuple values count ({Values.Count})");

        _newValueSet = true;
    }
    
    

    public override void WithValue(object? value)
    {
        if (value is not TTuple typed)
            throw new ArgumentException($"Expected {typeof(TTuple)}, got {value?.GetType()}");

        WithValue(typed);
    }
}