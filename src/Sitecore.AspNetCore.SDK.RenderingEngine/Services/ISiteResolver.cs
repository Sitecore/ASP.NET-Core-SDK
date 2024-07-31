namespace Sitecore.AspNetCore.SDK.RenderingEngine.Services;

/// <summary>
/// Provides methods to resolve Sites.
/// </summary>
public interface ISiteResolver
{
    /// <summary>
    /// Gets a Site name by host name.
    /// </summary>
    /// <param name="hostName">The host name.</param>
    /// <returns>A <see cref="Task"/> returning a <see cref="string"/> site name or null when no site was found.</returns>
    Task<string?> GetByHost(string hostName);
}