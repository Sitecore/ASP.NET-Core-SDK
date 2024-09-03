using Sitecore.AspNetCore.SDK.RenderingEngine.Binding.Attributes;

namespace Sitecore.AspNetCore.SDK.RenderingEngine.Integration.Tests.ComponentModels;

public class PartialDesignDynamicPlaceholder
{
    [SitecoreComponentParameter(Name ="sig")]
    public string? Sig { get; set; }
}