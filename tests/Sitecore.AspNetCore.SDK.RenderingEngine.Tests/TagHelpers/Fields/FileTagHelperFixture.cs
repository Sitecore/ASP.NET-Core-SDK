using System.Globalization;
using AutoFixture;
using AutoFixture.Idioms;
using FluentAssertions;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Sitecore.AspNetCore.SDK.AutoFixture.Attributes;
using Sitecore.AspNetCore.SDK.AutoFixture.Extensions;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Response.Model.Fields;
using Sitecore.AspNetCore.SDK.RenderingEngine.TagHelpers.Fields;
using Xunit;

namespace Sitecore.AspNetCore.SDK.RenderingEngine.Tests.TagHelpers.Fields;

public class FileTagHelperFixture
{
    private const string FileLinkTag = "a";

    private const string HrefAttribute = "href";

    private const string TypeAttribute = "type";

    private const string TitleAttribute = "title";

    private const string Target = "target";

    // ReSharper disable once UnusedMember.Global - Used by testing framework
    public static Action<IFixture> AutoSetup => f =>
    {
        TagHelperContext tagHelperContext = new([], new Dictionary<object, object>(), Guid.NewGuid().ToString("N", CultureInfo.InvariantCulture));

        TagHelperOutput tagHelperOutput = new("a", [], (_, _) =>
        {
            DefaultTagHelperContent tagHelperContent = new();
            return Task.FromResult<TagHelperContent>(tagHelperContent);
        });

        f.Register(() => new FileTagHelper());

        f.Inject(tagHelperContext);
        f.Inject(tagHelperOutput);
    };

    [Theory]
    [AutoNSubstituteData]
    public void Ctor_InvalidArgs_Throws(GuardClauseAssertion guard)
    {
        guard.VerifyConstructors<RichTextTagHelper>();
    }

    [Theory]
    [AutoNSubstituteData]
    public void Process_IsGuarded(
        FileTagHelper sut,
        TagHelperContext tagHelperContext,
        TagHelperOutput tagHelperOutput)
    {
        Action allNull =
            () => sut.Process(null!, null!);
        Action outputNull =
            () => sut.Process(tagHelperContext, null!);
        Action contextNull =
            () => sut.Process(null!, tagHelperOutput);

        allNull.Should().Throw<ArgumentNullException>();
        outputNull.Should().Throw<ArgumentNullException>();
        contextNull.Should().Throw<ArgumentNullException>();
    }

    [Theory]
    [AutoNSubstituteData]
    public async Task Process_ScFileTagWithNullForAttribute_GeneratesEmptyOutput(
        FileTagHelper sut,
        TagHelperContext tagHelperContext,
        TagHelperOutput tagHelperOutput)
    {
        // Arrange
        tagHelperOutput.TagName = RenderingEngineConstants.SitecoreTagHelpers.FileHtmlTag;
        sut.For = null!;

        // Act
        await sut.ProcessAsync(tagHelperContext, tagHelperOutput);

        // Assert
        tagHelperOutput.Content.GetContent().Should().Be(string.Empty);
    }

    [Theory]
    [AutoNSubstituteData]
    public async Task Process_ScFileTagWithNullDateModelAttribute_GeneratesEmptyOutput(
        FileTagHelper sut,
        TagHelperContext tagHelperContext,
        TagHelperOutput tagHelperOutput)
    {
        // Arrange
        tagHelperOutput.TagName = RenderingEngineConstants.SitecoreTagHelpers.FileHtmlTag;
        sut.For = null!;
        sut.FileModel = null;

        // Act
        await sut.ProcessAsync(tagHelperContext, tagHelperOutput);

        // Assert
        tagHelperOutput.Content.GetContent().Should().Be(string.Empty);
    }

