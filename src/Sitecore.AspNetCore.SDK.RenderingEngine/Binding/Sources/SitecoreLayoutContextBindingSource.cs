using System.Text.Json;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Response.Model;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Serialization;
using Sitecore.AspNetCore.SDK.RenderingEngine.Interfaces;

namespace Sitecore.AspNetCore.SDK.RenderingEngine.Binding.Sources;

/// <summary>
/// Binding source for binding Sitecore <see cref="Context"/> data.
/// </summary>
public class SitecoreLayoutContextBindingSource : SitecoreLayoutBindingSource
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SitecoreLayoutContextBindingSource"/> class.
    /// </summary>
    public SitecoreLayoutContextBindingSource()
        : base(nameof(Context), nameof(Context), false, false)
    {
    }

    /// <inheritdoc/>
    public override object? GetModel(IServiceProvider serviceProvider, ModelBindingContext bindingContext, ISitecoreRenderingContext renderingContext)
    {
        ArgumentNullException.ThrowIfNull(serviceProvider);
        ArgumentNullException.ThrowIfNull(bindingContext);
        ArgumentNullException.ThrowIfNull(renderingContext);

        string? rawData = renderingContext.Response?.Content?.ContextRawData;
        if (rawData == null)
        {
            return null;
        }

        JsonDocument doc = JsonDocument.Parse(rawData);
        JsonProperty prop = doc.RootElement
            .EnumerateObject()
            .FirstOrDefault(p => p.Name.Equals(bindingContext.FieldName, StringComparison.InvariantCultureIgnoreCase));
        if (prop.Value.ValueKind != JsonValueKind.Undefined)
        {
            string data = prop.Value.ToString();
            if (!string.IsNullOrWhiteSpace(data))
            {
                return JsonSerializer.Deserialize(data, bindingContext.ModelMetadata.ModelType, JsonLayoutServiceSerializer.GetDefaultSerializerOptions());
            }
        }

        return JsonSerializer.Deserialize(rawData, bindingContext.ModelMetadata.ModelType, JsonLayoutServiceSerializer.GetDefaultSerializerOptions());
    }
}