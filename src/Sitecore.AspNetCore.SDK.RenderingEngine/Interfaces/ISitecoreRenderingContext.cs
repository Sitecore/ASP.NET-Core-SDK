using Microsoft.AspNetCore.Mvc;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Response;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Response.Model;
using Sitecore.AspNetCore.SDK.RenderingEngine.Rendering;

namespace Sitecore.AspNetCore.SDK.RenderingEngine.Interfaces;

/// <summary>
/// Represents the context data for Sitecore rendering logic.
/// </summary>
public interface ISitecoreRenderingContext
{
    /// <summary>
    /// Gets or sets the current <see cref="SitecoreLayoutResponse"/>.
    /// </summary>
    SitecoreLayoutResponse? Response { get; set; }

    /// <summary>
    /// Gets or sets the current <see cref="Component"/>.
    /// </summary>
    public Component? Component { get; set; }

    /// <summary>
    /// Gets or sets the current <see cref="ControllerBase"/>.
    /// </summary>
    public ControllerBase? Controller { get; set; }

    /// <summary>
    /// Gets or sets the current <see cref="RenderingHelpers"/>.
    /// </summary>
    public RenderingHelpers? RenderingHelpers { get; set; }
}