using System.Linq.Expressions;

namespace BulkValidation.Core.Interfaces;

public interface IHasKeySelector
{
    LambdaExpression KeySelector { get; }
    Type KeyType { get; }
}