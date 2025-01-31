using System.Text;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Response.Model.Fields;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Response.Model.Properties;
using Sitecore.AspNetCore.SDK.RenderingEngine.Rendering;

namespace Sitecore.AspNetCore.SDK.RenderingEngine.TagHelpers.Fields;

/// <summary>
/// Tag helper that renders HTML hyperlink for a Sitecore <see cref="HyperLinkField"/>.
/// </summary>
[HtmlTargetElement(RenderingEngineConstants.SitecoreTagHelpers.LinkHtmlTag, Attributes = RenderingEngineConstants.SitecoreTagHelpers.AspForTagHelperAttribute, TagStructure = TagStructure.NormalOrSelfClosing)]
[HtmlTargetElement(RenderingEngineConstants.SitecoreTagHelpers.LinkHtmlTag, Attributes = RenderingEngineConstants.SitecoreTagHelpers.LinkTagHelperAttribute, TagStructure = TagStructure.NormalOrSelfClosing)]
[HtmlTargetElement("a", Attributes = RenderingEngineConstants.SitecoreTagHelpers.AspForTagHelperAttribute)]
[HtmlTargetElement("a", Attributes = RenderingEngineConstants.SitecoreTagHelpers.LinkTagHelperAttribute)]
public class LinkTagHelper(IEditableChromeRenderer chromeRenderer) : TagHelper
{
    private const string HrefAttribute = "href";
    private const string TargetAttribute = "target";
    private const string TitleAttribute = "title";
    private const string ClassAttribute = "class";
    private const string AnchorTag = "a";
    private const string RelAttribute = "rel";
    private const string BlankValue = "_blank";
    private const string AnchorValue = "#";
    private readonly IEditableChromeRenderer chromeRenderer = chromeRenderer;

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
    /// Gets or sets the link value.
    /// </summary>
    [HtmlAttributeName(RenderingEngineConstants.SitecoreTagHelpers.LinkTagHelperAttribute)]
    public HyperLinkField? LinkModel { get; set; }

    /// <inheritdoc/>
    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(output);

        HyperLinkField? field = LinkModel ?? For?.Model as HyperLinkField;
        bool outputEditableMarkup = Editable &&
                                    ((!string.IsNullOrEmpty(field?.EditableMarkupFirst) && !string.IsNullOrWhiteSpace(field?.EditableMarkupLast)) || (field?.OpeningChrome != null && field?.ClosingChrome != null));

        if (field == null || (string.IsNullOrWhiteSpace(field.Value.Href) && !outputEditableMarkup))
        {
            return;
        }

        if (output.TagName != null && (output.TagName.Equals(RenderingEngineConstants.SitecoreTagHelpers.LinkHtmlTag, StringComparison.OrdinalIgnoreCase) || outputEditableMarkup))
        {
            output.TagName = null;
        }

