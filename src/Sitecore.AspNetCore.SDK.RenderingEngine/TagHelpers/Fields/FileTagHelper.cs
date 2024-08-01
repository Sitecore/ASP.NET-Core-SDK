using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Response.Model.Fields;
using File = Sitecore.AspNetCore.SDK.LayoutService.Client.Response.Model.Properties.File;

namespace Sitecore.AspNetCore.SDK.RenderingEngine.TagHelpers.Fields;

/// <summary>
/// Tag helper that renders HTML file download link for a Sitecore <see cref="FileField"/>.
/// </summary>
[HtmlTargetElement(RenderingEngineConstants.SitecoreTagHelpers.FileHtmlTag, Attributes = RenderingEngineConstants.SitecoreTagHelpers.AspForTagHelperAttribute, TagStructure = TagStructure.NormalOrSelfClosing)]
[HtmlTargetElement(RenderingEngineConstants.SitecoreTagHelpers.FileHtmlTag, Attributes = RenderingEngineConstants.SitecoreTagHelpers.FileTagHelperAttribute, TagStructure = TagStructure.NormalOrSelfClosing)]
[HtmlTargetElement("a", Attributes = RenderingEngineConstants.SitecoreTagHelpers.AspForTagHelperAttribute)]
[HtmlTargetElement("a", Attributes = RenderingEngineConstants.SitecoreTagHelpers.FileTagHelperAttribute)]
public class FileTagHelper : TagHelper
{
    private const string FileLinkTag = "a";
    private const string HrefAttribute = "href";
    private const string TypeAttribute = "type";
    private const string TitleAttribute = "title";
    private const string TargetAttribute = "target";

    /// <summary>
    /// Gets or sets the model value.
    /// </summary>
    [HtmlAttributeName(RenderingEngineConstants.SitecoreTagHelpers.AspForTagHelperAttribute)]
    public ModelExpression? For { get; set; }

    /// <summary>
    /// Gets or sets a value indicating target attribute.
    /// </summary>
    public string Target { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the file value.
    /// </summary>
    [HtmlAttributeName(RenderingEngineConstants.SitecoreTagHelpers.FileTagHelperAttribute)]
    public FileField? FileModel { get; set; }

    /// <inheritdoc/>
    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(output);

        if (output.TagName != null && output.TagName.Equals(RenderingEngineConstants.SitecoreTagHelpers.FileHtmlTag, StringComparison.OrdinalIgnoreCase))
        {
            output.TagName = FileLinkTag;
            output.TagMode = TagMode.StartTagAndEndTag;
        }

        FileField? field = FileModel ?? For?.Model as FileField;

        File? file = field?.Value;
        if (file == null || string.IsNullOrWhiteSpace(file.Src))
        {
            return;
        }

        if (!output.Attributes.ContainsName(HrefAttribute))
        {
            output.Attributes.Add(HrefAttribute, file.Src);
        }

        if (!string.IsNullOrWhiteSpace(file.MimeType) && !output.Attributes.ContainsName(TypeAttribute))
        {
            output.Attributes.Add(TypeAttribute, file.MimeType);
        }

        if (!string.IsNullOrWhiteSpace(file.Description) && !output.Attributes.ContainsName(TitleAttribute))
        {
            output.Attributes.Add(TitleAttribute, file.Description);
        }

        if (!string.IsNullOrWhiteSpace(Target))
        {
            output.Attributes.Add(TargetAttribute, Target);
        }

        string? innerContent = output.GetChildContentAsync()?.Result?.GetContent();
        if (!string.IsNullOrWhiteSpace(innerContent))
        {
            output.Content.AppendHtml(innerContent);
        }
        else if (!string.IsNullOrWhiteSpace(file.Title))
        {
            output.Content.Append(file.Title);
        }
    }
}