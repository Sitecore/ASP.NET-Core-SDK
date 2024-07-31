using Sitecore.AspNetCore.SDK.LayoutService.Client.Response.Model.Fields;
using Route = Sitecore.AspNetCore.SDK.LayoutService.Client.Response.Model.Route;

namespace Sitecore.AspNetCore.SDK.RenderingEngine.Integration.Tests.ViewModels;

public class WithBoundSitecoreRouteViewModel
{
    public Route? SitecoreRoute { get; set; }

    public string? DatabaseName { get; set; }

    public TextField? PageTitle { get; set; }
}