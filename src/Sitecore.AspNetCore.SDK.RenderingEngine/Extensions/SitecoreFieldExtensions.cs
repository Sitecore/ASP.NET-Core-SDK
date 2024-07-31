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