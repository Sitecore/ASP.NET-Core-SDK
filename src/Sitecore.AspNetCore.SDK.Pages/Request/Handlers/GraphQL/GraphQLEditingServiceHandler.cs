using System.Text.Json;
using GraphQL;
using GraphQL.Client.Abstractions;
using Microsoft.Extensions.Logging;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Exceptions;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Interfaces;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Request;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Request.Handlers.GraphQL;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Response;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Response.Model;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Serialization;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Serialization.Fields;
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

    private static void GenerateMetaDataChromes(SitecoreLayoutResponseContent? content)
    {
        if (content?.Sitecore?.Route == null)
        {
            return;
        }

        foreach (var placeholder in content.Sitecore.Route.Placeholders)
        {
            string name = placeholder.Key;
            Placeholder placeholderFeatures = placeholder.Value;

            content.Sitecore.Route.Placeholders[name] = ProcessPlaceholder(name, Guid.Empty.ToString(), placeholderFeatures);
        }
    }

    private static Placeholder ProcessPlaceholder(string name, string id, Placeholder placeholderFeatures)
    {
        Placeholder updatedPlaceholders = [];

        AddPlaceholderOpeningChrome(name, id, updatedPlaceholders);

        foreach (var feature in placeholderFeatures)
        {
            if (feature is Component component)
            {
                AddRenderingOpeningChrome(updatedPlaceholders, component);

                var updatedFields = new Dictionary<string, IFieldReader>();
                foreach (var field in component.Fields)
                {
                    ProcessField(updatedFields, field);
                }

                component.Fields = updatedFields;

                updatedPlaceholders.Add(component);

                foreach (var componentPlaceholder in component.Placeholders)
                {
                    {
                        string componentPlaceholderName = componentPlaceholder.Key;
                        Placeholder componentPlaceholderFeatures = componentPlaceholder.Value;

                        component.Placeholders[componentPlaceholderName] = ProcessPlaceholder("container-{*}", component.Id, componentPlaceholderFeatures);
                    }
                }

                AddRenderingClosingChrome(updatedPlaceholders);
            }
        }

        AddPlaceholderClosingChrome(updatedPlaceholders);

        return updatedPlaceholders;
    }

    private static void AddRenderingClosingChrome(Placeholder updatedPlaceholders)
    {
        updatedPlaceholders.Add(GenerateEditableChrome("rendering", "close", string.Empty, string.Empty));
    }

    private static void AddRenderingOpeningChrome(Placeholder updatedPlaceholders, Component component)
    {
        updatedPlaceholders.Add(GenerateEditableChrome("rendering", "open", component.Id, string.Empty));
    }

    private static void AddPlaceholderClosingChrome(Placeholder updatedPlaceholders)
    {
        updatedPlaceholders.Add(GenerateEditableChrome("placeholder", "close", string.Empty, string.Empty));
    }

    private static void AddPlaceholderOpeningChrome(string name, string id, Placeholder updatedPlaceholders)
    {
        updatedPlaceholders.Add(GenerateEditableChrome("placeholder", "open", $"{name}_{id}", string.Empty));
    }

    private static void ProcessField(Dictionary<string, IFieldReader> updatedFields, KeyValuePair<string, IFieldReader> field)
    {
        if (field.Value is JsonSerializedField serialisedField && field.Key != "CustomContent")
        {
            var editableField = serialisedField.Read<EditableField<object>>();
            if (editableField == null)
            {
                return;
            }

            object openingChromeContent = new
            {
                datasource = new
                {
                    id = editableField?.MetaData?.DataSource?.Id,
                    language = editableField?.MetaData?.DataSource?.Language,
                    revision = editableField?.MetaData?.DataSource?.Revision,
                    version = editableField?.MetaData?.DataSource?.Version
                },
                title = editableField?.MetaData?.Title,
                fieldId = editableField?.MetaData?.FieldId,
                fieldType = editableField?.MetaData?.FieldType,
                rawValue = editableField?.MetaData?.RawValue
            };

            editableField.OpeningChrome = GenerateEditableChrome("field", "open", string.Empty, JsonSerializer.Serialize(openingChromeContent));
            editableField.ClosingChrome = GenerateEditableChrome("field", "close", string.Empty, string.Empty);

            var editableFieldWithChromesJson = JsonSerializer.SerializeToDocument(editableField);
            var updatedJsonSerialisedField = new JsonSerializedField(editableFieldWithChromesJson);

            updatedFields.Add(field.Key, updatedJsonSerialisedField);
        }
        else
        {
            updatedFields.Add(field.Key, field.Value);
        }
    }

    private static EditableChrome GenerateEditableChrome(string chrometype, string kind, string id, string content)
    {
        EditableChrome editableChrome = new EditableChrome
        {
            Attributes =
                {
                    { "chrometype", chrometype },
                    { "class", "scpm" },
                    { "kind", kind },
                    { "type", "text/sitecore" }
                }
        };

        if (id != string.Empty)
        {
            editableChrome.Attributes.Add("id", id);
        }

        if (content != string.Empty)
        {
            editableChrome.Content = content;
        }

        return editableChrome;
    }

    private static string GetRequestArgValue(SitecoreLayoutRequest request, string argName)
    {
        if (!request.ContainsKey("sc_request_headers_key") ||
            request["sc_request_headers_key"] is not Dictionary<string, string[]> headers ||
            !headers.ContainsKey(argName))
        {
            throw new ArgumentException($"Unable to parse arg:{argName} for Pages MetaData Render request.");
        }

        return headers[argName].FirstOrDefault() ?? string.Empty;
    }

    private static async Task GetFullDictionaryInformation(SitecoreLayoutRequest request, string requestLanguage, IGraphQLClient client, GraphQLResponse<EditingLayoutQueryResponse> response)
    {
        var hasNext = response.Data?.Site?.SiteInfo?.Dictionary?.PageInfo?.HasNext ?? false;
        var endCursor = response.Data?.Site?.SiteInfo?.Dictionary?.PageInfo?.EndCursor ?? string.Empty;
        while (hasNext && endCursor != string.Empty)
        {
            GraphQLRequest dictionaryRequest = BuildEditingDictionaryRequest(request, requestLanguage, endCursor);

            GraphQLResponse<EditingDictionaryResponse> dictionaryResponse = await client.SendQueryAsync<EditingDictionaryResponse>(dictionaryRequest).ConfigureAwait(false);
            response.Data?.Site?.SiteInfo?.Dictionary?.Results.AddRange(dictionaryResponse.Data?.Site?.SiteInfo?.Dictionary?.Results ?? []);

            hasNext = dictionaryResponse.Data?.Site?.SiteInfo?.Dictionary?.PageInfo?.HasNext ?? false;
            endCursor = dictionaryResponse.Data?.Site?.SiteInfo?.Dictionary?.PageInfo?.EndCursor ?? string.Empty;
        }
    }

    private static GraphQLRequest BuildEditingDictionaryRequest(SitecoreLayoutRequest request, string requestLanguage, string endCursor)
    {
        return new()
        {
            Query = @"
                        query EditingDictionaryQuery(
                            $siteName: String!
                            $language: String!
                            $after: String
                            $pageSize: Int
                          ) {
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
            OperationName = "EditingDictionaryQuery",
            Variables = new
            {
                language = requestLanguage,
                siteName = request.SiteName(),
                pageSize = 10,
                after = endCursor
            }
        };
    }

    private static GraphQLRequest BuildEditingLayoutRequest(SitecoreLayoutRequest request, string requestLanguage)
    {
        return new()
        {
            Query = @"
                    query EditingQuery(
                        $siteName: String!, 
                        $itemId: String!, 
                        $language: String!, 
                        $version: String, 
                        $after: String, 
                        $pageSize: Int
                     ) {
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
                pageSize = 10,
                after = string.Empty
            }
        };
    }

    private async Task<SitecoreLayoutResponseContent?> HandleEditingLayoutRequest(SitecoreLayoutRequest request, string requestLanguage, List<SitecoreLayoutServiceClientException> errors)
    {
        IGraphQLClient client = clientFactory.GenerateClient(GetRequestArgValue(request, "sc_layoutKind"), GetRequestArgValue(request, "mode") == "edit");
        GraphQLResponse<EditingLayoutQueryResponse> response = await client.SendQueryAsync<EditingLayoutQueryResponse>(BuildEditingLayoutRequest(request, requestLanguage)).ConfigureAwait(false);

        await GetFullDictionaryInformation(request, requestLanguage, client, response).ConfigureAwait(false);

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

            GenerateMetaDataChromes(content);

            if (logger.IsEnabled(LogLevel.Debug))
            {
                object? formattedDeserializeObject = JsonSerializer.Deserialize<object?>(json);
                logger.LogDebug("Layout Service Response JSON : {formattedDeserializeObject}", formattedDeserializeObject);
            }
        }

        if (response.Errors != null)
        {
            errors.AddRange(response.Errors.Select(e => new SitecoreLayoutServiceClientException(new LayoutServiceGraphQlException(e))));
        }

        return content;
    }
}