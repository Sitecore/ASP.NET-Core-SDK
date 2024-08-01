using GraphQL.Client.Abstractions.Websocket;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.SystemTextJson;

namespace Sitecore.AspNetCore.SDK.GraphQL.Client.Models;

/// <summary>
/// GraphQL Client options needed for Preview or Edge schemas.
/// </summary>
public class SitecoreGraphQlClientOptions : GraphQLHttpClientOptions
{
    /// <summary>
    /// Gets or sets ApiKey.
    /// </summary>
    public string? ApiKey { get; set; }

    /// <summary>
    /// Gets or sets Default site name, used by middlewares which use GraphQl client.
    /// </summary>
    public string? DefaultSiteName { get; set; }

    /// <summary>
    /// Gets or sets GraphQLJsonSerializer, which could be SystemTextJsonSerializer or NewtonsoftJsonSerializer, SystemTextJsonSerializer by default.
    /// </summary>
    public IGraphQLWebsocketJsonSerializer GraphQlJsonSerializer { get; set; } = new SystemTextJsonSerializer();
}