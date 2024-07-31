using System.Net;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Configuration;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Exceptions;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Interfaces;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Properties;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Response;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Serialization;

namespace Sitecore.AspNetCore.SDK.LayoutService.Client.Request.Handlers;

/// <inheritdoc />
public class HttpLayoutRequestHandler : ILayoutRequestHandler
{
    private readonly ISitecoreLayoutSerializer _serializer;
    private readonly HttpClient _client;
    private readonly IOptionsSnapshot<HttpLayoutRequestHandlerOptions> _options;
    private readonly ILogger<HttpLayoutRequestHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="HttpLayoutRequestHandler"/> class.
    /// </summary>
    /// <param name="client">The <see cref="HttpClient"/> to handle requests.</param>
    /// <param name="serializer">The serializer to handle response data.</param>
    /// <param name="options">An <see cref="IOptionsSnapshot{HttpLayoutRequestHandlerOptions}"/> to access specific options for this instance.</param>
    /// <param name="logger">The <see cref="ILogger"/> to use for logging.</param>
    public HttpLayoutRequestHandler(
        HttpClient client,
        ISitecoreLayoutSerializer serializer,
        IOptionsSnapshot<HttpLayoutRequestHandlerOptions> options,
        ILogger<HttpLayoutRequestHandler> logger)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
        _options = options ?? throw new ArgumentNullException(nameof(options));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        ArgumentNullException.ThrowIfNull(client.BaseAddress);
    }

    /// <inheritdoc />
    public async Task<SitecoreLayoutResponse> Request(SitecoreLayoutRequest request, string handlerName)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(handlerName);

        SitecoreLayoutResponseContent? content = null;
        ILookup<string, string>? metadata = null;
        List<SitecoreLayoutServiceClientException> errors = [];

        try
        {
            HttpLayoutRequestHandlerOptions options = _options.Get(handlerName);
            HttpRequestMessage httpMessage;
            try
            {
                httpMessage = BuildMessage(request, options);
                if (_logger.IsEnabled(LogLevel.Debug))
                {
                    _logger.LogDebug("Layout Service Http Request Message : {httpMessage}", httpMessage);
                }
            }
            catch (Exception ex)
            {
                // an exception is recorded if there is an error configuring the HTTP message
                errors = AddError(errors, new SitecoreLayoutServiceMessageConfigurationException(ex));
                if (_logger.IsEnabled(LogLevel.Debug))
                {
                    _logger.LogDebug("An error configuring the HTTP message  : {ex}", ex);
                }

                return new SitecoreLayoutResponse(request, errors);
            }

            HttpResponseMessage httpResponse = await GetResponseAsync(httpMessage).ConfigureAwait(false);
            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug("Layout Service Http Response : {httpResponse}", httpResponse);
            }

            int responseStatusCode = (int)httpResponse.StatusCode;
            if (!httpResponse.IsSuccessStatusCode)
            {
                errors = responseStatusCode switch
                {
                    404 => AddError(errors, new ItemNotFoundSitecoreLayoutServiceClientException(), responseStatusCode),
                    >= 400 and < 500 => AddError(errors, new InvalidRequestSitecoreLayoutServiceClientException(), responseStatusCode),
                    >= 500 => AddError(errors, new InvalidResponseSitecoreLayoutServiceClientException(new SitecoreLayoutServiceServerException()), responseStatusCode),
                    _ => AddError(errors, new SitecoreLayoutServiceClientException(), responseStatusCode),
                };
            }

            if (httpResponse.IsSuccessStatusCode || httpResponse.StatusCode == HttpStatusCode.NotFound)
            {
                try
                {
                    // content is only processed if a success or 404 status is returned
                    string json = await httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
                    content = _serializer.Deserialize(json);
                    if (_logger.IsEnabled(LogLevel.Debug))
                    {
                        object? formattedDeserializeObject = JsonSerializer.Deserialize<object?>(json);
                        _logger.LogDebug("Layout Service Response JSON : {formattedDeserializeObject}", formattedDeserializeObject);
                    }
                }
                catch (Exception ex)
                {
                    // an exception is recorded if there is a deserialization error
                    errors = AddError(errors, new InvalidResponseSitecoreLayoutServiceClientException(ex), responseStatusCode);
                }
            }

            try
            {
                metadata = httpResponse.Headers
                    .SelectMany(x => x.Value.Select(y => new { x.Key, Value = y }))
                    .ToLookup(k => k.Key, v => v.Value);
            }
            catch (Exception ex)
            {
                // an exception is recorded if there is an error reading the response headers
                errors = AddError(errors, new InvalidResponseSitecoreLayoutServiceClientException(ex), responseStatusCode);
            }
        }
        catch (Exception ex)
        {
            // an exception is recorded if there is a transport error
            errors.Add(new CouldNotContactSitecoreLayoutServiceClientException(ex));
        }

        return new SitecoreLayoutResponse(request, errors)
        {
            Content = content,
            Metadata = metadata
        };
    }

    /// <summary>
    /// Build a new <see cref="HttpRequestMessage"/> using the layout request and handler options provided.
    /// </summary>
    /// <param name="request">The <see cref="SitecoreLayoutRequest"/>.</param>
    /// <param name="options">The <see cref="HttpLayoutRequestHandlerOptions"/>.</param>
    /// <returns>A configured <see cref="HttpRequestMessage"/>.</returns>
    protected virtual HttpRequestMessage BuildMessage(SitecoreLayoutRequest request, HttpLayoutRequestHandlerOptions? options)
    {
        HttpRequestMessage message = new(HttpMethod.Get, _client.BaseAddress);

        if (options != null)
        {
            foreach (Action<SitecoreLayoutRequest, HttpRequestMessage> map in options.RequestMap)
            {
                map(request, message);
            }
        }

        return message;
    }

    /// <summary>
    /// Get the <see cref="HttpResponseMessage"/> returned by the provided URI.
    /// </summary>
    /// <param name="message">The <see cref="HttpRequestMessage"/> to be sent to the provided URI.</param>
    /// <returns>A <see cref="HttpResponseMessage"/>.</returns>
    protected virtual async Task<HttpResponseMessage> GetResponseAsync(HttpRequestMessage message)
    {
        return await _client.SendAsync(message).ConfigureAwait(false);
    }

    private static List<SitecoreLayoutServiceClientException> AddError(List<SitecoreLayoutServiceClientException> errors, SitecoreLayoutServiceClientException error, int statusCode = 0)
    {
        if (statusCode > 0)
        {
            error.Data.Add(Resources.HttpStatusCode_KeyName, statusCode);
        }

        errors.Add(error);
        return errors;
    }
}