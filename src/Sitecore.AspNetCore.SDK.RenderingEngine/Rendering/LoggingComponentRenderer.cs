using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sitecore.AspNetCore.SDK.RenderingEngine.Interfaces;

namespace Sitecore.AspNetCore.SDK.RenderingEngine.Rendering;

/// <summary>
/// An <see cref="IComponentRenderer"/> that writes to a specified log instead of generating HTML output.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="LoggingComponentRenderer"/> class.
/// </remarks>
/// <param name="logger">The <see cref="ILogger"/> to use for logging.</param>
public class LoggingComponentRenderer(ILogger<LoggingComponentRenderer> logger)
    : IComponentRenderer
{
    private static readonly IHtmlContent HtmlContent = new HtmlString(string.Empty);

    private readonly ILogger<LoggingComponentRenderer> _logger = logger != null ? logger : throw new ArgumentNullException(nameof(logger));

    /// <summary>
    /// Creates an instance of a <see cref="ComponentRendererDescriptor"/> for the <see cref="LoggingComponentRenderer"/> class.
    /// </summary>
    /// <param name="match">A predicate to use when attempting to match a layout component.</param>
    /// <returns>An instance of <see cref="ComponentRendererDescriptor"/> that describes the <see cref="LoggingComponentRenderer"/>.</returns>
    public static ComponentRendererDescriptor Describe(Predicate<string> match)
    {
        ArgumentNullException.ThrowIfNull(match);

        return new ComponentRendererDescriptor(
            match,
            sp => ActivatorUtilities.CreateInstance<LoggingComponentRenderer>(sp));
    }

    /// <inheritdoc />
    public Task<IHtmlContent> Render(ISitecoreRenderingContext renderingContext, ViewContext viewContext)
    {
        ArgumentNullException.ThrowIfNull(renderingContext);
        ArgumentNullException.ThrowIfNull(viewContext);

        _logger.LogWarning("{TypeName}: Render method called. Component name: {ComponentName}", GetType().Name, renderingContext.Component?.Name);

        return Task.FromResult(HtmlContent);
    }
}