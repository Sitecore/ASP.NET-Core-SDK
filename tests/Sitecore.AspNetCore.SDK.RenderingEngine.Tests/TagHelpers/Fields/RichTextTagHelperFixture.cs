using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using AutoFixture;
using AutoFixture.Idioms;
using AwesomeAssertions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using NSubstitute;
using Sitecore.AspNetCore.SDK.AutoFixture.Attributes;
using Sitecore.AspNetCore.SDK.AutoFixture.Extensions;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Response.Model;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Response.Model.Fields;
using Sitecore.AspNetCore.SDK.RenderingEngine.Rendering;
using Sitecore.AspNetCore.SDK.RenderingEngine.TagHelpers.Fields;
using Xunit;

namespace Sitecore.AspNetCore.SDK.RenderingEngine.Tests.TagHelpers.Fields;

public class RichTextTagHelperFixture
{
    private const string TestHtml = "<p>This is the test text</p>";

    private const string TestEditableMarkup = "<input id='fld_585596CA7903500B8DF20357DD6E3FAC_2F459C90888656D6B124751CFAFE6785_en_1_b6270e9a40b34bf9bfeaafd8999a003b_284' class='scFieldValue' name='fld_585596CA7903500B8DF20357DD6E3FAC_2F459C90888656D6B124751CFAFE6785_en_1_b6270e9a40b34bf9bfeaafd8999a003b_284' type='hidden' value=\"&lt;p&gt;This is a live set of examples of how to use JSS. For more information on using JSS, please see &lt;a href=&quot;https://jss.sitecore.net&quot; target=&quot;_blank&quot; rel=&quot;noopener noreferrer&quot;&gt;the documentation&lt;/a&gt;.&lt;/p&gt;\n&lt;p&gt;The content and layout of this page is defined in &lt;code&gt;/data/routes/styleguide/en.yml&lt;/code&gt;&lt;/p&gt;\n\" /><span class=\"scChromeData\">{\"commands\":[{\"click\":\"chrome:field:editcontrol({command:\\\"webedit:edithtml\\\"})\",\"header\":\"Edit Text\",\"icon\":\"/temp/iconcache/office/16x16/pencil.png\",\"disabledIcon\":\"/temp/pencil_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"Edit the text\",\"type\":null},{\"click\":\"chrome:field:execute({command:\\\"bold\\\", userInterface:true, value:true})\",\"header\":\"\",\"icon\":\"/temp/iconcache/office/16x16/font_style_bold.png\",\"disabledIcon\":\"/temp/font_style_bold_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"Bold\",\"type\":null},{\"click\":\"chrome:field:execute({command:\\\"Italic\\\", userInterface:true, value:true})\",\"header\":\"\",\"icon\":\"/temp/iconcache/office/16x16/font_style_italics.png\",\"disabledIcon\":\"/temp/font_style_italics_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"Italic\",\"type\":null},{\"click\":\"chrome:field:execute({command:\\\"Underline\\\", userInterface:true, value:true})\",\"header\":\"\",\"icon\":\"/temp/iconcache/office/16x16/font_style_underline.png\",\"disabledIcon\":\"/temp/font_style_underline_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"Underline\",\"type\":null},{\"click\":\"chrome:field:insertexternallink\",\"header\":\"\",\"icon\":\"/temp/iconcache/office/16x16/earth_link.png\",\"disabledIcon\":\"/temp/earth_link_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"Insert an external link into the text field.\",\"type\":null},{\"click\":\"chrome:field:insertlink\",\"header\":\"\",\"icon\":\"/temp/iconcache/office/16x16/link.png\",\"disabledIcon\":\"/temp/link_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"Insert a link into the text field.\",\"type\":null},{\"click\":\"chrome:field:removelink\",\"header\":\"\",\"icon\":\"/temp/iconcache/office/16x16/link_broken.png\",\"disabledIcon\":\"/temp/link_broken_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"Remove link.\",\"type\":null},{\"click\":\"chrome:field:insertimage\",\"header\":\"Insert image\",\"icon\":\"/temp/iconcache/office/16x16/photo_landscape.png\",\"disabledIcon\":\"/temp/photo_landscape_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"Insert an image into the text field.\",\"type\":null},{\"click\":\"chrome:common:edititem({command:\\\"webedit:open\\\"})\",\"header\":\"Edit the related item\",\"icon\":\"/temp/iconcache/office/16x16/cubes.png\",\"disabledIcon\":\"/temp/cubes_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"Edit the related item in the Content Editor.\",\"type\":\"common\"},{\"click\":\"chrome:rendering:personalize({command:\\\"webedit:personalize\\\"})\",\"header\":\"Personalize\",\"icon\":\"/temp/iconcache/office/16x16/users_family.png\",\"disabledIcon\":\"/temp/users_family_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"Create or edit personalization for this component.\",\"type\":\"sticky\"}],\"contextItemUri\":\"sitecore://master/{585596CA-7903-500B-8DF2-0357DD6E3FAC}?lang=en&ver=1\",\"custom\":{},\"displayName\":\"content\",\"expandedDisplayName\":null}</span><span scFieldType=\"rich text\" scDefaultText=\"[No text in field]\" contenteditable=\"true\" class=\"scWebEditInput\" id=\"fld_585596CA7903500B8DF20357DD6E3FAC_2F459C90888656D6B124751CFAFE6785_en_1_b6270e9a40b34bf9bfeaafd8999a003b_284_edit\"><p>This is a live set of examples of how to use JSS. For more information on using JSS, please see <a href=\"https://jss.sitecore.net\" target=\"_blank\" rel=\"noopener noreferrer\">the documentation</a>.</p>\n<p>The content and layout of this page is defined in <code>/data/routes/styleguide/en.yml</code></p>\n</span>";