        if (outputEditableMarkup)
        {
            RenderEditableMarkup(output, field);
        }
        else
        {
            RenderMarkup(output, field);
        }
    }

    private void RenderMarkup(TagHelperOutput output, HyperLinkField field)
    {
        if (output.TagName == null)
        {
            output.Content.SetHtmlContent(GenerateLink(field.Value, output));
        }
        else
        {
            HyperLink hyperLink = field.Value;

            output.Attributes.Add(HrefAttribute, BuildHref(hyperLink));

            if (!string.IsNullOrWhiteSpace(hyperLink.Target) && !output.Attributes.ContainsName(TargetAttribute))
            {
                output.Attributes.Add(TargetAttribute, hyperLink.Target);
            }

            if (!string.IsNullOrWhiteSpace(hyperLink.Title) && !output.Attributes.ContainsName(TitleAttribute))
            {
                output.Attributes.Add(TitleAttribute, hyperLink.Title);
            }

            if (!string.IsNullOrWhiteSpace(hyperLink.Class) && !output.Attributes.ContainsName(ClassAttribute))
            {
                output.Attributes.Add(ClassAttribute, hyperLink.Class);
            }

            if (hyperLink.Target == BlankValue && !output.Attributes.ContainsName(RelAttribute))
            {
                // information disclosure attack prevention keeps target blank site from getting ref to window.opener
                output.Attributes.Add(RelAttribute, "noopener noreferrer");
            }

            string? innerContent = output.GetChildContentAsync()?.Result?.GetContent();
            if (string.IsNullOrWhiteSpace(innerContent) && !string.IsNullOrWhiteSpace(hyperLink.Text))
            {
                output.Content.Append(field.Value.Text);
            }
        }
    }

    private void RenderEditableMarkup(TagHelperOutput output, HyperLinkField field)
    {
        if (field.OpeningChrome != null && field.ClosingChrome != null)
        {
            output.Content.AppendHtml(chromeRenderer.Render(field.OpeningChrome));

            if (field.Value.Href == string.Empty)
            {
                output.Content.AppendHtml("<span tabindex=\"0\" style=\"cursor: pointer;\">[No text in field]</span>");
            }
            else
            {
                output.Content.AppendHtml(GenerateLink(field.Value, output));
            }

            output.Content.AppendHtml(chromeRenderer.Render(field.ClosingChrome));
        }
        else
        {
            DefaultTagHelperContent content = new();
            _ = content.AppendHtml(new HtmlString(field.EditableMarkupFirst));
            _ = content.AppendHtml(new HtmlString(field.EditableMarkupLast));
            output.Content.SetHtmlContent(content);
        }
    }

    /// <summary>
    /// Generates anchor HTML tag.
    /// </summary>
    /// <param name="hyperLink">The <see cref="HyperLinkField"/> field.</param>
    /// <param name="output">Tag helper output.</param>
    /// <returns>Anchor HTML tag.</returns>
    private static TagBuilder GenerateLink(HyperLink hyperLink, TagHelperOutput output)
    {
        ArgumentNullException.ThrowIfNull(hyperLink);

        TagBuilder tagBuilder = new(AnchorTag);

        SetTagContent(hyperLink, output, tagBuilder);

        SetAttributes(hyperLink, output, tagBuilder);

        return tagBuilder;
    }

    private static void SetAttributes(HyperLink hyperLink, TagHelperOutput output, TagBuilder tagBuilder)
    {
        tagBuilder.Attributes.Add(HrefAttribute, BuildHref(hyperLink));

        if (!string.IsNullOrWhiteSpace(hyperLink.Target))
        {
            tagBuilder.MergeAttribute(TargetAttribute, hyperLink.Target);
        }

        if (!string.IsNullOrWhiteSpace(hyperLink.Title))
        {
            tagBuilder.MergeAttribute(TitleAttribute, hyperLink.Title);
        }

        if (!string.IsNullOrWhiteSpace(hyperLink.Class))
        {
            tagBuilder.MergeAttribute(ClassAttribute, hyperLink.Class);
        }

        foreach (TagHelperAttribute? attribute in output.Attributes)
        {
            tagBuilder.MergeAttribute(attribute.Name, attribute.Value.ToString(), true);
        }

        if (hyperLink.Target == BlankValue && !tagBuilder.Attributes.ContainsKey(RelAttribute))
        {
            // information disclosure attack prevention keeps target blank site from getting ref to window.opener
            tagBuilder.MergeAttribute(RelAttribute, "noopener noreferrer");
        }
    }

    private static void SetTagContent(HyperLink hyperLink, TagHelperOutput output, TagBuilder tagBuilder)
    {
        TagHelperContent? childContext = output.GetChildContentAsync().Result;
        string? existingTagContent = childContext.GetContent();

        // user tag content has priority
        if (!string.IsNullOrWhiteSpace(existingTagContent))
        {
            tagBuilder.InnerHtml.SetHtmlContent(existingTagContent);
        }
        else
        {
            tagBuilder.InnerHtml.SetContent(hyperLink.Text!);
        }
    }

    private static string BuildHref(HyperLink hyperLink)
    {
        StringBuilder sb = new();
        sb.Append(hyperLink.Href);

        if (!string.IsNullOrWhiteSpace(hyperLink.Anchor))
        {
            sb.Append(AnchorValue[0]);
            sb.Append(hyperLink.Anchor);
        }

        return sb.ToString();
    }
}