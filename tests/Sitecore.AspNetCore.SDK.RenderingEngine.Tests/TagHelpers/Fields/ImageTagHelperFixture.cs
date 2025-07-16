using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using AutoFixture;
using AutoFixture.Idioms;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using NSubstitute;
using Sitecore.AspNetCore.SDK.AutoFixture.Attributes;
using Sitecore.AspNetCore.SDK.AutoFixture.Extensions;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Response.Model;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Response.Model.Fields;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Response.Model.Properties;
using Sitecore.AspNetCore.SDK.RenderingEngine.Rendering;
using Sitecore.AspNetCore.SDK.RenderingEngine.TagHelpers.Fields;
using Xunit;

// ReSharper disable StringLiteralTypo
namespace Sitecore.AspNetCore.SDK.RenderingEngine.Tests.TagHelpers.Fields;

public class ImageTagHelperFixture
{
    private const string Html = "<img alt=\"Sitecore Logo\" border=\"1\" class=\"test\" height=\"100\" hspace=\"10\" src=\"http://styleguide/-/jssmedia/styleguide/data/media/img/sc_logo.png?iar=0&amp;hash=F313AD90AE547CAB09277E42509E289B\" title=\"title\" vspace=\"10\" width=\"100\" />";
    private const string EditableHtml = "<input id='fld_2121847759D950C4B7F4FBCD69760250_2AE326C130E557708C99E099EE76D4D3_en_1_aeede4c4d2924ff0bfafee1419803eb4_42' class='scFieldValue' name='fld_2121847759D950C4B7F4FBCD69760250_2AE326C130E557708C99E099EE76D4D3_en_1_aeede4c4d2924ff0bfafee1419803eb4_42' type='hidden' value=\"&lt;image alt=&quot;Sitecore Logo&quot; mediaid=&quot;{47408259-ECC4-553D-9585-73E504EEEDCE}&quot; /&gt;\" /><code id=\"fld_2121847759D950C4B7F4FBCD69760250_2AE326C130E557708C99E099EE76D4D3_en_1_aeede4c4d2924ff0bfafee1419803eb4_42_edit\" type=\"text/sitecore\" chromeType=\"field\" scFieldType=\"image\" class=\"scpm\" kind=\"open\">{\"commands\":[{\"click\":\"chrome:field:editcontrol({command:\\\"webedit:chooseimage\\\"})\",\"header\":\"Choose Image\",\"icon\":\"/sitecore/shell/themes/standard/custom/16x16/photo_landscape2.png\",\"disabledIcon\":\"/temp/photo_landscape2_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"Choose an image.\",\"type\":\"\"},{\"click\":\"chrome:field:editcontrol({command:\\\"webedit:editimage\\\"})\",\"header\":\"Properties\",\"icon\":\"/sitecore/shell/themes/standard/custom/16x16/photo_landscape2_edit.png\",\"disabledIcon\":\"/temp/photo_landscape2_edit_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"Modify image appearance.\",\"type\":\"\"},{\"click\":\"chrome:field:editcontrol({command:\\\"webedit:clearimage\\\"})\",\"header\":\"Clear\",\"icon\":\"/sitecore/shell/themes/standard/custom/16x16/photo_landscape2_delete.png\",\"disabledIcon\":\"/temp/photo_landscape2_delete_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"Remove the image.\",\"type\":\"\"},{\"click\":\"chrome:common:edititem({command:\\\"webedit:open\\\"})\",\"header\":\"Edit the related item\",\"icon\":\"/temp/iconcache/office/16x16/cubes.png\",\"disabledIcon\":\"/temp/cubes_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"Edit the related item in the Content Editor.\",\"type\":\"common\"},{\"click\":\"chrome:rendering:personalize({command:\\\"webedit:personalize\\\"})\",\"header\":\"Personalize\",\"icon\":\"/temp/iconcache/office/16x16/users_family.png\",\"disabledIcon\":\"/temp/users_family_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"Create or edit personalization for this component.\",\"type\":\"sticky\"},{\"click\":\"chrome:rendering:editvariations({command:\\\"webedit:editvariations\\\"})\",\"header\":\"Edit variations\",\"icon\":\"/temp/iconcache/office/16x16/windows.png\",\"disabledIcon\":\"/temp/windows_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"Edit the variations.\",\"type\":\"sticky\"}],\"contextItemUri\":\"sitecore://master/{21218477-59D9-50C4-B7F4-FBCD69760250}?lang=en&ver=1\",\"custom\":{},\"displayName\":\"sample1\",\"expandedDisplayName\":null}</code><img src=\"/sitecore/shell/-/jssmedia/styleguide/data/media/img/sc_logo.png?iar=0\" alt=\"Sitecore Logo\" /><code class=\"scpm\" type=\"text/sitecore\" chromeType=\"field\" kind=\"close\"></code>";
    private const string EditableHtmlWithCustomAttributes = "<input id='fld_2121847759D950C4B7F4FBCD69760250_2AE326C130E557708C99E099EE76D4D3_en_1_aeede4c4d2924ff0bfafee1419803eb4_42' class='scFieldValue' name='fld_2121847759D950C4B7F4FBCD69760250_2AE326C130E557708C99E099EE76D4D3_en_1_aeede4c4d2924ff0bfafee1419803eb4_42' type='hidden' value=\"&lt;image alt=&quot;Sitecore Logo&quot; mediaid=&quot;{47408259-ECC4-553D-9585-73E504EEEDCE}&quot; /&gt;\" /><code id=\"fld_2121847759D950C4B7F4FBCD69760250_2AE326C130E557708C99E099EE76D4D3_en_1_aeede4c4d2924ff0bfafee1419803eb4_42_edit\" type=\"text/sitecore\" chromeType=\"field\" scFieldType=\"image\" class=\"scpm\" kind=\"open\">{\"commands\":[{\"click\":\"chrome:field:editcontrol({command:\\\"webedit:chooseimage\\\"})\",\"header\":\"Choose Image\",\"icon\":\"/sitecore/shell/themes/standard/custom/16x16/photo_landscape2.png\",\"disabledIcon\":\"/temp/photo_landscape2_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"Choose an image.\",\"type\":\"\"},{\"click\":\"chrome:field:editcontrol({command:\\\"webedit:editimage\\\"})\",\"header\":\"Properties\",\"icon\":\"/sitecore/shell/themes/standard/custom/16x16/photo_landscape2_edit.png\",\"disabledIcon\":\"/temp/photo_landscape2_edit_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"Modify image appearance.\",\"type\":\"\"},{\"click\":\"chrome:field:editcontrol({command:\\\"webedit:clearimage\\\"})\",\"header\":\"Clear\",\"icon\":\"/sitecore/shell/themes/standard/custom/16x16/photo_landscape2_delete.png\",\"disabledIcon\":\"/temp/photo_landscape2_delete_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"Remove the image.\",\"type\":\"\"},{\"click\":\"chrome:common:edititem({command:\\\"webedit:open\\\"})\",\"header\":\"Edit the related item\",\"icon\":\"/temp/iconcache/office/16x16/cubes.png\",\"disabledIcon\":\"/temp/cubes_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"Edit the related item in the Content Editor.\",\"type\":\"common\"},{\"click\":\"chrome:rendering:personalize({command:\\\"webedit:personalize\\\"})\",\"header\":\"Personalize\",\"icon\":\"/temp/iconcache/office/16x16/users_family.png\",\"disabledIcon\":\"/temp/users_family_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"Create or edit personalization for this component.\",\"type\":\"sticky\"},{\"click\":\"chrome:rendering:editvariations({command:\\\"webedit:editvariations\\\"})\",\"header\":\"Edit variations\",\"icon\":\"/temp/iconcache/office/16x16/windows.png\",\"disabledIcon\":\"/temp/windows_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"Edit the variations.\",\"type\":\"sticky\"}],\"contextItemUri\":\"sitecore://master/{21218477-59D9-50C4-B7F4-FBCD69760250}?lang=en&ver=1\",\"custom\":{},\"displayName\":\"sample1\",\"expandedDisplayName\":null}</code><img src=\"/sitecore/shell/-/jssmedia/styleguide/data/media/img/sc_logo.png?mw=100&mh=50\" alt=\"testAlt\" class=\"testClass\" width=\"50\" height=\"50\" /><code class=\"scpm\" type=\"text/sitecore\" chromeType=\"field\" kind=\"close\"></code>";
    private readonly Image _image = new() { Src = "http://styleguide/-/media/styleguide/data/media/img/sc_logo.png?iar=0&hash=F313AD90AE547CAB09277E42509E289B", Alt = "Sitecore Logo", Width = 100, Height = 100, VSpace = 10, HSpace = 10, Border = 1, Class = "test", Title = "title" };

