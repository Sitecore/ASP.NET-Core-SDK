using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Response.Model;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Response.Model.Fields;
using Sitecore.AspNetCore.SDK.RenderingEngine.Rendering;

namespace Sitecore.AspNetCore.SDK.RenderingEngine.TagHelpers.Fields;

/// <summary>
/// Tag helper that renders text for a Sitecore <see cref="TextField"/>.
/// </summary>
[HtmlTargetElement("*", Attributes = RenderingEngineConstants.SitecoreTagHelpers.AspForTagHelperAttribute)]
public partial class TextFieldTagHelper(IEditableChromeRenderer chromeRenderer) : TagHelper
{
    private readonly IEditableChromeRenderer chromeRenderer = chromeRenderer;

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

        if (Editable && field.OpeningChrome != null)
        {
            output.Content.AppendHtml(chromeRenderer.Render(field.OpeningChrome));
            output.Content.AppendHtml("<div>");
        }

        if (outputEditableMarkup || (ConvertNewLines && NewLineRegex().IsMatch(value)))
        {
            value = NewLineRegex().Replace(value, "<br />");
            output.Content.AppendHtml(value);
        }
        else
        {
            output.Content.Append(value);
        }

        if (Editable && field.ClosingChrome != null)
        {
            output.Content.AppendHtml("</div>");
            output.Content.AppendHtml(chromeRenderer.Render(field.ClosingChrome));
        }
    }

    [GeneratedRegex("\r?\n")]
    private static partial Regex NewLineRegex();
}