using System.Collections.Concurrent;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Response.Model;
using Sitecore.AspNetCore.SDK.RenderingEngine.Configuration;
using Sitecore.AspNetCore.SDK.RenderingEngine.Properties;

namespace Sitecore.AspNetCore.SDK.RenderingEngine.Rendering;

/// <inheritdoc />
public class ComponentRendererFactory : IComponentRendererFactory
{
    private readonly IServiceProvider _services;
    private readonly RenderingEngineOptions _options;
    private readonly ConcurrentDictionary<string, IComponentRenderer> _cache = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="ComponentRendererFactory"/> class.
    /// </summary>
    /// <param name="options">The <see cref="RenderingEngineOptions"/> for this instance.</param>
    /// <param name="services">The services used for component renderer resolution.</param>
    public ComponentRendererFactory(
        IOptions<RenderingEngineOptions> options,
        IServiceProvider services)
    {
        ArgumentNullException.ThrowIfNull(options);
        _services = services ?? throw new ArgumentNullException(nameof(services));
        _options = options.Value;
    }

    /// <inheritdoc />
    public IComponentRenderer GetRenderer(Component component)
    {
        ArgumentNullException.ThrowIfNull(component);
        ArgumentException.ThrowIfNullOrWhiteSpace(component.Name, nameof(component) + nameof(component.Name));

        return _cache.GetOrAdd(component.Name, _ =>
        {
            ComponentRendererDescriptor? defaultRendererDescriptor = _options.DefaultRenderer;
            KeyValuePair<int, ComponentRendererDescriptor> matchedRendererDescriptor =
                _options.RendererRegistry.FirstOrDefault(x => x.Value.Match(component.Name));

            ComponentRendererDescriptor rendererDescriptor =
                (matchedRendererDescriptor.Value ?? defaultRendererDescriptor) ??
                throw new InvalidOperationException(
                    string.Format(Resources.Exception_ComponentRendererDescriptorIsNull, component.Name));
            using IServiceScope scope = _services.CreateScope();
            return rendererDescriptor.GetOrCreate(scope.ServiceProvider);
        });
    }
}