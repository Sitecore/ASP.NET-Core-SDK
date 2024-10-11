using GraphQL;
using GraphQL.Client.Abstractions;
using Sitecore.AspNetCore.SDK.RenderingEngine.Middleware.Models;

namespace Sitecore.AspNetCore.SDK.RenderingEngine.Services;

/// <summary>
/// Service to get personalize information.
/// </summary>
/// <param name="client">The <see cref="IGraphQLClient"/> to use to query.</param>
public class PersonalizeService(IGraphQLClient client) : IPersonalizeService
{
    private const string Query = """
                                 query($siteName: String!, $itemPath: String!, $language: String!) {
                                   layout(site: $siteName, routePath: $itemPath, language: $language) {
                                     item {
                                       id
                                       version
                                       personalization {
                                         variantIds
                                       }
                                     }
                                   }
                                 }
                                 """;

    /// <inheritdoc />
    public async Task<PersonalizeInfo?> GetPersonalizeInfo(string itemPath, string language, string siteName)
    {
        GraphQLResponse<PersonalizeQueryResult> response = await client.SendQueryAsync<PersonalizeQueryResult>(
            Query,
            new { siteName, itemPath, language },
            "PersonalizeInformationQuery");
        return response.Data.Layout?.Item;
    }
}