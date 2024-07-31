using Sitecore.AspNetCore.SDK.RenderingEngine.Middleware.Models;

namespace Sitecore.AspNetCore.SDK.RenderingEngine.Services;

/// <summary>
/// Service Interface to retrieve the Site Collection.
/// </summary>
public interface ISiteCollectionService
{
    /// <summary>
    /// Get Sites Collection.
    /// </summary>
    /// <returns>A <see cref="Task{TResult}"/> returning a <see cref="SiteInfo"/> array or null on completion.</returns>
    Task<SiteInfo?[]?> GetSitesCollection();
}