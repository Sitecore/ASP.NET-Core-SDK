using Microsoft.AspNetCore.Mvc;
using Sitecore.AspNetCore.SDK.RenderingEngine.Extensions;
using Sitecore.AspNetCore.SDK.RenderingEngine.Interfaces;

namespace Sitecore.AspNetCore.SDK.RenderingEngine.Integration.Tests.Controllers;

public class PagesController : Controller
{
    public IActionResult Index()
    {
        ISitecoreRenderingContext? context = HttpContext.GetSitecoreRenderingContext();
        return View("~/Views/Shared/HeadlessSxaLayout.cshtml", context);
    }
}