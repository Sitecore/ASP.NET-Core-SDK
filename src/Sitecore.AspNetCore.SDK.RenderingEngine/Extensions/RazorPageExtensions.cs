using Microsoft.AspNetCore.Mvc.Razor;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Response.Model;

namespace Sitecore.AspNetCore.SDK.RenderingEngine.Extensions;

/// <summary>
/// Extensions to support Razor pages and views.
/// </summary>
public static class RazorPageExtensions
{
    /// <summary>
    /// Gets the current Sitecore <see cref="Route"/> data.
    /// </summary>
    /// <param name="page">The current <see cref="IRazorPage"/>.</param>
    /// <returns>The current instance of <see cref="Route"/>.</returns>
    public static Route? SitecoreRoute(this IRazorPage page)
    {
        ArgumentNullException.ThrowIfNull(page);

        Route? data = page.ViewContext.HttpContext.GetSitecoreRenderingContext()?.Response?.Content?.Sitecore?.Route;
        return data ?? default;
    }

    /// <summary>
    /// Gets the current Sitecore <see cref="Context"/>.
    /// </summary>
    /// <param name="page">The current <see cref="IRazorPage"/>.</param>
    /// <returns>The current instance of <see cref="Context"/>.</returns>
    public static Context? SitecoreContext(this IRazorPage page)
    {
        ArgumentNullException.ThrowIfNull(page);

        Context? data = page.ViewContext.HttpContext.GetSitecoreRenderingContext()?.Response?.Content?.Sitecore?.Context;
        return data ?? default;
    }

    /// <summary>
    /// Gets the current Sitecore <see cref="Component"/>.
    /// </summary>
    /// <param name="page">The current <see cref="IRazorPage"/>.</param>
    /// <returns>The current instance of <see cref="Component"/>.</returns>
    public static Component? SitecoreComponent(this IRazorPage page)
    {
        ArgumentNullException.ThrowIfNull(page);

        return page.ViewContext.HttpContext.GetSitecoreRenderingContext()?.Component;
    }
}