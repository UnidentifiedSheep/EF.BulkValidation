using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace BulkValidation.SourceGeneration.Tests;

public static class RoslynTestHelper
{
    public static (Compilation OutputCompilation, GeneratorDriverRunResult Result) Run(
        string source,
        IIncrementalGenerator generator)
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(source);

        var references = AppDomain.CurrentDomain
            .GetAssemblies()
            .Where(a => !a.IsDynamic && !string.IsNullOrWhiteSpace(a.Location))
            .Select(a => MetadataReference.CreateFromFile(a.Location))
            .ToList();

        var compilation = CSharpCompilation.Create(
            assemblyName: "Tests",
            syntaxTrees: new[] { syntaxTree },
            references: references,
            options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        GeneratorDriver driver = CSharpGeneratorDriver.Create(generator.AsSourceGenerator());

        driver = driver.RunGeneratorsAndUpdateCompilation(
            compilation,
            out var outputCompilation,
            out _);

        var result = driver.GetRunResult();

        return (outputCompilation, result);
    }

    public static string GetGeneratedText(Compilation compilation)
    {
        return string.Join(
            "\n",
            compilation.SyntaxTrees.Skip(1).Select(x => x.ToString()));
    }
}