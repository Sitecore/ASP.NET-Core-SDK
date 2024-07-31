using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Request;
using Sitecore.AspNetCore.SDK.RenderingEngine.Configuration;
using Sitecore.AspNetCore.SDK.RenderingEngine.Extensions;
using Sitecore.AspNetCore.SDK.RenderingEngine.Services;

namespace Sitecore.AspNetCore.SDK.RenderingEngine.Middleware;

/// <summary>
/// Multisite Middleware.
/// </summary>
internal class MultisiteMiddleware
{
    private readonly RequestDelegate _next;

    private readonly ISiteResolver _siteResolver;

    private readonly IMemoryCache _memoryCache;

    private readonly MultisiteOptions _multisiteOptions;

    /// <summary>
    /// Initializes a new instance of the <see cref="MultisiteMiddleware"/> class.
    /// </summary>
    /// <param name="next">Next middleware to run in the chain.</param>
    /// <param name="siteResolver">Site resolver.</param>
    /// <param name="memoryCache">Memory Cache.</param>
    /// <param name="renderingEngineOptions">Rendering Engine Options.</param>
    /// <param name="multisiteOptions">Multisite options.</param>
    /// <exception cref="ArgumentNullException">When next, siteResolver or memoryCache arguments are null.</exception>
    public MultisiteMiddleware(RequestDelegate next, ISiteResolver siteResolver, IMemoryCache memoryCache, IOptions<RenderingEngineOptions> renderingEngineOptions, IOptions<MultisiteOptions> multisiteOptions)
    {
        _next = next ?? throw new ArgumentNullException(nameof(next));
        _siteResolver = siteResolver ?? throw new ArgumentNullException(nameof(siteResolver));
        _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
        _multisiteOptions = multisiteOptions != null ? multisiteOptions.Value : throw new ArgumentNullException(nameof(multisiteOptions));

        ArgumentNullException.ThrowIfNull(renderingEngineOptions);
        renderingEngineOptions.Value.MapToRequest(SiteNameChangingAction);
    }

    /// <summary>
    /// Execution method of the middleware.
    /// </summary>
    /// <param name="httpContext">The Http Context.</param>
    /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
    public async Task Invoke(HttpContext httpContext)
    {
        ArgumentNullException.ThrowIfNull(httpContext);

        string hostName = httpContext.Request.Host.Value;
        string? resolvedSiteName;

        // Site name can be forced by query string parameter
        if (httpContext.Request.Query.TryGetValue("sc_site", out StringValues scSiteFromQuery))
        {
            resolvedSiteName = scSiteFromQuery;
        }
        else
        {
            resolvedSiteName = await _memoryCache.GetOrCreateAsync($"{hostName}_siteName_key", async cacheEntry =>
            {
                cacheEntry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(_multisiteOptions.ResolvingCacheTimeout);

                return await _siteResolver.GetByHost(httpContext.Request.Host.Value).ConfigureAwait(false);
            }).ConfigureAwait(false);
        }

        if (!string.IsNullOrWhiteSpace(resolvedSiteName))
        {
            httpContext.SetResolvedSiteName(resolvedSiteName);
        }

        await _next(httpContext).ConfigureAwait(false);
    }

    private static void SiteNameChangingAction(HttpRequest httpRequest, SitecoreLayoutRequest layoutRequest)
    {
        if (httpRequest.HttpContext.TryGetResolvedSiteName(out string? resolvedSiteName))
        {
            layoutRequest.SiteName(resolvedSiteName);
        }
    }
}