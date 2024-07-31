using Sitecore.AspNetCore.SDK.LayoutService.Client.Response.Model.Fields;

namespace Sitecore.AspNetCore.SDK.RenderingEngine.Integration.Tests.ComponentModels;

public class ComponentWithLinks
{
    public HyperLinkField? InternalLink { get; set; }

    public HyperLinkField? ParamsLink { get; set; }

    public TextField? Text { get; set; }
}