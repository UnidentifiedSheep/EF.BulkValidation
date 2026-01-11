using Microsoft.CodeAnalysis;

namespace BulkValidation.SourceGeneration;

[Generator]
public class SourceGeneratorValidationPlanShortCut : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var optionsProvider = context.AnalyzerConfigOptionsProvider
            .Select((provider, _) =>
            {
                provider.GlobalOptions.TryGetValue(
                    "mygenerator_mode",
                    out var mode);

                return mode;
            });
        
        context.RegisterSourceOutput(
            context.CompilationProvider.Combine(optionsProvider),
            (_, t) =>
            {
                var compilation = t.Left;
                var coreNamespace = t.Right;
            });
    }
}