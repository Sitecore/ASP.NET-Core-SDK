using Microsoft.AspNetCore.Mvc;
using Sitecore.AspNetCore.SDK.RenderingEngine.Attributes;

namespace Sitecore.AspNetCore.SDK.RenderingEngine.Integration.Tests.Controllers;

public class ControllerMiddlewareController : Controller
{
    [UseSitecoreRendering]
    public JsonResult Index() => new("success");

    [UseSitecoreRendering]
    public JsonResult QueryStringTest(string param1, string param2)
    {
        return new JsonResult("success: " + param1 + ";" + param2);
    }
}