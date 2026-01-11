using Microsoft.CodeAnalysis;


namespace BulkValidation.SourceGeneration;

[Generator]
public class SourceGeneratorAttributes : IIncrementalGenerator
{
    public const string AttributeNamespace = "BulkValidation.Core.Attributes";
    public const string ValidateAttributeText = """
                                                namespace BulkValidation.Core.Attributes
                                                {
                                                    [System.AttributeUsage(System.AttributeTargets.Property)]
                                                    public sealed class ValidateAttribute : System.Attribute
                                                    {
                                                    }
                                                }
                                                """;

    public const string ValidateTupleAttributeText = """
                                                     namespace BulkValidation.Core.Attributes;
                                                     
                                                     [System.AttributeUsage(System.AttributeTargets.Property)]
                                                     public sealed class ValidateTupleAttribute : System.Attribute
                                                     {
                                                         public readonly string Association;
                                                         public ValidateTupleAttribute(string association)
                                                         {
                                                             if (string.IsNullOrWhiteSpace(association)) 
                                                                throw new System.ArgumentNullException(nameof(association));
                                                             Association = association;
                                                         }
                                                     }
                                                     """;
    
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(ctx =>
        {
            ctx.AddSource("ValidateAttribute.g.cs", ValidateAttributeText);
            ctx.AddSource("ValidateTupleAttribute.g.cs", ValidateTupleAttributeText);
        });
    }
}