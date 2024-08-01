using System.Diagnostics.CodeAnalysis;

namespace Sitecore.AspNetCore.SDK.LayoutService.Client.Response.Model.Fields;

/// <summary>
/// Represents a checkbox field.
/// </summary>
public class CheckboxField : EditableField<bool>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CheckboxField"/> class.
    /// </summary>
    [SetsRequiredMembers]
    public CheckboxField()
    {
        Value = default;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CheckboxField"/> class.
    /// </summary>
    /// <param name="value">The initial value.</param>
    [SetsRequiredMembers]
    public CheckboxField(bool value)
    {
        Value = value;
    }
}