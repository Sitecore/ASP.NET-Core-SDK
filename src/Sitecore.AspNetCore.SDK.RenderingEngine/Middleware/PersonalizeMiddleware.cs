using System.Collections.Concurrent;
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
            PersonalizeExperienceParams experienceParams = GetExperienceParams(httpContext.Request, _options.ExperienceParamKeyOptions);
            List<PersonalizeExecution> executions = GetPersonalizeExecutions(personalizeInfo, language, _options.Scope, _options.DefaultVariant);
            ConcurrentBag<string> identifiedVariantIds = [];
            await Parallel.ForEachAsync(executions, async (execution, token) =>
            {
                string? variantId = await Personalize(execution.FriendlyId, execution.VariantIds, experienceParams, language, 400);
                if (!string.IsNullOrWhiteSpace(variantId) && execution.VariantIds.Contains(variantId))
                {
                    identifiedVariantIds.Add(variantId);
                }
                else if (!string.IsNullOrWhiteSpace(variantId) && !execution.VariantIds.Contains(variantId))
                {
                    logger.LogInformation("Invalid Variant Id '{VariantId}'.", variantId);
                }
            });

            if (identifiedVariantIds.Count > 0)
            {
                string basePath = httpContext.Response.Headers["x-sc-rewrite"].FirstOrDefault() ?? path;
                string rewritePath = ""; // TODO figure out what this is?
            }
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
    private static string GetPageVariantId(string pageId, string language, string variantId, string? scope, string defaultVariant)
    {
        string formattedPageId = GuidReplacement().Replace(pageId, string.Empty);
        string formattedLanguage = language.Replace('-', '_');
        string scopeId = string.IsNullOrWhiteSpace(scope) ? string.Empty : $"{NormalizeScope().Replace(scope, string.Empty)}_";
        string formattedVariantId = string.IsNullOrWhiteSpace(variantId) || variantId.Equals(defaultVariant) ? "default" : variantId;
        return $"{scopeId}{formattedPageId}_{formattedLanguage}_{formattedVariantId}".ToLowerInvariant();
    }

    // https://github.com/Sitecore/jss/blob/8bced38c251a598c04b1e6fa22b80ef7025eeb4e/packages/sitecore-jss-nextjs/src/middleware/personalize-middleware.ts#L168
    private static PersonalizeExperienceParams GetExperienceParams(HttpRequest req, PersonalizeOptions.ExperienceParamKeys keys)
    {
        PersonalizeExperienceParams result = new()
        {
            Campaign = req.GetValueFromQueryOrCookies(keys.Campaign),
            Content = req.GetValueFromQueryOrCookies(keys.Content),
            Medium = req.GetValueFromQueryOrCookies(keys.Medium),
            Source = req.GetValueFromQueryOrCookies(keys.Source),
            Referrer = req.Headers.Referer
        };
        return result;
    }

    // https://github.com/Sitecore/jss/blob/8bced38c251a598c04b1e6fa22b80ef7025eeb4e/packages/sitecore-jss-nextjs/src/middleware/personalize-middleware.ts#L196
    private static List<PersonalizeExecution> GetPersonalizeExecutions(PersonalizeInfo info, string language, string? scope, string defaultVariant)
    {
        List<PersonalizeExecution> result = [];
        foreach (string variantId in info.VariantIds ?? [])
        {
            if (variantId.Contains('_') && !string.IsNullOrWhiteSpace(info.Id))
            {
                string componentId = variantId.Split('_')[0];
                string friendlyId = GetComponentFriendlyId(info.Id, componentId, language, scope);
                PersonalizeExecution? existing = result.Find(e => e.FriendlyId == friendlyId);
                if (existing != null)
                {
                    existing.VariantIds.Add(variantId);
                }
                else
                {
                    // The default/control variant (format "<ComponentID>_default") is also a valid value returned by the execution
                    string defaultVariantId = $"{componentId}{defaultVariant}";
                    result.Add(new PersonalizeExecution
                    {
                        FriendlyId = friendlyId,
                        VariantIds = [defaultVariantId, variantId]
                    });
                }
            }
            else if (!string.IsNullOrWhiteSpace(info.Id))
            {
                // Embedded (page-level) personalization in format "<VariantID>"
                string friendlyId = GetPageFriendlyId(info.Id, language, scope);
                PersonalizeExecution? existing = result.Find(e => e.FriendlyId == friendlyId);
                if (existing != null)
                {
                    existing.VariantIds.Add(variantId);
                }
                else
                {
                    result.Add(new PersonalizeExecution
                    {
                        FriendlyId = friendlyId,
                        VariantIds = [variantId]
                    });
                }
            }
        }

        return result;
    }

    private static async Task<string?> Personalize(
        string friendlyId,
        List<string> variantIds,
        PersonalizeExperienceParams experienceParams,
        string language,
        int? timeout)
    {
        return "default"; // TODO This is where we call out to Personalize API
    }
}