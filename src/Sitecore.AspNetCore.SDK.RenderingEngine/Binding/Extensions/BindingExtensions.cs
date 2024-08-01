using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Sitecore.AspNetCore.SDK.RenderingEngine.Binding.Providers;
using Sitecore.AspNetCore.SDK.RenderingEngine.Binding.Sources;

namespace Sitecore.AspNetCore.SDK.RenderingEngine.Binding.Extensions;

/// <summary>
/// Extensions to support model binding for Sitecore layout data.
/// </summary>
public static class BindingExtensions
{
    /// <summary>
    /// Gets the <see cref="SitecoreLayoutModelBinder{TSource}"/> for the specified model type and binding source.
    /// </summary>
    /// <param name="context">The <see cref="ModelBinderProviderContext"/> instance.</param>
    /// <typeparam name="TSource">The type of the binding source.</typeparam>
    /// <typeparam name="TType">The model type to bind to.</typeparam>
    /// <returns>The <see cref="BinderTypeModelBinder"/> instance.</returns>
    public static BinderTypeModelBinder? GetModelBinder<TSource, TType>(this ModelBinderProviderContext context)
        where TSource : SitecoreLayoutBindingSource
    {
        ArgumentNullException.ThrowIfNull(context);

        Type modelType = context.Metadata.UnderlyingOrModelType;

        if (modelType == typeof(TType) || typeof(TType).IsAssignableFrom(modelType))
        {
            return new BinderTypeModelBinder(typeof(SitecoreLayoutModelBinder<TSource>));
        }

        return context.GetModelBinder<TSource>();
    }

    /// <summary>
    /// Gets the <see cref="SitecoreLayoutModelBinder{TSource}"/> for the specified binding source.
    /// </summary>
    /// <param name="context">The <see cref="ModelBinderProviderContext"/> instance.</param>
    /// <typeparam name="TSource">The type of the binding source.</typeparam>
    /// <returns>The <see cref="BinderTypeModelBinder"/> instance.</returns>
    public static BinderTypeModelBinder? GetModelBinder<TSource>(this ModelBinderProviderContext context)
        where TSource : SitecoreLayoutBindingSource
    {
        ArgumentNullException.ThrowIfNull(context);
        return context.BindingInfo.BindingSource is TSource ? new BinderTypeModelBinder(typeof(SitecoreLayoutModelBinder<TSource>)) : null;
    }

    /// <summary>
    /// Adds the default Sitecore model binder providers to the <see cref="MvcOptions"/>.
    /// </summary>
    /// <param name="options">The <see cref="MvcOptions"/> to configure.</param>
    public static void AddSitecoreModelBinderProviders(this MvcOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        options.ModelBinderProviders.Insert(0, new SitecoreLayoutResponseModelBinderProvider());
        options.ModelBinderProviders.Insert(0, new SitecoreLayoutComponentModelBinderProvider());
        options.ModelBinderProviders.Insert(0, new SitecoreLayoutContextModelBinderProvider());
        options.ModelBinderProviders.Insert(0, new SitecoreLayoutRouteModelBinderProvider());
    }
}