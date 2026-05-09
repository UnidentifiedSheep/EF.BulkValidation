using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.CSharp.Testing;

namespace BulkValidation.SourceGeneration.Tests;

public sealed class GeneratorTest<TGenerator>
    : CSharpSourceGeneratorTest<TGenerator, DefaultVerifier>
    where TGenerator : IIncrementalGenerator, new()
{
    public GeneratorTest()
    {
        ReferenceAssemblies = ReferenceAssemblies.Net.Net100;
    }
}