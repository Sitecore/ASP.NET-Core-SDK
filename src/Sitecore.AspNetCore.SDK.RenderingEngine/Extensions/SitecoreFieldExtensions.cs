using System.Collections.Specialized;
using System.Text.RegularExpressions;
using System.Web;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.WebUtilities;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Response.Model.Fields;

namespace Sitecore.AspNetCore.SDK.RenderingEngine.Extensions;

/// <summary>
/// Set of extension methods for Sitecore fields.
/// </summary>
public static partial class SitecoreFieldExtensions
{
    /// <summary>
    /// Gets modified URL string to Sitecore media item.
    /// </summary>
    /// <param name="imageField">The image field.</param>
    /// <param name="imageParams">Image parameters, example: new { mw = 100, mh = 50 }. **IMPORTANT**: All the parameters you pass must be whitelisted for resizing to occur. See /sitecore/config/*.config (search for 'allowedMediaParams').</param>
    /// <returns>Media item URL.</returns>
    public static string? GetMediaLink(this ImageField imageField, object? imageParams)
    {
        ArgumentNullException.ThrowIfNull(imageField);
        string? urlStr = imageField.Value.Src;

        string? result = null;
        if (urlStr != null)
        {
            result = GetSitecoreMediaUri(urlStr, imageParams);
        }

        return result;
    }

    /// <summary>
    /// Gets modified URL string to Sitecore media item for srcSet.
    /// This method preserves existing URL parameters and merges them with new ones.
    /// </summary>
    /// <param name="imageField">The image field.</param>
    /// <param name="imageParams">Base image parameters.</param>
    /// <param name="srcSetParams">SrcSet specific parameters that override imageParams.</param>
    /// <returns>Media item URL.</returns>
    public static string? GetMediaLinkForSrcSet(this ImageField imageField, object? imageParams, object? srcSetParams)
    {
        ArgumentNullException.ThrowIfNull(imageField);
        string? urlStr = imageField.Value.Src;

        if (urlStr == null)
        {
            return null;
        }

        Dictionary<string, object?> mergedParams = MergeParameters(imageParams, srcSetParams);
        return GetSitecoreMediaUriWithPreservation(urlStr, mergedParams);
    }

    /// <summary>
    /// Merges base parameters with override parameters.
    /// </summary>
    /// <param name="imageParams">Base image parameters.</param>
    /// <param name="srcSetParams">SrcSet specific parameters that take precedence.</param>
    /// <returns>Merged parameters as dictionary.</returns>
    private static Dictionary<string, object?> MergeParameters(object? imageParams, object? srcSetParams)
    {
        Dictionary<string, object?> result = new(StringComparer.OrdinalIgnoreCase);

        // Add base parameters first
        AddParametersToResult(result, imageParams);

        // Override with srcSet parameters
        AddParametersToResult(result, srcSetParams);

        return result;
    }

    /// <summary>
    /// Adds parameters from an object to the result dictionary.
    /// </summary>
    /// <param name="result">The result dictionary to add parameters to.</param>
    /// <param name="parameters">The parameters object (can be Dictionary or any object with properties).</param>
    /// <param name="skipNullValues">Whether to skip null values when adding parameters.</param>
    private static void AddParametersToResult(Dictionary<string, object?> result, object? parameters, bool skipNullValues = false)
    {
        switch (parameters)
        {
            case null:
                break;
            case Dictionary<string, object?> paramDict:
                foreach (KeyValuePair<string, object?> kvp in paramDict.Where(kvp => !skipNullValues || kvp.Value != null))
                {
                    result[kvp.Key] = kvp.Value;
                }

                break;
            default:
                RouteValueDictionary routeValues = new(parameters);
                foreach (KeyValuePair<string, object?> kvp in routeValues.Where(kvp => !skipNullValues || kvp.Value != null))
                {
                    result[kvp.Key] = kvp.Value;
                }

                break;
        }
    }

