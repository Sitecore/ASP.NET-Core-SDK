using Sitecore.AspNetCore.SDK.SearchOptimization.Models;

namespace Sitecore.AspNetCore.SDK.SearchOptimization.Services;

/// <summary>
/// Redirect Service Interface.
/// </summary>
internal interface IRedirectsService
{
    /// <summary>
    /// Get the redirects for a named site.
    /// </summary>
    /// <param name="siteName">Site name.</param>
    /// <returns>Redirect info of the Site.</returns>
    public Task<RedirectInfo[]?> GetRedirects(string? siteName);
}