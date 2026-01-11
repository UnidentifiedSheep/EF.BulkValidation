using System.Linq.Expressions;

namespace BulkValidation.Core.Interfaces;

public interface IHasKeySelectors
{
    IReadOnlyList<LambdaExpression> KeySelectors { get; }
}