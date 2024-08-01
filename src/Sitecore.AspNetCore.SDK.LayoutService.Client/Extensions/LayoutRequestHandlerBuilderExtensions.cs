using System.Net.Http.Headers;
using Microsoft.Extensions.DependencyInjection;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Configuration;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Interfaces;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Request;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Request.Handlers;

namespace Sitecore.AspNetCore.SDK.LayoutService.Client.Extensions;

/// <summary>
/// Extension methods to support configuration of Sitecore layout request handler services.
/// </summary>
public static class LayoutRequestHandlerBuilderExtensions
{
    /// <summary>
    /// Sets the current handler being built as the default handler for Sitecore layout service client requests.
    /// </summary>
    /// <typeparam name="THandler">The type of handler being configured.</typeparam>
    /// <param name="builder">The builder being configured.</param>
    /// <returns>The configured <see cref="ILayoutRequestHandlerBuilder{THandler}"/>.</returns>
    public static ILayoutRequestHandlerBuilder<THandler> AsDefaultHandler<THandler>(
        this ILayoutRequestHandlerBuilder<THandler> builder)
        where THandler : ILayoutRequestHandler
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.Services.Configure<SitecoreLayoutClientOptions>(options => options.DefaultHandler = builder.HandlerName);

        return builder;
    }

    /// <summary>
    /// Registers the default Sitecore layout service request options for the given handler.
    /// </summary>
    /// <typeparam name="THandler">The type of handler being configured.</typeparam>
    /// <param name="builder">The <see cref="ILayoutRequestHandlerBuilder{THandler}"/> being configured.</param>
    /// <param name="configureRequest">The <see cref="SitecoreLayoutRequest"/> request options configuration.</param>
    /// <returns>The configured <see cref="ILayoutRequestHandlerBuilder{THandler}"/>.</returns>
    public static ILayoutRequestHandlerBuilder<THandler> WithRequestOptions<THandler>(
        this ILayoutRequestHandlerBuilder<THandler> builder,
        Action<SitecoreLayoutRequest> configureRequest)
        where THandler : ILayoutRequestHandler
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(configureRequest);

        builder.Services.Configure<SitecoreLayoutRequestOptions>(builder.HandlerName, options => configureRequest(options.RequestDefaults));

        return builder;
    }

    /// <summary>
    /// Registers a <see cref="HttpRequestMessage"/> configuration action as named <see cref="HttpLayoutRequestHandlerOptions"/> for the given handler.
    /// </summary>
    /// <param name="builder">The <see cref="ILayoutRequestHandlerBuilder{THandler}"/> to configure.</param>
    /// <param name="configureHttpRequestMessage">The <see cref="HttpRequestMessage"/> configuration based on <see cref="SitecoreLayoutRequest"/>.</param>
    /// <returns>The configured <see cref="ILayoutRequestHandlerBuilder{THandler}"/>.</returns>
    public static ILayoutRequestHandlerBuilder<HttpLayoutRequestHandler> MapFromRequest(
        this ILayoutRequestHandlerBuilder<HttpLayoutRequestHandler> builder, Action<SitecoreLayoutRequest, HttpRequestMessage> configureHttpRequestMessage)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(configureHttpRequestMessage);

        builder.Services.Configure<HttpLayoutRequestHandlerOptions>(builder.HandlerName, options => options.RequestMap.Add(configureHttpRequestMessage));

        return builder;
    }

    /// <summary>
    /// Adds default configuration for the HTTP request message.
    /// </summary>
    /// <param name="httpHandlerBuilder">The <see cref="ILayoutRequestHandlerBuilder{THandler}"/> to configure.</param>
    /// <param name="nonValidatedHeaders">The list of headers which should not be validated.</param>
    /// <returns>The <see cref="ILayoutRequestHandlerBuilder{HttpLayoutRequestHandler}"/> so that additional calls can be chained.</returns>
    public static ILayoutRequestHandlerBuilder<HttpLayoutRequestHandler> ConfigureRequest(
        this ILayoutRequestHandlerBuilder<HttpLayoutRequestHandler> httpHandlerBuilder, string[] nonValidatedHeaders)
    {
        ArgumentNullException.ThrowIfNull(httpHandlerBuilder);
        ArgumentNullException.ThrowIfNull(nonValidatedHeaders);

        httpHandlerBuilder.MapFromRequest((request, message) =>
        {
            message.RequestUri = message.RequestUri != null
                ? request.BuildDefaultSitecoreLayoutRequestUri(message.RequestUri)
                : null;

            if (request.TryReadValue(RequestKeys.AuthHeaderKey, out string? headerValue))
            {
                message.Headers.Authorization = new AuthenticationHeaderValue("Bearer", headerValue);
            }

            if (request.TryGetHeadersCollection(out Dictionary<string, string[]>? headers))
            {
                foreach (KeyValuePair<string, string[]> h in headers ?? [])
                {
                    if (nonValidatedHeaders.Contains(h.Key))
                    {
                        message.Headers.TryAddWithoutValidation(h.Key, h.Value);
                    }
                    else
                    {
                        message.Headers.Add(h.Key, h.Value);
                    }
                }
            }
        });

        return httpHandlerBuilder;
    }
}