    // ReSharper disable once UnusedMember.Global - Used by testing framework
    [ExcludeFromCodeCoverage]
    public static Action<IFixture> AutoSetup => f =>
    {
        TagHelperContext tagHelperContext = new(
            [],
            new Dictionary<object, object>(),
            Guid.NewGuid().ToString("N", CultureInfo.InvariantCulture));

        TagHelperOutput tagHelperOutput = new(string.Empty, [], (_, _) =>
        {
            DefaultTagHelperContent tagHelperContent = new();
            tagHelperContent.SetHtmlContent(string.Empty);
            return Task.FromResult<TagHelperContent>(tagHelperContent);
        });

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
        RichTextTagHelper sut,
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

    #region sc-text tag with asp-for attribute tests
    [Theory]
    [AutoNSubstituteData]
    public async Task Process_ScTextTagWithNullForAndTextAttribute_GeneratesEmptyOutput(
        RichTextTagHelper sut,
        TagHelperContext tagHelperContext,
        TagHelperOutput tagHelperOutput)
    {
        // Arrange
        tagHelperOutput.TagName = RenderingEngineConstants.SitecoreTagHelpers.RichTextHtmlTag;
        sut.For = null;
        sut.TextModel = null;

        // Act
        await sut.ProcessAsync(tagHelperContext, tagHelperOutput);

        // Assert
        tagHelperOutput.Content.GetContent().Should().Be(string.Empty);
    }

    [Theory]
    [AutoNSubstituteData]
    public void Process_ScTextTagWithValidForAttribute_GeneratesCorrectOutput(
        RichTextTagHelper sut,
        TagHelperContext tagHelperContext,
        TagHelperOutput tagHelperOutput)
    {
        // Arrange
        tagHelperOutput.TagName = RenderingEngineConstants.SitecoreTagHelpers.RichTextHtmlTag;
        sut.For = GetModelExpression(new RichTextField(TestHtml, false));
        sut.TextModel = null;

        // Act
        sut.Process(tagHelperContext, tagHelperOutput);

        // Assert
        tagHelperOutput.Content.GetContent().Should().Be(TestHtml);
    }

    [Theory]
    [AutoNSubstituteData]
    public void Process_ScTextTagWithValidForAttribute_GeneratesOutputWithoutOuterScTextTag(
        RichTextTagHelper sut,
        TagHelperContext tagHelperContext,
        TagHelperOutput tagHelperOutput)
    {
        // Arrange
        tagHelperOutput.TagName = RenderingEngineConstants.SitecoreTagHelpers.RichTextHtmlTag;
        sut.For = GetModelExpression(new RichTextField(TestHtml, false));
        sut.TextModel = null;

        // Act
        sut.Process(tagHelperContext, tagHelperOutput);

        // Assert
        tagHelperOutput.TagName.Should().BeNull();
    }

    [Theory]
    [AutoNSubstituteData]
    public void Process_ScTextTagWithInvalidForAttribute_GeneratesEmptyOutput(
        RichTextTagHelper sut,
        TagHelperContext tagHelperContext,
        TagHelperOutput tagHelperOutput)
    {
        // Arrange
        tagHelperOutput.TagName = RenderingEngineConstants.SitecoreTagHelpers.RichTextHtmlTag;
        sut.For = GetModelExpression(new TextField(TestHtml));
        sut.TextModel = null;

        // Act
        sut.Process(tagHelperContext, tagHelperOutput);

        // Assert
        tagHelperOutput.Content.GetContent().Should().Be(string.Empty);
        tagHelperOutput.Content.GetContent().Should().NotContain(RenderingEngineConstants.SitecoreTagHelpers.RichTextHtmlTag);
    }

    [Theory]
    [AutoNSubstituteData]
    public void Process_ScTextTagWithEmptyValueInForAttribute_GeneratesEmptyOutput(
        RichTextTagHelper sut,
        TagHelperContext tagHelperContext,
        TagHelperOutput tagHelperOutput)
    {
        // Arrange
        tagHelperOutput.TagName = RenderingEngineConstants.SitecoreTagHelpers.RichTextHtmlTag;
        sut.For = GetModelExpression(new TextField(string.Empty));
        sut.TextModel = null;

        // Act
        sut.Process(tagHelperContext, tagHelperOutput);

        // Assert
        tagHelperOutput.Content.GetContent().Should().Be(string.Empty);
    }

    [Theory]
    [AutoNSubstituteData]
    public void Process_ScTextTagWithEditableFieldAndEditableSetToTrue_GeneratesCorrectOutput(
        RichTextTagHelper sut,
        TagHelperContext tagHelperContext,
        TagHelperOutput tagHelperOutput)
    {
        // Arrange
        tagHelperOutput.TagName = RenderingEngineConstants.SitecoreTagHelpers.RichTextHtmlTag;
        RichTextField testField = new(TestHtml, false)
        {
            EditableMarkup = TestEditableMarkup
        };
        sut.For = GetModelExpression(testField);
        sut.TextModel = null;

        // Act
        sut.Process(tagHelperContext, tagHelperOutput);

        // Assert
        tagHelperOutput.Content.GetContent().Should().Be(TestEditableMarkup);
    }

    [Theory]
    [AutoNSubstituteData]
    public void Process_ScTextTagWithEmptyEditableFieldAndEditableSetToTrue_GeneratesCorrectOutput(
        RichTextTagHelper sut,
        TagHelperContext tagHelperContext,
        TagHelperOutput tagHelperOutput)
    {
        // Arrange
        tagHelperOutput.TagName = RenderingEngineConstants.SitecoreTagHelpers.RichTextHtmlTag;
        RichTextField testField = new(TestHtml, false)
        {
            EditableMarkup = string.Empty
        };
        sut.For = GetModelExpression(testField);
        sut.TextModel = null;

        // Act
        sut.Process(tagHelperContext, tagHelperOutput);

        // Assert
        tagHelperOutput.Content.GetContent().Should().Be(TestHtml);
    }

    [Theory]
    [AutoNSubstituteData]
    public void Process_ScTextTagWithEditableFieldAndEditableSetToFalse_GeneratesCorrectOutput(
        RichTextTagHelper sut,
        TagHelperContext tagHelperContext,
        TagHelperOutput tagHelperOutput)
    {
        // Arrange
        tagHelperOutput.TagName = RenderingEngineConstants.SitecoreTagHelpers.RichTextHtmlTag;
        RichTextField testField = new(TestHtml, false)
        {
            EditableMarkup = TestEditableMarkup
        };
        sut.For = GetModelExpression(testField);
        sut.Editable = false;
        sut.TextModel = null;

        // Act
        sut.Process(tagHelperContext, tagHelperOutput);

        // Assert
        tagHelperOutput.Content.GetContent().Should().Be(TestHtml);
    }

    #endregion

    #region sc-text tag with rich-text attribute tests

    [Theory]
    [AutoNSubstituteData]
    public void Process_ScTextTagWithValidTextAttribute_GeneratesCorrectOutput(
        RichTextTagHelper sut,
        TagHelperContext tagHelperContext,
        TagHelperOutput tagHelperOutput)
    {
        // Arrange
        tagHelperOutput.TagName = RenderingEngineConstants.SitecoreTagHelpers.RichTextHtmlTag;
        sut.TextModel = new RichTextField(TestHtml, false);
        sut.For = null!;

        // Act
        sut.Process(tagHelperContext, tagHelperOutput);

        // Assert
        tagHelperOutput.Content.GetContent().Should().Be(TestHtml);
    }

    [Theory]
    [AutoNSubstituteData]
    public void Process_ScTextTagWithValidTextAttribute_GeneratesOutputWithoutOuterScTextTag(
        RichTextTagHelper sut,
        TagHelperContext tagHelperContext,
        TagHelperOutput tagHelperOutput)
    {
        // Arrange
        tagHelperOutput.TagName = RenderingEngineConstants.SitecoreTagHelpers.RichTextHtmlTag;
        sut.TextModel = new RichTextField(TestHtml, false);
        sut.For = null!;

        // Act
        sut.Process(tagHelperContext, tagHelperOutput);

        // Assert
        tagHelperOutput.TagName.Should().BeNull();
    }

    [Theory]
    [AutoNSubstituteData]
    public void Process_ScTextTagWithInvalidTextAttribute_GeneratesEmptyOutput(
        RichTextTagHelper sut,
        TagHelperContext tagHelperContext,
        TagHelperOutput tagHelperOutput)
    {
        // Arrange
        tagHelperOutput.TagName = RenderingEngineConstants.SitecoreTagHelpers.RichTextHtmlTag;
        sut.TextModel = new TextField(TestHtml);
        sut.For = null!;

        // Act
        sut.Process(tagHelperContext, tagHelperOutput);

        // Assert
        tagHelperOutput.Content.GetContent().Should().Be(string.Empty);
        tagHelperOutput.Content.GetContent().Should().NotContain(RenderingEngineConstants.SitecoreTagHelpers.RichTextHtmlTag);
    }

    [Theory]
    [AutoNSubstituteData]
    public void Process_ScTextTagWithEmptyValueInTextAttribute_GeneratesEmptyOutput(
        RichTextTagHelper sut,
        TagHelperContext tagHelperContext,
        TagHelperOutput tagHelperOutput)
    {
        // Arrange
        tagHelperOutput.TagName = RenderingEngineConstants.SitecoreTagHelpers.RichTextHtmlTag;
        sut.TextModel = new TextField(string.Empty);
        sut.For = null!;

        // Act
        sut.Process(tagHelperContext, tagHelperOutput);

        // Assert
        tagHelperOutput.Content.GetContent().Should().Be(string.Empty);
    }

    [Theory]
    [AutoNSubstituteData]
    public void Process_ScTextTagWithEditableFieldAndEditableSetToTrueAndTextAttribute_GeneratesCorrectOutput(
        RichTextTagHelper sut,
        TagHelperContext tagHelperContext,
        TagHelperOutput tagHelperOutput)
    {
        // Arrange
        tagHelperOutput.TagName = RenderingEngineConstants.SitecoreTagHelpers.RichTextHtmlTag;
        RichTextField testField = new(TestHtml, false)
        {
            EditableMarkup = TestEditableMarkup
        };
        sut.TextModel = testField;
        sut.For = null!;

        // Act
        sut.Process(tagHelperContext, tagHelperOutput);

        // Assert
        tagHelperOutput.Content.GetContent().Should().Be(TestEditableMarkup);
    }

    [Theory]
    [AutoNSubstituteData]
    public void Process_ScTextTagWithEmptyEditableFieldAndEditableSetToTrueAndTextAttribute_GeneratesCorrectOutput(
        RichTextTagHelper sut,
        TagHelperContext tagHelperContext,
        TagHelperOutput tagHelperOutput)
    {
        // Arrange
        tagHelperOutput.TagName = RenderingEngineConstants.SitecoreTagHelpers.RichTextHtmlTag;
        RichTextField testField = new(TestHtml, false)
        {
            EditableMarkup = string.Empty
        };
        sut.TextModel = testField;
        sut.For = null!;

        // Act
        sut.Process(tagHelperContext, tagHelperOutput);

        // Assert
        tagHelperOutput.Content.GetContent().Should().Be(TestHtml);
    }

    [Theory]
    [AutoNSubstituteData]
    public void Process_ScTextTagWithEditableFieldAndEditableSetToFalseAndTextAttribute_GeneratesCorrectOutput(
        RichTextTagHelper sut,
        TagHelperContext tagHelperContext,
        TagHelperOutput tagHelperOutput)
    {
        // Arrange
        tagHelperOutput.TagName = RenderingEngineConstants.SitecoreTagHelpers.RichTextHtmlTag;
        RichTextField testField = new(TestHtml, false)
        {
            EditableMarkup = TestEditableMarkup
        };
        sut.Editable = false;
        sut.TextModel = testField;
        sut.For = null!;

        // Act
        sut.Process(tagHelperContext, tagHelperOutput);

        // Assert
        tagHelperOutput.Content.GetContent().Should().Be(TestHtml);
    }

    #endregion

    #region div tag with asp-for attribute tests
    [Theory]
    [AutoNSubstituteData]
    public async Task Process_DivTagWithNullForAndTextAttribute_GeneratesEmptyOutput(
        RichTextTagHelper sut,
        TagHelperContext tagHelperContext,
        TagHelperOutput tagHelperOutput)
    {
        // Arrange
        tagHelperOutput.TagName = "div";
        sut.For = null!;
        sut.TextModel = null;

        // Act
        await sut.ProcessAsync(tagHelperContext, tagHelperOutput);

        // Assert
        tagHelperOutput.Content.GetContent().Should().Be(string.Empty);
    }

    [Theory]
    [AutoNSubstituteData]
    public void Process_DivTagWithValidForAttribute_GeneratesCorrectOutput(
        RichTextTagHelper sut,
        TagHelperContext tagHelperContext,
        TagHelperOutput tagHelperOutput)
    {
        // Arrange
        tagHelperOutput.TagName = "div";
        sut.For = GetModelExpression(new RichTextField(TestHtml, false));
        sut.TextModel = null;

        // Act
        sut.Process(tagHelperContext, tagHelperOutput);

        // Assert
        tagHelperOutput.Content.GetContent().Should().Be(TestHtml);
    }

    [Theory]
    [AutoNSubstituteData]
    public void Process_DivTagWithValidForAttribute_GeneratesOutputWithOuterDivTag(
        RichTextTagHelper sut,
        TagHelperContext tagHelperContext,
        TagHelperOutput tagHelperOutput)
    {
        // Arrange
        tagHelperOutput.TagName = "div";
        sut.For = GetModelExpression(new RichTextField(TestHtml, false));
        sut.TextModel = null;

        // Act
        sut.Process(tagHelperContext, tagHelperOutput);

        // Assert
        tagHelperOutput.TagName.Should().Be("div");
    }

    [Theory]
    [AutoNSubstituteData]
    public void Process_DivTagWithInvalidForAttribute_GeneratesEmptyOutput(
        RichTextTagHelper sut,
        TagHelperContext tagHelperContext,
        TagHelperOutput tagHelperOutput)
    {
        // Arrange
        tagHelperOutput.TagName = "div";
        sut.For = GetModelExpression(new TextField(TestHtml));
        sut.TextModel = null;

        // Act
        sut.Process(tagHelperContext, tagHelperOutput);

        // Assert
        tagHelperOutput.Content.GetContent().Should().Be(string.Empty);
    }

    [Theory]
    [AutoNSubstituteData]
    public void Process_DivTagWithEmptyValueInForAttribute_GeneratesEmptyOutput(
        RichTextTagHelper sut,
        TagHelperContext tagHelperContext,
        TagHelperOutput tagHelperOutput)
    {
        // Arrange
        tagHelperOutput.TagName = "div";
        sut.For = GetModelExpression(new TextField(string.Empty));
        sut.TextModel = null;

        // Act
        sut.Process(tagHelperContext, tagHelperOutput);

        // Assert
        tagHelperOutput.Content.GetContent().Should().Be(string.Empty);
    }

    [Theory]
    [AutoNSubstituteData]
    public void Process_DivTagWithEditableFieldAndEditableSetToTrue_GeneratesCorrectOutput(
        RichTextTagHelper sut,
        TagHelperContext tagHelperContext,
        TagHelperOutput tagHelperOutput)
    {
        // Arrange
        tagHelperOutput.TagName = "div";
        RichTextField testField = new(TestHtml, false)
        {
            EditableMarkup = TestEditableMarkup
        };
        sut.For = GetModelExpression(testField);
        sut.TextModel = null;

        // Act
        sut.Process(tagHelperContext, tagHelperOutput);

        // Assert
        tagHelperOutput.Content.GetContent().Should().Be(TestEditableMarkup);
    }

    [Theory]
    [AutoNSubstituteData]
    public void Process_DivTagWithEmptyEditableFieldAndEditableSetToTrue_GeneratesCorrectOutput(
        RichTextTagHelper sut,
        TagHelperContext tagHelperContext,
        TagHelperOutput tagHelperOutput)
    {
        // Arrange
        tagHelperOutput.TagName = "div";
        RichTextField testField = new(TestHtml, false)
        {
            EditableMarkup = string.Empty
        };
        sut.For = GetModelExpression(testField);
        sut.TextModel = null;

        // Act
        sut.Process(tagHelperContext, tagHelperOutput);

        // Assert
        tagHelperOutput.Content.GetContent().Should().Be(TestHtml);
    }

    [Theory]
    [AutoNSubstituteData]
    public void Process_DivTagWithEditableFieldAndEditableSetToFalse_GeneratesCorrectOutput(
        RichTextTagHelper sut,
        TagHelperContext tagHelperContext,
        TagHelperOutput tagHelperOutput)
    {
        // Arrange
        tagHelperOutput.TagName = "div";
        RichTextField testField = new(TestHtml, false)
        {
            EditableMarkup = TestEditableMarkup
        };
        sut.For = GetModelExpression(testField);
        sut.Editable = false;
        sut.TextModel = null;

        // Act
        sut.Process(tagHelperContext, tagHelperOutput);

        // Assert
        tagHelperOutput.Content.GetContent().Should().Be(TestHtml);
    }

    #endregion

    #region div tag with rich-text attribute tests

    [Theory]
    [AutoNSubstituteData]
    public void Process_DivTagWithValidTextAttribute_GeneratesCorrectOutput(
        RichTextTagHelper sut,
        TagHelperContext tagHelperContext,
        TagHelperOutput tagHelperOutput)
    {
        // Arrange
        tagHelperOutput.TagName = "div";
        sut.For = null!;
        sut.TextModel = new RichTextField(TestHtml, false);

        // Act
        sut.Process(tagHelperContext, tagHelperOutput);

        // Assert
        tagHelperOutput.Content.GetContent().Should().Be(TestHtml);
    }

    [Theory]
    [AutoNSubstituteData]
    public void Process_DivTagWithValidTextAttribute_GeneratesOutputWithOuterDivTag(
        RichTextTagHelper sut,
        TagHelperContext tagHelperContext,
        TagHelperOutput tagHelperOutput)
    {
        // Arrange
        tagHelperOutput.TagName = "div";
        sut.TextModel = new RichTextField(TestHtml, false);
        sut.For = null!;

        // Act
        sut.Process(tagHelperContext, tagHelperOutput);

        // Assert
        tagHelperOutput.TagName.Should().Be("div");
    }

    [Theory]
    [AutoNSubstituteData]
    public void Process_DivTagWithInvalidTextAttribute_GeneratesEmptyOutput(
        RichTextTagHelper sut,
        TagHelperContext tagHelperContext,
        TagHelperOutput tagHelperOutput)
    {
        // Arrange
        tagHelperOutput.TagName = "div";
        sut.For = null!;
        sut.TextModel = new TextField(TestHtml);

        // Act
        sut.Process(tagHelperContext, tagHelperOutput);

        // Assert
        tagHelperOutput.Content.GetContent().Should().Be(string.Empty);
    }

    [Theory]
    [AutoNSubstituteData]
    public void Process_DivTagWithEmptyValueInTextAttribute_GeneratesEmptyOutput(
        RichTextTagHelper sut,
        TagHelperContext tagHelperContext,
        TagHelperOutput tagHelperOutput)
    {
        // Arrange
        tagHelperOutput.TagName = "div";
        sut.For = null!;
        sut.TextModel = new TextField(string.Empty);

        // Act
        sut.Process(tagHelperContext, tagHelperOutput);

        // Assert
        tagHelperOutput.Content.GetContent().Should().Be(string.Empty);
    }

    [Theory]
    [AutoNSubstituteData]
    public void Process_DivTagWithEditableFieldAndEditableSetToTrueAndTextAttribute_GeneratesCorrectOutput(
        RichTextTagHelper sut,
        TagHelperContext tagHelperContext,
        TagHelperOutput tagHelperOutput)
    {
        // Arrange
        tagHelperOutput.TagName = "div";
        RichTextField testField = new(TestHtml, false)
        {
            EditableMarkup = TestEditableMarkup
        };
        sut.For = null!;
        sut.TextModel = testField;

        // Act
        sut.Process(tagHelperContext, tagHelperOutput);

        // Assert
        tagHelperOutput.Content.GetContent().Should().Be(TestEditableMarkup);
    }

    [Theory]
    [AutoNSubstituteData]
    public void Process_DivTagWithEmptyEditableFieldAndEditableSetToTrueAndTextAttribute_GeneratesCorrectOutput(
        RichTextTagHelper sut,
        TagHelperContext tagHelperContext,
        TagHelperOutput tagHelperOutput)
    {
        // Arrange
        tagHelperOutput.TagName = "div";
        RichTextField testField = new(TestHtml, false)
        {
            EditableMarkup = string.Empty
        };
        sut.TextModel = testField;
        sut.For = null!;

        // Act
        sut.Process(tagHelperContext, tagHelperOutput);

        // Assert
        tagHelperOutput.Content.GetContent().Should().Be(TestHtml);
    }

    [Theory]
    [AutoNSubstituteData]
    public void Process_DivTagWithEditableFieldAndEditableSetToFalseAndTextAttribute_GeneratesCorrectOutput(
        RichTextTagHelper sut,
        TagHelperContext tagHelperContext,
        TagHelperOutput tagHelperOutput)
    {
        // Arrange
        tagHelperOutput.TagName = "div";
        RichTextField testField = new(TestHtml, false)
        {
            EditableMarkup = TestEditableMarkup
        };
        sut.For = null!;
        sut.Editable = false;
        sut.TextModel = testField;

        // Act
        sut.Process(tagHelperContext, tagHelperOutput);

        // Assert
        tagHelperOutput.Content.GetContent().Should().Be(TestHtml);
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
        RichTextTagHelper sut = new(chromeRenderer);
        EditableChrome openingChrome = Substitute.For<EditableChrome>();
        EditableChrome closingChrome = Substitute.For<EditableChrome>();
        tagHelperOutput.TagName = RenderingEngineConstants.SitecoreTagHelpers.RichTextHtmlTag;
        RichTextField testField = new(TestHtml, false)
        {
            OpeningChrome = openingChrome,
            ClosingChrome = closingChrome
        };
        sut.TextModel = testField;

        // Act
        sut.Process(tagHelperContext, tagHelperOutput);

        // Assert
        chromeRenderer.Received().Render(openingChrome);
        chromeRenderer.Received().Render(closingChrome);
        tagHelperOutput.Content.GetContent().Should().Be($"{chromeRenderer.Render(openingChrome)}<span>{TestHtml}</span>{chromeRenderer.Render(closingChrome)}");
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