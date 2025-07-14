using System.Reflection;
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

        if (urlStr == null)
        {
            return null;
        }

        return GetSitecoreMediaUri(urlStr, imageParams);
    }

    /// <summary>
    /// Gets modified URL string to Sitecore media item with merged parameters.
    /// </summary>
    /// <param name="imageField">The image field.</param>
    /// <param name="imageParams">Base image parameters.</param>
    /// <param name="srcSetParams">SrcSet specific parameters that override imageParams.</param>
    /// <returns>Media item URL.</returns>
    public static string? GetMediaLink(this ImageField imageField, object? imageParams, object? srcSetParams)
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
    /// Gets modified URL string to Sitecore media item for srcSet with parameter preservation.
    /// This method preserves critical Sitecore parameters needed for proper image processing.
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
    /// <param name="baseParams">Base parameters.</param>
    /// <param name="overrideParams">Override parameters that take precedence.</param>
    /// <returns>Merged parameters as dictionary.</returns>
    private static Dictionary<string, object?> MergeParameters(object? baseParams, object? overrideParams)
    {
        Dictionary<string, object?> result = new Dictionary<string, object?>();

        // Add base parameters first
        if (baseParams != null)
        {
            if (baseParams is Dictionary<string, object> baseDict)
            {
                foreach (KeyValuePair<string, object> kvp in baseDict)
                {
                    result[kvp.Key] = kvp.Value;
                }
            }
            else
            {
                PropertyInfo[] baseProps = baseParams.GetType().GetProperties();
                foreach (PropertyInfo prop in baseProps)
                {
                    result[prop.Name] = prop.GetValue(baseParams);
                }
            }
        }

        // Override with srcSet parameters
        if (overrideParams != null)
        {
            if (overrideParams is Dictionary<string, object> overrideDict)
            {
                foreach (KeyValuePair<string, object> kvp in overrideDict)
                {
                    result[kvp.Key] = kvp.Value;
                }
            }
            else
            {
                PropertyInfo[] overrideProps = overrideParams.GetType().GetProperties();
                foreach (PropertyInfo prop in overrideProps)
                {
                    result[prop.Name] = prop.GetValue(overrideParams);
                }
            }
        }

        return result;
    }

    /// <summary>
    /// Gets modified URL string to Sitecore media item with parameter preservation.
    /// This preserves existing query parameters while adding new ones.
    /// </summary>
    /// <param name="url">Media item source URL.</param>
    /// <param name="parameters">Additional parameters to merge with existing ones.</param>
    /// <returns>Media item URL with preserved and merged parameters.</returns>
    private static string GetSitecoreMediaUriWithPreservation(string url, object? parameters)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            return url;
        }

        // Parse the existing URL to separate base URL and query string
        Uri uri = new Uri(url, UriKind.RelativeOrAbsolute);
        string baseUrl = uri.GetLeftPart(UriPartial.Path);
        Dictionary<string, Microsoft.Extensions.Primitives.StringValues> existingQuery = QueryHelpers.ParseQuery(uri.Query);

        // Convert parameters to dictionary and merge with existing query parameters
        Dictionary<string, string>? paramDict = ConvertToStringDictionary(parameters);
        if (paramDict != null)
        {
            foreach (KeyValuePair<string, string> param in paramDict)
            {
                // QueryHelpers.ParseQuery returns StringValues, so we need to handle this properly
                existingQuery[param.Key] = param.Value;
            }
        }

        // Apply the media URL prefix transformation (jssmedia replacement)
        string finalBaseUrl = baseUrl;
        Match match = MediaUrlPrefixRegex().Match(finalBaseUrl);
        if (match.Success)
        {
            finalBaseUrl = finalBaseUrl.Replace(match.Value, $"/{match.Groups[1]}/jssmedia/", StringComparison.InvariantCulture);
        }

        // Build the final URL with merged parameters
        string queryString = QueryHelpers.AddQueryString(string.Empty, existingQuery.ToDictionary(kvp => kvp.Key, kvp => (string?)kvp.Value.ToString()));
        return queryString.StartsWith("?") ? $"{finalBaseUrl}{queryString}" : finalBaseUrl;
    }

    /// <summary>
    /// Converts an object to a string dictionary.
    /// </summary>
    /// <param name="obj">The object to convert.</param>
    /// <returns>Dictionary representation of the object.</returns>
    private static Dictionary<string, string>? ConvertToStringDictionary(object? obj)
    {
        if (obj == null)
        {
            return null;
        }

        Dictionary<string, string> result = new Dictionary<string, string>();

        if (obj is Dictionary<string, object> dict)
        {
            foreach (KeyValuePair<string, object> kvp in dict)
            {
                result[kvp.Key] = kvp.Value?.ToString() ?? string.Empty;
            }
        }
        else
        {
            PropertyInfo[] props = obj.GetType().GetProperties();
            foreach (PropertyInfo prop in props)
            {
                object? value = prop.GetValue(obj);
                result[prop.Name] = value?.ToString() ?? string.Empty;
            }
        }

        return result;
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