    // ReSharper disable once UnusedMember.Global - Used by testing framework
    [ExcludeFromCodeCoverage]
    public static Action<IFixture> AutoSetup => f =>
    {
        TagHelperContext tagHelperContext = new([], new Dictionary<object, object>(), Guid.NewGuid().ToString("N", CultureInfo.InvariantCulture));

        TagHelperOutput tagHelperOutput = new("a", [], (_, _) =>
        {
            DefaultTagHelperContent tagHelperContent = new();
            return Task.FromResult<TagHelperContent>(tagHelperContent);
        });

        f.Register(() => new ImageTagHelper(new EditableChromeRenderer()));

        f.Inject(tagHelperContext);
        f.Inject(tagHelperOutput);
    };

    [Theory]
    [AutoNSubstituteData]
    public void Ctor_InvalidArgs_Throws(GuardClauseAssertion guard)
    {
        guard.VerifyConstructors<ImageTagHelper>();
    }

    [Theory]
    [AutoNSubstituteData]
    public void Process_IsGuarded(
        ImageTagHelper sut,
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
    public async Task Process_ScImgTagWithNullForAttribute_GeneratesEmptyOutput(
        ImageTagHelper sut,
        TagHelperContext tagHelperContext,
        TagHelperOutput tagHelperOutput)
    {
        // Arrange
        tagHelperOutput.TagName = RenderingEngineConstants.SitecoreTagHelpers.ImageHtmlTag;
        sut.For = null!;

        // Act
        await sut.ProcessAsync(tagHelperContext, tagHelperOutput);

        // Assert
        tagHelperOutput.Content.GetContent().Should().Be(string.Empty);
    }

    [Theory]
    [AutoNSubstituteData]
    public void Process_ScImgTagWithValidForAttribute_GeneratesCorrectOutput(
        ImageTagHelper sut,
        TagHelperContext tagHelperContext,
        TagHelperOutput tagHelperOutput)
    {
        // Arrange
        tagHelperOutput.TagName = RenderingEngineConstants.SitecoreTagHelpers.ImageHtmlTag;
        sut.For = GetModelExpression(new ImageField(_image));

        // Act
        sut.Process(tagHelperContext, tagHelperOutput);

        // Assert
        tagHelperOutput.Content.GetContent().Should().Be(Html);
    }

    [Theory]
    [AutoNSubstituteData]
    public void Process_ScImgTagWithValidForAttribute_GeneratesOutputWithoutOuterScTextTag(
        ImageTagHelper sut,
        TagHelperContext tagHelperContext,
        TagHelperOutput tagHelperOutput)
    {
        // Arrange
        tagHelperOutput.TagName = RenderingEngineConstants.SitecoreTagHelpers.ImageHtmlTag;
        sut.For = GetModelExpression(new ImageField(_image));

        // Act
        sut.Process(tagHelperContext, tagHelperOutput);

        // Assert
        tagHelperOutput.TagName.Should().BeNull();
    }

    [Theory]
    [AutoNSubstituteData]
    public void Process_ScImgTagWithInvalidForAttribute_GeneratesEmptyOutput(
        ImageTagHelper sut,
        TagHelperContext tagHelperContext,
        TagHelperOutput tagHelperOutput)
    {
        // Arrange
        tagHelperOutput.TagName = RenderingEngineConstants.SitecoreTagHelpers.ImageHtmlTag;
        sut.For = GetModelExpression(new RichTextField());

        // Act
        sut.Process(tagHelperContext, tagHelperOutput);

        // Assert
        tagHelperOutput.Content.GetContent().Should().Be(string.Empty);
        tagHelperOutput.Content.GetContent().Should().NotContain(RenderingEngineConstants.SitecoreTagHelpers.ImageHtmlTag);
    }

    [Theory]
    [AutoNSubstituteData]
    public void Process_ScImgTagWithEmptyValueInForAttribute_GeneratesEmptyOutput(
        ImageTagHelper sut,
        TagHelperContext tagHelperContext,
        TagHelperOutput tagHelperOutput)
    {
        // Arrange
        tagHelperOutput.TagName = RenderingEngineConstants.SitecoreTagHelpers.ImageHtmlTag;
        sut.For = GetModelExpression(new ImageField(new Image()));

        // Act
        sut.Process(tagHelperContext, tagHelperOutput);

        // Assert
        tagHelperOutput.Content.GetContent().Should().Be(string.Empty);
    }

    [Theory]
    [AutoNSubstituteData]
    public void Process_ScImgTagWithEditableFieldAndEditableSetToTrue_GeneratesCorrectOutput(
        ImageTagHelper sut,
        TagHelperContext tagHelperContext,
        TagHelperOutput tagHelperOutput)
    {
        // Arrange
        tagHelperOutput.TagName = RenderingEngineConstants.SitecoreTagHelpers.ImageHtmlTag;
        ImageField testField = new(new Image { Alt = "Sitecore Logo", Src = "/sitecore/shell/-/media/styleguide/data/media/img/sc_logo.png?iar=0" })
        {
            EditableMarkup = EditableHtml
        };
        sut.For = GetModelExpression(testField);

        // Act
        sut.Process(tagHelperContext, tagHelperOutput);

        // Assert
        tagHelperOutput.Content.GetContent().Should().Be(EditableHtml);
    }

    [Theory]
    [AutoNSubstituteData]
    public void Process_ScImgTagWithEmptyEditableFieldAndEditableSetToTrue_GeneratesCorrectOutput(
        ImageTagHelper sut,
        TagHelperContext tagHelperContext,
        TagHelperOutput tagHelperOutput)
    {
        // Arrange
        tagHelperOutput.TagName = RenderingEngineConstants.SitecoreTagHelpers.ImageHtmlTag;
        ImageField testField = new(_image)
        {
            EditableMarkup = string.Empty
        };
        sut.For = GetModelExpression(testField);

        // Act
        sut.Process(tagHelperContext, tagHelperOutput);

        // Assert
        tagHelperOutput.Content.GetContent().Should().Be(Html);
    }

    [Theory]
    [AutoNSubstituteData]
    public void Process_ScImgTagWithEditableFieldAndEditableSetToFalse_GeneratesCorrectOutput(
        ImageTagHelper sut,
        TagHelperContext tagHelperContext,
        TagHelperOutput tagHelperOutput)
    {
        // Arrange
        tagHelperOutput.TagName = RenderingEngineConstants.SitecoreTagHelpers.ImageHtmlTag;
        ImageField testField = new(_image)
        {
            EditableMarkup = EditableHtml
        };
        sut.For = GetModelExpression(testField);
        sut.Editable = false;

        // Act
        sut.Process(tagHelperContext, tagHelperOutput);

        // Assert
        tagHelperOutput.Content.GetContent().Should().Be(Html);
    }

    [Theory]
    [AutoNSubstituteData]
    public void Process_ScImgTagWithParamsAttribute_GeneratesProperImageUrl(
        ImageTagHelper sut,
        TagHelperContext tagHelperContext,
        TagHelperOutput tagHelperOutput)
    {
        // Arrange
        tagHelperOutput.TagName = RenderingEngineConstants.SitecoreTagHelpers.ImageHtmlTag;
        sut.For = GetModelExpression(new ImageField(_image));
        sut.ImageParams = new { mw = 100, mh = 50 };

        // Act
        sut.Process(tagHelperContext, tagHelperOutput);

        // Assert
        tagHelperOutput.Content.GetContent().Should().Contain("?mw=100&amp;mh=50");
        tagHelperOutput.Content.GetContent().Should().NotContain("iar=0");
        tagHelperOutput.Content.GetContent().Should().NotContain("hash=F313AD90AE547CAB09277E42509E289B");
    }

    [Theory]
    [AutoNSubstituteData]
    public async Task Process_ImgTagWithNullForAttribute_GeneratesEmptyOutput(
        ImageTagHelper sut,
        TagHelperContext tagHelperContext,
        TagHelperOutput tagHelperOutput)
    {
        // Arrange
        tagHelperOutput.TagName = "img";
        sut.For = null!;

        // Act
        await sut.ProcessAsync(tagHelperContext, tagHelperOutput);

        // Assert
        tagHelperOutput.Content.GetContent().Should().Be(string.Empty);
    }

    [Theory]
    [AutoNSubstituteData]
    public void Process_ImgTagWithValidForAttribute_GeneratesOutputWithOuterDivTag(
        ImageTagHelper sut,
        TagHelperContext tagHelperContext,
        TagHelperOutput tagHelperOutput)
    {
        // Arrange
        tagHelperOutput.TagName = "img";
        sut.For = GetModelExpression(new ImageField(_image));

        // Act
        sut.Process(tagHelperContext, tagHelperOutput);

        // Assert
        tagHelperOutput.TagName.Should().Be("img");
    }

    [Theory]
    [AutoNSubstituteData]
    public void Process_ImgTagWithValidForAttribute_AddsSrcAndAltAttributes(
        ImageTagHelper sut,
        TagHelperContext tagHelperContext,
        TagHelperOutput tagHelperOutput)
    {
        // Arrange
        tagHelperOutput.TagName = "img";
        sut.For = GetModelExpression(new ImageField(_image));

        // Act
        sut.Process(tagHelperContext, tagHelperOutput);

        // Assert
        tagHelperOutput.Attributes.Should().Contain(a => a.Name == "src");
        tagHelperOutput.Attributes.Should().Contain(a => a.Name == "alt");
    }

    [Theory]
    [AutoNSubstituteData]
    public void Process_ImgTagWithEditableFieldAndEditableSetToTrue_GeneratesCorrectOutput(
        ImageTagHelper sut,
        TagHelperContext tagHelperContext,
        TagHelperOutput tagHelperOutput)
    {
        // Arrange
        tagHelperOutput.TagName = "img";
        ImageField testField = new(new Image { Alt = "Sitecore Logo", Src = "/sitecore/shell/-/media/styleguide/data/media/img/sc_logo.png?iar=0" })
        {
            EditableMarkup = EditableHtml
        };
        sut.For = GetModelExpression(testField);

        // Act
        sut.Process(tagHelperContext, tagHelperOutput);

        // Assert
        tagHelperOutput.Content.GetContent().Should().Be(EditableHtml);
    }

    [Theory]
    [AutoNSubstituteData]
    public void Process_ImgTagWithParamsAttribute_GeneratesProperImageUrl(
        ImageTagHelper sut,
        TagHelperContext tagHelperContext,
        TagHelperOutput tagHelperOutput)
    {
        // Arrange
        tagHelperOutput.TagName = "img";
        sut.For = GetModelExpression(new ImageField(_image));
        sut.ImageParams = new { mw = 100, mh = 50 };

        // Act
        sut.Process(tagHelperContext, tagHelperOutput);
        string? url = tagHelperOutput.Attributes["src"].Value.ToString();

        // Assert
        url.Should().Contain("?mw=100&mh=50");
    }

    [Theory]
    [AutoNSubstituteData]
    public void Process_ScImgTagWithEditableFieldAndEditableSetToTrue_GeneratesCorrectOutputWithCustomAttributes(
        ImageTagHelper sut,
        TagHelperContext tagHelperContext,
        TagHelperOutput tagHelperOutput)
    {
        // Arrange
        tagHelperOutput.TagName = RenderingEngineConstants.SitecoreTagHelpers.ImageHtmlTag;
        ImageField testField = new(new Image { Alt = "Sitecore Logo", Src = "/sitecore/shell/-/jssmedia/styleguide/data/media/img/sc_logo.png?iar=0" })
        {
            EditableMarkup = EditableHtml
        };
        sut.ImageParams = new { mw = 100, mh = 50 };
        sut.For = GetModelExpression(testField);

        tagHelperOutput.Attributes.Add("class", "testClass");
        tagHelperOutput.Attributes.Add("alt", "testAlt");
        tagHelperOutput.Attributes.Add("width", "50");
        tagHelperOutput.Attributes.Add("height", "50");

        // Act
        sut.Process(tagHelperContext, tagHelperOutput);

        // Assert
        tagHelperOutput.Content.GetContent().Should().Be(EditableHtmlWithCustomAttributes);
    }

    [Theory]
    [AutoNSubstituteData]
    public void Process_ImgTagWithAllAttributes_AddsAllAttributes(
        ImageTagHelper sut,
        TagHelperContext tagHelperContext,
        TagHelperOutput tagHelperOutput)
    {
        // Arrange
        tagHelperOutput.TagName = "img";
        sut.For = GetModelExpression(new ImageField(_image));

        // Act
        sut.Process(tagHelperContext, tagHelperOutput);

        // Assert
        tagHelperOutput.Attributes.Should().Contain(a => a.Name == "src");
        tagHelperOutput.Attributes.Should().Contain(a => a.Name == "alt");
        tagHelperOutput.Attributes.Should().Contain(a => a.Name == "width");
        tagHelperOutput.Attributes.Should().Contain(a => a.Name == "height");
        tagHelperOutput.Attributes.Should().Contain(a => a.Name == "vspace");
        tagHelperOutput.Attributes.Should().Contain(a => a.Name == "hspace");
        tagHelperOutput.Attributes.Should().Contain(a => a.Name == "border");
        tagHelperOutput.Attributes.Should().Contain(a => a.Name == "title");
    }

    #region asp-image attribute
    [Theory]
    [AutoNSubstituteData]
    public async Task Process_ScImgTagWithNullAspImageAttribute_GeneratesEmptyOutput(
        ImageTagHelper sut,
        TagHelperContext tagHelperContext,
        TagHelperOutput tagHelperOutput)
    {
        // Arrange
        tagHelperOutput.TagName = RenderingEngineConstants.SitecoreTagHelpers.ImageHtmlTag;
        sut.ImageModel = null;

        // Act
        await sut.ProcessAsync(tagHelperContext, tagHelperOutput);

        // Assert
        tagHelperOutput.Content.GetContent().Should().Be(string.Empty);
    }

    [Theory]
    [AutoNSubstituteData]
    public void Process_ScImgTagWithValidAspImageAttribute_GeneratesCorrectOutput(
        ImageTagHelper sut,
        TagHelperContext tagHelperContext,
        TagHelperOutput tagHelperOutput)
    {
        // Arrange
        tagHelperOutput.TagName = RenderingEngineConstants.SitecoreTagHelpers.ImageHtmlTag;
        sut.ImageModel = new ImageField(_image);

        // Act
        sut.Process(tagHelperContext, tagHelperOutput);

        // Assert
        tagHelperOutput.Content.GetContent().Should().Be(Html);
    }

    [Theory]
    [AutoNSubstituteData]
    public void Process_ScImgTagWithValidAspImageAttribute_GeneratesOutputWithoutOuterScTextTag(
        ImageTagHelper sut,
        TagHelperContext tagHelperContext,
        TagHelperOutput tagHelperOutput)
    {
        // Arrange
        tagHelperOutput.TagName = RenderingEngineConstants.SitecoreTagHelpers.ImageHtmlTag;
        sut.ImageModel = new ImageField(_image);

        // Act
        sut.Process(tagHelperContext, tagHelperOutput);

        // Assert
        tagHelperOutput.TagName.Should().BeNull();
    }

    [Theory]
    [AutoNSubstituteData]
    public void Process_ScImgTagWithEmptyValueInAspImageAttribute_GeneratesEmptyOutput(
        ImageTagHelper sut,
        TagHelperContext tagHelperContext,
        TagHelperOutput tagHelperOutput)
    {
        // Arrange
        tagHelperOutput.TagName = RenderingEngineConstants.SitecoreTagHelpers.ImageHtmlTag;
        sut.ImageModel = new ImageField(new Image());

        // Act
        sut.Process(tagHelperContext, tagHelperOutput);

        // Assert
        tagHelperOutput.Content.GetContent().Should().Be(string.Empty);
    }

    [Theory]
    [AutoNSubstituteData]
    public void Process_ScImgTagWithAspImageAttributeWithEditableFieldAndEditableSetToTrue_GeneratesCorrectOutput(
        ImageTagHelper sut,
        TagHelperContext tagHelperContext,
        TagHelperOutput tagHelperOutput)
    {
        // Arrange
        tagHelperOutput.TagName = RenderingEngineConstants.SitecoreTagHelpers.ImageHtmlTag;
        ImageField testField = new(new Image { Alt = "Sitecore Logo", Src = "/sitecore/shell/-/media/styleguide/data/media/img/sc_logo.png?iar=0" })
        {
            EditableMarkup = EditableHtml
        };
        sut.ImageModel = testField;

        // Act
        sut.Process(tagHelperContext, tagHelperOutput);

        // Assert
        tagHelperOutput.Content.GetContent().Should().Be(EditableHtml);
    }

    [Theory]
    [AutoNSubstituteData]
    public void Process_ScImgTagWithAspImageAttributeEmptyEditableFieldAndEditableSetToTrue_GeneratesCorrectOutput(
        ImageTagHelper sut,
        TagHelperContext tagHelperContext,
        TagHelperOutput tagHelperOutput)
    {
        // Arrange
        tagHelperOutput.TagName = RenderingEngineConstants.SitecoreTagHelpers.ImageHtmlTag;
        ImageField testField = new(_image)
        {
            EditableMarkup = string.Empty
        };
        sut.ImageModel = testField;

        // Act
        sut.Process(tagHelperContext, tagHelperOutput);

        // Assert
        tagHelperOutput.Content.GetContent().Should().Be(Html);
    }

    [Theory]
    [AutoNSubstituteData]
    public void Process_ScImgTagAspImageAttributeWithEditableFieldAndEditableSetToFalse_GeneratesCorrectOutput(
        ImageTagHelper sut,
        TagHelperContext tagHelperContext,
        TagHelperOutput tagHelperOutput)
    {
        // Arrange
        tagHelperOutput.TagName = RenderingEngineConstants.SitecoreTagHelpers.ImageHtmlTag;
        ImageField testField = new(_image)
        {
            EditableMarkup = EditableHtml
        };
        sut.ImageModel = testField;
        sut.Editable = false;

        // Act
        sut.Process(tagHelperContext, tagHelperOutput);

        // Assert
        tagHelperOutput.Content.GetContent().Should().Be(Html);
    }

    [Theory]
    [AutoNSubstituteData]
    public void Process_ScImgTagAspImageAttributeWithParamsAttribute_GeneratesProperImageUrl(
        ImageTagHelper sut,
        TagHelperContext tagHelperContext,
        TagHelperOutput tagHelperOutput)
    {
        // Arrange
        tagHelperOutput.TagName = RenderingEngineConstants.SitecoreTagHelpers.ImageHtmlTag;
        sut.ImageModel = new ImageField(_image);
        sut.ImageParams = new { mw = 100, mh = 50 };

        // Act
        sut.Process(tagHelperContext, tagHelperOutput);

        // Assert
        tagHelperOutput.Content.GetContent().Should().Contain("mw=100&amp;mh=50");
        tagHelperOutput.Content.GetContent().Should().NotContain("iar=0");
        tagHelperOutput.Content.GetContent().Should().NotContain("hash=F313AD90AE547CAB09277E42509E289B");
    }

    [Theory]
    [AutoNSubstituteData]
    public async Task Process_ImgTagAspImageAttributeWithNullForAttribute_GeneratesEmptyOutput(
        ImageTagHelper sut,
        TagHelperContext tagHelperContext,
        TagHelperOutput tagHelperOutput)
    {
        // Arrange
        tagHelperOutput.TagName = "img";
        sut.ImageModel = null;

        // Act
        await sut.ProcessAsync(tagHelperContext, tagHelperOutput);

        // Assert
        tagHelperOutput.Content.GetContent().Should().Be(string.Empty);
    }

    [Theory]
    [AutoNSubstituteData]
    public void Process_ImgTagAspImageAttributeWithValidForAttribute_GeneratesOutputWithOuterDivTag(
        ImageTagHelper sut,
        TagHelperContext tagHelperContext,
        TagHelperOutput tagHelperOutput)
    {
        // Arrange
        tagHelperOutput.TagName = "img";
        sut.ImageModel = new ImageField(_image);

        // Act
        sut.Process(tagHelperContext, tagHelperOutput);

        // Assert
        tagHelperOutput.TagName.Should().Be("img");
    }

    [Theory]
    [AutoNSubstituteData]
    public void Process_ImgTagAspImageAttributeWithValidForAttribute_AddsSrcAndAltAttributes(
        ImageTagHelper sut,
        TagHelperContext tagHelperContext,
        TagHelperOutput tagHelperOutput)
    {
        // Arrange
        tagHelperOutput.TagName = "img";
        sut.ImageModel = new ImageField(_image);

        // Act
        sut.Process(tagHelperContext, tagHelperOutput);

        // Assert
        tagHelperOutput.Attributes.Should().Contain(a => a.Name == "src");
        tagHelperOutput.Attributes.Should().Contain(a => a.Name == "alt");
    }

    [Theory]
    [AutoNSubstituteData]
    public void Process_ImgTagAspImageAttributeWithEditableFieldAndEditableSetToTrue_GeneratesCorrectOutput(
        ImageTagHelper sut,
        TagHelperContext tagHelperContext,
        TagHelperOutput tagHelperOutput)
    {
        // Arrange
        tagHelperOutput.TagName = "img";
        ImageField testField = new(new Image { Alt = "Sitecore Logo", Src = "/sitecore/shell/-/media/styleguide/data/media/img/sc_logo.png?iar=0" })
        {
            EditableMarkup = EditableHtml
        };
        sut.ImageModel = testField;

        // Act
        sut.Process(tagHelperContext, tagHelperOutput);

        // Assert
        tagHelperOutput.Content.GetContent().Should().Be(EditableHtml);
    }

    [Theory]
    [AutoNSubstituteData]
    public void Process_ImgTagAspImageAttributeWithParamsAttribute_GeneratesProperImageUrl(
        ImageTagHelper sut,
        TagHelperContext tagHelperContext,
        TagHelperOutput tagHelperOutput)
    {
        // Arrange
        tagHelperOutput.TagName = "img";
        sut.ImageModel = new ImageField(_image);
        sut.ImageParams = new { mw = 100, mh = 50 };

        // Act
        sut.Process(tagHelperContext, tagHelperOutput);
        string? url = tagHelperOutput.Attributes["src"].Value.ToString();

        // Assert
        url.Should().Contain("?mw=100&mh=50");
    }

    [Theory]
    [AutoNSubstituteData]
    public void Process_ScImgTagAspImageAttributeWithEditableFieldAndEditableSetToTrue_GeneratesCorrectOutputWithCustomAttributes(
        ImageTagHelper sut,
        TagHelperContext tagHelperContext,
        TagHelperOutput tagHelperOutput)
    {
        // Arrange
        tagHelperOutput.TagName = RenderingEngineConstants.SitecoreTagHelpers.ImageHtmlTag;
        ImageField testField = new(new Image { Alt = "Sitecore Logo", Src = "/sitecore/shell/-/jssmedia/styleguide/data/media/img/sc_logo.png?iar=0" })
        {
            EditableMarkup = EditableHtml
        };
        sut.ImageParams = new { mw = 100, mh = 50 };
        sut.ImageModel = testField;

        tagHelperOutput.Attributes.Add("class", "testClass");
        tagHelperOutput.Attributes.Add("alt", "testAlt");
        tagHelperOutput.Attributes.Add("width", "50");
        tagHelperOutput.Attributes.Add("height", "50");

        // Act
        sut.Process(tagHelperContext, tagHelperOutput);

        // Assert
        tagHelperOutput.Content.GetContent().Should().Be(EditableHtmlWithCustomAttributes);
    }

    [Theory]
    [AutoNSubstituteData]
    public void Process_ImgTagAspImageAttributeWithAllAttributes_AddsAllAttributes(
        ImageTagHelper sut,
        TagHelperContext tagHelperContext,
        TagHelperOutput tagHelperOutput)
    {
        // Arrange
        tagHelperOutput.TagName = "img";
        sut.ImageModel = new ImageField(_image);

        // Act
        sut.Process(tagHelperContext, tagHelperOutput);

        // Assert
        tagHelperOutput.Attributes.Should().Contain(a => a.Name == "src");
        tagHelperOutput.Attributes.Should().Contain(a => a.Name == "alt");
        tagHelperOutput.Attributes.Should().Contain(a => a.Name == "width");
        tagHelperOutput.Attributes.Should().Contain(a => a.Name == "height");
        tagHelperOutput.Attributes.Should().Contain(a => a.Name == "vspace");
        tagHelperOutput.Attributes.Should().Contain(a => a.Name == "hspace");
        tagHelperOutput.Attributes.Should().Contain(a => a.Name == "border");
        tagHelperOutput.Attributes.Should().Contain(a => a.Name == "title");
    }
    #endregion

    [Theory]
    [AutoNSubstituteData]
    public void Process_RenderingChromesAreNotNull_ChromesAreOutput(
       TagHelperContext tagHelperContext,
       TagHelperOutput tagHelperOutput)
    {
        // Arrange
        IEditableChromeRenderer chromeRenderer = Substitute.For<IEditableChromeRenderer>();
        ImageTagHelper sut = new(chromeRenderer);
        EditableChrome openingChrome = Substitute.For<EditableChrome>();
        EditableChrome closingChrome = Substitute.For<EditableChrome>();
        tagHelperOutput.TagName = RenderingEngineConstants.SitecoreTagHelpers.ImageHtmlTag;
        ImageField testField = new(new Image { Alt = "Sitecore Logo", Src = "/sitecore/shell/-/media/styleguide/data/media/img/sc_logo.png?iar=0" })
        {
            OpeningChrome = openingChrome,
            ClosingChrome = closingChrome
        };
        sut.ImageModel = testField;

        // Act
        sut.Process(tagHelperContext, tagHelperOutput);

        // Assert
        chromeRenderer.Received().Render(openingChrome);
        chromeRenderer.Received().Render(closingChrome);
    }

    [Theory]
    [AutoNSubstituteData]
    public void Process_ScImgTagWithSrcSet_GeneratesSrcSetAttribute(
        ImageTagHelper sut,
        TagHelperContext tagHelperContext,
        TagHelperOutput tagHelperOutput)
    {
        // Arrange
        tagHelperOutput.TagName = RenderingEngineConstants.SitecoreTagHelpers.ImageHtmlTag;
        sut.For = GetModelExpression(new ImageField(_image));
        sut.SrcSet = new object[] { new { w = 400 }, new { w = 200 } };

        // Act
        sut.Process(tagHelperContext, tagHelperOutput);

        // Assert - <sc-img> generates HTML content, not attributes
        string content = tagHelperOutput.Content.GetContent();
        content.Should().Contain("srcset=");
        content.Should().Contain("400w");
        content.Should().Contain("200w");
    }

    [Theory]
    [AutoNSubstituteData]
    public void Process_SrcSetWithContentSDKBehavior_MatchesExpectedOutput(
        ImageTagHelper sut,
        TagHelperContext tagHelperContext,
        TagHelperOutput tagHelperOutput)
    {
        // Arrange
        tagHelperOutput.TagName = "img";
        sut.For = GetModelExpression(new ImageField(_image));
        sut.ImageParams = new { h = 1000 }; // Base parameters
        sut.SrcSet = new object[] { new { h = 1000, w = 1000 }, new { mh = 250, mw = 250 } };

        // Act
        sut.Process(tagHelperContext, tagHelperOutput);

        // Assert
        tagHelperOutput.Attributes.Should().ContainSingle(a => a.Name == "srcset");
        string srcSetValue = tagHelperOutput.Attributes["srcset"].Value.ToString()!;

        // Should contain "1000w" (from w parameter taking priority over h)
        srcSetValue.Should().Contain("1000w");

        // Should contain "250w" (from mw parameter)
        srcSetValue.Should().Contain("250w");

        // Verify it contains the expected format: "url 1000w, url 250w"
        string[] entries = srcSetValue.Split(", ");
        entries.Should().HaveCount(2);
        entries[0].Should().EndWith("1000w");
        entries[1].Should().EndWith("250w");
    }

    [Theory]
    [AutoNSubstituteData]
    public void Process_SrcSetWithoutValidWidth_FiltersEntries(
        ImageTagHelper sut,
        TagHelperContext tagHelperContext,
        TagHelperOutput tagHelperOutput)
    {
        // Arrange
        tagHelperOutput.TagName = "img";
        sut.For = GetModelExpression(new ImageField(_image));
        sut.SrcSet = new object[] { new { h = 1000 }, new { mw = 250 }, new { quality = 80 } };

        // Act
        sut.Process(tagHelperContext, tagHelperOutput);

        // Assert
        tagHelperOutput.Attributes.Should().ContainSingle(a => a.Name == "srcset");
        string srcSetValue = tagHelperOutput.Attributes["srcset"].Value.ToString()!;

        // Should only contain the entry with mw parameter
        srcSetValue.Should().Contain("250w");

        // Should not contain entries without width parameters
        string[] entries = srcSetValue.Split(", ");
        entries.Should().HaveCount(1);
    }

    [Theory]
    [AutoNSubstituteData]
    public void Process_EditableImageWithSrcSet_MergesSrcSetIntoEditableMarkup(
        ImageTagHelper sut,
        TagHelperContext tagHelperContext,
        TagHelperOutput tagHelperOutput)
    {
        // Arrange
        Image image = new() { Src = "http://styleguide/-/media/img/sc_logo.png", Alt = "Sitecore Logo" };
        ImageField imageField = new(image)
        {
            EditableMarkup = "<img src=\"/sitecore/shell/-/jssmedia/img/sc_logo.png\" alt=\"Sitecore Logo\" />"
        };

        tagHelperOutput.TagName = RenderingEngineConstants.SitecoreTagHelpers.ImageHtmlTag;
        sut.For = GetModelExpression(imageField);
        sut.SrcSet = new object[] { new { mw = 600 }, new { mw = 300 } };
        sut.Sizes = "(min-width: 768px) 600px, 300px";

        // Act
        sut.Process(tagHelperContext, tagHelperOutput);

        // Assert
        string content = tagHelperOutput.Content.GetContent();
        content.Should().Contain("srcset=");
        content.Should().Contain("600w");
        content.Should().Contain("300w");
        content.Should().Contain("sizes=\"(min-width: 768px) 600px, 300px\"");
    }

    [Theory]
    [AutoNSubstituteData]
    public void Process_SrcSetWithJsonString_GeneratesSrcSetAttribute(
        ImageTagHelper sut,
        TagHelperContext tagHelperContext,
        TagHelperOutput tagHelperOutput)
    {
        // Arrange
        tagHelperOutput.TagName = "img";
        sut.For = GetModelExpression(new ImageField(_image));
        sut.SrcSet = "[{\"mw\": 500}, {\"mw\": 250}]";

        // Act
        sut.Process(tagHelperContext, tagHelperOutput);

        // Assert
        tagHelperOutput.Attributes.Should().ContainSingle(a => a.Name == "srcset");
        string srcSetValue = tagHelperOutput.Attributes["srcset"].Value.ToString()!;
        srcSetValue.Should().Contain("500w");
        srcSetValue.Should().Contain("250w");
    }

    [Theory]
    [AutoNSubstituteData]
    public void Process_SrcSetWithMixedParameterTypes_GeneratesSrcSetAttribute(
        ImageTagHelper sut,
        TagHelperContext tagHelperContext,
        TagHelperOutput tagHelperOutput)
    {
        // Arrange
        tagHelperOutput.TagName = RenderingEngineConstants.SitecoreTagHelpers.ImageHtmlTag;
        sut.For = GetModelExpression(new ImageField(_image));
        sut.SrcSet = new object[]
        {
            // Only w and mw are supported by Content SDK for srcSet
            new { w = 800 },  // Anonymous object with 'w'
            new { mw = 400 }, // Anonymous object with 'mw'
        };

        // Act
        sut.Process(tagHelperContext, tagHelperOutput);

        // Assert
        string content = tagHelperOutput.Content.GetContent();
        content.Should().Contain("srcset=");
        content.Should().Contain("800w");
        content.Should().Contain("400w");
    }

    [Theory]
    [AutoNSubstituteData]
    public void Process_SrcSetWithInvalidJsonString_FallsBackGracefully(
        ImageTagHelper sut,
        TagHelperContext tagHelperContext,
        TagHelperOutput tagHelperOutput)
    {
        // Arrange
        tagHelperOutput.TagName = "img";
        sut.For = GetModelExpression(new ImageField(_image));
        sut.SrcSet = "invalid json string";

        // Act
        sut.Process(tagHelperContext, tagHelperOutput);

        // Assert
        // Should not throw and should not add srcset attribute
        tagHelperOutput.Attributes.Should().NotContain(a => a.Name == "srcset");
    }

    [Theory]
    [AutoNSubstituteData]
    public void Process_SrcSetWithImageParamsConflict_SrcSetParametersWin(
        ImageTagHelper sut,
        TagHelperContext tagHelperContext,
        TagHelperOutput tagHelperOutput)
    {
        // Arrange
        tagHelperOutput.TagName = "img";
        sut.For = GetModelExpression(new ImageField(_image));
        sut.ImageParams = new { w = 1000, quality = 50, format = "jpg" };
        sut.SrcSet = new object[] { new { w = 320, quality = 75 } };

        // Act
        sut.Process(tagHelperContext, tagHelperOutput);

        // Assert
        tagHelperOutput.Attributes.Should().ContainSingle(a => a.Name == "srcset");
        string srcSetValue = tagHelperOutput.Attributes["srcset"].Value.ToString()!;
        srcSetValue.Should().Contain("w=320");
        srcSetValue.Should().Contain("quality=75");
        srcSetValue.Should().Contain("format=jpg"); // Inherited from ImageParams
        srcSetValue.Should().NotContain("w=1000");
        srcSetValue.Should().NotContain("quality=50");
    }

    [Theory]
    [AutoNSubstituteData]
    public void Process_SrcSetWithWidthParameterPriority_UsesCorrectPrecedence(
        ImageTagHelper sut,
        TagHelperContext tagHelperContext,
        TagHelperOutput tagHelperOutput)
    {
        // Arrange - Test priority: w > mw > width > maxWidth
        tagHelperOutput.TagName = "img";
        sut.For = GetModelExpression(new ImageField(_image));
        sut.SrcSet = new object[]
        {
            new { w = 100, mw = 200, width = 300, maxWidth = 400 }, // Should use w=100
            new { mw = 200, width = 300, maxWidth = 400 }, // Should use mw=200
            new { width = 300, maxWidth = 400 }, // Should use width=300
            new { maxWidth = 400 } // Should use maxWidth=400
        };

        // Act
        sut.Process(tagHelperContext, tagHelperOutput);

        // Assert
        tagHelperOutput.Attributes.Should().ContainSingle(a => a.Name == "srcset");
        string srcSetValue = tagHelperOutput.Attributes["srcset"].Value.ToString()!;
        srcSetValue.Should().Contain("100w");
        srcSetValue.Should().Contain("200w");
        srcSetValue.Should().Contain("300w");
        srcSetValue.Should().Contain("400w");
    }

    [Theory]
    [AutoNSubstituteData]
    public void Process_SrcSetWithZeroOrNegativeWidths_SkipsInvalidEntries(
        ImageTagHelper sut,
        TagHelperContext tagHelperContext,
        TagHelperOutput tagHelperOutput)
    {
        // Arrange
        tagHelperOutput.TagName = "img";
        sut.For = GetModelExpression(new ImageField(_image));
        sut.SrcSet = new object[]
        {
            new { w = 0, quality = 75 },    // Should skip
            new { w = -100, quality = 80 }, // Should skip
            new { w = 320, quality = 75 },  // Should include
            new { quality = 90 } // Should skip - no width
        };

        // Act
        sut.Process(tagHelperContext, tagHelperOutput);

        // Assert
        tagHelperOutput.Attributes.Should().ContainSingle(a => a.Name == "srcset");
        string srcSetValue = tagHelperOutput.Attributes["srcset"].Value.ToString()!;
        srcSetValue.Should().Contain("320w");
        srcSetValue.Should().NotContain(" 0w");
        srcSetValue.Should().NotContain(" -100w");

        // Should only have one entry
        string[] entries = srcSetValue.Split(", ");
        entries.Should().HaveCount(1);
    }

    [Theory]
    [AutoNSubstituteData]
    public void Process_SrcSetWithExistingUrlParameters_PreservesAndMergesParameters(
        ImageTagHelper sut,
        TagHelperContext tagHelperContext,
        TagHelperOutput tagHelperOutput)
    {
        // Arrange
        Image imageWithParams = new Image
        {
            Src = "https://edge.sitecorecloud.io/media/image.jpg?h=2001&iar=0&hash=abc123&w=3000",
            Alt = "Test Image"
        };

        tagHelperOutput.TagName = "img";
        sut.For = GetModelExpression(new ImageField(imageWithParams));
        sut.SrcSet = new object[] { new { w = 320, quality = 75 } };

        // Act
        sut.Process(tagHelperContext, tagHelperOutput);

        // Assert
        tagHelperOutput.Attributes.Should().ContainSingle(a => a.Name == "srcset");
        string srcSetValue = tagHelperOutput.Attributes["srcset"].Value.ToString()!;

        // Should preserve existing parameters and merge with new ones
        srcSetValue.Should().Contain("h=2001");
        srcSetValue.Should().Contain("iar=0");
        srcSetValue.Should().Contain("hash=abc123");
        srcSetValue.Should().Contain("quality=75");

        // New w parameter should override existing w=3000
        srcSetValue.Should().Contain("w=320");
        srcSetValue.Should().NotContain("w=3000");
        srcSetValue.Should().Contain("320w");
    }

    [Theory]
    [AutoNSubstituteData]
    public void Process_SrcSetWithMediaUrlTransformation_TransformsCorrectly(
        ImageTagHelper sut,
        TagHelperContext tagHelperContext,
        TagHelperOutput tagHelperOutput)
    {
        // Arrange
        Image imageWithMediaUrl = new Image
        {
            Src = "/~/media/images/test.jpg",
            Alt = "Test Image"
        };

        tagHelperOutput.TagName = "img";
        sut.For = GetModelExpression(new ImageField(imageWithMediaUrl));
        sut.SrcSet = new object[] { new { w = 320, quality = 75 } };

        // Act
        sut.Process(tagHelperContext, tagHelperOutput);

        // Assert
        tagHelperOutput.Attributes.Should().ContainSingle(a => a.Name == "srcset");
        string srcSetValue = tagHelperOutput.Attributes["srcset"].Value.ToString()!;

        // Should transform /~/media/ to /~/jssmedia/
        srcSetValue.Should().Contain("/~/jssmedia/images/test.jpg");
        srcSetValue.Should().NotContain("/~/media/images/test.jpg");
        srcSetValue.Should().Contain("320w");
    }

    [Theory]
    [AutoNSubstituteData]
    public void Process_SrcSetWithNullAndEmptyEntries_HandlesGracefully(
        ImageTagHelper sut,
        TagHelperContext tagHelperContext,
        TagHelperOutput tagHelperOutput)
    {
        // Arrange
        tagHelperOutput.TagName = "img";
        sut.For = GetModelExpression(new ImageField(_image));
        sut.SrcSet = new object?[]
        {
            new { w = 320, quality = 75 },
            null, // Should skip
            new { w = 480, quality = 80 },
            new { } // Should skip - no width parameter
        };

        // Act
        sut.Process(tagHelperContext, tagHelperOutput);

        // Assert
        tagHelperOutput.Attributes.Should().ContainSingle(a => a.Name == "srcset");
        string srcSetValue = tagHelperOutput.Attributes["srcset"].Value.ToString()!;
        srcSetValue.Should().Contain("320w");
        srcSetValue.Should().Contain("480w");

        // Should only have two entries
        string[] entries = srcSetValue.Split(", ");
        entries.Should().HaveCount(2);
    }

    [Theory]
    [AutoNSubstituteData]
    public void Process_SrcSetWithComplexParameters_HandlesAllParameters(
        ImageTagHelper sut,
        TagHelperContext tagHelperContext,
        TagHelperOutput tagHelperOutput)
    {
        // Arrange
        tagHelperOutput.TagName = "img";
        sut.For = GetModelExpression(new ImageField(_image));
        sut.SrcSet = new object[]
        {
            new { w = 320, quality = 75, format = "webp", dpr = 2, fit = "crop" }
        };

        // Act
        sut.Process(tagHelperContext, tagHelperOutput);

        // Assert
        tagHelperOutput.Attributes.Should().ContainSingle(a => a.Name == "srcset");
        string srcSetValue = tagHelperOutput.Attributes["srcset"].Value.ToString()!;
        srcSetValue.Should().Contain("320w");
        srcSetValue.Should().Contain("quality=75");
        srcSetValue.Should().Contain("format=webp");
        srcSetValue.Should().Contain("dpr=2");
        srcSetValue.Should().Contain("fit=crop");
    }

    [Theory]
    [AutoNSubstituteData]
    public void Process_SrcSetWithDictionaryParameters_GeneratesSrcSetAttribute(
        ImageTagHelper sut,
        TagHelperContext tagHelperContext,
        TagHelperOutput tagHelperOutput)
    {
        // Arrange
        tagHelperOutput.TagName = "img";
        sut.For = GetModelExpression(new ImageField(_image));
        sut.SrcSet = new object[]
        {
            new Dictionary<string, object> { { "w", 320 }, { "quality", 75 } },
            new Dictionary<string, object> { { "mw", 480 }, { "quality", 80 } }
        };

        // Act
        sut.Process(tagHelperContext, tagHelperOutput);

        // Assert
        tagHelperOutput.Attributes.Should().ContainSingle(a => a.Name == "srcset");
        string srcSetValue = tagHelperOutput.Attributes["srcset"].Value.ToString()!;
        srcSetValue.Should().Contain("320w");
        srcSetValue.Should().Contain("480w");
        srcSetValue.Should().Contain("quality=75");
        srcSetValue.Should().Contain("quality=80");
    }

    [Theory]
    [AutoNSubstituteData]
    public void Process_SrcSetWithSizesAttribute_AddsBothAttributes(
        ImageTagHelper sut,
        TagHelperContext tagHelperContext,
        TagHelperOutput tagHelperOutput)
    {
        // Arrange
        tagHelperOutput.TagName = "img";
        sut.For = GetModelExpression(new ImageField(_image));
        sut.SrcSet = new object[] { new { w = 320 }, new { w = 480 } };
        sut.Sizes = "(min-width: 768px) 480px, 320px";

        // Act
        sut.Process(tagHelperContext, tagHelperOutput);

        // Assert
        tagHelperOutput.Attributes.Should().Contain(a => a.Name == "srcset");
        tagHelperOutput.Attributes.Should().Contain(a => a.Name == "sizes");

        string sizesValue = tagHelperOutput.Attributes["sizes"].Value.ToString()!;
        sizesValue.Should().Be("(min-width: 768px) 480px, 320px");
    }

    private static ModelExpression GetModelExpression(Field model)
    {
        DefaultModelMetadata? modelMetadata = Substitute.For<DefaultModelMetadata>(
            Substitute.For<IModelMetadataProvider>(),
            Substitute.For<ICompositeMetadataDetailsProvider>(),
            Substitute.For<DefaultMetadataDetails>(ModelMetadataIdentity.ForType(model.GetType()), ModelAttributes.GetAttributesForType(model.GetType())));
        ModelExplorer? explorer = Substitute.For<ModelExplorer>(Substitute.For<IModelMetadataProvider>(), modelMetadata, model);
        return new ModelExpression("test", explorer);
    }
}