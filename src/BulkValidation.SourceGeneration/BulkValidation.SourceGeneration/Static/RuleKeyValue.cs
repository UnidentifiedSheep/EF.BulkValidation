using System.Linq;
using Microsoft.CodeAnalysis;

namespace BulkValidation.SourceGeneration.Static;

public static class RuleKeyValue
{
    public static string GenerateKeyValue(INamedTypeSymbol entity, IPropertySymbol key, string varName)
    {
        var entityName = entity.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        var keyType = key.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        
        return $"""
                 global::BulkValidation.Core.Models.KeyValue<{entityName}, {keyType}> {varName} =
                    new global::BulkValidation.Core.Models.KeyValue<{entityName}, {keyType}>(x => x.{key.Name});
                 """;
    }
    
    public static string GenerateKeyValues(INamedTypeSymbol entity, IPropertySymbol key, string varName)
    {
        var entityName = entity.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        var keyType = key.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        
        return $"""
                global::BulkValidation.Core.Models.KeyValues<{entityName}, {keyType}> {varName} =
                   new global::BulkValidation.Core.Models.KeyValues<{entityName}, {keyType}>(x => x.{key.Name});
                """;
    }
    
    public static string GenerateTupleKeyValue(INamedTypeSymbol entity, IPropertySymbol[] keys, string varName)
    {
        var entityName = entity.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        
        var tupleTypes = string.Join(", ", keys
            .Select(k => k.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)));
        var selectors = string.Join(", ", keys.Select(k => $"x => x.{k.Name}"));

        return $$"""
                  global::BulkValidation.Core.Models.TupleKeyValue<{{entityName}},({{tupleTypes}})> {{varName}} = 
                      new global::BulkValidation.Core.Models.TupleKeyValue<{{entityName}},({{tupleTypes}})>(
                                  new Expression<Func<{{entityName}}, object>>[] {{{selectors}}}
                              );
                  """;
    }
}