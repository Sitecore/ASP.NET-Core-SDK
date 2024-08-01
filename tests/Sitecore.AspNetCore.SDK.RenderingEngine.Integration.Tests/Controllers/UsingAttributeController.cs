using Microsoft.AspNetCore.Mvc;
using Sitecore.AspNetCore.SDK.RenderingEngine.Attributes;

namespace Sitecore.AspNetCore.SDK.RenderingEngine.Integration.Tests.Controllers;

public class UsingAttributeController : Controller
{
    [HttpGet]
    [UseSitecoreRendering]
    public IActionResult UseLocalizeWithAttribute()
    {
        return new JsonResult("success");
    }
}