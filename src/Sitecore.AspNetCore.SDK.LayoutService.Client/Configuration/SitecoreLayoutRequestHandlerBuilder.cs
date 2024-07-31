using Microsoft.Extensions.DependencyInjection;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Interfaces;

namespace Sitecore.AspNetCore.SDK.LayoutService.Client.Configuration;

/// <inheritdoc />
/// <summary>
/// Initializes a new instance of the <see cref="SitecoreLayoutRequestHandlerBuilder{THandler}"/> class.
/// </summary>
public class SitecoreLayoutRequestHandlerBuilder<THandler> : ILayoutRequestHandlerBuilder<THandler>
    where THandler : ILayoutRequestHandler
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SitecoreLayoutRequestHandlerBuilder{THandler}"/> class.
    /// </summary>
    /// <param name="handlerName">The name of the handler being configured.</param>
    /// <param name="services">The initial <see cref="IServiceCollection"/>.</param>
    public SitecoreLayoutRequestHandlerBuilder(string handlerName, IServiceCollection services)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(handlerName);
        ArgumentNullException.ThrowIfNull(services);

        Services = services;
        HandlerName = handlerName;
    }

    /// <inheritdoc/>
    public IServiceCollection Services { get; }

    /// <inheritdoc/>
    public string HandlerName { get; }
}