using Sitecore.AspNetCore.SDK.LayoutService.Client.Response.Model.Fields;
using Sitecore.AspNetCore.SDK.RenderingEngine.Binding.Attributes;

// ReSharper disable IdentifierTypo
namespace Sitecore.AspNetCore.SDK.RenderingEngine.Integration.Tests.ComponentModels;

public class ComponentWithoutId
{
    [SitecoreComponentProperty(Name = "Id")]
    public string? InexistentComponentId { get; set; }

    public RichTextField? InexistentComponentRichTextField { get; set; }

    public TextField? InexistentComponentTextField { get; set; }
}