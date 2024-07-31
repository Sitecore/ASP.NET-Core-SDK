using Sitecore.AspNetCore.SDK.RenderingEngine.Binding.Attributes;

namespace Sitecore.AspNetCore.SDK.RenderingEngine.Integration.Tests.ComponentModels;

public class CustomModelContextComponent
{
    public CustomContext? CustomContext { get; set; }

    [SitecoreContext]
    public CustomContextIndividual? CustomContextIndividual { get; set; }

    [SitecoreContext]
    public TestClass1? TestClass1 { get; set; }

    [SitecoreContextProperty]
    public string? SingleProperty { get; set; }
}