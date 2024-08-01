using Microsoft.AspNetCore.Mvc;
using Sitecore.AspNetCore.SDK.RenderingEngine.Attributes;
using Sitecore.AspNetCore.SDK.TestData;
using Route = Sitecore.AspNetCore.SDK.LayoutService.Client.Response.Model.Route;

namespace Sitecore.AspNetCore.SDK.RenderingEngine.Integration.Tests.Controllers;

[UseSitecoreRendering]
public class TrackingController : Controller
{
    [Route("/AttributeBased")]
    [HttpGet]
    public IActionResult Index(Route? route)
    {
        string view = route?.LayoutId switch
        {
            TestConstants.VisitorIdentificationPageLayoutId => "VisitorIdentificationLayout",
            _ => nameof(Index),
        };
        return View(view);
    }
}