using Sitecore.AspNetCore.SDK.RenderingEngine.Middleware.Models;

namespace Sitecore.AspNetCore.SDK.RenderingEngine.Services;

/// <summary>
/// Personalize service interface.
/// </summary>
public interface IPersonalizeService
{
    /// <summary>
    /// Get personalize information.
    /// </summary>
    /// <param name="itemPath">The path to query.</param>
    /// <param name="language">The language to query.</param>
    /// <param name="siteName">The site to query.</param>
    /// <returns>Personalization information.</returns>
    Task<PersonalizeInfo?> GetPersonalizeInfo(string itemPath, string language, string siteName);
}