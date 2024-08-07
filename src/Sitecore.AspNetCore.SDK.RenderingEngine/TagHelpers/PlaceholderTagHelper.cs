using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Response.Model;
using Sitecore.AspNetCore.SDK.RenderingEngine.Extensions;
using Sitecore.AspNetCore.SDK.RenderingEngine.Interfaces;
using Sitecore.AspNetCore.SDK.RenderingEngine.Properties;
using Sitecore.AspNetCore.SDK.RenderingEngine.Rendering;

namespace Sitecore.AspNetCore.SDK.RenderingEngine.TagHelpers;

/// <summary>
/// Tag helper for the Sitecore placeholder element.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="PlaceholderTagHelper"/> class.
/// </remarks>
/// <param name="componentFactory">An instance of <see cref="IComponentRendererFactory"/>.</param>
/// <param name="chromeRenderer">An instance of <see cref="IEditableChromeRenderer"/>.</param>
[HtmlTargetElement(RenderingEngineConstants.SitecoreTagHelpers.PlaceholderHtmlTag)]
public class PlaceholderTagHelper(
    IComponentRendererFactory componentFactory,
    IEditableChromeRenderer chromeRenderer)
    : TagHelper
{
    private readonly IComponentRendererFactory _componentRendererFactory = componentFactory ?? throw new ArgumentNullException(nameof(componentFactory));

    private readonly IEditableChromeRenderer _chromeRenderer = chromeRenderer ?? throw new ArgumentNullException(nameof(chromeRenderer));

    /// <summary>
    /// Gets or sets the current view context for the tag helper.
    /// </summary>
    [HtmlAttributeNotBound]
    [ViewContext]
    public ViewContext? ViewContext { get; set; }

    /// <summary>
    /// Gets or sets the name of the placeholder to be rendered.
    /// </summary>
    public string? Name { get; set; }

    /// <inheritdoc />
    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(output);

        string? placeholderName = Name;

        if (string.IsNullOrEmpty(placeholderName))
        {
            output.Content.SetHtmlContent($"<!-- {Resources.Warning_PlaceholderNameWasNotDefined} -->");
            return;
        }

        output.TagName = string.Empty;
        if (ViewContext == null)
        {
            throw new NullReferenceException(Resources.Exception_ViewContextCannotBeNull);
        }

        ISitecoreRenderingContext renderingContext = ViewContext?.HttpContext.GetSitecoreRenderingContext() ??
                                                     throw new NullReferenceException(Resources.Exception_SitecoreLayoutCannotBeNull);
        Placeholder? placeholderFeatures = GetPlaceholderFeatures(placeholderName, renderingContext);

        if (placeholderFeatures == null)
        {
            output.Content.SetHtmlContent($"<!-- {Resources.Warning_PlaceholderWasNotDefined} -->");
            return;
        }

        if (IsInEditingMode(renderingContext) && IsPlaceHolderEmpty(placeholderFeatures))
        {
            output.Content.AppendHtml("<div class=\"sc-empty-placeholder\">");
        }

        bool foundPlaceholderFeatures = false;
        foreach (IPlaceholderFeature placeholderFeature in placeholderFeatures.OfType<IPlaceholderFeature>())
        {
            foundPlaceholderFeatures = true;

            IHtmlContent html;
            switch (placeholderFeature)
            {
                case Component component:
                    using (new ComponentHolder(renderingContext, component))
                    {
                        html = await RenderComponent(renderingContext, component)
                            .ConfigureAwait(false);
                    }

                    break;
                case EditableChrome chrome:
                    html = _chromeRenderer.Render(chrome);
                    break;
                default:
                    html = HtmlString.Empty;
                    break;
            }

            output.Content.AppendHtml(html);
        }

        if (IsInEditingMode(renderingContext) && IsPlaceHolderEmpty(placeholderFeatures))
        {
            output.Content.AppendHtml("</div>");
        }

        if (!foundPlaceholderFeatures)
        {
            output.Content.SetHtmlContent($"<div className=\"sc-jss-empty-placeholder\"></div>");
        }
    }

    private static bool IsInEditingMode(ISitecoreRenderingContext renderingContext)
    {
        return renderingContext?.Response?.Content?.Sitecore?.Context?.IsEditing ?? false;
    }

    private static bool IsPlaceHolderEmpty(Placeholder placeholderFeatures)
    {
        return !placeholderFeatures.Exists(x => x is Component);
    }

    private static Placeholder? GetPlaceholderFeatures(string placeholderName, ISitecoreRenderingContext renderingContext)
    {
        Placeholder? placeholderFeatures = null;

        // try to get the placeholder from the "context" component
        if (renderingContext.Component?.Placeholders.TryGetValue(placeholderName, out placeholderFeatures) ?? false)
        {
            return placeholderFeatures;
        }

        // top level placeholders do not have a "context" component set, so their component list can be retrieved directly from the Sitecore Route object
        Route? route = renderingContext.Response?.Content?.Sitecore?.Route;
        route?.Placeholders.TryGetValue(placeholderName, out placeholderFeatures);

        return placeholderFeatures;
    }

    private Task<IHtmlContent> RenderComponent(ISitecoreRenderingContext renderingContext, Component component)
    {
        if (ViewContext == null)
        {
            throw new NullReferenceException(Resources.Exception_RenderingContextCannotBeNull);
        }

        IComponentRenderer renderer = _componentRendererFactory.GetRenderer(component);
        return renderer.Render(renderingContext, ViewContext);
    }
}