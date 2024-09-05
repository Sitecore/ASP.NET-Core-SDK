using Sitecore.AspNetCore.SDK.LayoutService.Client.Serialization.Converter;
using Sitecore.AspNetCore.SDK.RenderingEngine.Binding.Attributes;

namespace Sitecore.AspNetCore.SDK.RenderingEngine.Integration.Tests.ComponentModels;

public class CustomResolver
{
    [SitecoreComponentField(Name = FieldParser.CustomContentFieldKey)]
    public CustomResolverModel[]? CustomContent { get; set; }
}