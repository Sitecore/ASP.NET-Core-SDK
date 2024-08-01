using Microsoft.AspNetCore.Mvc.ModelBinding;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Response.Model;
using Sitecore.AspNetCore.SDK.RenderingEngine.Binding.Sources;

namespace Sitecore.AspNetCore.SDK.RenderingEngine.Binding.Attributes;

/// <summary>
/// Binds a Sitecore <see cref="Context"/> property.
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter)]
public class SitecoreContextPropertyAttribute : Attribute, IBindingSourceMetadata
{
    /// <summary>
    /// Gets or sets the name of the context property to use for binding.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <inheritdoc />
    public BindingSource BindingSource => new SitecoreLayoutContextPropertyBindingSource(Name);
}