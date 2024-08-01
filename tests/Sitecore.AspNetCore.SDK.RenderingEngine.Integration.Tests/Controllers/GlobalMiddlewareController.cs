using Microsoft.AspNetCore.Mvc;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Response;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Response.Model;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Response.Model.Fields;
using Sitecore.AspNetCore.SDK.RenderingEngine.Binding.Attributes;
using Sitecore.AspNetCore.SDK.RenderingEngine.Integration.Tests.ViewModels;
using Sitecore.AspNetCore.SDK.TestData;
using Route = Sitecore.AspNetCore.SDK.LayoutService.Client.Response.Model.Route;

namespace Sitecore.AspNetCore.SDK.RenderingEngine.Integration.Tests.Controllers;

public class GlobalMiddlewareController : Controller
{
    public JsonResult Index() => new("success");

    [Route("WithRoute")]
    [HttpGet]
    public IActionResult WithRoute(Route route)
    {
        string view = route.LayoutId switch
        {
            TestConstants.MissingComponentPageLayoutId => "ViewWithMissingComponent",
            _ => nameof(WithRoute)
        };

        return View(view);
    }

    [Route("WithBoundSitecoreRoute")]
    [HttpGet]
    public IActionResult WithBoundSitecoreRoute(Route route, [SitecoreRouteProperty(Name = "DatabaseName")] string dbName, [SitecoreRouteField] TextField pageTitle)
    {
        WithBoundSitecoreRouteViewModel viewModel = new()
        {
            SitecoreRoute = route,
            DatabaseName = dbName,
            PageTitle = pageTitle
        };

        return View(viewModel);
    }

    [Route("WithBoundSitecoreContext")]
    [HttpGet]
    public IActionResult WithBoundSitecoreContext(Context context, [SitecoreContextProperty(Name = "IsEditing")] bool isPageEditorMode, [SitecoreContextProperty(Name = "Language")] string language)
    {
        WithBoundSitecoreContextViewModel viewModel = new()
        {
            SitecoreContext = context,
            IsPageEditing = isPageEditorMode,
            Language = language
        };
        return View(viewModel);
    }

    [Route("WithBoundSitecoreResponse")]
    [HttpGet]
    public IActionResult WithBoundSitecoreResponse(SitecoreLayoutResponse response)
    {
        return View(response);
    }
}