using Microsoft.AspNetCore.Mvc;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Response;
using Sitecore.AspNetCore.SDK.RenderingEngine.Extensions;

namespace Sitecore.AspNetCore.SDK.RenderingEngine.Integration.Tests.Controllers;

public class ErrorController : Controller
{
    public IActionResult Index()
    {
        SitecoreLayoutResponse? response = HttpContext.GetSitecoreRenderingContext()?.Response;

        ViewData["ErrorMessage"] = response?.HasErrors ?? false
            ? string.Join(",", response.Errors)
            : string.Empty;

        return View("ViewForHandlingError");
    }
}