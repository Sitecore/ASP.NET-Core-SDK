using Microsoft.AspNetCore.Mvc;
using Sitecore.AspNetCore.SDK.RenderingEngine.Middleware;

namespace Sitecore.AspNetCore.SDK.RenderingEngine.Attributes;

/// <summary>
/// Injects middleware to support the Sitecore Rendering logic.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="UseSitecoreRenderingAttribute"/> class.
/// </remarks>
/// <param name="configurationType">The type which configures the middleware for the request.</param>
public class UseSitecoreRenderingAttribute(Type configurationType)
    : MiddlewareFilterAttribute(configurationType)
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UseSitecoreRenderingAttribute"/> class.
    /// </summary>
    public UseSitecoreRenderingAttribute()
        : this(typeof(RenderingEnginePipeline))
    {
    }
}