using Microsoft.Extensions.DependencyInjection;

namespace Sitecore.AspNetCore.SDK.LayoutService.Client.Interfaces;

/// <summary>
/// Contract for configuring named Sitecore layout service request handlers.
/// </summary>
/// <typeparam name="THandler">The type of handler being configured.</typeparam>
public interface ILayoutRequestHandlerBuilder<THandler>
    where THandler : ILayoutRequestHandler
{
    /// <summary>
    /// Gets the <see cref="IServiceCollection"/> where Sitecore layout services are configured.
    /// </summary>
    IServiceCollection Services { get; }

    /// <summary>
    /// Gets the name of the handler being configured.
    /// </summary>
    string HandlerName { get; }
}