namespace Sitecore.AspNetCore.SDK.ExperienceEditor.TagHelpers;

/// <summary>
/// Contract that supports serialization for the Chrome Data.
/// </summary>
public interface IChromeDataSerializer
{
    /// <summary>
    /// Serializes the given data to the string in JSON format.
    /// </summary>
    /// <param name="chromeData">The data for serialization.</param>
    /// <returns>The JSON string.</returns>
    string Serialize(Dictionary<string, object?> chromeData);
}