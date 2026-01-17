using System.Collections.Concurrent;
using BulkValidation.Core.Enums;
using BulkValidation.Core.Models;

namespace BulkValidation.Core.Configuration;

public static class ConfigureDbValidation
{
    private static readonly ConcurrentDictionary<(string, KeyValueType), ValidationConfig> Configs = new();

    public static void AddConfig(string func, KeyValueType kvType, Func<ValidationConfig> configAction)
        => Configs.GetOrAdd((func, kvType), _ => configAction());
    
    public static ValidationConfig? GetConfig(string func, KeyValueType kvType)
        => Configs.GetValueOrDefault((func, kvType));
    
}