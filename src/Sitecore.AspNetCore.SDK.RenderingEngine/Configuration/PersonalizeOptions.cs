﻿using Sitecore.AspNetCore.SDK.LayoutService.Client.Extensions;

namespace Sitecore.AspNetCore.SDK.RenderingEngine.Configuration;

/// <summary>
/// Options for personalization.
/// </summary>
public class PersonalizeOptions
{
    /// <summary>
    /// Gets or sets the channel for the personalization call.
    /// </summary>
    /// <remarks>
    /// Matches https://github.com/Sitecore/jss/blob/48d1fb1a44cb0678d350f34e740b927dcf759755/packages/sitecore-jss-nextjs/src/middleware/personalize-middleware.ts#L152 implementation.
    /// </remarks>
    public string Channel { get; set; } = "WEB";

    /// <summary>
    /// Gets or sets the currency for the personalization call.
    /// </summary>
    /// <remarks>
    /// Matches https://github.com/Sitecore/jss/blob/48d1fb1a44cb0678d350f34e740b927dcf759755/packages/sitecore-jss-nextjs/src/middleware/personalize-middleware.ts#L153 implementation.
    /// </remarks>
    public string Currency { get; set; } = "USD";

    /// <summary>
    /// Gets or sets the endpoint.
    /// </summary>
    public Uri Endpoint { get; set; } = new("https://edge-platform.sitecorecloud.io/v1/personalize");

    /// <summary>
    /// Gets or sets the context id.
    /// </summary>
    public string ContextId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the query string key for the context id.
    /// </summary>
    public string ContextIdQueryStringKey { get; set; } = "sitecoreContextId";

    /// <summary>
    /// Gets or sets the default variant value.
    /// </summary>
    public string DefaultVariant { get; set; } = "_default";

    /// <summary>
    /// Gets or sets the default language.
    /// </summary>
    public string DefaultLanguage { get; set; } = "en";

    /// <summary>
    /// Gets or sets the default site.
    /// </summary>
    public string DefaultSite { get; set; } = "website";

    /// <summary>
    /// Gets or sets the cache duration for PersonalizeInfo.
    /// </summary>
    public TimeSpan CacheDuration { get; set; } = TimeSpan.FromMinutes(5);

    /// <summary>
    /// Gets the personalize endpoint URI.
    /// </summary>
    /// <returns>Endpoint or Endpoint with ContextId query string if filled.</returns>
    public Uri? GetPersonalizeEndpointUri()
    {
        return string.IsNullOrWhiteSpace(ContextId) ? Endpoint : Endpoint.AddQueryString(ContextIdQueryStringKey, ContextId);
    }
}