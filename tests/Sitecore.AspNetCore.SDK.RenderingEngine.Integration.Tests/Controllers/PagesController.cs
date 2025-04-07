using Microsoft.AspNetCore.Mvc;
using Sitecore.AspNetCore.SDK.RenderingEngine.Extensions;

namespace Sitecore.AspNetCore.SDK.RenderingEngine.Integration.Tests.Controllers;

public class PagesController : Controller
{
    public IActionResult Index()
    {
        var context = HttpContext.GetSitecoreRenderingContext();
        return View("~/Views/Shared/HeadlessSxaLayout.cshtml", context);
    }
}