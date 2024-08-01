using System.Diagnostics.CodeAnalysis;

namespace Sitecore.AspNetCore.SDK.LayoutService.Client.Response.Model.Fields;

/// <summary>
/// Represents a text field.
/// </summary>
public class TextField : EditableField<string>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TextField"/> class.
    /// </summary>
    [SetsRequiredMembers]
    public TextField()
    {
        Value = string.Empty;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TextField"/> class.
    /// </summary>
    /// <param name="value">The initial value.</param>
    [SetsRequiredMembers]
    public TextField(string value)
    {
        // ReSharper disable once NullCoalescingConditionIsAlwaysNotNullAccordingToAPIContract - Guard to ensure the contract
        Value = value ?? string.Empty;
    }
}