using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Sitecore.AspNetCore.SDK.RenderingEngine.Extensions;
using Sitecore.AspNetCore.SDK.RenderingEngine.Interfaces;
using Sitecore.AspNetCore.SDK.RenderingEngine.Rendering;

namespace Sitecore.AspNetCore.SDK.RenderingEngine.Filters;

/// <summary>
/// Identifies the current controller for the <see cref="SitecoreRenderingContext"/>.
/// </summary>
public class SitecoreLayoutContextControllerFilter : IActionFilter
{
    /// <inheritdoc />
    public void OnActionExecuting(ActionExecutingContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        ISitecoreRenderingContext? renderingContext = context.HttpContext.GetSitecoreRenderingContext();
        if (renderingContext != null)
        {
            renderingContext.Controller = context.Controller as ControllerBase;
        }
    }

    /// <inheritdoc />
    public void OnActionExecuted(ActionExecutedContext context)
    {
    }
}