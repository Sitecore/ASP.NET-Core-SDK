using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace Sitecore.AspNetCore.SDK.RenderingEngine.Extensions;

/// <summary>
/// Extension methods for the <see cref="HttpRequest"/>.
/// </summary>
internal static class HttpRequestExtensions
{
    /// <summary>
    /// Checks that value for the specified key present in the <see cref="HttpRequest"/> query string or cookies.
    /// </summary>
    /// <param name="httpRequest">The <see cref="HttpRequest"/> instance.</param>
    /// <param name="key">The key for the request value to get.</param>
    /// <param name="value">A query string value if the key is matched in the query string, otherwise a cookie value if the key is matched in the cookies, otherwise null.</param>
    /// <returns><c>true</c> if value exist in query string or cookies, otherwise <c>false</c>.</returns>
    public static bool TryGetValueFromQueryOrCookies(this HttpRequest httpRequest, string key, out string? value)
    {
        ArgumentNullException.ThrowIfNull(httpRequest);
        ArgumentNullException.ThrowIfNull(key);

        value = GetValueFromQueryOrCookies(httpRequest, key);

        return value != null;
    }

    /// <summary>
    /// Gets the value for the specified key from the <see cref="HttpRequest"/> query string or cookies.
    /// </summary>
    /// <param name="httpRequest">The <see cref="HttpRequest"/> instance.</param>
    /// <param name="key">The key for the request value to get.</param>
    /// <returns>A query string value if the key is matched in the query string, otherwise a cookie value if the key is matched in the cookies, otherwise null.</returns>
    public static string? GetValueFromQueryOrCookies(this HttpRequest httpRequest, string key)
    {
        ArgumentNullException.ThrowIfNull(httpRequest);
        ArgumentNullException.ThrowIfNull(key);

        StringValues queryValue = httpRequest.Query[key];
        if (!string.IsNullOrEmpty(queryValue))
        {
            return queryValue;
        }

        string? cookieValue = httpRequest.Cookies[key];
        if (!string.IsNullOrEmpty(cookieValue))
        {
            return cookieValue;
        }

        return null;
    }
}