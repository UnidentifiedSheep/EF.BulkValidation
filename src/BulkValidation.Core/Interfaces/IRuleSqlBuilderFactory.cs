using System.Data.Common;
using BulkValidation.Core.Abstractions;

namespace BulkValidation.Core.Interfaces;

public interface IRuleSqlBuilderFactory<TParameter> where TParameter : DbParameter 
{
    RuleSqlBuilderBase<TParameter> GetBuilder(Type ruleType);
}