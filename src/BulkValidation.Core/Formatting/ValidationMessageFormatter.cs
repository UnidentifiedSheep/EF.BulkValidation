using System.Collections;

namespace BulkValidation.Core.Formatting;

public static class ValidationMessageFormatter
{
    public static string SafeFormat(this string? template, object? value)
    {
        if (string.IsNullOrWhiteSpace(template))
            return string.Empty;

        try
        {
            return string.Format(template, value);
        }
        catch (FormatException)
        {
            return template;
        }
    }

    public static string? FormatValue(this object? value)
    {
        switch (value)
        {
            case null:
                return null;
            case string s:
                return s;
        }

        if (value is not IEnumerable enumerable)
            return value.ToString();

        var items = enumerable.Cast<object?>()
            .Select(v => v?.ToString());

        return string.Join(", ", items);
    }
}