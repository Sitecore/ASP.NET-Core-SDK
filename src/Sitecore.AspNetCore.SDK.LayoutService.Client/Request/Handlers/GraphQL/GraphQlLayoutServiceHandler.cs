using System.Diagnostics.Metrics;
using System.Text.Json;
using GraphQL;
using GraphQL.Client.Abstractions;
using Microsoft.Extensions.Logging;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Exceptions;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Interfaces;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Response;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Serialization;

namespace Sitecore.AspNetCore.SDK.LayoutService.Client.Request.Handlers.GraphQL;

/// <inheritdoc cref="ILayoutRequestHandler" />
/// <summary>
/// Initializes a new instance of the <see cref="GraphQlLayoutServiceHandler"/> class.
/// </summary>
/// <param name="logger">The <see cref="ILogger"/> to use for logging.</param>
/// <param name="client">The graphQl client to handle response data.</param>
/// <param name="serializer">The serializer to handle response data.</param>
public class GraphQlLayoutServiceHandler(
    IGraphQLClient client,
    ISitecoreLayoutSerializer serializer,
    ILogger<GraphQlLayoutServiceHandler> logger)
    : ILayoutRequestHandler
{
    private readonly ISitecoreLayoutSerializer _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
    private readonly ILogger<GraphQlLayoutServiceHandler> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly IGraphQLClient _client = client ?? throw new ArgumentNullException(nameof(client));

    /// <inheritdoc />
    public async Task<SitecoreLayoutResponse> Request(SitecoreLayoutRequest request, string handlerName)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentException.ThrowIfNullOrWhiteSpace(handlerName);

        List<SitecoreLayoutServiceClientException> errors = [];
        SitecoreLayoutResponseContent? content = null;

        string? requestLanguage = request.Language();

        if (string.IsNullOrWhiteSpace(requestLanguage))
        {
            errors.Add(new ItemNotFoundSitecoreLayoutServiceClientException());
        }
        else
        {
            content = IsEditingRequest(request)
                ? await HandleEditingLayoutRequest(request, requestLanguage, errors, content).ConfigureAwait(false)
                : await HandleLayoutRequest(request, requestLanguage, errors, content).ConfigureAwait(false);
        }

        return new SitecoreLayoutResponse(request, errors)
        {
            Content = content,
            Metadata = new Dictionary<string, string>().ToLookup(k => k.Key, v => v.Value)
        };
    }

    private static bool IsEditingRequest(SitecoreLayoutRequest request)
    {
        if (!request.ContainsKey("sc_request_headers_key") ||
            request["sc_request_headers_key"] is not Dictionary<string, string[]> headers ||
            !headers.ContainsKey("mode"))
        {
            return false;
        }

        return headers["mode"].Contains("edit");
    }

    private async Task<SitecoreLayoutResponseContent?> HandleEditingLayoutRequest(SitecoreLayoutRequest request, string requestLanguage, List<SitecoreLayoutServiceClientException> errors, SitecoreLayoutResponseContent? content)
    {
        // TODO: Handle population of Dictionary for large size with extra GQL requests
        GraphQLRequest layoutRequest = new()
        {
            Query = @"
                    query EditingQuery($siteName: String!, $itemId: String!, $language: String!, $version: String, $after: String, $pageSize: Int = 10) {
	                    item(path: $itemId, language: $language, version: $version) {
	                        rendered
	                    }
	                    site {
	                        siteInfo(site: $siteName) {
	                            dictionary(language: $language, first: $pageSize, after: $after) {
	                                results {
					                    key
	                                    value
	                                }
	                                pageInfo {
	                                  endCursor
	                                  hasNext
	                                }
	                            }
	                        }
	                    }
                    }    
                    ",
            OperationName = "EditingQuery",
            Variables = new
            {
                itemId = GetRequestArgValue(request, "sc_itemid"),
                language = requestLanguage,
                siteName = request.SiteName(),
                version = GetRequestArgValue(request, "sc_version"),
                pageSize = 50,
                after = string.Empty
            }
        };

        GraphQLResponse<EditingLayoutQueryResponse> response = await _client.SendQueryAsync<EditingLayoutQueryResponse>(layoutRequest).ConfigureAwait(false);
        if (_logger.IsEnabled(LogLevel.Debug))
        {
            _logger.LogDebug("Layout Service GraphQL Response : {responseDataLayout}", response.Data.Item);
        }

        // ReSharper disable once ConditionalAccessQualifierIsNonNullableAccordingToAPIContract - Data can be null due to bad implementation of dependency library
        string? json = response.Data?.Item?.Rendered.ToString();
        if (json == null)
        {
            errors.Add(new ItemNotFoundSitecoreLayoutServiceClientException());
        }
        else
        {
            content = _serializer.Deserialize(json);
            if (_logger.IsEnabled(LogLevel.Debug))
            {
                object? formattedDeserializeObject = JsonSerializer.Deserialize<object?>(json);
                _logger.LogDebug("Layout Service Response JSON : {formattedDeserializeObject}", formattedDeserializeObject);
            }
        }

        if (response.Errors != null)
        {
            errors.AddRange(
                response.Errors.Select(e => new SitecoreLayoutServiceClientException(new LayoutServiceGraphQlException(e))));
        }

        return content;
    }

    private object GetRequestArgValue(SitecoreLayoutRequest request, string argName)
    {
        if (!request.ContainsKey("sc_request_headers_key") ||
            request["sc_request_headers_key"] is not Dictionary<string, string[]> headers ||
            !headers.ContainsKey(argName))
        {
            throw new ArgumentException($"Unable to parse arg:{argName} for Pages MetaData Render request.");
        }

        return headers[argName].FirstOrDefault() ?? string.Empty;
    }

    private async Task<SitecoreLayoutResponseContent?> HandleLayoutRequest(SitecoreLayoutRequest request, string requestLanguage, List<SitecoreLayoutServiceClientException> errors, SitecoreLayoutResponseContent? content)
    {
        GraphQLRequest layoutRequest = new()
        {
            Query = @"
                        query LayoutQuery($path: String!, $language: String!, $site: String!) {
                            layout(routePath: $path, language: $language, site: $site) {
                                item {
                                    rendered
                                }
                            }
                        }",
            OperationName = "LayoutQuery",
            Variables = new
            {
                path = request.Path(),
                language = requestLanguage,
                site = request.SiteName()
            }
        };

        GraphQLResponse<LayoutQueryResponse> response = await _client.SendQueryAsync<LayoutQueryResponse>(layoutRequest).ConfigureAwait(false);
        if (_logger.IsEnabled(LogLevel.Debug))
        {
            _logger.LogDebug("Layout Service GraphQL Response : {responseDataLayout}", response.Data.Layout);
        }

        // ReSharper disable once ConditionalAccessQualifierIsNonNullableAccordingToAPIContract - Data can be null due to bad implementation of dependency library
        string? json = response.Data?.Layout?.Item?.Rendered.ToString();
        if (json == null)
        {
            errors.Add(new ItemNotFoundSitecoreLayoutServiceClientException());
        }
        else
        {
            content = _serializer.Deserialize(json);
            if (_logger.IsEnabled(LogLevel.Debug))
            {
                object? formattedDeserializeObject = JsonSerializer.Deserialize<object?>(json);
                _logger.LogDebug("Layout Service Response JSON : {formattedDeserializeObject}", formattedDeserializeObject);
            }
        }

        if (response.Errors != null)
        {
            errors.AddRange(
                response.Errors.Select(e => new SitecoreLayoutServiceClientException(new LayoutServiceGraphQlException(e))));
        }

        return content;
    }
}