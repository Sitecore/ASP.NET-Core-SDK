using System.Text.Json;
using GraphQL;
using GraphQL.Client.Abstractions;
using Microsoft.Extensions.Logging;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Exceptions;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Interfaces;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Request;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Request.Handlers.GraphQL;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Response;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Serialization;
using Sitecore.AspNetCore.SDK.Pages.GraphQL;

namespace Sitecore.AspNetCore.SDK.Pages.Request.Handlers.GraphQL;

/// <inheritdoc cref="ILayoutRequestHandler" />
/// <summary>
/// Initializes a new instance of the <see cref="GraphQLEditingServiceHandler"/> class.
/// </summary>
/// <param name="logger">The <see cref="ILogger"/> to use for logging.</param>
/// <param name="clientFactory">The GraphQlClientFactory used to generate instances of the GraphQl client.</param>
/// <param name="serializer">The serializer to handle response data.</param>
public class GraphQLEditingServiceHandler(IGraphQLClientFactory clientFactory,
    ISitecoreLayoutSerializer serializer,
    ILogger<GraphQLEditingServiceHandler> logger)
    : ILayoutRequestHandler
{
    private readonly IGraphQLClientFactory clientFactory = clientFactory;
    private readonly ISitecoreLayoutSerializer serializer = serializer;
    private readonly ILogger<GraphQLEditingServiceHandler> logger = logger;

    /// <inheritdoc />
    public async Task<SitecoreLayoutResponse> Request(SitecoreLayoutRequest request, string handlerName)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentException.ThrowIfNullOrWhiteSpace(handlerName);

        if (!IsEditingRequest(request))
        {
            throw new ArgumentException("GraphQLEditingServiceHandler: Error attempting to process non-editing request");
        }

        List<SitecoreLayoutServiceClientException> errors = [];
        SitecoreLayoutResponseContent? content = null;

        string? requestLanguage = request.Language();

        if (string.IsNullOrWhiteSpace(requestLanguage))
        {
            errors.Add(new ItemNotFoundSitecoreLayoutServiceClientException());
        }
        else
        {
            content = await HandleEditingLayoutRequest(request, requestLanguage, errors);
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

    private async Task<SitecoreLayoutResponseContent?> HandleEditingLayoutRequest(SitecoreLayoutRequest request, string requestLanguage, List<SitecoreLayoutServiceClientException> errors)
    {
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

        IGraphQLClient client = clientFactory.GenerateClient(GetRequestArgValue(request, "sc_layoutKind"), GetRequestArgValue(request, "mode"));
        GraphQLResponse<EditingLayoutQueryResponse> response = await client.SendQueryAsync<EditingLayoutQueryResponse>(layoutRequest).ConfigureAwait(false);

        if (logger.IsEnabled(LogLevel.Debug))
        {
            logger.LogDebug("Layout Service GraphQL Response : {responseDataLayout}", response.Data.Item);
        }

        SitecoreLayoutResponseContent? content = null;
        string? json = response.Data?.Item?.Rendered.ToString();
        if (json == null)
        {
            errors.Add(new ItemNotFoundSitecoreLayoutServiceClientException());
        }
        else
        {
            content = serializer.Deserialize(json);
            if (logger.IsEnabled(LogLevel.Debug))
            {
                object? formattedDeserializeObject = JsonSerializer.Deserialize<object?>(json);
                logger.LogDebug("Layout Service Response JSON : {formattedDeserializeObject}", formattedDeserializeObject);
            }
        }

        if (response.Errors != null)
        {
            errors.AddRange(
                response.Errors.Select(e => new SitecoreLayoutServiceClientException(new LayoutServiceGraphQlException(e))));
        }

        return content;
    }

    private string GetRequestArgValue(SitecoreLayoutRequest request, string argName)
    {
        if (!request.ContainsKey("sc_request_headers_key") ||
            request["sc_request_headers_key"] is not Dictionary<string, string[]> headers ||
            !headers.ContainsKey(argName))
        {
            throw new ArgumentException($"Unable to parse arg:{argName} for Pages MetaData Render request.");
        }

        return headers[argName].FirstOrDefault() ?? string.Empty;
    }
}