    /// <summary>
    /// Gets URL to Sitecore media item.
    /// </summary>
    /// <param name="url">The image URL.</param>
    /// <param name="imageParams">Image parameters.</param>
    /// <returns>Media item URL.</returns>
    private static string GetSitecoreMediaUri(string url, object? imageParams)
    {
        // TODO What's the reason we strip away existing querystring?
        if (imageParams != null)
        {
            string[] urlParts = url.Split('?');
            if (urlParts.Length > 1)
            {
                url = urlParts[0];
            }

            RouteValueDictionary parameters = new(imageParams);
            foreach (string key in parameters.Keys)
            {
                url = QueryHelpers.AddQueryString(url, key, parameters[key]?.ToString() ?? string.Empty);
            }
        }

        return ApplyJssMediaUrlPrefix(url);
    }

    /// <summary>
    /// Gets modified URL string to Sitecore media item with parameter preservation.
    /// This method preserves existing URL parameters and merges them with new ones.
    /// </summary>
    /// <param name="urlStr">The URL string.</param>
    /// <param name="parameters">Parameters to merge.</param>
    /// <returns>Modified URL string.</returns>
    private static string GetSitecoreMediaUriWithPreservation(string urlStr, object? parameters)
    {
        if (string.IsNullOrEmpty(urlStr))
        {
            return urlStr;
        }

        // Parse existing query parameters and build merged parameters dictionary
        Dictionary<string, object?> mergedParams = new(StringComparer.OrdinalIgnoreCase);
        Uri? uri = null;
        if (!string.IsNullOrEmpty(urlStr))
        {
            Uri.TryCreate(urlStr, UriKind.RelativeOrAbsolute, out uri);
        }

        string url = ParseUrlParams(uri, mergedParams);

        // Add new parameters (these will override existing ones)
        AddParametersToResult(mergedParams, parameters, skipNullValues: true);

        // Add query parameters
        foreach (KeyValuePair<string, object?> kvp in mergedParams)
        {
            if (kvp.Value != null)
            {
                url = QueryHelpers.AddQueryString(url, kvp.Key, kvp.Value.ToString() ?? string.Empty);
            }
        }

        return ApplyJssMediaUrlPrefix(url);
    }

    /// <summary>
    /// Parses URL query string parameters and adds them to the provided dictionary.
    /// </summary>
    /// <param name="uri">The Uri with potential query parameters.</param>
    /// <param name="parameters">The dictionary to add parsed parameters to.</param>
    /// <returns>The URL without query parameters.</returns>
    private static string ParseUrlParams(Uri? uri, Dictionary<string, object?> parameters)
    {
        if (uri == null)
        {
            return string.Empty;
        }

        if (uri.IsAbsoluteUri)
        {
            string url = $"{uri.Scheme}://{uri.Host}{uri.AbsolutePath}";
            NameValueCollection queryParams = HttpUtility.ParseQueryString(uri.Query);
            foreach (string? param in queryParams.AllKeys)
            {
                if (!string.IsNullOrEmpty(param))
                {
                    parameters[param] = queryParams[param];
                }
            }

            return url;
        }
        else
        {
            // For relative URIs, accessing Uri.Query throws InvalidOperationException, so we use string manipulation
            string original = uri.OriginalString;
            int queryIndex = original.IndexOf('?');

            if (queryIndex >= 0)
            {
                string query = original.Substring(queryIndex);
                var parsedQuery = QueryHelpers.ParseQuery(query);
                foreach (var kvp in parsedQuery)
                {
                    parameters[kvp.Key] = kvp.Value.Count > 0 ? kvp.Value[0] : null;
                }

                return original.Substring(0, queryIndex);
            }

            return original;
        }
    }

    /// <summary>
    /// Applies JSS media URL prefix replacement to the given URL.
    /// </summary>
    /// <param name="url">The URL to transform.</param>
    /// <returns>The URL with JSS media prefix applied if applicable.</returns>
    private static string ApplyJssMediaUrlPrefix(string url)
    {
        // TODO Review hardcoded matching and replacement
        Match match = MediaUrlPrefixRegex().Match(url);
        if (match.Success)
        {
            url = url.Replace(match.Value, $"/{match.Groups[1]}/jssmedia/", StringComparison.InvariantCulture);
        }

        return url;
    }

    [GeneratedRegex("/([-~]{1})/media/")]
    private static partial Regex MediaUrlPrefixRegex();
}