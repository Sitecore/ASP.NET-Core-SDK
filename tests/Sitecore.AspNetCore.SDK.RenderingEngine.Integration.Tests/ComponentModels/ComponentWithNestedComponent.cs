using Sitecore.AspNetCore.SDK.LayoutService.Client.Response.Model.Fields;

namespace Sitecore.AspNetCore.SDK.RenderingEngine.Integration.Tests.ComponentModels;

public class ComponentWithNestedComponent
{
    public TextField? TestField { get; set; }

    public RichTextField? RichTextField { get; set; }

    public Component5? NestedComponent { get; set; }
}