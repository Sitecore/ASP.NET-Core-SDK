using System.Text.RegularExpressions;
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

        var mergedParams = MergeParameters(imageParams, srcSetParams);
        return GetSitecoreMediaUri(urlStr, mergedParams);
    }

    /// <summary>
    /// Merges base parameters with override parameters.
    /// </summary>
    /// <param name="baseParams">Base parameters.</param>
    /// <param name="overrideParams">Override parameters that take precedence.</param>
    /// <returns>Merged parameters as dictionary.</returns>
    private static Dictionary<string, object?> MergeParameters(object? baseParams, object? overrideParams)
    {
        var result = new Dictionary<string, object?>();

        // Add base parameters first
        if (baseParams != null)
        {
            if (baseParams is Dictionary<string, object> baseDict)
            {
                foreach (var kvp in baseDict)
                {
                    result[kvp.Key] = kvp.Value;
                }
            }
            else
            {
                var baseProps = baseParams.GetType().GetProperties();
                foreach (var prop in baseProps)
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
                foreach (var kvp in overrideDict)
                {
                    result[kvp.Key] = kvp.Value;
                }
            }
            else
            {
                var overrideProps = overrideParams.GetType().GetProperties();
                foreach (var prop in overrideProps)
                {
                    result[prop.Name] = prop.GetValue(overrideParams);
                }
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