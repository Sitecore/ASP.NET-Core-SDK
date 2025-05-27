using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Response.Model;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Response.Model.Fields;
using Sitecore.AspNetCore.SDK.RenderingEngine.Rendering;

namespace Sitecore.AspNetCore.SDK.RenderingEngine.TagHelpers.Fields;

/// <summary>
/// Tag helper that renders HTML text for a Sitecore <see cref="RichTextField"/>.
/// </summary>
[HtmlTargetElement(RenderingEngineConstants.SitecoreTagHelpers.RichTextHtmlTag, Attributes = RenderingEngineConstants.SitecoreTagHelpers.AspForTagHelperAttribute, TagStructure = TagStructure.NormalOrSelfClosing)]
[HtmlTargetElement(RenderingEngineConstants.SitecoreTagHelpers.RichTextHtmlTag, Attributes = RenderingEngineConstants.SitecoreTagHelpers.TextTagHelperAttribute, TagStructure = TagStructure.NormalOrSelfClosing)]
[HtmlTargetElement("*", Attributes = RenderingEngineConstants.SitecoreTagHelpers.AspForTagHelperAttribute)]
[HtmlTargetElement("*", Attributes = RenderingEngineConstants.SitecoreTagHelpers.TextTagHelperAttribute)]
public class RichTextTagHelper(IEditableChromeRenderer chromeRenderer) : TagHelper
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
    /// Gets or sets the rich text value.
    /// </summary>
    [HtmlAttributeName(RenderingEngineConstants.SitecoreTagHelpers.TextTagHelperAttribute)]
    public EditableField<string>? TextModel { get; set; }

    /// <inheritdoc/>
    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(output);

        if (output.TagName != null && output.TagName.Equals(RenderingEngineConstants.SitecoreTagHelpers.RichTextHtmlTag, StringComparison.OrdinalIgnoreCase))
        {
            output.TagName = null;
        }

        if ((TextModel ?? For?.Model) is not RichTextField richTextField)
        {
            return;
        }

        string html = string.Empty;
        if (Editable && richTextField.OpeningChrome != null)
        {
            html += _chromeRenderer.Render(richTextField.OpeningChrome);
            html += "<div>";
        }

        bool outputEditableMarkup = Editable && !string.IsNullOrEmpty(richTextField.EditableMarkup);
        html += outputEditableMarkup
            ? richTextField.EditableMarkup
            : richTextField.Value;

        if (Editable && richTextField.ClosingChrome != null)
        {
            html += "</div>";
            html += _chromeRenderer.Render(richTextField.ClosingChrome);
        }

        output.Content.SetHtmlContent(new HtmlString(html));
    }
}