using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sitecore.AspNetCore.SDK.RenderingEngine.Extensions;
using Sitecore.AspNetCore.SDK.SearchOptimization.Models;
using Sitecore.AspNetCore.SDK.SearchOptimization.Redirects.Models;
using Sitecore.AspNetCore.SDK.SearchOptimization.Redirects.Rules;
using Sitecore.AspNetCore.SDK.SearchOptimization.Services;

namespace Sitecore.AspNetCore.SDK.SearchOptimization.Redirects.Middleware;

/// <summary>
/// AspNetCore middleware for Sitecore rewriting.
/// </summary>
internal class SitecoreRewriteMiddleware
{
    private const string RewriteOptionsCacheKey = "rewrite_options";
    private readonly RewriteOptions _staticRewriteOptions;
    private readonly RequestDelegate _next;
    private readonly ILogger _logger;
    private readonly IRedirectsService _redirectsService;
    private readonly IMemoryCache _memoryCache;
    private readonly SitecoreRewriteOptions _sitecoreRewriteOptions;

    /// <summary>
    /// Initializes a new instance of the <see cref="SitecoreRewriteMiddleware"/> class.
    /// </summary>
    /// <param name="next">Next middleware to execute.</param>
    /// <param name="loggerFactory">Logger factory to use.</param>
    /// <param name="redirectsService">Redirect service to use.</param>
    /// <param name="memoryCache">Memory cache to use.</param>
    /// <param name="rewriteOptions">Rewrite configuration Options.</param>
    /// <param name="staticRewriteOptions">Static rewrite configuration options.</param>
    public SitecoreRewriteMiddleware(
        RequestDelegate next,
        ILoggerFactory loggerFactory,
        IRedirectsService redirectsService,
        IMemoryCache memoryCache,
        IOptions<SitecoreRewriteOptions> rewriteOptions,
        IOptions<RewriteOptions> staticRewriteOptions)
    {
        ArgumentNullException.ThrowIfNull(next);
        ArgumentNullException.ThrowIfNull(redirectsService);

        _next = next;
        _redirectsService = redirectsService;
        _logger = loggerFactory.CreateLogger<SitecoreRewriteMiddleware>();
        _memoryCache = memoryCache;
        _sitecoreRewriteOptions = rewriteOptions.Value;
        _staticRewriteOptions = staticRewriteOptions.Value;
    }

    /// <summary>
    /// Executes the middleware.
    /// </summary>
    /// <param name="context">Context data.</param>
    /// <returns><see cref="Task"/> for the execution.</returns>
    /// <exception cref="ArgumentNullException">When context is null.</exception>
    public async Task InvokeAsync(HttpContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        context.TryGetResolvedSiteName(out string? resolvedSiteName);

        RewriteOptions? options = await _memoryCache.GetOrCreateAsync($"{RewriteOptionsCacheKey}_{resolvedSiteName}", async cacheEntry =>
        {
            cacheEntry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(_sitecoreRewriteOptions.CacheTimeout);
            return await GetSitecoreRewriteOptionsAsync(resolvedSiteName).ConfigureAwait(false);
        }).ConfigureAwait(false);

        RewriteContext rewriteContext = new()
        {
            HttpContext = context,
            StaticFileProvider = options!.StaticFileProvider,
            Logger = _logger,
            Result = RuleResult.ContinueRules
        };

        RunRules(rewriteContext, options, context, _logger);
        if (rewriteContext.Result == RuleResult.EndResponse)
        {
            return;
        }

        await _next(context).ConfigureAwait(false);
    }

    private static void RunRules(RewriteContext rewriteContext, RewriteOptions options, HttpContext httpContext, ILogger logger)
    {
        foreach (IRule rule in options.Rules)
        {
            rule.ApplyRule(rewriteContext);
            switch (rewriteContext.Result)
            {
                case RuleResult.ContinueRules:
                    logger.LogDebug("Request is continuing in applying rules. Current url is {EncodedUrl}", httpContext.Request.GetEncodedUrl());
                    break;
                case RuleResult.EndResponse:
                    logger.LogDebug("Request is done processing. Location header '{LocationHeader}' with status code '{StatusCode}'.", httpContext.Response.Headers.Location, httpContext.Response.StatusCode);
                    return;
                case RuleResult.SkipRemainingRules:
                    logger.LogDebug("Request is done applying rules. Url was rewritten to {EncodedUrl}", httpContext.Request.GetEncodedUrl());
                    return;
                default:
                    throw new ArgumentOutOfRangeException($"Invalid rule termination {rewriteContext.Result}");
            }
        }
    }

    private async Task<RewriteOptions> GetSitecoreRewriteOptionsAsync(string? siteName)
    {
        RewriteOptions options = new();
        RedirectInfo[]? redirectsResult = await _redirectsService.GetRedirects(siteName).ConfigureAwait(false);
        if (redirectsResult != null && redirectsResult.Length != 0)
        {
            foreach (RedirectInfo redirectInfo in redirectsResult)
            {
                if (redirectInfo.RedirectType != null && !string.IsNullOrWhiteSpace(redirectInfo.Pattern) && !string.IsNullOrWhiteSpace(redirectInfo.Target))
                {
                    bool isRegex = redirectInfo.Pattern.StartsWith('^') && redirectInfo.Pattern.EndsWith('$');
                    string formattedRegex = $"^{(isRegex ? redirectInfo.Pattern.TrimStart('^').TrimEnd('$').Trim('/') : redirectInfo.Pattern.Trim('/'))}[/]?$";

                    switch (redirectInfo.RedirectType)
                    {
                        case RedirectType.SERVER_TRANSFER:
                            options.Add(new SitecoreRewriteRule(formattedRegex, $"{redirectInfo.Target}", true, redirectInfo.IsQueryStringPreserved));
                            break;
                        case RedirectType.REDIRECT_301:
                            options.Add(new SitecoreRedirectRule(formattedRegex, $"{redirectInfo.Target}", 301, redirectInfo.IsQueryStringPreserved));
                            break;
                        case RedirectType.REDIRECT_302:
                            options.Add(new SitecoreRedirectRule(formattedRegex, $"{redirectInfo.Target}", 302, redirectInfo.IsQueryStringPreserved));
                            break;
                    }
                }
            }
        }

        if (_staticRewriteOptions.Rules.Any())
        {
            foreach (IRule rule in _staticRewriteOptions.Rules)
            {
                options.Add(rule);
            }
        }

        return options;
    }
}