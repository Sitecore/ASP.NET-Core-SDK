namespace Sitecore.AspNetCore.SDK.ExperienceEditor.TagHelpers.Model;

/// <summary>
/// This class represents the edit button which allows manipulation with defined fields.
/// </summary>
public class FieldEditButton : EditButtonBase
{
    /// <summary>
    /// Gets the collection of the field names.
    /// </summary>
    public IEnumerable<string> Fields { get; init; } = [];
}