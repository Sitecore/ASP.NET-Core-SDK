using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Sitecore.AspNetCore.SDK.RenderingEngine.Configuration;
using Sitecore.AspNetCore.SDK.RenderingEngine.Interfaces;

namespace Sitecore.AspNetCore.SDK.RenderingEngine.Extensions;

/// <summary>
/// Http context extensions.
/// </summary>
public static class HttpContextExtensions
{
    /// <summary>
    /// Extension method for copying of Http headers.
    /// </summary>
    /// <param name="source">Source collection.</param>
    /// <param name="headerKey">Header name to copy.</param>
    /// <param name="destination">Destination collection.</param>
    public static void CopyHeader(this IEnumerable<KeyValuePair<string, StringValues>> source, string headerKey, IDictionary<string, string[]> destination)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentException.ThrowIfNullOrWhiteSpace(headerKey);
        ArgumentNullException.ThrowIfNull(destination);

        foreach (KeyValuePair<string, StringValues> keyValuePair in source)
        {
            if (keyValuePair.Key.Equals(headerKey, StringComparison.OrdinalIgnoreCase))
            {
                destination.Add(keyValuePair.Key, keyValuePair.Value!);
                break;
            }
        }
    }

    /// <summary>
    /// Extension method for copying of Http headers.
    /// </summary>
    /// <param name="source">Source collection.</param>
    /// <param name="headerKey">Header name to copy.</param>
    /// <param name="destination">Destination collection.</param>
    public static void CopyHeader(this ILookup<string, string> source, string headerKey, IDictionary<string, string[]> destination)
    {
        // NOTE Other parameters are validated in the called method.
        ArgumentNullException.ThrowIfNull(source);

        IEnumerable<KeyValuePair<string, StringValues>> modifiedSource = source.Select(group =>
            new KeyValuePair<string, StringValues>(group.Key, new StringValues(group.ToArray())));

        CopyHeader(modifiedSource, headerKey, destination);
    }

    /// <summary>
    /// Extension method for collection manipulation simplifications. It inserts/appends string value to dictionary item.
    /// Is useful for headers manipulations.
    /// </summary>
    /// <param name="collection">Destination collection.</param>
    /// <param name="key">Key name.</param>
    /// <param name="value">Value to be added to values array.</param>
    public static void AppendValue(this IDictionary<string, string[]> collection, string key, string value)
    {
        ArgumentNullException.ThrowIfNull(collection);
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        ArgumentException.ThrowIfNullOrWhiteSpace(value);

        string[] valueToAdd =
        [
            value
        ];

        if (collection.TryAdd(key, valueToAdd))
        {
            return;
        }

        string[] values = collection[key];
        collection[key] = [.. values, .. valueToAdd];
    }

    /// <summary>
    /// Updates response with metadata from sitecore rendering context.
    /// </summary>
    /// <param name="renderingContext">Sitecore rendering context.</param>
    /// <param name="context">Http context.</param>
    public static void UpdateResponseWithLayoutMetadata(this ISitecoreRenderingContext renderingContext, HttpContext context)
    {
        ArgumentNullException.ThrowIfNull(renderingContext);
        ArgumentNullException.ThrowIfNull(context);

        ForwardHeadersOptions options = context.RequestServices.GetRequiredService<IOptions<ForwardHeadersOptions>>().Value;

        ILookup<string, string>? metadata = renderingContext.Response?.Metadata;

        if (metadata != null)
        {
            Dictionary<string, string[]> filteredHeaders = [];

            foreach (Action<ILookup<string, string>, IDictionary<string, string[]>> filter in options.ResponseHeadersFilters)
            {
                filter(metadata, filteredHeaders);
            }

            foreach (KeyValuePair<string, string[]> meta in filteredHeaders)
            {
                context.Response.Headers.Append(meta.Key, meta.Value);
            }
        }
    }

    /// <summary>
    /// Sets request resolved site name globally for using it between middlewares.
    /// </summary>
    /// <param name="context">Http context.</param>
    /// <param name="siteName">Sitecore rendering context.</param>
    public static void SetResolvedSiteName(this HttpContext context, string siteName)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentException.ThrowIfNullOrWhiteSpace(siteName);
        context.Items.TryAdd("resolvedSiteName", siteName);
    }

    /// <summary>
    /// Try get request resolved site name.
    /// </summary>
    /// <param name="context">Http context.</param>
    /// <param name="resolvedSiteName">Resolved site name out param.</param>
    /// <returns>Result boolean value.</returns>
    public static bool TryGetResolvedSiteName(this HttpContext context, out string? resolvedSiteName)
    {
        ArgumentNullException.ThrowIfNull(context);
        bool result = context.Items.TryGetValue("resolvedSiteName", out object? resolvedSiteNameObject);
        resolvedSiteName = resolvedSiteNameObject?.ToString();

        return result;
    }

    /// <summary>
    /// Gets the <see cref="ISitecoreRenderingContext"/> from the <see cref="HttpContext"/>.
    /// </summary>
    /// <param name="context">The <see cref="HttpContext"/> instance to retrieve the Sitecore rendering context from.</param>
    /// <returns>The <see cref="ISitecoreRenderingContext"/> instance.</returns>
    public static ISitecoreRenderingContext? GetSitecoreRenderingContext(this HttpContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        return context.Features.Get<ISitecoreRenderingContext>();
    }

    /// <summary>
    /// Sets the <see cref="ISitecoreRenderingContext"/> in the current <see cref="HttpContext.Features"/> collection.
    /// </summary>
    /// <param name="context">The current <see cref="HttpContext"/>.</param>
    /// <param name="renderingContext">The <see cref="ISitecoreRenderingContext"/> to save in the feature collection.</param>
    public static void SetSitecoreRenderingContext(this HttpContext context, ISitecoreRenderingContext renderingContext)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(renderingContext);
        context.Features.Set(renderingContext);
    }
}