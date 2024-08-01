using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Sitecore.AspNetCore.SDK.ExperienceEditor.Properties;
using Sitecore.AspNetCore.SDK.ExperienceEditor.TagHelpers.Model;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Response.Model;
using Sitecore.AspNetCore.SDK.RenderingEngine.Extensions;
using Sitecore.AspNetCore.SDK.RenderingEngine.Interfaces;

namespace Sitecore.AspNetCore.SDK.ExperienceEditor.TagHelpers;

/// <summary>
/// Tag helper for the Sitecore placeholder element.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="EditFrameTagHelper"/> class.
/// </remarks>
/// <param name="chromeDataBuilder">An instance of <see cref="IChromeDataBuilder"/>.</param>
/// <param name="chromeDataSerializer">An instance of <see cref="IChromeDataSerializer"/>.</param>
[HtmlTargetElement(ExperienceEditorConstants.SitecoreTagHelpers.EditFrameHtmlTag)]
public class EditFrameTagHelper(IChromeDataBuilder chromeDataBuilder, IChromeDataSerializer chromeDataSerializer)
    : TagHelper
{
    private readonly IChromeDataBuilder _chromeDataBuilder = chromeDataBuilder ?? throw new ArgumentNullException(nameof(chromeDataBuilder));
    private readonly IChromeDataSerializer _chromeDataSerializer = chromeDataSerializer ?? throw new ArgumentNullException(nameof(chromeDataSerializer));

    /// <summary>
    /// Gets or sets the current view context for the tag helper.
    /// </summary>
    [HtmlAttributeNotBound]
    [ViewContext]
    public ViewContext? ViewContext { get; set; }

    /// <summary>
    /// Gets or sets the title of edit frame.
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// Gets or sets the tooltip of edit frame.
    /// </summary>
    public string? Tooltip { get; set; }

    /// <summary>
    /// Gets or sets the CSS class which be applied for edit frame.
    /// </summary>
    public string? CssClass { get; set; }

    /// <summary>
    /// Gets or sets the collection of edit frame buttons.
    /// </summary>
    public IEnumerable<EditButtonBase>? Buttons { get; set; }

    /// <summary>
    /// Gets or sets the data source of edit frame.
    /// </summary>
    public EditFrameDataSource? Source { get; set; }

    /// <summary>
    /// Gets or sets the parameters of edit frame.
    /// </summary>
    public IDictionary<string, object?>? Parameters { get; set; }

    /// <inheritdoc />
    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(output);

        SitecoreData? sitecoreData = GetSitecoreData();
        output.TagName = string.Empty;

        if (sitecoreData?.Context is not { IsEditing: true })
        {
            return;
        }

        Dictionary<string, object?> chromeData = [];
        Dictionary<string, string> frameProps = [];

        if (Source != null)
        {
            string? databaseName = Source.DatabaseName ?? sitecoreData.Route?.DatabaseName;
            string language = Source.Language ?? sitecoreData.Context.Language;

            chromeData["contextItemUri"] = frameProps["sc_item"] = $"sitecore://{databaseName}/{Source.ItemId}?lang={language}";
        }

        frameProps["class"] = $"scLooseFrameZone {CssClass}".Trim();
        chromeData["displayName"] = Title;
        chromeData["expandedDisplayName"] = Tooltip;
        chromeData["commands"] = Buttons?.Select(btn => _chromeDataBuilder.MapButtonToCommand(btn, Source?.ItemId, Parameters)).ToList();

        TagBuilder chromeDataTagBuilder = new("span");
        chromeDataTagBuilder.AddCssClass("scChromeData");
        chromeDataTagBuilder.InnerHtml.Append(_chromeDataSerializer.Serialize(chromeData));

        TagBuilder frameZoneTagBuilder = new("div");
        foreach ((string key, string? value) in frameProps)
        {
            frameZoneTagBuilder.Attributes.Add(key, value);
        }

        frameZoneTagBuilder.InnerHtml.AppendHtml(chromeDataTagBuilder);

        TagHelperContent? innerContent = await output.GetChildContentAsync().ConfigureAwait(false);
        frameZoneTagBuilder.InnerHtml.AppendHtml(innerContent.GetContent());

        output.Content.SetHtmlContent(frameZoneTagBuilder);
    }

    private SitecoreData? GetSitecoreData()
    {
        if (ViewContext == null)
        {
            throw new NullReferenceException(Resources.Exception_ViewContextCannotBeNull);
        }

        ISitecoreRenderingContext? renderingContext = ViewContext.HttpContext.GetSitecoreRenderingContext();
        return renderingContext?.Response?.Content?.Sitecore;
    }
}