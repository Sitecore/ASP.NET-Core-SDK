using Sitecore.AspNetCore.SDK.LayoutService.Client.Response.Model;

namespace Sitecore.AspNetCore.SDK.RenderingEngine.Tests.Mocks;

public class TestPlaceholderFeature : IPlaceholderFeature
{
    public string? Content { get; set; }
}