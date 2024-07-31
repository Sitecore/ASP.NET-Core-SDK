using Sitecore.AspNetCore.SDK.LayoutService.Client.Response.Model;

namespace Sitecore.AspNetCore.SDK.RenderingEngine.Integration.Tests.ComponentModels;

public class CustomContext : Context
{
    public TestClass1? TestClass1 { get; set; }

    public TestClass2? TestClass2 { get; set; }

    public string? SingleProperty { get; set; }
}