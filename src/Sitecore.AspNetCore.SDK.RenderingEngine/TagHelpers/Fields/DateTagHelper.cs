using System.Globalization;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Response.Model.Fields;
using Sitecore.AspNetCore.SDK.RenderingEngine.Rendering;

namespace Sitecore.AspNetCore.SDK.RenderingEngine.TagHelpers.Fields;

/// <summary>
/// Tag helper that renders HTML date for a Sitecore <see cref="DateField"/>.
/// </summary>
[HtmlTargetElement(RenderingEngineConstants.SitecoreTagHelpers.DateHtmlTag, Attributes = RenderingEngineConstants.SitecoreTagHelpers.AspForTagHelperAttribute, TagStructure = TagStructure.NormalOrSelfClosing)]
[HtmlTargetElement(RenderingEngineConstants.SitecoreTagHelpers.DateHtmlTag, Attributes = RenderingEngineConstants.SitecoreTagHelpers.DateTagHelperAttribute, TagStructure = TagStructure.NormalOrSelfClosing)]
[HtmlTargetElement("*", Attributes = RenderingEngineConstants.SitecoreTagHelpers.AspForTagHelperAttribute)]
[HtmlTargetElement("*", Attributes = RenderingEngineConstants.SitecoreTagHelpers.DateTagHelperAttribute)]
public class DateTagHelper(IEditableChromeRenderer chromeRenderer) : TagHelper
{
    private readonly IEditableChromeRenderer _chromeRenderer = chromeRenderer ?? throw new ArgumentNullException(nameof(chromeRenderer));

    /// <summary>
    /// Gets or sets the model value.
    /// </summary>
    [HtmlAttributeName(RenderingEngineConstants.SitecoreTagHelpers.AspForTagHelperAttribute)]
    public ModelExpression? For { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the field can be edited.
    /// </summary>
    public bool Editable { get; set; } = true;

    /// <summary>
    /// Gets or sets a format for the date.
    /// </summary>
    public string? DateFormat { get; set; }

    /// <summary>
    /// Gets or sets a culture for the number.
    /// </summary>
    public string? Culture { get; set; }

    /// <summary>
    /// Gets or sets the date value.
    /// </summary>
    [HtmlAttributeName(RenderingEngineConstants.SitecoreTagHelpers.DateTagHelperAttribute)]
    public DateField? DateModel { get; set; }

    /// <inheritdoc/>
    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(output);

        if (output.TagName != null && output.TagName.Equals(RenderingEngineConstants.SitecoreTagHelpers.DateHtmlTag, StringComparison.OrdinalIgnoreCase))
        {
            output.TagName = null;
        }

        DateField? field = DateModel ?? For?.Model as DateField;

        bool outputEditableMarkup = Editable && !string.IsNullOrEmpty(field?.EditableMarkup);

        if (field == null || (field.Value.Equals(DateTime.MinValue) && !outputEditableMarkup))
        {
            return;
        }

        CultureInfo culture = !string.IsNullOrWhiteSpace(Culture) ? CultureInfo.CreateSpecificCulture(Culture) : CultureInfo.CurrentCulture;

        string formattedDate = !string.IsNullOrWhiteSpace(DateFormat) ? field.Value.ToString(DateFormat, !string.IsNullOrWhiteSpace(Culture) ? culture : CultureInfo.InvariantCulture) : field.Value.ToString(culture);

        HtmlString html = outputEditableMarkup ? new HtmlString(field.EditableMarkup) : new HtmlString(formattedDate);

        if (field.OpeningChrome != null)
        {
            output.Content.AppendHtml(_chromeRenderer.Render(field.OpeningChrome));
        }

        output.Content.AppendHtml(html);

        if (field.ClosingChrome != null)
        {
            output.Content.AppendHtml(_chromeRenderer.Render(field.ClosingChrome));
        }
    }
}