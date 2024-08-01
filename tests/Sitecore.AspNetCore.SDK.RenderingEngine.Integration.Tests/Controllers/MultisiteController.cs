using Microsoft.AspNetCore.Mvc;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Request;
using Sitecore.AspNetCore.SDK.RenderingEngine.Extensions;

namespace Sitecore.AspNetCore.SDK.RenderingEngine.Integration.Tests.Controllers;

public class MultisiteController : Controller
{
    [Route("/Home/TestMultisite")]
    [HttpGet]
    public JsonResult Index()
    {
        string? siteName = HttpContext.GetSitecoreRenderingContext()?.Response?.Request.SiteName();

        return new JsonResult(siteName);
    }
}