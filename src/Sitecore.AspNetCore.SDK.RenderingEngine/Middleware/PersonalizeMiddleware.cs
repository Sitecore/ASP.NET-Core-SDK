using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sitecore.AspNetCore.SDK.RenderingEngine.Configuration;
using Sitecore.AspNetCore.SDK.RenderingEngine.Extensions;
using Sitecore.AspNetCore.SDK.RenderingEngine.Interfaces;
using Sitecore.AspNetCore.SDK.RenderingEngine.Middleware.Models;
using Sitecore.AspNetCore.SDK.RenderingEngine.Services;

namespace Sitecore.AspNetCore.SDK.RenderingEngine.Middleware;

/// <summary>
/// Personalization Middleware.
/// </summary>
/// <param name="next">Next in chain <see cref="RequestDelegate"/>.</param>
/// <remarks>
/// Based on https://github.com/Sitecore/jss/blob/48d1fb1a44cb0678d350f34e740b927dcf759755/packages/sitecore-jss-nextjs/src/middleware/personalize-middleware.ts#L243 implementation.
/// </remarks>
public partial class PersonalizeMiddleware(RequestDelegate next, IOptions<PersonalizeOptions> options, ILogger<PersonalizeMiddleware> logger, IMemoryCache cache, IPersonalizeService personalizeService)
{
    private readonly RequestDelegate _next = next ?? throw new ArgumentNullException(nameof(next));

    private readonly PersonalizeOptions _options = options.Value;

    /// <summary>
    /// Invokes the middleware for personalization.
    /// </summary>
    /// <param name="httpContext">The current <see cref="HttpContext"/>.</param>
    /// <returns>Task of the next in chain request delegate.</returns>
    public async Task Invoke(HttpContext httpContext)
    {
        ISitecoreRenderingContext? scContext = httpContext.GetSitecoreRenderingContext();
        PathString path = httpContext.Request.Path;
        string language = scContext?.Response?.Content?.Sitecore?.Context?.Language ?? httpContext.Request.Headers.AcceptLanguage.FirstOrDefault() ?? _options.DefaultLanguage;
        HostString hostname = httpContext.Request.Host;
        string site = (httpContext.TryGetResolvedSiteName(out string? resolvedSite) ? resolvedSite : null) ?? _options.DefaultSite;

        PersonalizeInfo? personalizeInfo = await cache.GetOrCreateAsync(
            $"personalizationInfo_{path}{language}{site}",
            entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = _options.CacheDuration;
                return personalizeService.GetPersonalizeInfo(path, language, site);
            });

        if (personalizeInfo is { VariantIds.Length: > 0 })
        {
            // TODO Execute personalization
        }

        await _next.Invoke(httpContext).ConfigureAwait(false);
    }

    [GeneratedRegex("[{}-]")]
    private static partial Regex GuidReplacement();

    [GeneratedRegex("[^a-zA-Z0-9]+")]
    private static partial Regex NormalizeScope();

    // https://github.com/Sitecore/jss/blob/48d1fb1a44cb0678d350f34e740b927dcf759755/packages/sitecore-jss/src/personalize/utils.ts#L130
    private static string GetComponentFriendlyId(string pageId, string componentId, string language, string? scope)
    {
        string formattedPageId = GuidReplacement().Replace(pageId, string.Empty);
        string formattedComponentId = GuidReplacement().Replace(componentId, string.Empty);
        string formattedLanguage = language.Replace('-', '_');
        string scopeId = string.IsNullOrWhiteSpace(scope) ? string.Empty : $"_{NormalizeScope().Replace(scope, string.Empty)}";
        return $"component{scopeId}_{formattedPageId}_{formattedComponentId}_{formattedLanguage}*".ToLowerInvariant();
    }

    // https://github.com/Sitecore/jss/blob/48d1fb1a44cb0678d350f34e740b927dcf759755/packages/sitecore-jss/src/personalize/utils.ts#L115
    private static string GetPageFriendlyId(string pageId, string language, string? scope)
    {
        string formattedPageId = GuidReplacement().Replace(pageId, string.Empty);
        string formattedLanguage = language.Replace('-', '_');
        string scopeId = string.IsNullOrWhiteSpace(scope) ? string.Empty : $"_{NormalizeScope().Replace(scope, string.Empty)}";
        return $"embedded{scopeId}_{formattedPageId}_{formattedLanguage}*".ToLowerInvariant();
    }

    // https://github.com/Sitecore/jss/blob/48d1fb1a44cb0678d350f34e740b927dcf759755/packages/sitecore-jss/src/personalize/utils.ts#L92
    private string GetPageVariantId(string pageId, string language, string variantId, string scope)
    {
        string formattedPageId = GuidReplacement().Replace(pageId, string.Empty);
        string formattedLanguage = language.Replace('-', '_');
        string scopeId = string.IsNullOrWhiteSpace(scope) ? string.Empty : $"{NormalizeScope().Replace(scope, string.Empty)}_";
        string formattedVariantId = string.IsNullOrWhiteSpace(variantId) || variantId.Equals(_options.DefaultVariant) ? "default" : variantId;
        return $"{scopeId}{formattedPageId}_{formattedLanguage}_{formattedVariantId}".ToLowerInvariant();
    }
}