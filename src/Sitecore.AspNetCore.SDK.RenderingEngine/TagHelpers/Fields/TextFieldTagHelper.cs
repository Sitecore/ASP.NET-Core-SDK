using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Response.Model.Fields;
using Sitecore.AspNetCore.SDK.RenderingEngine.Rendering;

namespace Sitecore.AspNetCore.SDK.RenderingEngine.TagHelpers.Fields;

/// <summary>
/// Tag helper that renders text for a Sitecore <see cref="TextField"/>.
/// </summary>
[HtmlTargetElement("*", Attributes = RenderingEngineConstants.SitecoreTagHelpers.AspForTagHelperAttribute)]
public partial class TextFieldTagHelper(IEditableChromeRenderer chromeRenderer) : TagHelper
{
    private readonly IEditableChromeRenderer chromeRenderer = chromeRenderer ?? throw new ArgumentNullException(nameof(chromeRenderer));

    /// <summary>
    /// Gets or sets the model value.
    /// </summary>
    [HtmlAttributeName(RenderingEngineConstants.SitecoreTagHelpers.AspForTagHelperAttribute)]
    public ModelExpression? For { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to convert line endings to <br /> tags.
    /// </summary>
    [HtmlAttributeName("convert-new-lines")]
    public bool ConvertNewLines { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether the field can be edited.
    /// </summary>
    public bool Editable { get; set; } = true;

    /// <inheritdoc />
    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(output);

        if (For?.Model is not TextField field)
        {
            return;
        }

        bool outputEditableMarkup = Editable && !string.IsNullOrEmpty(field.EditableMarkup);
        string value = outputEditableMarkup ? field.EditableMarkup : field.Value;

        string html = string.Empty;
        bool isHtml = false;
        if (Editable && field.OpeningChrome != null)
        {
            html += chromeRenderer.Render(field.OpeningChrome);
            html += "<div>";
        }

        if (outputEditableMarkup || (ConvertNewLines && NewLineRegex().IsMatch(value)))
        {
            html += NewLineRegex().Replace(value, "<br />");
            isHtml = true;
        }
        else
        {
            html += value;
        }

        if (Editable && field.ClosingChrome != null)
        {
            html += "<div>";
            html += chromeRenderer.Render(field.ClosingChrome);
        }

        if (isHtml)
        {
            output.Content.SetHtmlContent(html);
        }
        else
        {
            output.Content.SetContent(html);
        }
    }

    [GeneratedRegex("\r?\n")]
    private static partial Regex NewLineRegex();
}