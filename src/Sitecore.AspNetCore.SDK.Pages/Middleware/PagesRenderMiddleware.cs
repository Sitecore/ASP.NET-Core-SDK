using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Sitecore.AspNetCore.SDK.Pages.Configuration;
using Sitecore.AspNetCore.SDK.Pages.Models;

namespace Sitecore.AspNetCore.SDK.Pages.Middleware;

/// <summary>
/// The Pages middleware implementation that handles GET requests from the Sitecore Pages in MetaData Editing mode
/// and wraps the response HTML in a JSON format.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="PagesConfigMiddleware"/> class.
/// </remarks>
/// <param name="next">The next middleware to call.</param>
/// <param name="options">The Sitecore Pages configuration options.</param>
/// <param name="logger">The <see cref="ILogger"/> to use for logging.</param>
public class PagesRenderMiddleware(RequestDelegate next, IOptions<PagesOptions> options, ILogger<PagesConfigMiddleware> logger)
{
    private readonly RequestDelegate next = next ?? throw new ArgumentNullException(nameof(next));
    private readonly PagesOptions options = options != null ? options.Value : throw new ArgumentNullException(nameof(options));
    private readonly ILogger<PagesConfigMiddleware> logger = logger ?? throw new ArgumentNullException(nameof(logger));

    /// <summary>
    /// The middleware Invoke method.
    /// </summary>
    /// <param name="httpContext">The current <see cref="HttpContext"/>.</param>
    /// <returns>A Task to support async calls.</returns>
    public async Task Invoke(HttpContext httpContext)
    {
        if (IsValidPagesRenderRequest(httpContext.Request))
        {
            logger.LogDebug("Processing valid Pages Render request");

            PerformPagesRedirect(httpContext, ParseQueryStringArgs(httpContext.Request));

            return;
        }

        await next(httpContext).ConfigureAwait(false);
    }

    private void PerformPagesRedirect(HttpContext httpContext, PagesRenderArgs args)
    {
        httpContext.Response.Redirect($"{args.Route}?mode={args.Mode}&sc_itemid={args.ItemId}&sc_version={args.Version}", permanent: false);
    }

    private PagesRenderArgs ParseQueryStringArgs(HttpRequest request)
    {
        return new PagesRenderArgs
        {
            ItemId = Guid.TryParse(request.Query["sc_itemid"].FirstOrDefault(), out Guid itemId) ? itemId : Guid.Empty,
            EditingSecret = request.Query["secret"].FirstOrDefault() ?? string.Empty,
            Language = request.Query["sc_lang"].FirstOrDefault() ?? string.Empty,
            LayoutKind = request.Query["sc_layoutKind"].FirstOrDefault() ?? string.Empty,
            Mode = request.Query["mode"].FirstOrDefault() ?? string.Empty,
            Route = request.Query["route"].FirstOrDefault() ?? string.Empty,
            Site = request.Query["sc_site"].FirstOrDefault() ?? string.Empty,
            Version = int.TryParse(request.Query["sc_version"].FirstOrDefault(), out int version) ? version : 0
        };
    }


    private bool IsValidEditingSecret(HttpRequest httpRequest)
    {
        if (httpRequest.Query.TryGetValue("secret", out StringValues editingSecretValues))
        {
            string editingSecret = editingSecretValues.FirstOrDefault() ?? string.Empty;
            if (editingSecret == options.EditingSecret)
            {
                return true;
            }
        }

        return false;
    }

    private bool IsValidPagesRenderRequest(HttpRequest httpRequest)
    {
        ArgumentNullException.ThrowIfNull(httpRequest);
        if (httpRequest.Method != HttpMethods.Get || !httpRequest.Path.Value!.Equals(options.RenderEndpoint, StringComparison.InvariantCultureIgnoreCase))
        {
            return false;
        }

        if (!IsValidEditingSecret(httpRequest))
        {
            logger.LogError("Invalid Pages Editing Secret Value");
            return false;
        }

        return true;
    }
}