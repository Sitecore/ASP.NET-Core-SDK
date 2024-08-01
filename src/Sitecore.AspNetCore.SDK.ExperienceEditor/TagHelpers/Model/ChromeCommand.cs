namespace Sitecore.AspNetCore.SDK.ExperienceEditor.TagHelpers.Model;

/// <summary>
/// This class contains fields for a command in the chrome data.
/// </summary>
public class ChromeCommand
{
    /// <summary>
    /// Gets or sets a value indicating whether is it divider or not.
    /// </summary>
    public bool IsDivider { get; set; }

    /// <summary>
    /// Gets or sets the type of command.
    /// </summary>
    public string? Type { get; set; }

    /// <summary>
    /// Gets or sets the header of command.
    /// </summary>
    public string Header { get; set; } = default!;

    /// <summary>
    /// Gets or sets the icon path of command.
    /// </summary>
    public string Icon { get; set; } = default!;

    /// <summary>
    /// Gets or sets the tooltip of command.
    /// </summary>
    public string? Tooltip { get; set; }

    /// <summary>
    /// Gets or sets the click action of command.
    /// </summary>
    public string Click { get; set; } = default!;
}