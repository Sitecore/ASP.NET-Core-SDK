using Microsoft.AspNetCore.Mvc.ModelBinding;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Response.Model;
using Sitecore.AspNetCore.SDK.RenderingEngine.Binding.Sources;

namespace Sitecore.AspNetCore.SDK.RenderingEngine.Binding.Attributes;

/// <summary>
/// Binds a Sitecore <see cref="Context"/> object.
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter)]
public class SitecoreContextAttribute : Attribute, IBindingSourceMetadata
{
    /// <inheritdoc />
    public BindingSource BindingSource => new SitecoreLayoutContextBindingSource();
}