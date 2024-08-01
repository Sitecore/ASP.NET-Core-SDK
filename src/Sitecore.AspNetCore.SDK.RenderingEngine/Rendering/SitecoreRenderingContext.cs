using Microsoft.AspNetCore.Mvc;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Response;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Response.Model;
using Sitecore.AspNetCore.SDK.RenderingEngine.Interfaces;

namespace Sitecore.AspNetCore.SDK.RenderingEngine.Rendering;

/// <inheritdoc />
public class SitecoreRenderingContext : ISitecoreRenderingContext
{
    /// <inheritdoc />
    public SitecoreLayoutResponse? Response { get; set; }

    /// <inheritdoc />
    public Component? Component { get; set; }

    /// <inheritdoc />
    public ControllerBase? Controller { get; set; }

    /// <inheritdoc />
    public RenderingHelpers? RenderingHelpers { get; set; }
}