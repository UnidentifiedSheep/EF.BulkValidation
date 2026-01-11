using BulkValidation.Core.Models;

namespace BulkValidation.Core.Interfaces;

public interface IHasValues
{
    IReadOnlyList<TypedObject> Values { get; }
}