    [Theory]
    [InlineAutoNSubstituteData("a")]
    [InlineAutoNSubstituteData(RenderingEngineConstants.SitecoreTagHelpers.FileHtmlTag)]
    public void Process_ScFileTagWithFileModel_AddsTypeAndTitleAttributeFromModel(string tagName, string target, FileField fileField, FileTagHelper sut, TagHelperContext tagHelperContext, TagHelperOutput tagHelperOutput)
    {
        // Arrange
        tagHelperOutput.TagName = tagName;
        sut.FileModel = fileField;
        tagHelperOutput.Attributes.Add(Target, target);

        // Act
        sut.Process(tagHelperContext, tagHelperOutput);

        // Assert
        tagHelperOutput.TagName.Should().Be(FileLinkTag);
        tagHelperOutput.Attributes.Should().HaveCount(4);
        tagHelperOutput.Attributes.TryGetAttribute(TypeAttribute, out TagHelperAttribute? typeAttribute).Should().BeTrue();
        typeAttribute.Value.Should().Be(fileField.Value.MimeType);
        tagHelperOutput.Attributes.TryGetAttribute(TitleAttribute, out TagHelperAttribute? descriptionAttribute).Should().BeTrue();
        descriptionAttribute.Value.Should().Be(fileField.Value.Description);
        tagHelperOutput.Attributes.TryGetAttribute(HrefAttribute, out TagHelperAttribute? hrefAttribute).Should().BeTrue();
        hrefAttribute.Value.Should().Be(fileField.Value.Src);
        tagHelperOutput.Attributes.TryGetAttribute(Target, out TagHelperAttribute? targetAttribute).Should().BeTrue();
        targetAttribute.Value.Should().Be(target);
        tagHelperOutput.Content.GetContent().Should().Be(fileField.Value.Title);
    }

    [Theory]
    [InlineAutoNSubstituteData("a")]
    [InlineAutoNSubstituteData(RenderingEngineConstants.SitecoreTagHelpers.FileHtmlTag)]
    public void Process_ScFileTagWithFileModel_AddsTypeAndTitleAttributeFromTagAttributes(string tagName, string target, string titleFromTag, string customAttributeValue, FileField fileField, FileTagHelper sut, TagHelperContext tagHelperContext)
    {
        // Arrange
        sut.FileModel = fileField;
        TagHelperOutput tagHelperOutput =
            new(
                tagName,
                [new TagHelperAttribute(Target, target), new TagHelperAttribute("custom-attribute", customAttributeValue)],
                (_, _) => Task.FromResult(new DefaultTagHelperContent().AppendHtml(titleFromTag)));

        // Act
        sut.Process(tagHelperContext, tagHelperOutput);

        // Assert
        tagHelperOutput.Attributes.TryGetAttribute(Target, out TagHelperAttribute? targetAttribute).Should().BeTrue();
        targetAttribute.Value.Should().Be(target);
        tagHelperOutput.Attributes.TryGetAttribute("custom-attribute", out TagHelperAttribute? customAttribute).Should().BeTrue();
        customAttribute.Value.Should().Be(customAttributeValue);
        tagHelperOutput.Content.GetContent().Should().Be(titleFromTag);
    }

    [Theory]
    [InlineAutoNSubstituteData("a")]
    [InlineAutoNSubstituteData(RenderingEngineConstants.SitecoreTagHelpers.FileHtmlTag)]
    public void Process_ScFileTagWithFileModel_TakesAttributesFromTag(string tagName, string hrefTagValue, string titleTagValue, string typeTagValue, FileField fileField, FileTagHelper sut, TagHelperContext tagHelperContext, TagHelperOutput tagHelperOutput)
    {
        // Arrange
        tagHelperOutput.TagName = tagName;
        sut.FileModel = fileField;
        tagHelperOutput.Attributes.Add(HrefAttribute, hrefTagValue);
        tagHelperOutput.Attributes.Add(TitleAttribute, titleTagValue);
        tagHelperOutput.Attributes.Add(TypeAttribute, typeTagValue);

        // Act
        sut.Process(tagHelperContext, tagHelperOutput);

        // Assert
        tagHelperOutput.TagName.Should().Be(FileLinkTag);
        tagHelperOutput.Attributes.TryGetAttribute(TypeAttribute, out TagHelperAttribute? typeAttribute).Should().BeTrue();
        typeAttribute.Value.Should().Be(typeTagValue);
        typeAttribute.Value.Should().NotBe(fileField.Value.MimeType);
        tagHelperOutput.Attributes.TryGetAttribute(TitleAttribute, out TagHelperAttribute? descriptionAttribute).Should().BeTrue();
        descriptionAttribute.Value.Should().Be(titleTagValue);
        typeAttribute.Value.Should().NotBe(fileField.Value.Description);
        tagHelperOutput.Attributes.TryGetAttribute(HrefAttribute, out TagHelperAttribute? hrefAttribute).Should().BeTrue();
        hrefAttribute.Value.Should().Be(hrefTagValue);
        typeAttribute.Value.Should().NotBe(fileField.Value.Src);
    }
}