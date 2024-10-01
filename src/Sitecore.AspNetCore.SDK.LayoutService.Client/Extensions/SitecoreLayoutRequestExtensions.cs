using System.Net;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Request;

namespace Sitecore.AspNetCore.SDK.LayoutService.Client.Extensions;

/// <summary>
/// HTTP related extension methods for the <see cref="SitecoreLayoutRequest"/>.
/// </summary>
internal static class SitecoreLayoutRequestExtensions
{
    private static readonly List<string> DefaultSitecoreRequestKeys =
    [
        RequestKeys.SiteName,
        RequestKeys.Path,
        RequestKeys.Language,
        RequestKeys.ApiKey,
        RequestKeys.Mode,
        RequestKeys.PreviewDate,
        RequestKeys.ContextId
    ];

    /// <summary>
    /// Build a URI using the default Sitecore layout entries in the provided request.
    /// </summary>
    /// <param name="request">The request object.</param>
    /// <param name="baseUri">The base URI used to compose the final URI.</param>
    /// <returns>A URI containing the base URI and the relevant entries in the request object added as query strings.</returns>
    public static Uri BuildDefaultSitecoreLayoutRequestUri(this SitecoreLayoutRequest request, Uri baseUri)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(baseUri);

        return request.BuildUri(baseUri, DefaultSitecoreRequestKeys);
    }

    /// <summary>
    /// Build a URI using the default Sitecore layout entries in the provided request.
    /// </summary>
    /// <param name="request">The request object.</param>
    /// <param name="baseUri">The base URI used to compose the final URI.</param>
    /// <param name="additionalQueryParameters">The additional URI query parameters to get from the request.</param>
    /// <returns>A URI containing the base URI and the relevant entries in the request object added as query strings.</returns>
    public static Uri BuildDefaultSitecoreLayoutRequestUri(this SitecoreLayoutRequest request, Uri baseUri, IEnumerable<string> additionalQueryParameters)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(baseUri);
        ArgumentNullException.ThrowIfNull(additionalQueryParameters);

        List<string> defaultKeys = [.. DefaultSitecoreRequestKeys];
        defaultKeys.AddRange(additionalQueryParameters);

        return request.BuildUri(baseUri, defaultKeys);
    }

    /// <summary>
    /// Build a URI using all the entries in the provided request.
    /// </summary>
    /// <param name="request">The request object.</param>
    /// <param name="baseUri">The base URI used to compose the final URL.</param>
    /// <param name="queryParameters">The URI query parameters to get from request.</param>
    /// <returns>A URI containing the base URI and all the valid entries in the request object added as query strings.</returns>
    public static Uri BuildUri(this SitecoreLayoutRequest request, Uri baseUri, IEnumerable<string> queryParameters)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(baseUri);
        ArgumentNullException.ThrowIfNull(queryParameters);

        List<KeyValuePair<string, object?>> entries = request.Where(entry => queryParameters.Contains(entry.Key)).ToList();
        IEnumerable<KeyValuePair<string, object>> validQueryParts = entries.Where(entry => entry.Value is string && !string.IsNullOrWhiteSpace(entry.Value.ToString()))!;
        string[] queryParts = validQueryParts.Select(kvp => $"{WebUtility.UrlEncode(kvp.Key)}={WebUtility.UrlEncode(kvp.Value.ToString())}").ToArray();

        if (queryParts.Length == 0)
        {
            return baseUri;
        }

        string queryString = $"?{string.Join("&", queryParts)}";

        UriBuilder builder = new(baseUri)
        {
            Query = queryString
        };

        return builder.Uri;
    }
}