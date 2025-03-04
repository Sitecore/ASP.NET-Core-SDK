﻿using HtmlAgilityPack;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Response.Model.Fields;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Response.Model.Properties;
using Sitecore.AspNetCore.SDK.RenderingEngine.Extensions;
using Sitecore.AspNetCore.SDK.RenderingEngine.Rendering;

namespace Sitecore.AspNetCore.SDK.RenderingEngine.TagHelpers.Fields;

/// <summary>
/// Tag helper that renders HTML hyperlink for a Sitecore <see cref="ImageField"/>.
/// </summary>
[HtmlTargetElement(RenderingEngineConstants.SitecoreTagHelpers.ImageHtmlTag, Attributes = RenderingEngineConstants.SitecoreTagHelpers.AspForTagHelperAttribute, TagStructure = TagStructure.NormalOrSelfClosing)]
[HtmlTargetElement(RenderingEngineConstants.SitecoreTagHelpers.ImageHtmlTag, Attributes = RenderingEngineConstants.SitecoreTagHelpers.ImageTagHelperAttribute, TagStructure = TagStructure.NormalOrSelfClosing)]
[HtmlTargetElement("img", Attributes = RenderingEngineConstants.SitecoreTagHelpers.AspForTagHelperAttribute)]
[HtmlTargetElement("img", Attributes = RenderingEngineConstants.SitecoreTagHelpers.ImageTagHelperAttribute)]
public class ImageTagHelper(IEditableChromeRenderer chromeRenderer) : TagHelper
{
    private const string ImgTag = "img";
    private const string ScrAttribute = "src";
    private const string AltAttribute = "alt";
    private const string ClassAttribute = "class";
    private const string WidthAttribute = "width";
    private const string HeightAttribute = "height";
    private const string HSpaceAttribute = "hspace";
    private const string VSpaceAttribute = "vspace";
    private const string TitleAttribute = "title";
    private const string BorderAttribute = "border";
    private readonly IEditableChromeRenderer chromeRenderer = chromeRenderer ?? throw new ArgumentNullException(nameof(chromeRenderer));

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
    /// Gets or sets the image value.
    /// </summary>
    [HtmlAttributeName(RenderingEngineConstants.SitecoreTagHelpers.ImageTagHelperAttribute)]
    public ImageField? ImageModel { get; set; }

    /// <summary>
    /// Gets or sets parameters that are passed to Sitecore to perform server-side resizing of the image.
    /// </summary>
    public object? ImageParams { get; set; }

    /// <inheritdoc/>
    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(output);

        if (output.TagName != null && output.TagName.Equals(RenderingEngineConstants.SitecoreTagHelpers.ImageHtmlTag, StringComparison.OrdinalIgnoreCase))
        {
            output.TagName = null;
        }

        ImageField? field = ImageModel ?? For?.Model as ImageField;

        if (field == null || (string.IsNullOrWhiteSpace(field.Value.Src) && (field.OpeningChrome == null && field.ClosingChrome == null)))
        {
            return;
        }

        bool outputEditableMarkup = Editable && !string.IsNullOrEmpty(field.EditableMarkup);

