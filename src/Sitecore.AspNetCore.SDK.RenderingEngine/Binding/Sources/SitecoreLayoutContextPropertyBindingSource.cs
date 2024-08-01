using System.ComponentModel;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Response.Model;
using Sitecore.AspNetCore.SDK.RenderingEngine.Interfaces;

namespace Sitecore.AspNetCore.SDK.RenderingEngine.Binding.Sources;

/// <summary>
/// Binding source for binding Sitecore <see cref="Context"/> property data.
/// </summary>
public class SitecoreLayoutContextPropertyBindingSource : SitecoreLayoutBindingSource
{
    private const string BindingSourceId = nameof(Context) + "Property";

    /// <summary>
    /// Initializes a new instance of the <see cref="SitecoreLayoutContextPropertyBindingSource"/> class.
    /// </summary>
    /// <param name="name">The name of the property in the Sitecore context to use for binding.</param>
    public SitecoreLayoutContextPropertyBindingSource(string name)
        : base(BindingSourceId, BindingSourceId, false, false)
    {
        Name = name;
    }

    /// <inheritdoc/>
    public override object? GetModel(IServiceProvider serviceProvider, ModelBindingContext bindingContext, ISitecoreRenderingContext renderingContext)
    {
        ArgumentNullException.ThrowIfNull(serviceProvider);
        ArgumentNullException.ThrowIfNull(bindingContext);
        ArgumentNullException.ThrowIfNull(renderingContext);

        Context? scContext = renderingContext.Response?.Content?.Sitecore?.Context;

        object? result = GetPropertyModel(bindingContext, scContext!);
        if (result != null)
        {
            return result;
        }

        string? innerObjectData = null;
        if (!string.IsNullOrWhiteSpace(renderingContext.Response?.Content?.ContextRawData))
        {
            JsonDocument doc = JsonDocument.Parse(renderingContext.Response?.Content?.ContextRawData!);
            JsonProperty prop = doc.RootElement
                .EnumerateObject()
                .FirstOrDefault(p => p.Name.Equals(bindingContext.FieldName, StringComparison.InvariantCultureIgnoreCase));
            if (prop.Value.ValueKind != JsonValueKind.Undefined)
            {
                innerObjectData = prop.Value.ToString();
            }
        }

        if (!string.IsNullOrWhiteSpace(innerObjectData))
        {
            TypeConverter converter = TypeDescriptor.GetConverter(bindingContext.ModelType);
            return converter.ConvertFrom(innerObjectData);
        }

        return null;
    }
}