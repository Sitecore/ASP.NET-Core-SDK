using Sitecore.AspNetCore.SDK.LayoutService.Client.Response.Model.Fields;
using Sitecore.AspNetCore.SDK.RenderingEngine.Binding.Attributes;

// ReSharper disable IdentifierTypo
namespace Sitecore.AspNetCore.SDK.RenderingEngine.Integration.Tests.ComponentModels;

public class ComponentWithMissingData
{
    public TextField? InexistentComponentTextField { get; set; }

    [SitecoreRouteProperty]
    public string? InexistentRouteProperty { get; set; }

    [SitecoreContextProperty]
    public bool? InexistentContextProperty { get; set; }

    [SitecoreRouteField]
    public TextField? InexistentRouteField { get; set; }
}