        if (outputEditableMarkup)
        {
            output.TagName = null;
            output.Content.SetHtmlContent(MergeEditableMarkupWithCustomAttributes(field.EditableMarkup, field, output));
        }
        else
        {
            if (output.TagName == null)
            {
                if (field.OpeningChrome != null)
                {
                    output.Content.AppendHtml(chromeRenderer.Render(field.OpeningChrome));
                }

                output.Content.AppendHtml(GenerateImage(field, output));

                if (field.ClosingChrome != null)
                {
                    output.Content.AppendHtml(chromeRenderer.Render(field.ClosingChrome));
                }
            }
            else
            {
                output.Attributes.Add(ScrAttribute, field.GetMediaLink(ImageParams));

                if (!string.IsNullOrWhiteSpace(field.Value.Alt))
                {
                    output.Attributes.Add(AltAttribute, field.Value.Alt);
                }

                if (!string.IsNullOrWhiteSpace(field.Value.Class))
                {
                    output.Attributes.Add(ClassAttribute, field.Value.Class);
                }

                if (field.Value.Border.HasValue)
                {
                    output.Attributes.Add(BorderAttribute, field.Value.Border.ToString());
                }

                if (!string.IsNullOrWhiteSpace(field.Value.Title))
                {
                    output.Attributes.Add(TitleAttribute, field.Value.Title);
                }

                if (field.Value.HSpace.HasValue)
                {
                    output.Attributes.Add(HSpaceAttribute, field.Value.HSpace.ToString());
                }

                if (field.Value.VSpace.HasValue)
                {
                    output.Attributes.Add(VSpaceAttribute, field.Value.VSpace.ToString());
                }

                if (field.Value.Width.HasValue)
                {
                    output.Attributes.Add(WidthAttribute, field.Value.Width.ToString());
                }

                if (field.Value.Height.HasValue)
                {
                    output.Attributes.Add(HeightAttribute, field.Value.Height.ToString());
                }
            }
        }
    }

    private TagBuilder GenerateImage(ImageField imageField, TagHelperOutput output)
    {
        Image image = imageField.Value;
        TagBuilder tagBuilder = new(ImgTag)
        {
            TagRenderMode = TagRenderMode.SelfClosing
        };

        if (!string.IsNullOrWhiteSpace(image.Src))
        {
            tagBuilder.Attributes.Add(ScrAttribute, imageField.GetMediaLink(ImageParams));
        }

        if (!string.IsNullOrWhiteSpace(image.Alt))
        {
            tagBuilder.MergeAttribute(AltAttribute, image.Alt);
        }

        if (!string.IsNullOrWhiteSpace(image.Class))
        {
            tagBuilder.MergeAttribute(ClassAttribute, image.Class);
        }

        if (image.Border.HasValue)
        {
            tagBuilder.MergeAttribute(BorderAttribute, image.Border.ToString());
        }

        if (!string.IsNullOrWhiteSpace(image.Title))
        {
            tagBuilder.MergeAttribute(TitleAttribute, image.Title);
        }

        if (image.HSpace.HasValue)
        {
            tagBuilder.MergeAttribute(HSpaceAttribute, image.HSpace.ToString());
        }

        if (image.VSpace.HasValue)
        {
            tagBuilder.MergeAttribute(VSpaceAttribute, image.VSpace.ToString());
        }

        if (image.Width.HasValue)
        {
            tagBuilder.MergeAttribute(WidthAttribute, image.Width.ToString());
        }

        if (image.Height.HasValue)
        {
            tagBuilder.MergeAttribute(HeightAttribute, image.Height.ToString());
        }

        foreach (TagHelperAttribute? attribute in output.Attributes)
        {
            tagBuilder.MergeAttribute(attribute.Name, attribute.Value.ToString(), true);
        }

        return tagBuilder;
    }

    private HtmlString MergeEditableMarkupWithCustomAttributes(string editableMarkUp, ImageField imageField, TagHelperOutput output)
    {
        // TODO Can we find a cheaper method to achieve this? Remove the HtmlAgilityPack dependency?
        HtmlDocument doc = new();
        doc.LoadHtml(editableMarkUp);
        doc.OptionOutputOriginalCase = true;
        doc.OptionWriteEmptyNodes = true;

        HtmlNode? imageNode = doc.DocumentNode.SelectSingleNode("img");

        if (imageNode != null)
        {
            foreach (TagHelperAttribute? attribute in output.Attributes)
            {
                if (attribute.Value != null)
                {
                    if (imageNode.Attributes[attribute.Name] == null)
                    {
                        imageNode.Attributes.Add(attribute.Name, attribute.Value?.ToString());
                    }
                    else
                    {
                        imageNode.SetAttributeValue(attribute.Name, attribute.Value?.ToString());
                    }
                }
            }

            imageNode.SetAttributeValue(ScrAttribute, imageField.GetMediaLink(ImageParams));
        }

        return new HtmlString(doc.DocumentNode.OuterHtml);
    }
}