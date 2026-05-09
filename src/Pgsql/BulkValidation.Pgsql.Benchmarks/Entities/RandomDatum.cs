using System;
using System.Collections.Generic;
using BulkValidation.Core.Attributes;

namespace BulkValidation.Pgsql.Benchmarks.Entities;

public partial class RandomDatum
{
    [Validate]
    public Guid Guid { get; set; }
    
    [Validate]
    public Name Name { get; set; } = new();
    
    [Validate]
    [ValidateTuple("PK")]
    public int First { get; set; }
    
    [Validate]
    [ValidateTuple("PK")]
    public int Second { get; set; }
}

public record Name
{
    [Validate]
    public string Value { get; set; } = string.Empty;
    
    public Name() {}

    public Name(string name)
    {
        Value = name;
    }
}