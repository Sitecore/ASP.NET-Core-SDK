using Sitecore.AspNetCore.SDK.LayoutService.Client.Response.Model;

namespace Sitecore.AspNetCore.SDK.RenderingEngine.Integration.Tests.ViewModels;

public class WithBoundSitecoreContextViewModel
{
    public Context? SitecoreContext { get; set; }

    public string? Language { get; set; }

    public bool IsPageEditing { get; set; }
}