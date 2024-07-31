using Microsoft.AspNetCore.Mvc;

namespace Sitecore.AspNetCore.SDK.RenderingEngine.Integration.Tests.Controllers;

public class UsingGlobalMiddlewareController : ControllerBase
{
    public JsonResult Index() => new("success");
}