using GraphQL;
using GraphQL.Client.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sitecore.AspNetCore.SDK.GraphQL.Client.Models;
using Sitecore.AspNetCore.SDK.GraphQL.Exceptions;
using Sitecore.AspNetCore.SDK.SearchOptimization.Models;

namespace Sitecore.AspNetCore.SDK.SearchOptimization.Services;

/// <summary>
/// Implements the Sitemap and Redirects services by GraphQL data retrieval from Edge or Preview Delivery API.
/// </summary>
/// <param name="options">Options to configure the GraphQL Client.</param>
/// <param name="graphQlClient">GraphQL Client.</param>
/// <param name="logger">Logger service.</param>
internal class GraphQLSiteInfoService(
    IOptions<SitecoreGraphQLClientOptions> options,
    IGraphQLClient graphQlClient,
    ILogger<GraphQLSiteInfoService> logger)
    : ISitemapService, IRedirectsService
{
    private const string SiteInfoQuerySitemap = """
                                                query SiteInfoQuery($site: String!) {
                                                    site {
                                                        siteInfo(site: $site) {
                                                            sitemap
                                                        }
                                                    }
                                                }
                                                """;

    private const string SiteInfoQueryRedirects = """
                                                  query SiteInfoQuery($site: String!) {
                                                      site {
                                                          siteInfo(site: $site) {
                                                              redirects {
                                                                  pattern
                                                                  target
                                                                  isQueryStringPreserved
                                                                  redirectType
                                                                  locale
                                                              }
                                                          }
                                                      }
                                                  }
                                                  """;

    private readonly SitecoreGraphQLClientOptions _options = options.Value;

    /// <inheritdoc />
    public async Task<string> GetSitemapUrl(string requestedUrl, string? siteName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(requestedUrl);

        siteName ??= _options.DefaultSiteName;
        EnsureSiteName(siteName);

        GraphQLRequest siteInfoRequest = new()
        {
            Query = SiteInfoQuerySitemap,
            OperationName = "SiteInfoQuery",
            Variables = new
            {
                site = siteName
            }
        };

        try
        {
            GraphQLResponse<SiteInfoResultModel> response = await graphQlClient.SendQueryAsync<SiteInfoResultModel>(siteInfoRequest).ConfigureAwait(false);
            if (response.Errors != null && response.Errors.Length != 0)
            {
                foreach (GraphQLError graphQlError in response.Errors)
                {
                    logger.LogError("[GraphQL Error] {Message}", graphQlError.Message);
                }
            }

            return response.Data.Site?.SiteInfo?.Sitemap?.FirstOrDefault(sitemap => sitemap.EndsWith(requestedUrl, StringComparison.InvariantCultureIgnoreCase))!;
        }
        catch (Exception exception)
        {
            logger.LogError("[GraphQL Client Error] {Message}", exception.Message);

            return string.Empty;
        }
    }

    /// <inheritdoc />
    public async Task<RedirectInfo[]?> GetRedirects(string? siteName)
    {
        siteName ??= _options.DefaultSiteName;
        EnsureSiteName(siteName);

        GraphQLRequest siteInfoRequest = new()
        {
            Query = SiteInfoQueryRedirects,
            OperationName = "SiteInfoQuery",
            Variables = new
            {
                site = siteName
            }
        };

        try
        {
            GraphQLResponse<SiteInfoResultModel> response = await graphQlClient.SendQueryAsync<SiteInfoResultModel>(siteInfoRequest).ConfigureAwait(false);

            if (response.Errors != null && response.Errors.Length != 0)
            {
                foreach (GraphQLError graphQlError in response.Errors)
                {
                    logger.LogError("[GraphQL Error] {Message}", graphQlError.Message);
                }
            }

            return response.Data.Site?.SiteInfo?.Redirects;
        }
        catch (Exception exception)
        {
            logger.LogError("[GraphQL Client Error] {Message}", exception.Message);
            return null;
        }
    }

    private static void EnsureSiteName(string? siteName)
    {
        if (string.IsNullOrWhiteSpace(siteName))
        {
            throw new InvalidGraphQLConfigurationException("Empty DefaultSiteName, provided in GraphQLClientOptions.");
        }
    }
}