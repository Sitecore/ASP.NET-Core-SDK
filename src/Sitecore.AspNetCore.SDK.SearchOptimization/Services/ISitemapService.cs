namespace Sitecore.AspNetCore.SDK.SearchOptimization.Services;

/// <summary>
/// Sitemap Service Interface.
/// </summary>
public interface ISitemapService
{
    /// <summary>
    /// Get the Sitemap Url for the requested url and named site.
    /// </summary>
    /// <param name="requestedUrl">Requested Url.</param>
    /// <param name="siteName">Site name.</param>
    /// <returns>Url in string representation to display the relevant sitemap.</returns>
    public Task<string> GetSitemapUrl(string requestedUrl, string? siteName);
}