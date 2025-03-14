using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Interfaces;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Request;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Response;
using Sitecore.AspNetCore.SDK.Pages.Configuration;
using Sitecore.AspNetCore.SDK.Pages.Properties;
using Sitecore.AspNetCore.SDK.RenderingEngine.Extensions;
using Sitecore.AspNetCore.SDK.RenderingEngine.Interfaces;
using Sitecore.AspNetCore.SDK.RenderingEngine.Rendering;

namespace Sitecore.AspNetCore.SDK.Pages.Middleware;

/// <summary>
/// The Pages middleware implementation that handles GET requests from the Sitecore Pages in MetaData Editing mode
/// and wraps the response HTML in a JSON format.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="PagesRenderMiddleware"/> class.
/// </remarks>
/// <param name="next">The next middleware to call.</param>
/// <param name="options">The Sitecore Pages configuration options.</param>
/// <param name="requestMapper">The <see cref="ISitecoreLayoutRequestMapper"/> to map the HttpRequest to a Layout Service request.</param>
/// <param name="layoutService">The layout service client.</param>
/// <param name="logger">The <see cref="ILogger"/> to use for logging.</param>
public class PagesRenderMiddleware(RequestDelegate next, IOptions<PagesOptions> options, ISitecoreLayoutRequestMapper requestMapper, ISitecoreLayoutClient layoutService, ILogger<PagesRenderMiddleware> logger)
{
    private readonly RequestDelegate next = next ?? throw new ArgumentNullException(nameof(next));
    private readonly PagesOptions options = options != null ? options.Value : throw new ArgumentNullException(nameof(options));
    private readonly ISitecoreLayoutRequestMapper _requestMapper = requestMapper ?? throw new ArgumentNullException(nameof(requestMapper));
    private readonly ISitecoreLayoutClient layoutService = layoutService ?? throw new ArgumentNullException(nameof(layoutService));
    private readonly ILogger<PagesRenderMiddleware> logger = logger ?? throw new ArgumentNullException(nameof(logger));

    /// <summary>
    /// The middleware Invoke method.
    /// </summary>
    /// <param name="httpContext">The current <see cref="HttpContext"/>.</param>
    /// <param name="viewComponentHelper">The current <see cref="IViewComponentHelper"/>.</param>
    /// <param name="htmlHelper">The current <see cref="IHtmlHelper"/>.</param>
    /// <returns>A Task to support async calls.</returns>
    public async Task Invoke(HttpContext httpContext, IViewComponentHelper viewComponentHelper, IHtmlHelper htmlHelper)
    {
        ArgumentNullException.ThrowIfNull(httpContext);
        ArgumentNullException.ThrowIfNull(viewComponentHelper);
        ArgumentNullException.ThrowIfNull(htmlHelper);

        if (IsEditingRequest(httpContext))
        {
            // this protects from multiple time executions when Global and Attribute based configurations are used at the same time.
            if (httpContext.Items.ContainsKey(nameof(PagesRenderMiddleware)))
            {
                throw new ApplicationException(Resources.Exception_PagesRenderMiddlewareAlreadyRegistered);
            }

            if (httpContext.GetSitecoreRenderingContext() == null)
            {
                SitecoreLayoutResponse response = await GetSitecoreLayoutResponse(httpContext).ConfigureAwait(false);

                SitecoreRenderingContext scContext = new()
                {
                    Response = response,
                    RenderingHelpers = new RenderingHelpers(viewComponentHelper, htmlHelper)
                };

                httpContext.SetSitecoreRenderingContext(scContext);
            }
            else
            {
                ISitecoreRenderingContext? scContext = httpContext.GetSitecoreRenderingContext();
                if (scContext != null)
                {
                    scContext.RenderingHelpers = new RenderingHelpers(viewComponentHelper, htmlHelper);
                }
            }

            httpContext.Items.Add(nameof(PagesRenderMiddleware), null);
        }

        await next(httpContext).ConfigureAwait(false);
    }

    private static bool IsEditingRequest(HttpContext context)
    {
        if (context.Request.Query.TryGetValue("mode", out var mode))
        {
            return mode == "edit";
        }

        return false;
    }

    private async Task<SitecoreLayoutResponse> GetSitecoreLayoutResponse(HttpContext httpContext)
    {
        SitecoreLayoutRequest request = requestMapper.Map(httpContext.Request);
        ArgumentNullException.ThrowIfNull(request);
        return await layoutService.Request(request, Constants.LayoutClients.Pages).ConfigureAwait(false);
    }
}