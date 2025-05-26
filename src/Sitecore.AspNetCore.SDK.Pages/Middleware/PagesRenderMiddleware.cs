using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
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
public class PagesRenderMiddleware(RequestDelegate next, IOptions<PagesOptions> options, ISitecoreLayoutRequestMapper requestMapper, ISitecoreLayoutClient layoutService)
{
    private readonly RequestDelegate _next = next ?? throw new ArgumentNullException(nameof(next));

    private readonly PagesOptions _options = options != null ? options.Value : throw new ArgumentNullException(nameof(options));

    private readonly ISitecoreLayoutClient _layoutService = layoutService ?? throw new ArgumentNullException(nameof(layoutService));

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

        if (IsValidEditingRequest(httpContext))
        {
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

        await _next(httpContext).ConfigureAwait(false);
    }

    private static bool IsInEditMode(HttpContext context)
    {
        return context.Request.Query.TryGetValue(Constants.QueryStringKeys.Mode, out StringValues mode)
               && mode == "edit";
    }

    private bool IsValidEditingRequest(HttpContext context)
    {
        return
            context.Request.Path != _options.RenderEndpoint
            && IsInEditMode(context)
            && IsValidEditingSecret(context.Request);
    }

    private bool IsValidEditingSecret(HttpRequest httpRequest)
    {
        bool result = false;
        if (httpRequest.Query.TryGetValue(Constants.QueryStringKeys.Secret, out StringValues editingSecretValues))
        {
            string editingSecret = editingSecretValues.FirstOrDefault() ?? string.Empty;
            if (editingSecret == _options.EditingSecret)
            {
                result = true;
            }
        }

        return result;
    }

    private async Task<SitecoreLayoutResponse> GetSitecoreLayoutResponse(HttpContext httpContext)
    {
        SitecoreLayoutRequest request = requestMapper.Map(httpContext.Request);
        ArgumentNullException.ThrowIfNull(request);
        return await _layoutService.Request(request, Constants.LayoutClients.Pages).ConfigureAwait(false);
    }
}