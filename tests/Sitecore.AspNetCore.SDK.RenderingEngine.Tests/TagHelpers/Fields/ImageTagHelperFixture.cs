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