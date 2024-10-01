using System.Web;

namespace Sitecore.AspNetCore.SDK.GraphQL.Extensions;

/// <summary>
/// Extension methods for <see cref="Uri"/>.
/// </summary>
public static class UriExtensions
{
    /// <summary>
    /// Adds a query string to the <see cref="Uri"/>.
    /// </summary>
    /// <param name="uri">The original <see cref="Uri"/>.</param>
    /// <param name="key">The key to use.</param>
    /// <param name="value">The value to use. This will be UrlEncoded.</param>
    /// <returns>New <see cref="Uri"/> with the query or the original if <paramref name="uri"/> is null or <paramref name="key"/> is null, empty or whitespace.</returns>
    public static Uri? AddQueryString(this Uri? uri, string key, string value)
    {
        Uri? result = uri;
        if (uri != null && !string.IsNullOrWhiteSpace(key))
        {
            int queryIndex = uri.OriginalString.IndexOf('?');
            result = queryIndex > 0
                ? new Uri(uri.OriginalString.Insert(queryIndex + 1, $"{key}={HttpUtility.UrlEncode(value)}&"), UriKind.RelativeOrAbsolute)
                : new Uri($"{uri.OriginalString}?{key}={HttpUtility.UrlEncode(value)}", UriKind.RelativeOrAbsolute);
        }

        return result;
    }
}