namespace Sitecore.AspNetCore.SDK.Tracking.VisitorIdentification;

/// <summary>
/// Options for Sitecore Visitor Identification functionality.
/// </summary>
public class SitecoreVisitorIdentificationOptions
{
    /// <summary>
    /// Gets or sets Uri to Sitecore instance, which will serve proxied requests.
    /// It is used to handle requests to Visitor identification pages located in [sitecore host name]/layouts/System.
    /// </summary>
    public Uri? SitecoreInstanceUri { get; set; }
}