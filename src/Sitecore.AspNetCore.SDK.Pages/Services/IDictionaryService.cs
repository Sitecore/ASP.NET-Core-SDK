using GraphQL.Client.Abstractions;
using Sitecore.AspNetCore.SDK.Pages.Request.Handlers.GraphQL;

namespace Sitecore.AspNetCore.SDK.Pages.Services
{
    /// <summary>
    /// DictionaryService used retrieve dictionary items for a Sitecore site.
    /// </summary>
    public interface IDictionaryService
    {
        /// <summary>
        /// Retrieves a list of site information based on the specified site and language.
        /// </summary>
        /// <param name="siteName">Specifies the name of the site for which information is being requested.</param>
        /// <param name="requestLanguage">Indicates the language in which the site information should be returned.</param>
        /// <param name="client">Represents the GraphQL client used to make the request for site information.</param>
        /// <returns>Returns a task that resolves to a list of site information dictionary items.</returns>
        Task<List<SiteInfoDictionaryItem>> GetSiteDictionary(string siteName, string requestLanguage, IGraphQLClient client);
    }
}