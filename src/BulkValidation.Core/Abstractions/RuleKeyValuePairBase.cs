using System.Linq.Expressions;
using BulkValidation.Core.Enums;

namespace BulkValidation.Core.Abstractions;

public abstract class RuleKeyValuePairBase<TEntity> : RuleKeyValuePairBase
{
    public override Type EntityType => typeof(TEntity);

    protected Type GetSelectorReturnType(Expression<Func<TEntity, object>> selector)
    {
        Expression body = selector.Body;

        if (body is UnaryExpression { NodeType: ExpressionType.Convert } unary)
            body = unary.Operand;

        return body.Type;
    }
}


public abstract class RuleKeyValuePairBase
{
    public KeyValueType KeyValueType { get; protected set; }
    public abstract Type EntityType { get; }
    public abstract void WithValue(object? value);
}