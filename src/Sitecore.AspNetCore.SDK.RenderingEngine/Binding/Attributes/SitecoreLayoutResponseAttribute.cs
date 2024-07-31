using Microsoft.AspNetCore.Mvc.ModelBinding;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Response;
using Sitecore.AspNetCore.SDK.RenderingEngine.Binding.Sources;

namespace Sitecore.AspNetCore.SDK.RenderingEngine.Binding.Attributes;

/// <summary>
/// Binds a <see cref="SitecoreLayoutResponse"/> object.
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter)]
public class SitecoreLayoutResponseAttribute : Attribute, IBindingSourceMetadata
{
    /// <inheritdoc />
    public BindingSource BindingSource => new SitecoreLayoutResponseBindingSource();
}