using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sitecore.AspNetCore.SDK.RenderingEngine.Binding.Sources;
using Sitecore.AspNetCore.SDK.RenderingEngine.Extensions;
using Sitecore.AspNetCore.SDK.RenderingEngine.Interfaces;

namespace Sitecore.AspNetCore.SDK.RenderingEngine.Binding;

/// <summary>
/// Implements model binding for Sitecore layout data specified by the <see cref="SitecoreLayoutBindingSource"/>.
/// </summary>
/// <typeparam name="T">The type of the binding source.</typeparam>
public class SitecoreLayoutModelBinder<T> : IModelBinder
    where T : SitecoreLayoutBindingSource
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<SitecoreLayoutModelBinder<T>> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="SitecoreLayoutModelBinder{T}"/> class.
    /// </summary>
    /// <param name="serviceProvider">The <see cref="IServiceProvider"/>.</param>
    /// <param name="logger">The <see cref="ILogger"/> to use for logging.</param>
    public SitecoreLayoutModelBinder(IServiceProvider serviceProvider, ILogger<SitecoreLayoutModelBinder<T>> logger)
    {
        ArgumentNullException.ThrowIfNull(serviceProvider);
        ArgumentNullException.ThrowIfNull(logger);
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    /// <inheritdoc />
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        ArgumentNullException.ThrowIfNull(bindingContext);
        T bindingSource = bindingContext.BindingSource as T ?? Activator.CreateInstance<T>();
        ISitecoreRenderingContext? context = bindingContext.HttpContext.GetSitecoreRenderingContext();

        using (IServiceScope scope = _serviceProvider.CreateScope())
        {
            object? model = context != null ? bindingSource.GetModel(scope.ServiceProvider, bindingContext, context) : null;

            if (model != null)
            {
                bindingContext.ValidationState.TryAdd(model, new ValidationStateEntry { SuppressValidation = true });
                bindingContext.Result = ModelBindingResult.Success(model);
            }
            else
            {
                string componentWarning = context?.Component != null
                    ? $"\nComponent : {context.Component?.Name}"
                    : string.Empty;
                _logger.LogDebug(
                    "\nFailed to bind {contextFieldName} to {contextModelTypeName} type.\nBinding Source : {sourceDisplayName}{componentWarning}",
                    bindingContext.FieldName,
                    bindingContext.ModelType.Name,
                    bindingSource.DisplayName,
                    componentWarning);
            }
        }

        return Task.CompletedTask;
    }
}