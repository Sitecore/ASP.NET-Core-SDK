using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Configuration;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Interfaces;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Properties;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Request;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Response;

namespace Sitecore.AspNetCore.SDK.LayoutService.Client;

/// <inheritdoc />
/// <summary>
/// Initializes a new instance of the <see cref="DefaultLayoutClient"/> class.
/// </summary>
/// <param name="services">The services used for handler resolution.</param>
/// <param name="layoutClientOptions">The <see cref="SitecoreLayoutClientOptions"/> for this instance.</param>
/// <param name="layoutRequestOptions">An <see cref="IOptionsSnapshot{LayoutRequestOptions}"/> to access specific options for the default client request.</param>
/// <param name="logger">The <see cref="ILogger"/> to use for logging.</param>
public class DefaultLayoutClient(
    IServiceProvider services,
    IOptions<SitecoreLayoutClientOptions> layoutClientOptions,
    IOptionsSnapshot<SitecoreLayoutRequestOptions> layoutRequestOptions,
    ILogger<DefaultLayoutClient> logger)
    : ISitecoreLayoutClient
{
    private readonly IServiceProvider _services = services ?? throw new ArgumentNullException(nameof(services));
    private readonly IOptions<SitecoreLayoutClientOptions> _layoutClientOptions = layoutClientOptions ?? throw new ArgumentNullException(nameof(layoutClientOptions));
    private readonly IOptionsSnapshot<SitecoreLayoutRequestOptions> _layoutRequestOptions = layoutRequestOptions ?? throw new ArgumentNullException(nameof(layoutRequestOptions));
    private readonly ILogger<DefaultLayoutClient> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    /// <inheritdoc />
    public async Task<SitecoreLayoutResponse> Request(SitecoreLayoutRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        return await Request(request, string.Empty).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<SitecoreLayoutResponse> Request(SitecoreLayoutRequest request, string handlerName)
    {
        ArgumentNullException.ThrowIfNull(request);

        string? finalHandlerName = !string.IsNullOrWhiteSpace(handlerName) ? handlerName : _layoutClientOptions.Value.DefaultHandler;

        if (string.IsNullOrWhiteSpace(finalHandlerName))
        {
            throw new ArgumentNullException(finalHandlerName, Resources.Exception_HandlerNameIsNull);
        }

        if (!_layoutClientOptions.Value.HandlerRegistry.TryGetValue(finalHandlerName, out Func<IServiceProvider, ILayoutRequestHandler>? value))
        {
            throw new KeyNotFoundException(string.Format(Resources.Exception_HandlerRegistryKeyNotFound, finalHandlerName));
        }

        SitecoreLayoutRequestOptions mergedLayoutRequestOptions = MergeLayoutRequestOptions(finalHandlerName);

        SitecoreLayoutRequest finalRequest = request.UpdateRequest(mergedLayoutRequestOptions.RequestDefaults);

        if (_logger.IsEnabled(LogLevel.Trace))
        {
            string serializedRequest = JsonSerializer.Serialize(finalRequest);
            _logger.LogTrace("Sitecore Layout Request {serializedRequest}", serializedRequest);
        }

        ILayoutRequestHandler handler = value.Invoke(_services);

        return await handler.Request(finalRequest, finalHandlerName).ConfigureAwait(false);
    }

    private static bool AreEqual(SitecoreLayoutRequest request1, SitecoreLayoutRequest request2)
    {
        if (request1.Count != request2.Count)
        {
            return false;
        }

        ICollection<string> dictionary1Keys = request1.Keys;
        foreach (string key in dictionary1Keys)
        {
            if (!(request2.TryGetValue(key, out object? value) &&
                  request1[key] == value))
            {
                return false;
            }
        }

        return true;
    }

    private SitecoreLayoutRequestOptions MergeLayoutRequestOptions(string handlerName)
    {
        SitecoreLayoutRequestOptions globalLayoutRequestOptions = _layoutRequestOptions.Value;
        SitecoreLayoutRequestOptions handlerLayoutRequestOptions = _layoutRequestOptions.Get(handlerName);

        if (AreEqual(globalLayoutRequestOptions.RequestDefaults, handlerLayoutRequestOptions.RequestDefaults))
        {
            return globalLayoutRequestOptions;
        }

        SitecoreLayoutRequest mergedRequestDefaults = globalLayoutRequestOptions.RequestDefaults;
        SitecoreLayoutRequest handlerRequestDefaults = handlerLayoutRequestOptions.RequestDefaults;

        foreach (KeyValuePair<string, object?> entry in handlerRequestDefaults)
        {
            mergedRequestDefaults[entry.Key] = handlerRequestDefaults[entry.Key];
        }

        globalLayoutRequestOptions.RequestDefaults = mergedRequestDefaults;

        return globalLayoutRequestOptions;
    }
}