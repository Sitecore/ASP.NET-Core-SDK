namespace Sitecore.AspNetCore.SDK.LayoutService.Client.Request;

/// <summary>
/// Extension methods for <see cref="SitecoreLayoutRequest"/>.
/// </summary>
public static class SitecoreLayoutRequestExtensions
{
    /// <summary>
    /// The key name for request headers data.
    /// </summary>
    private const string HeadersKey = "sc_request_headers_key";

    /// <summary>
    /// Gets the API key of the request.
    /// </summary>
    /// <param name="request">The <see cref="SitecoreLayoutRequest"/>.</param>
    /// <returns>The API key string value, otherwise null.</returns>
    public static string? ApiKey(this SitecoreLayoutRequest request)
        => ReadValue<string>(request, RequestKeys.ApiKey);

    /// <summary>
    /// Sets the API key of the request.
    /// If a null value is provided, the API key is removed from the request.
    /// </summary>
    /// <param name="request">The <see cref="SitecoreLayoutRequest"/>.</param>
    /// <param name="value">The value to set.</param>
    /// <returns>The <paramref name="request"/>.</returns>
    public static SitecoreLayoutRequest ApiKey(this SitecoreLayoutRequest request, string? value)
        => WriteValue(request, RequestKeys.ApiKey, value);

    /// <summary>
    /// Gets the site name of the request.
    /// </summary>
    /// <param name="request">The <see cref="SitecoreLayoutRequest"/>.</param>
    /// <returns>The site name string value, otherwise null.</returns>
    public static string? SiteName(this SitecoreLayoutRequest request)
        => ReadValue<string>(request, RequestKeys.SiteName);

    /// <summary>
    /// Sets the site name of the request.
    /// If a null value is provided, the site name is removed from the request.
    /// </summary>
    /// <param name="request">The <see cref="SitecoreLayoutRequest"/>.</param>
    /// <param name="value">The value to set.</param>
    /// <returns>The <paramref name="request"/>.</returns>
    public static SitecoreLayoutRequest SiteName(this SitecoreLayoutRequest request, string? value)
        => WriteValue(request, RequestKeys.SiteName, value);

    /// <summary>
    /// Gets the language of the request.
    /// </summary>
    /// <param name="request">The <see cref="SitecoreLayoutRequest"/>.</param>
    /// <returns>The language string value, otherwise null.</returns>
    public static string? Language(this SitecoreLayoutRequest request)
        => ReadValue<string>(request, RequestKeys.Language);

    /// <summary>
    /// Sets the language of the request.
    /// If a null value is provided, the language is removed from the request.
    /// </summary>
    /// <param name="request">The <see cref="SitecoreLayoutRequest"/>.</param>
    /// <param name="value">The value to set.</param>
    /// <returns>The <paramref name="request"/>.</returns>
    public static SitecoreLayoutRequest Language(this SitecoreLayoutRequest request, string? value)
        => WriteValue(request, RequestKeys.Language, value);

    /// <summary>
    /// Gets the path of the request.
    /// </summary>
    /// <param name="request">The <see cref="SitecoreLayoutRequest"/>.</param>
    /// <returns>The path string value, otherwise null.</returns>
    public static string? Path(this SitecoreLayoutRequest request)
        => ReadValue<string>(request, RequestKeys.Path);

    /// <summary>
    /// Sets the path of the request.
    /// If a null value is provided, the path is removed from the request.
    /// </summary>
    /// <param name="request">The <see cref="SitecoreLayoutRequest"/>.</param>
    /// <param name="value">The value to set.</param>
    /// <returns>The <paramref name="request"/>.</returns>
    public static SitecoreLayoutRequest Path(this SitecoreLayoutRequest request, string? value)
        => WriteValue(request, RequestKeys.Path, value);

    /// <summary>
    /// Gets the mode of the request.
    /// </summary>
    /// <param name="request">The <see cref="SitecoreLayoutRequest"/>.</param>
    /// <returns>The mode string value, otherwise null.</returns>
    public static string? Mode(this SitecoreLayoutRequest request)
        => ReadValue<string>(request, RequestKeys.Mode);

    /// <summary>
    /// Sets the mode of the request.
    /// If a null value is provided, the mode is removed from the request.
    /// </summary>
    /// <param name="request">The <see cref="SitecoreLayoutRequest"/>.</param>
    /// <param name="value">The value to set.</param>
    /// <returns>The <paramref name="request"/>.</returns>
    public static SitecoreLayoutRequest Mode(this SitecoreLayoutRequest request, string? value)
        => WriteValue(request, RequestKeys.Mode, value);

