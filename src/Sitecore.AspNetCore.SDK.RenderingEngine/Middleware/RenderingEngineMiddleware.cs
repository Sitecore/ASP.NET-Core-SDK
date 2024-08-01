using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Interfaces;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Request;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Response;
using Sitecore.AspNetCore.SDK.RenderingEngine.Configuration;
using Sitecore.AspNetCore.SDK.RenderingEngine.Extensions;
using Sitecore.AspNetCore.SDK.RenderingEngine.Interfaces;
using Sitecore.AspNetCore.SDK.RenderingEngine.Properties;
using Sitecore.AspNetCore.SDK.RenderingEngine.Rendering;

namespace Sitecore.AspNetCore.SDK.RenderingEngine.Middleware;

/// <summary>
/// The Rendering Engine middleware implementation that calls the Sitecore layout service
/// and stores the <see cref="ISitecoreRenderingContext"/> as a <see cref="HttpContext"/> feature.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="RenderingEngineMiddleware"/> class.
/// </remarks>
/// <param name="next">The next middleware to call.</param>
/// <param name="requestMapper">The <see cref="ISitecoreLayoutRequestMapper"/> to map the HttpRequest to a Layout Service request.</param>
/// <param name="layoutService">The layout service client.</param>
/// <param name="options">Rendering Engine options.</param>
public class RenderingEngineMiddleware(RequestDelegate next, ISitecoreLayoutRequestMapper requestMapper, ISitecoreLayoutClient layoutService, IOptions<RenderingEngineOptions> options)
{
    private readonly RequestDelegate _next = next ?? throw new ArgumentNullException(nameof(next));

    private readonly ISitecoreLayoutRequestMapper _requestMapper = requestMapper ?? throw new ArgumentNullException(nameof(requestMapper));

    private readonly ISitecoreLayoutClient _layoutService = layoutService ?? throw new ArgumentNullException(nameof(layoutService));

    private readonly RenderingEngineOptions _options = options != null ? options.Value : throw new ArgumentNullException(nameof(options));

    /// <summary>
    /// The middleware Invoke method.
    /// </summary>
    /// <param name="httpContext">The current <see cref="HttpContext"/>.</param>
    /// <param name="viewComponentHelper">The current <see cref="IViewComponentHelper"/>.</param>
    /// <param name="htmlHelper">The current <see cref="IHtmlHelper"/>.</param>
    /// <returns>A <see cref="Task"/> to support async calls.</returns>
    public async Task Invoke(HttpContext httpContext, IViewComponentHelper viewComponentHelper, IHtmlHelper htmlHelper)
    {
        ArgumentNullException.ThrowIfNull(httpContext);
        ArgumentNullException.ThrowIfNull(viewComponentHelper);
        ArgumentNullException.ThrowIfNull(htmlHelper);

        // this protects from multiple time executions when Global and Attribute based configurations are used at the same time.
        if (httpContext.Items.ContainsKey(nameof(RenderingEngineMiddleware)))
        {
            throw new ApplicationException(Resources.Exception_InvalidRenderingEngineConfiguration);
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

        foreach (Action<HttpContext> action in _options.PostRenderingActions)
        {
            action(httpContext);
        }

        httpContext.Items.Add(nameof(RenderingEngineMiddleware), null);

        await _next(httpContext).ConfigureAwait(false);
    }

    private async Task<SitecoreLayoutResponse> GetSitecoreLayoutResponse(HttpContext httpContext)
    {
        SitecoreLayoutRequest request = _requestMapper.Map(httpContext.Request);
        ArgumentNullException.ThrowIfNull(request);
        return await _layoutService.Request(request).ConfigureAwait(false);
    }
}