using Sitecore.AspNetCore.SDK.LayoutService.Client.Request;

namespace Sitecore.AspNetCore.SDK.LayoutService.Client.Configuration;

/// <summary>
/// Options to control the <see cref="SitecoreLayoutRequest"/>.
/// </summary>
public class SitecoreLayoutRequestOptions
{
    private SitecoreLayoutRequest _requestDefaults = [];

    /// <summary>
    /// Gets or sets the default parameters for all requests made using the <see cref="DefaultLayoutClient"/>.
    /// </summary>
    public SitecoreLayoutRequest RequestDefaults
    {
        get => _requestDefaults;
        set => _requestDefaults = value ?? throw new ArgumentNullException(nameof(value));
    }
}