    /// <summary>
    /// Sets the preview date of the request.
    /// If a null value is provided, the preview date is removed from the request.
    /// </summary>
    /// <param name="request">The <see cref="SitecoreLayoutRequest"/>.</param>
    /// <returns>The <paramref name="request"/>.</returns>
    public static string? PreviewDate(this SitecoreLayoutRequest request)
        => ReadValue<string>(request, RequestKeys.PreviewDate);

    /// <summary>
    /// Gets the preview date of the request.
    /// </summary>
    /// <param name="request">The <see cref="SitecoreLayoutRequest"/>.</param>
    /// <param name="value">The value to set.</param>
    /// <returns>The preview date string value, otherwise null.</returns>
    public static SitecoreLayoutRequest PreviewDate(this SitecoreLayoutRequest request, string? value)
        => WriteValue(request, RequestKeys.PreviewDate, value);

    /// <summary>
    /// Sets the authentication header of the request.
    /// If a null value is provided, the authentication header is removed from the request.
    /// </summary>
    /// <param name="request">The <see cref="SitecoreLayoutRequest"/>.</param>
    /// <returns>The <paramref name="request"/>.</returns>
    public static string? AuthenticationHeader(this SitecoreLayoutRequest request)
        => ReadValue<string>(request, RequestKeys.AuthHeaderKey);

    /// <summary>
    /// Gets the authentication header of the request.
    /// </summary>
    /// <param name="request">The <see cref="SitecoreLayoutRequest"/>.</param>
    /// <param name="value">The value to set.</param>
    /// <returns>The authentication header string value, otherwise null.</returns>
    public static SitecoreLayoutRequest AuthenticationHeader(this SitecoreLayoutRequest request, string? value)
        => WriteValue(request, RequestKeys.AuthHeaderKey, value);

    /// <summary>
    /// Update missing values in the original request with values in the default request.
    /// </summary>
    /// <param name="request">The original request object.</param>
    /// <param name="requestDefaults">The default request object.</param>
    /// <returns>The updated request object.</returns>
    public static SitecoreLayoutRequest UpdateRequest(this SitecoreLayoutRequest request, Dictionary<string, object?>? requestDefaults)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (requestDefaults == null || requestDefaults.Count == 0)
        {
            return request;
        }

        // Create a base SitecoreLayoutRequest from the fallback parameters provided.
        SitecoreLayoutRequest baseRequest = [];

        foreach (KeyValuePair<string, object?> entry in requestDefaults)
        {
            baseRequest[entry.Key] = entry.Value;
        }

        // Create the final request to be returned using the base request as the starting point
        SitecoreLayoutRequest mergedRequest = baseRequest;

        foreach (KeyValuePair<string, object?> entry in request)
        {
            if (entry.Value == null)
            {
                mergedRequest.Remove(entry.Key);
            }
            else
            {
                mergedRequest[entry.Key] = entry.Value;
            }
        }

        return mergedRequest;
    }

    /// <summary>
    /// Adds the header with <paramref name="key"/> and <paramref name="value"/> to the headers collection stored in layout request.
    /// </summary>
    /// <param name="request">Layout request instance.</param>
    /// <param name="key">Header key.</param>
    /// <param name="value">header value.</param>
    public static void AddHeader(this SitecoreLayoutRequest request, string key, string[] value)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (!TryGetHeadersCollection(request, out Dictionary<string, string[]>? headers))
        {
            headers = [];
            request.Add(HeadersKey, headers);
        }

        if (headers != null && !headers.TryAdd(key, value))
        {
            string[] prev = headers[key];
            headers[key] = [.. prev, .. value];
        }
    }

    /// <summary>
    /// Adds headers collection to the headers collection stored in layout request.
    /// </summary>
    /// <param name="request">Layout request instance.</param>
    /// <param name="headers">Headers collection.</param>
    public static void AddHeaders(this SitecoreLayoutRequest request, IDictionary<string, string[]> headers)
    {
        ArgumentNullException.ThrowIfNull(request);

        foreach (KeyValuePair<string, string[]> h in headers)
        {
            request.AddHeader(h.Key, h.Value);
        }
    }

    /// <summary>
    /// Tries to get headers collection from layout request.
    /// </summary>
    /// <param name="request">Layout request instance.</param>
    /// <param name="headers">Headers.</param>
    /// <returns><c>false</c> if there is no headers collection otherwise <c>true</c>.</returns>
    public static bool TryGetHeadersCollection(this SitecoreLayoutRequest request, out Dictionary<string, string[]>? headers)
    {
        ArgumentNullException.ThrowIfNull(request);
        return request.TryReadValue(HeadersKey, out headers);
    }

    private static T? ReadValue<T>(SitecoreLayoutRequest request, string key)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        return request.TryReadValue(key, out T? result) ? result : default;
    }

    private static SitecoreLayoutRequest WriteValue<T>(SitecoreLayoutRequest request, string key, T? value)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        request[key] = value;
        return request;
    }
}