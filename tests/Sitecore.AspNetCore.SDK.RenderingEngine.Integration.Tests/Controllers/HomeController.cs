using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using Sitecore.AspNetCore.SDK.RenderingEngine.Extensions;
using Sitecore.AspNetCore.SDK.TestData;
using Route = Sitecore.AspNetCore.SDK.LayoutService.Client.Response.Model.Route;

namespace Sitecore.AspNetCore.SDK.RenderingEngine.Integration.Tests.Controllers;

public class HomeController : Controller
{
    public IActionResult Index(Route route)
    {
        string view = route.LayoutId switch
        {
            TestConstants.NestedPlaceholderPageLayoutId => "NestedPlaceholderPageLayout",
            TestConstants.VisitorIdentificationPageLayoutId => "VisitorIdentificationLayout",
            TestConstants.HeadlessSxaLayoutId => "HeadlessSxaLayout",
            _ => nameof(Index)
        };

        return View(view);
    }

    [Route("/relativeurls")]
    [Route("/relativeurls/nested")]
    [Route("/relative urls/nested")]
    public IActionResult RelativeUrls() => View();

    [Route("/jss-render")]
    [Route("/sample-Post-endpoint")]
    [Route("/Home/Sample")]
    public JsonResult Sample()
    {
        return new JsonResult(HttpContext.GetSitecoreRenderingContext()?.Response?.Content);
    }

    [Route("/Home/UseRequestRouting")]
    [Route("/sample-Post-Route-endpoint")]
    public JsonResult UseRequestRouting()
    {
        return new JsonResult(HttpContext.Request.Method.ToUpper(CultureInfo.InvariantCulture));
    }

    public JsonResult Default()
    {
        return new JsonResult("success");
    }

    [Route("/Home/Sample/master")]
    public JsonResult CustomDatabaseRoute()
    {
        return new JsonResult("master");
    }
}