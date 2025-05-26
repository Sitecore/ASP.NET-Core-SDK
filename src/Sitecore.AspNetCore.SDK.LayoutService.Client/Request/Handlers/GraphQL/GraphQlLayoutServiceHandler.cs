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
/// Initializes a new instance of the <see cref="GraphQLLayoutServiceHandler"/> class.
/// </summary>
/// <param name="logger">The <see cref="ILogger"/> to use for logging.</param>
/// <param name="client">The graphQl client to handle response data.</param>
/// <param name="serializer">The serializer to handle response data.</param>
public class GraphQLLayoutServiceHandler(
    IGraphQLClient client,
    ISitecoreLayoutSerializer serializer,
    ILogger<GraphQLLayoutServiceHandler> logger)
    : ILayoutRequestHandler
{
    private readonly ISitecoreLayoutSerializer _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
    private readonly ILogger<GraphQLLayoutServiceHandler> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
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
                    response.Errors.Select(e => new SitecoreLayoutServiceClientException(new LayoutServiceGraphQLException(e))));
            }
        }

        return new SitecoreLayoutResponse(request, errors)
        {
            Content = content,
            Metadata = new Dictionary<string, string>().ToLookup(k => k.Key, v => v.Value)
        };
    }
}