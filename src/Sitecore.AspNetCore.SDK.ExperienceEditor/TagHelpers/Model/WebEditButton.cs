namespace Sitecore.AspNetCore.SDK.ExperienceEditor.TagHelpers.Model;

/// <summary>
/// This class represents web edit button.
/// </summary>
public class WebEditButton : EditButtonBase
{
    /// <summary>
    /// Gets or sets the click action of the button.
    /// </summary>
    public string? Click { get; set; }

    /// <summary>
    /// Gets or sets the type of button.
    /// </summary>
    public string? Type { get; set; }

    /// <summary>
    /// Gets or sets the additional parameters of the button.
    /// </summary>
    public IDictionary<string, object?>? Parameters { get; set; }
}