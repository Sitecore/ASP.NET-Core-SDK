using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Sitecore.AspNetCore.SDK.RenderingEngine.Binding.Sources;
using Sitecore.AspNetCore.SDK.RenderingEngine.Interfaces;

namespace Sitecore.AspNetCore.SDK.RenderingEngine.Tests.Mocks;

[ExcludeFromCodeCoverage]
public class TestSitecoreLayoutBindingSource()
    : SitecoreLayoutBindingSource("test", "test", false, false)
{
    public override object GetModel(IServiceProvider serviceProvider, ModelBindingContext bindingContext, ISitecoreRenderingContext context)
    {
        return null!;
    }
}