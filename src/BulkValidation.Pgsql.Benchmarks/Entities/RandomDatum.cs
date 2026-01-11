using System;
using System.Collections.Generic;
using BulkValidation.Core.Attributes;

namespace BulkValidation.Pgsql.Benchmarks.Entities;

public partial class RandomDatum
{
    [Validate]
    public Guid Guid { get; set; }
    
    [Validate]
    [ValidateTuple("PK")]
    public int First { get; set; }
    
    [Validate]
    [ValidateTuple("PK")]
    public int Second { get; set; }
}
