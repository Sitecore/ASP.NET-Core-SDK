using Microsoft.Extensions.DependencyInjection;

namespace Sitecore.AspNetCore.SDK.LayoutService.Client.Interfaces;

/// <summary>
/// Contract for configuring Sitecore layout service clients.
/// </summary>
public interface ISitecoreLayoutClientBuilder
{
    /// <summary>
    /// Gets the <see cref="IServiceCollection"/> where Sitecore layout services are configured.
    /// </summary>
    IServiceCollection Services { get; }
}