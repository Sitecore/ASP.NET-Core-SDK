using System.Text.Json;
using GraphQL;
using GraphQL.Client.Abstractions;
using Microsoft.Extensions.Logging;
using Sitecore.AspNetCore.SDK.GraphQL.Request;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Exceptions;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Interfaces;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Request;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Request.Handlers.GraphQL;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Response;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Response.Model;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Serialization;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Serialization.Fields;
using Sitecore.AspNetCore.SDK.Pages.Properties;
using Sitecore.AspNetCore.SDK.Pages.Services;

namespace Sitecore.AspNetCore.SDK.Pages.Request.Handlers.GraphQL;

/// <inheritdoc cref="ILayoutRequestHandler" />
/// <summary>
/// Initializes a new instance of the <see cref="GraphQLEditingServiceHandler"/> class.
/// </summary>
/// <param name="client">The GraphQL Client used for requests.</param>
/// <param name="logger">The <see cref="ILogger"/> to use for logging.</param>
/// <param name="dictionaryService">DictionaryService used to return all dictionary items for a Sitecore site.</param>
/// <param name="serializer">The serializer to handle response data.</param>
public partial class GraphQLEditingServiceHandler(IGraphQLClient client,
    ISitecoreLayoutSerializer serializer,
    ILogger<GraphQLEditingServiceHandler> logger,
    IDictionaryService dictionaryService)
    : ILayoutRequestHandler
{
    private readonly IGraphQLClient client = client ?? throw new ArgumentNullException(nameof(client));
    private readonly ISitecoreLayoutSerializer serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
    private readonly ILogger<GraphQLEditingServiceHandler> logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly IDictionaryService dictionaryService = dictionaryService ?? throw new ArgumentNullException(nameof(dictionaryService));

    /// <inheritdoc />
    public async Task<SitecoreLayoutResponse> Request(SitecoreLayoutRequest request, string handlerName)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentException.ThrowIfNullOrWhiteSpace(handlerName);

        if (!IsEditingRequest(request))
        {
            throw new ArgumentException(Resources.Exception_ErrorAttemptingToProcessNonEditingRequest);
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
        Placeholder result = new();

        // Create a separate class outside of this method for the work item
        // to avoid nested class compilation issues
        var workStack = new Stack<PlaceholderWorkItem>();
        workStack.Push(new PlaceholderWorkItem(name, id, placeholderFeatures, result));

        while (workStack.Count > 0)
        {
            var current = workStack.Pop();
            var output = current.Output;

            // Add opening chrome for placeholder
            AddPlaceholderOpeningChrome(current.Name, current.Id, output);

            // Process all features in this placeholder
            foreach (var feature in current.Features)
            {
                if (feature is Component component)
                {
                    // Add opening chrome
                    AddRenderingOpeningChrome(output, component);

                    // Process fields
                    Dictionary<string, IFieldReader> updatedFields = new();
                    foreach (var field in component.Fields)
                    {
                        ProcessField(updatedFields, field);
                    }

                    component.Fields = updatedFields;

                    // Add the component to the output
                    output.Add(component);

                    // Process component placeholders before adding closing chrome
                    if (component.Placeholders.Count > 0)
                    {
                        // For each placeholder in the component, add it to the work stack
                        foreach (var placeholder in component.Placeholders.ToList())
                        {
                            string placeholderKey = placeholder.Key;
                            Placeholder placeholderValue = placeholder.Value;

                            // Create a new placeholder to hold the processed content
                            Placeholder processedPlaceholder = new();

                            // Add a work item to process this placeholder
                            workStack.Push(new PlaceholderWorkItem(
                                "container-{*}",
                                component.Id,
                                placeholderValue,
                                processedPlaceholder,
                                component,
                                placeholderKey));

                            // Store the processed placeholder for later assignment
                            component.Placeholders[placeholderKey] = processedPlaceholder;
                        }
                    }

                    // Add closing chrome for the component
                    AddRenderingClosingChrome(output);
                }
            }

            // Add closing chrome for placeholder
            AddPlaceholderClosingChrome(output);
        }

        return result;
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
                    id = editableField.MetaData?.DataSource?.Id,
                    language = editableField.MetaData?.DataSource?.Language,
                    revision = editableField.MetaData?.DataSource?.Revision,
                    version = editableField.MetaData?.DataSource?.Version
                },
                title = editableField.MetaData?.Title,
                fieldId = editableField.MetaData?.FieldId,
                fieldType = editableField.MetaData?.FieldType,
                rawValue = editableField.MetaData?.RawValue
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

    private static GraphQLHttpRequestWithHeaders BuildEditingLayoutRequest(SitecoreLayoutRequest request, string requestLanguage)
    {
        return new()
        {
            Query = @"
                    query EditingQuery(
		                    $itemId: String!, 
                            $language: String!, 
                            $version: String
                        ) {
                        item(path: $itemId, language: $language, version: $version) {
                            rendered
                        }
                      }
                    ",
            OperationName = "EditingQuery",
            Variables = new
            {
                itemId = GetRequestArgValue(request, "sc_itemid"),
                language = requestLanguage,
                version = GetRequestArgValue(request, "sc_version")
            },
            Headers = new Dictionary<string, string>
            {
                { "sc_layoutKind", GetRequestArgValue(request, "sc_layoutKind") },
                { "sc_editmode", (GetRequestArgValue(request, "mode") == "edit").ToString() }
            }
        };
    }

    private async Task<SitecoreLayoutResponseContent?> HandleEditingLayoutRequest(SitecoreLayoutRequest request, string requestLanguage, List<SitecoreLayoutServiceClientException> errors)
    {
        GraphQLResponse<EditingLayoutQueryResponse> response = await client.SendQueryAsync<EditingLayoutQueryResponse>(BuildEditingLayoutRequest(request, requestLanguage)).ConfigureAwait(false);

        if (response?.Data == null)
        {
            throw new Exception(Resources.Exception_UableToProcessEditingResponse);
        }

        response.Data.Site.SiteInfo.Dictionary.Results = await dictionaryService.GetSiteDictionary(request.SiteName() ?? string.Empty, requestLanguage, client);

        if (logger.IsEnabled(LogLevel.Debug))
        {
            logger.LogDebug(Resources.Debug_LayoutServiceGraphQLResponse, response.Data.Item);
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
                logger.LogDebug(Resources.Debug_LayoutServiceResponseJSON, formattedDeserializeObject);
            }
        }

        if (response.Errors != null)
        {
            errors.AddRange(response.Errors.Select(e => new SitecoreLayoutServiceClientException(new LayoutServiceGraphQlException(e))));
        }

        return content;
    }
}