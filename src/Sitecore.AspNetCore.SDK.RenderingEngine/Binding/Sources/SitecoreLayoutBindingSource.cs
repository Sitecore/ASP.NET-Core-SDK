using System.Reflection;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Response.Model;
using Sitecore.AspNetCore.SDK.RenderingEngine.Interfaces;

namespace Sitecore.AspNetCore.SDK.RenderingEngine.Binding.Sources;

/// <summary>
/// Utilities for binding Sitecore layout data.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="SitecoreLayoutBindingSource"/> class.
/// </remarks>
/// <param name="id">The binding source ID.</param>
/// <param name="displayName">The display name.</param>
/// <param name="isGreedy">A value indicating whether the source is greedy.</param>
/// <param name="isFromRequest">A value indicating whether the data comes from the HTTP request.</param>
public abstract class SitecoreLayoutBindingSource(string id, string displayName, bool isGreedy, bool isFromRequest)
    : BindingSource(id, displayName, isGreedy, isFromRequest)
{
    /// <summary>
    /// Gets or sets the binding source name.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Gets the model for binding.
    /// </summary>
    /// <param name="serviceProvider">The <see cref="IServiceProvider"/>.</param>
    /// <param name="bindingContext">The <see cref="ModelBindingContext"/>.</param>
    /// <param name="context">The <see cref="ISitecoreRenderingContext"/>.</param>
    /// <returns>The bound model.</returns>
    public abstract object? GetModel(IServiceProvider serviceProvider, ModelBindingContext bindingContext, ISitecoreRenderingContext context);

    /// <summary>
    /// Gets the value of an object's property.
    /// </summary>
    /// <param name="bindingContext">The <see cref="ModelBindingContext"/>.</param>
    /// <param name="source">The source object to process.</param>
    /// <typeparam name="T">The type of the source object.</typeparam>
    /// <returns>A property object.</returns>
    protected object? GetPropertyModel<T>(ModelBindingContext bindingContext, T? source)
        where T : class
    {
        if (source == null)
        {
            return null;
        }

        string? propertyName = !string.IsNullOrWhiteSpace(Name) ? Name : bindingContext.FieldName;

        if (string.IsNullOrWhiteSpace(propertyName))
        {
            return null;
        }

        PropertyInfo? property = source.GetType().GetProperty(propertyName);
        if (property != null && (bindingContext.ModelType == property.PropertyType || bindingContext.ModelType == typeof(object)))
        {
            return property.GetValue(source, null);
        }

        return null;
    }

    /// <summary>
    /// Get the value of an object's field.
    /// </summary>
    /// <param name="bindingContext">The <see cref="ModelBindingContext"/>.</param>
    /// <param name="source">The source object to process.</param>
    /// <param name="currentRoute">The route for the current page.</param>
    /// <typeparam name="T">The type of the source object.</typeparam>
    /// <returns>A field object.</returns>
    protected object? GetFieldModel<T>(ModelBindingContext bindingContext, T? source, Route? currentRoute)
        where T : IFieldsReader
    {
        if (source == null)
        {
            return null;
        }

        string? fieldName = !string.IsNullOrWhiteSpace(Name) ? Name : bindingContext.FieldName;

        if (string.IsNullOrWhiteSpace(fieldName))
        {
            return null;
        }

        if (source.TryReadField(bindingContext.ModelMetadata.ModelType, fieldName, out object? field))
        {
            return field;
        }

        if (currentRoute != null && currentRoute.TryReadField(bindingContext.ModelMetadata.ModelType, fieldName, out object? routeField))
        {
            return routeField;
        }

        return null;
    }
}