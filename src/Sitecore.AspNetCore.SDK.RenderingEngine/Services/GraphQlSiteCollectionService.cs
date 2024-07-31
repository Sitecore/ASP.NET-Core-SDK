using GraphQL;
using GraphQL.Client.Abstractions;
using Microsoft.Extensions.Logging;
using Sitecore.AspNetCore.SDK.RenderingEngine.Middleware.Models;

namespace Sitecore.AspNetCore.SDK.RenderingEngine.Services;

/// <summary>
/// GraphQl Site Collection Service.
/// </summary>
/// <param name="graphQlClient">The GraphQl Client.</param>
/// <param name="logger">The Logger.</param>
internal class GraphQlSiteCollectionService(IGraphQLClient graphQlClient, ILogger<GraphQlSiteCollectionService> logger)
    : ISiteCollectionService
{
    private const string SiteInfoCollectionQuery = """
                                                       query SiteInfoCollectionQuery {
                                                           site {
                                                               siteInfoCollection {
                                                                      name
                                                                      hostname
                                                                   }
                                                                 }
                                                       }
                                                   """;

    /// <summary>
    /// Get the Sites Collection.
    /// </summary>
    /// <returns>A <see cref="Task"/> returning an array of <see cref="SiteInfo"/> or null of none were returned.</returns>
    public async Task<SiteInfo?[]?> GetSitesCollection()
    {
        GraphQLRequest siteInfoCollectionRequest = new()
        {
            Query = SiteInfoCollectionQuery,
            OperationName = "SiteInfoCollectionQuery"
        };

        try
        {
            GraphQLResponse<SiteInfoCollectionResult> response = await graphQlClient.SendQueryAsync<SiteInfoCollectionResult>(siteInfoCollectionRequest).ConfigureAwait(false);

            if (response.Errors != null && response.Errors.Length != 0)
            {
                foreach (GraphQLError graphQlError in response.Errors)
                {
                    logger.LogError("[GraphQL Client Error] {Message}", graphQlError.Message);
                }
            }

            return response.Data.Site?.SiteInfoCollection;
        }
        catch (Exception exception)
        {
            logger.LogError("[GraphQL Client Error] {exceptionMessage}", exception.Message);
            return null;
        }
    }
}