using System.Diagnostics.CodeAnalysis;

namespace Sitecore.AspNetCore.SDK.LayoutService.Client.Response.Model.Fields;

/// <summary>
/// Represents a textfield that supports HTML.
/// </summary>
public class RichTextField : EditableField<string>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RichTextField"/> class.
    /// </summary>
    [SetsRequiredMembers]
    public RichTextField()
    {
        Value = string.Empty;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RichTextField"/> class.
    /// </summary>
    /// <param name="value">The initial value.</param>
    /// <param name="encoded">True if the value is encoded, otherwise false. Defaults to true.</param>
    [SetsRequiredMembers]
    public RichTextField(string value, bool encoded = true)
    {
        // ReSharper disable once NullCoalescingConditionIsAlwaysNotNullAccordingToAPIContract - Guard to ensure the contract
        value ??= string.Empty;
        Value = Build(value, encoded);
    }

    private static string Build(string value, bool encoded)
    {
        return encoded ? System.Web.HttpUtility.UrlDecode(value) : value;
    }
}