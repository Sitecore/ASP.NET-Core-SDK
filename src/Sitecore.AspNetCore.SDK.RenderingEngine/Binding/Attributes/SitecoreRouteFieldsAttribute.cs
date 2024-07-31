using Microsoft.AspNetCore.Mvc.ModelBinding;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Response.Model;
using Sitecore.AspNetCore.SDK.RenderingEngine.Binding.Sources;

namespace Sitecore.AspNetCore.SDK.RenderingEngine.Binding.Attributes;

/// <summary>
/// Binds all fields for a Sitecore <see cref="Route"/>.
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter)]
public class SitecoreRouteFieldsAttribute : Attribute, IBindingSourceMetadata
{
    /// <inheritdoc />
    public BindingSource BindingSource => new SitecoreLayoutRouteFieldsBindingSource();
}