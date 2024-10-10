using Sitecore.AspNetCore.SDK.LayoutService.Client.Response.Model.Fields;

namespace Sitecore.AspNetCore.SDK.RenderingEngine.Integration.Tests.ComponentModels;

public class CustomResolverModel
{
    public List<string>? Styles { get; set; } = [];

    public List<CustomResolverModel>? Children { get; set; } = [];

    public string? Href { get; set; }

    public string? Querystring { get; set; }

    public TextField? NavigationTitle { get; set; }
}