using System.Diagnostics.CodeAnalysis;
using File = Sitecore.AspNetCore.SDK.LayoutService.Client.Response.Model.Properties.File;

namespace Sitecore.AspNetCore.SDK.LayoutService.Client.Response.Model.Fields;

/// <summary>
/// Represents a file field.
/// </summary>
public class FileField : EditableField<File>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FileField"/> class.
    /// </summary>
    public FileField()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FileField"/> class.
    /// </summary>
    /// <param name="value">The initial value.</param>
    [SetsRequiredMembers]
    public FileField(File value)
    {
        ArgumentNullException.ThrowIfNull(value);
        Value = value;
    }
}