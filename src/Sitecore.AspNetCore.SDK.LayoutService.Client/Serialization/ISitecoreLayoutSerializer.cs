using Sitecore.AspNetCore.SDK.LayoutService.Client.Response;

namespace Sitecore.AspNetCore.SDK.LayoutService.Client.Serialization;

/// <summary>
/// Contract that supports serialization for the Sitecore layout service.
/// </summary>
public interface ISitecoreLayoutSerializer
{
    /// <summary>
    /// Deserializes the given data to a <see cref="SitecoreLayoutResponseContent"/>.
    /// </summary>
    /// <param name="data">The data to deserialize.</param>
    /// <returns>The deserialized <see cref="SitecoreLayoutResponseContent"/>.</returns>
    SitecoreLayoutResponseContent? Deserialize(string data);
}