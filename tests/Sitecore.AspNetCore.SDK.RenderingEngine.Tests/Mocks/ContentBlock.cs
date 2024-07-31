using Sitecore.AspNetCore.SDK.LayoutService.Client.Response.Model.Fields;

namespace Sitecore.AspNetCore.SDK.RenderingEngine.Tests.Mocks;

public class ContentBlock
{
    public TextField? Heading { get; set; }

    public RichTextField? Content { get; set; }
}