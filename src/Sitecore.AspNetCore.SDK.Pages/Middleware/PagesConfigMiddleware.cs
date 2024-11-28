using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Sitecore.AspNetCore.SDK.Pages.Configuration;
using Sitecore.AspNetCore.SDK.RenderingEngine.Configuration;

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
/// <param name="renderingEngineOptions">The RenderingEngineOptions, used to retriece a list of all registered renderings for the applications</param>
public class PagesConfigMiddleware(RequestDelegate next, IOptions<PagesOptions> options, ILogger<PagesConfigMiddleware> logger, IOptions<RenderingEngineOptions> renderingEngineOptions)
{
    private readonly RequestDelegate next = next ?? throw new ArgumentNullException(nameof(next));
    private readonly PagesOptions options = options != null ? options.Value : throw new ArgumentNullException(nameof(options));
    private readonly ILogger<PagesConfigMiddleware> logger = logger ?? throw new ArgumentNullException(nameof(logger));
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

        await next(httpContext).ConfigureAwait(false);
    }

    private async Task BuildResponse(HttpResponse httpResponse)
    {
        httpResponse.Headers.ContentSecurityPolicy = $"frame-ancestors 'self' {options.ValidOrigins} {options.ValidEditingOrigin}";
        httpResponse.Headers.AccessControlAllowOrigin = options.ValidEditingOrigin;
        httpResponse.Headers.AccessControlAllowMethods = "GET, POST, OPTIONS, PUT, PATCH, DELETE";

        httpResponse.StatusCode = StatusCodes.Status200OK;
        httpResponse.ContentType = "application/json";

        // TODO: change the Components list to not be hardcoded!
        string responseBody = @"
            {
                ""components"": [
                    ""Title"",
                    ""Container"",
                    ""ColumnSplitter"",
                    ""RowSplitter"",
                    ""PageContent"",
                    ""RichText"",
                    ""Promo"",
                    ""LinkList"",
                    ""Image"",
                    ""PartialDesignDynamicPlaceholder"",
                    ""Navigation""
                ],
                ""packages"": {
                    ""@sitecore/byoc"": ""0.2.15"",
                    ""@sitecore/components"": ""1.1.10"",
                    ""@sitecore/engage"": ""1.4.3"",
                    ""@sitecore-cloudsdk/core"": ""0.3.1"",
                    ""@sitecore-cloudsdk/events"": ""0.3.1"",
                    ""@sitecore-cloudsdk/personalize"": ""0.3.1"",
                    ""@sitecore-cloudsdk/utils"": ""0.3.1"",
                    ""@sitecore-feaas/clientside"": ""0.5.18"",
                    ""@sitecore-jss/sitecore-jss"": ""22.1.3"",
                    ""@sitecore-jss/sitecore-jss-cli"": ""22.1.3"",
                    ""@sitecore-jss/sitecore-jss-dev-tools"": ""22.1.3"",
                    ""@sitecore-jss/sitecore-jss-nextjs"": ""22.1.3"",
                    ""@sitecore-jss/sitecore-jss-react"": ""22.1.3""
                },
                ""editMode"": ""metadata""
            }
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