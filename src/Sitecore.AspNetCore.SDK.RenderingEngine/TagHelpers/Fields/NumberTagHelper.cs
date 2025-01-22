using System.Globalization;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Response.Model.Fields;
using Sitecore.AspNetCore.SDK.RenderingEngine.Rendering;

namespace Sitecore.AspNetCore.SDK.RenderingEngine.TagHelpers.Fields;

/// <summary>
/// Tag helper that renders text for a Sitecore <see cref="NumberField"/>.
/// </summary>
[HtmlTargetElement(RenderingEngineConstants.SitecoreTagHelpers.NumberHtmlTag, Attributes = RenderingEngineConstants.SitecoreTagHelpers.AspForTagHelperAttribute, TagStructure = TagStructure.NormalOrSelfClosing)]
[HtmlTargetElement(RenderingEngineConstants.SitecoreTagHelpers.NumberHtmlTag, Attributes = RenderingEngineConstants.SitecoreTagHelpers.NumberTagHelperAttribute, TagStructure = TagStructure.NormalOrSelfClosing)]
[HtmlTargetElement("*", Attributes = RenderingEngineConstants.SitecoreTagHelpers.AspForTagHelperAttribute)]
[HtmlTargetElement("*", Attributes = RenderingEngineConstants.SitecoreTagHelpers.NumberTagHelperAttribute)]
public class NumberTagHelper(IEditableChromeRenderer chromeRenderer) : TagHelper
{
    private readonly IEditableChromeRenderer chromeRenderer = chromeRenderer;

    /// <summary>
    /// Gets or sets the model value.
    /// </summary>
    [HtmlAttributeName(RenderingEngineConstants.SitecoreTagHelpers.AspForTagHelperAttribute)]
    public ModelExpression? For { get; set; }

    /// <summary>
    /// Gets or sets a format for the number.
    /// </summary>
    public string? NumberFormat { get; set; }

    /// <summary>
    /// Gets or sets a culture for the number.
    /// </summary>
    public string? Culture { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the field can be edited.
    /// </summary>
    public bool Editable { get; set; } = true;

    /// <summary>
    /// Gets or sets the number value.
    /// </summary>
    [HtmlAttributeName(RenderingEngineConstants.SitecoreTagHelpers.NumberTagHelperAttribute)]
    public NumberField? NumberModel { get; set; }

    /// <inheritdoc />
    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(output);

        if (output.TagName != null && output.TagName.Equals(RenderingEngineConstants.SitecoreTagHelpers.NumberHtmlTag, StringComparison.OrdinalIgnoreCase))
        {
            output.TagName = null;
        }

        NumberField? field = NumberModel ?? For?.Model as NumberField;
        if (field == null)
        {
            return;
        }

        CultureInfo culture = !string.IsNullOrWhiteSpace(Culture) ? CultureInfo.CreateSpecificCulture(Culture) : CultureInfo.CurrentCulture;

        string formattedNumber = string.Empty;

        if (field.Value.HasValue)
        {
            formattedNumber = !string.IsNullOrWhiteSpace(NumberFormat) ? field.Value.Value.ToString(NumberFormat, culture) : field.Value.Value.ToString(culture);
        }

        bool outputEditableMarkup = Editable && !string.IsNullOrEmpty(field.EditableMarkup);
        string value = outputEditableMarkup ? field.EditableMarkup : formattedNumber;

        if (field.OpeningChrome != null)
        {
            output.Content.AppendHtml(chromeRenderer.Render(field.OpeningChrome));
        }

        output.Content.AppendHtml(value);

        if (field.ClosingChrome != null)
        {
            output.Content.AppendHtml(chromeRenderer.Render(field.ClosingChrome));
        }
    }
}