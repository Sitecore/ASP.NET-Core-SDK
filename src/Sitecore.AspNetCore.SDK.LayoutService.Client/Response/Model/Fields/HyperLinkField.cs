using System.Diagnostics.CodeAnalysis;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Response.Model.Properties;

namespace Sitecore.AspNetCore.SDK.LayoutService.Client.Response.Model.Fields;

/// <summary>
/// Represents a hyperlink field.
/// </summary>
public class HyperLinkField : WrappedEditableField<HyperLink>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="HyperLinkField"/> class.
    /// </summary>
    public HyperLinkField()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="HyperLinkField"/> class.
    /// </summary>
    /// <param name="value">The initial value.</param>
    [SetsRequiredMembers]
    public HyperLinkField(HyperLink value)
    {
        ArgumentNullException.ThrowIfNull(value);
        Value = value;
    }
}