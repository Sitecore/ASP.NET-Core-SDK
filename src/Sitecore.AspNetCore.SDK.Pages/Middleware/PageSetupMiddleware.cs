using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Sitecore.AspNetCore.SDK.Pages.Configuration;
using Sitecore.AspNetCore.SDK.Pages.Models;
using Sitecore.AspNetCore.SDK.RenderingEngine.Configuration;
using Sitecore.AspNetCore.SDK.RenderingEngine.Extensions;

namespace Sitecore.AspNetCore.SDK.Pages.Middleware;

/// <summary>
/// The Pages middleware implementation that handles GET requests from the Sitecore Pages in MetaData Editing mode
/// and wraps the response HTML in a JSON format.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="PageSetupMiddleware"/> class.
/// </remarks>
/// <param name="next">The next middleware to call.</param>
/// <param name="options">The Sitecore Pages configuration options.</param>
/// <param name="logger">The <see cref="ILogger"/> to use for logging.</param>
/// <param name="renderingEngineOptions">The RenderingEngineOptions, used to retriece a list of all registered renderings for the applications</param>
public class PageSetupMiddleware(RequestDelegate next, IOptions<PagesOptions> options, ILogger<PageSetupMiddleware> logger, IOptions<RenderingEngineOptions> renderingEngineOptions)
{
    private readonly RequestDelegate next = next ?? throw new ArgumentNullException(nameof(next));
    private readonly PagesOptions options = options != null ? options.Value : throw new ArgumentNullException(nameof(options));
    private readonly ILogger<PageSetupMiddleware> logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly RenderingEngineOptions renderingEngineOptions = renderingEngineOptions != null ? renderingEngineOptions.Value : throw new ArgumentNullException(nameof(renderingEngineOptions));

    /// <summary>
    /// The middleware Invoke method.
    /// </summary>
    /// <param name="httpContext">The current <see cref="HttpContext"/>.</param>
    /// <returns>A Task to support async calls.</returns>
    public async Task Invoke(HttpContext httpContext)
    {
        if (IsValidPagesConfigRequest(httpContext.Request))
        {
            logger.LogDebug("Processing valid Pages Config request");
            await BuildResponse(httpContext.Response);
            return;
        }

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
        httpContext.Response.Headers.ContentSecurityPolicy = $"frame-ancestors 'self' {options.ValidOrigins} {options.ValidEditingOrigin}";
        httpContext.Response.Redirect($"{args.Route}?mode={args.Mode}&sc_itemid={args.ItemId}&sc_version={args.Version}&sc_lang={args.Language}&sc_site={args.Site}&sc_layoutKind={args.LayoutKind}&secret={args.EditingSecret}&tenant_id={args.TenantId}&route={args.Route}", permanent: false);
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
            Version = int.TryParse(request.Query["sc_version"].FirstOrDefault(), out int version) ? version : 0,
            TenantId = request.Query["tenant_id"].FirstOrDefault() ?? string.Empty,
        };
    }

    private async Task BuildResponse(HttpResponse httpResponse)
    {
        httpResponse.Headers.ContentSecurityPolicy = $"frame-ancestors 'self' {options.ValidOrigins} {options.ValidEditingOrigin}";
        httpResponse.Headers.AccessControlAllowOrigin = options.ValidEditingOrigin;
        httpResponse.Headers.AccessControlAllowMethods = "GET, POST, OPTIONS, PUT, PATCH, DELETE";

        httpResponse.StatusCode = StatusCodes.Status200OK;
        httpResponse.ContentType = "application/json";

        string componentNames = string.Join(",\r\n", renderingEngineOptions.RendererRegistry.Select(x => $"\"{x.Value.ComponentName}\""));
        string responseBody = $@"
            {{
                ""components"": [
                    {componentNames}
                ],
                ""packages"": {{
                }},
                ""editMode"": ""metadata""
            }}
        ";

        await httpResponse.WriteAsync(responseBody);
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

    private bool RequestHasValidEditingOrigin(HttpRequest httpRequest)
    {
        if (httpRequest.Headers.Origin == options.ValidEditingOrigin)
        {
            return true;
        }

        return false;
    }

    private bool IsValidPagesConfigRequest(HttpRequest httpRequest)
    {
        ArgumentNullException.ThrowIfNull(httpRequest);
        if (httpRequest.Method != HttpMethods.Get || !httpRequest.Path.Value!.Equals(options.ConfigEndpoint, StringComparison.InvariantCultureIgnoreCase))
        {
            return false;
        }

        if (!IsValidEditingSecret(httpRequest))
        {
            logger.LogError("Invalid Pages Editing Secret Value");
            return false;
        }

        if (!RequestHasValidEditingOrigin(httpRequest))
        {
            logger.LogError("Invalid Pages Editing Origin");
            return false;
        }

        return true;
    }
}