using GraphQL.Client.Abstractions;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.SystemTextJson;
using Sitecore.AspNetCore.SDK.GraphQL.Extensions;

namespace Sitecore.AspNetCore.SDK.Pages.GraphQL;

/// <summary>
/// GraphQLClientFactory used to generate instances of of GraphQLClients authenticated using a ContextId.
/// <param name="contextId">The contextId for the envionment being used.</param>
/// </summary>
public class GraphQLClientFactory(string contextId)
    : IGraphQLClientFactory
{
    private readonly string contextId = contextId;

    /// <inheritdoc />
    public IGraphQLClient GenerateClient(Uri? uri, string layoutKind, bool editMode)
    {
        uri ??= new Uri("https://edge-platform.sitecorecloud.io/v1/content/api/graphql/v1");
        uri = uri.AddQueryString("sitecoreContextId", contextId)!;

        GraphQLHttpClient client = new(uri, new SystemTextJsonSerializer());
        client.HttpClient.DefaultRequestHeaders.Add("sc_layoutKind", layoutKind);
        client.HttpClient.DefaultRequestHeaders.Add("sc_editmode", editMode.ToString());
        return client;
    }

    /// <inheritdoc />
    public IGraphQLClient GenerateClient(string layoutKind, bool editMode)
    {
        return GenerateClient(null, layoutKind, editMode);
    }
}
