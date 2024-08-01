using System.Diagnostics.CodeAnalysis;

namespace Sitecore.AspNetCore.SDK.LayoutService.Client.Response.Model.Fields;

/// <summary>
/// Represents a date field.
/// </summary>
public class DateField : EditableField<DateTime>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DateField"/> class.
    /// </summary>
    [SetsRequiredMembers]
    public DateField()
    {
        Value = DateTime.MinValue;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DateField"/> class.
    /// </summary>
    /// <param name="value">The initial value.</param>
    [SetsRequiredMembers]
    public DateField(DateTime value)
    {
        ArgumentNullException.ThrowIfNull(value);
        Value = value;
    }
}