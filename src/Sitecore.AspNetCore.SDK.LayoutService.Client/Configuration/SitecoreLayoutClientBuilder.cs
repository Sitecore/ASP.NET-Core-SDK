using Microsoft.Extensions.DependencyInjection;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Interfaces;

namespace Sitecore.AspNetCore.SDK.LayoutService.Client.Configuration;

/// <inheritdoc />
/// <summary>
/// Initializes a new instance of the <see cref="SitecoreLayoutClientBuilder"/> class.
/// </summary>
/// <param name="services">The initial <see cref="IServiceCollection"/>.</param>
public class SitecoreLayoutClientBuilder(IServiceCollection services)
    : ISitecoreLayoutClientBuilder
{
    /// <inheritdoc />
    public IServiceCollection Services { get; protected set; } = services ?? throw new ArgumentNullException(nameof(services));
}