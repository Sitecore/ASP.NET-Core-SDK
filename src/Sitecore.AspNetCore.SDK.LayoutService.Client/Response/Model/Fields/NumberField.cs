using System.Diagnostics.CodeAnalysis;

namespace Sitecore.AspNetCore.SDK.LayoutService.Client.Response.Model.Fields;

/// <summary>
/// Represents a number field.
/// </summary>
public class NumberField : EditableField<decimal?>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NumberField"/> class.
    /// </summary>
    public NumberField()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NumberField"/> class.
    /// </summary>
    /// <param name="value">The initial value.</param>
    [SetsRequiredMembers]
    public NumberField(double value)
    {
        ArgumentNullException.ThrowIfNull(value);
        Value = Convert.ToDecimal(value);
    }
}