namespace Sitecore.AspNetCore.SDK.ExperienceEditor.TagHelpers.Model;

/// <summary>
/// This class contains base fields for edit buttons.
/// </summary>
public abstract class EditButtonBase
{
    /// <summary>
    /// Gets or sets the title of the button.
    /// </summary>
    public virtual string? Header { get; set; }

    /// <summary>
    /// Gets or sets the icon path.
    /// </summary>
    public virtual string? Icon { get; set; }

    /// <summary>
    /// Gets or sets the tooltip of the button.
    /// </summary>
    public virtual string? Tooltip { get; set; }
}