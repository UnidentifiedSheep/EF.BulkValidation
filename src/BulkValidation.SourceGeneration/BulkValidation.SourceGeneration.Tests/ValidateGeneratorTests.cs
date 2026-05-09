using FluentAssertions;
using Xunit;

namespace BulkValidation.SourceGeneration.Tests;

public class ValidateGeneratorTests
{
    [Fact]
    public void Should_Generate_For_Nested_Value_Object()
    {
        var source = """
                     using BulkValidation.Core.Attributes;

                     namespace Test;

                     public class User
                     {
                         [Validate]
                         public Name Name { get; set; }
                     }

                     public record Name
                     {
                         [Validate]
                         public string Value { get; init; }
                     }
                     """;

        var (compilation, result) =
            RoslynTestHelper.Run(source, new SourceGeneratorValidateAttr());

        var generated = RoslynTestHelper.GetGeneratedText(compilation);

        result.Diagnostics.Should().BeEmpty();

        generated.Should().Contain("ValidateUserExistsName");
        generated.Should().Contain("ValidateUserExistsNameValue");

        generated.Should().Contain("x => x.Name");
        generated.Should().Contain("x => x.Name.Value");
    }
}