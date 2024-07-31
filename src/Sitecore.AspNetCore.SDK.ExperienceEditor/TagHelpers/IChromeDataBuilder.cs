using Sitecore.AspNetCore.SDK.ExperienceEditor.TagHelpers.Model;

namespace Sitecore.AspNetCore.SDK.ExperienceEditor.TagHelpers;

/// <summary>
/// Contract for configuring Chrome Data clients.
/// </summary>
public interface IChromeDataBuilder
{
    /// <summary>
    /// Maps <see cref="EditButtonBase"/> object to <see cref="ChromeCommand"/>.
    /// </summary>
    /// <param name="button">The edit button to build a ChromeCommand.</param>
    /// <param name="itemId">The ID of the item the EditFrame is associated with.</param>
    /// <param name="parameters">Additional parameters passed to the EditFrame.</param>
    /// <returns>Instance of <see cref="ChromeCommand"/>.</returns>
    ChromeCommand MapButtonToCommand(EditButtonBase button, string? itemId, IDictionary<string, object?>? parameters);
}