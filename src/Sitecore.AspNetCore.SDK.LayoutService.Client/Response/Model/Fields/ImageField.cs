using System.Diagnostics.CodeAnalysis;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Response.Model.Properties;

namespace Sitecore.AspNetCore.SDK.LayoutService.Client.Response.Model.Fields;

/// <summary>
/// Represents an image field.
/// </summary>
public class ImageField : EditableField<Image>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ImageField"/> class.
    /// </summary>
    public ImageField()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ImageField"/> class.
    /// </summary>
    /// <param name="value">The initial value.</param>
    [SetsRequiredMembers]
    public ImageField(Image value)
    {
        ArgumentNullException.ThrowIfNull(value);
        Value = value;
    }
}