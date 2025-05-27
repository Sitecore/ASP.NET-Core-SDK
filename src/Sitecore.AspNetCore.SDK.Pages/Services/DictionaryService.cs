using GraphQL;
using GraphQL.Client.Abstractions;
using Microsoft.Extensions.Options;
using Sitecore.AspNetCore.SDK.Pages.Configuration;
using Sitecore.AspNetCore.SDK.Pages.Request.Handlers.GraphQL;

namespace Sitecore.AspNetCore.SDK.Pages.Services;

/// <summary>
/// DictionaryService used retrieve dictionary items for a Sitecore site.
/// </summary>
public class DictionaryService(IOptions<PagesOptions> options) : IDictionaryService
{
    private readonly PagesOptions _options = options != null ? options.Value : throw new ArgumentNullException(nameof(options));

    /// <summary>
    /// Retrieves a list of site information dictionary items based on the specified site and language.
    /// </summary>
    /// <param name="siteName">Specifies the name of the site for which the dictionary items are being retrieved.</param>
    /// <param name="requestLanguage">Indicates the language in which the dictionary items should be returned.</param>
    /// <param name="client">Represents the GraphQL client used to send requests and receive responses.</param>
    /// <returns>Returns a list of site information dictionary items.</returns>
    public async Task<List<SiteInfoDictionaryItem>> GetSiteDictionary(string siteName, string requestLanguage, IGraphQLClient client)
    {
        if (string.IsNullOrWhiteSpace(siteName) || string.IsNullOrWhiteSpace(requestLanguage) || client == null)
        {
            throw new ArgumentNullException(nameof(siteName));
        }

        List<SiteInfoDictionaryItem> dictionary = [];
        GraphQLResponse<EditingDictionaryResponse> dictionaryPageResponse = await GetSinglePageOfDictionaryItems(siteName, requestLanguage, client, dictionary, string.Empty).ConfigureAwait(false);

        while (dictionaryPageResponse.Data.Site?.SiteInfo.Dictionary.PageInfo?.HasNext ?? false)
        {
            dictionaryPageResponse = await GetSinglePageOfDictionaryItems(siteName, requestLanguage, client, dictionary, dictionaryPageResponse.Data.Site?.SiteInfo.Dictionary.PageInfo?.EndCursor ?? string.Empty).ConfigureAwait(false);
        }

        return dictionary;
    }

    private async Task<GraphQLResponse<EditingDictionaryResponse>> GetSinglePageOfDictionaryItems(string siteName, string requestLanguage, IGraphQLClient client, List<SiteInfoDictionaryItem> dictionary, string endCursor)
    {
        GraphQLRequest dictionaryPageRequest = BuildEditingDictionaryRequest(siteName, requestLanguage, endCursor);
        GraphQLResponse<EditingDictionaryResponse> dictionaryPageResponse = await client.SendQueryAsync<EditingDictionaryResponse>(dictionaryPageRequest).ConfigureAwait(false);
        dictionary.AddRange(dictionaryPageResponse.Data.Site?.SiteInfo.Dictionary.Results ?? []);
        return dictionaryPageResponse;
    }

    private GraphQLRequest BuildEditingDictionaryRequest(string siteName, string requestLanguage, string endCursor)
    {
        return new()
        {
            Query = Constants.GraphQlQueries.EditingDictionaryRequest,
            OperationName = "DictionaryQuery",
            Variables = new
            {
                language = requestLanguage,
                siteName = siteName,
                pageSize = _options.DictionaryPageSize,
                after = endCursor
            }
        };
    }
}
