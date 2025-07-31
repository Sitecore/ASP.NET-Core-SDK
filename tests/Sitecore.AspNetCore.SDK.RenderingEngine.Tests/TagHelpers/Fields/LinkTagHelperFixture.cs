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
using Sitecore.AspNetCore.SDK.LayoutService.Client.Response.Model.Properties;
using Sitecore.AspNetCore.SDK.RenderingEngine.Rendering;
using Sitecore.AspNetCore.SDK.RenderingEngine.TagHelpers.Fields;
using Xunit;

namespace Sitecore.AspNetCore.SDK.RenderingEngine.Tests.TagHelpers.Fields;

public class LinkTagHelperFixture
{
    private const string Editable = "<input id='fld_4252A3F257F64F2985F2C6B14E3EF5A0_86473463D14843CFB0C7771DD2F35AE4_en_1_227c3f0a91ec4ba29ab66a858726fd37_564' class='scFieldValue' name='fld_4252A3F257F64F2985F2C6B14E3EF5A0_86473463D14843CFB0C7771DD2F35AE4_en_1_227c3f0a91ec4ba29ab66a858726fd37_564' type='hidden' value=\"&lt;link text=&quot;This is description&quot; anchor=&quot;&quot; linktype=&quot;internal&quot; class=&quot;sample-css-class&quot; title=&quot;Sample alternate text&quot;  querystring=&quot;&quot; id=&quot;{B0BD73D0-7F5D-4272-BFA0-B43AB65069CC}&quot; /&gt;\" /><code id=\"fld_4252A3F257F64F2985F2C6B14E3EF5A0_86473463D14843CFB0C7771DD2F35AE4_en_1_227c3f0a91ec4ba29ab66a858726fd37_564_edit\" type=\"text/sitecore\" chromeType=\"field\" scFieldType=\"general link\" class=\"scpm\" kind=\"open\">{\"commands\":[{\"click\":\"chrome:field:editcontrol({command:\\\"webedit:editlink\\\"})\",\"header\":\"Edit link\",\"icon\":\"/temp/iconcache/networkv2/16x16/link_edit.png\",\"disabledIcon\":\"/temp/link_edit_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"Edits the link destination and appearance\",\"type\":\"\"},{\"click\":\"chrome:field:editcontrol({command:\\\"webedit:clearlink\\\"})\",\"header\":\"Clear Link\",\"icon\":\"/temp/iconcache/networkv2/16x16/link_delete.png\",\"disabledIcon\":\"/temp/link_delete_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"Clears The Link\",\"type\":\"\"},{\"click\":\"chrome:field:editcontrol({command:\\\"webedit:followlink\\\"})\",\"header\":\"Follow Link\",\"icon\":\"/temp/iconcache/applications/16x16/arrow_right_blue.png\",\"disabledIcon\":\"/temp/arrow_right_blue_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"Follow Link\",\"type\":\"sticky\"},{\"click\":\"chrome:common:edititem({command:\\\"webedit:open\\\"})\",\"header\":\"Edit the related item\",\"icon\":\"/temp/iconcache/office/16x16/cubes.png\",\"disabledIcon\":\"/temp/cubes_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"Edit the related item in the Content Editor.\",\"type\":\"common\"},{\"click\":\"chrome:rendering:personalize({command:\\\"webedit:personalize\\\"})\",\"header\":\"Personalize\",\"icon\":\"/temp/iconcache/office/16x16/users_family.png\",\"disabledIcon\":\"/temp/users_family_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"Create or edit personalization for this component.\",\"type\":\"sticky\"},{\"click\":\"chrome:rendering:editvariations({command:\\\"webedit:editvariations\\\"})\",\"header\":\"Edit variations\",\"icon\":\"/temp/iconcache/office/16x16/windows.png\",\"disabledIcon\":\"/temp/windows_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"Edit the variations.\",\"type\":\"sticky\"}],\"contextItemUri\":\"sitecore://master/{4252A3F2-57F6-4F29-85F2-C6B14E3EF5A0}?lang=en&ver=1\",\"custom\":{},\"displayName\":\"GenealLink\",\"expandedDisplayName\":null}</code><a class=\"sample-css-class\" title=\"Sample alternate text\" href=\"/demo?sc_site=Sample\">This is description</a><code class=\"scpm\" type=\"text/sitecore\" chromeType=\"field\" kind=\"close\"></code>";
    private const string EditableFirstPart = "<input id='fld_4252A3F257F64F2985F2C6B14E3EF5A0_86473463D14843CFB0C7771DD2F35AE4_en_1_227c3f0a91ec4ba29ab66a858726fd37_564' class='scFieldValue' name='fld_4252A3F257F64F2985F2C6B14E3EF5A0_86473463D14843CFB0C7771DD2F35AE4_en_1_227c3f0a91ec4ba29ab66a858726fd37_564' type='hidden' value=\"&lt;link text=&quot;This is description&quot; anchor=&quot;&quot; linktype=&quot;internal&quot; class=&quot;sample-css-class&quot; title=&quot;Sample alternate text&quot;  querystring=&quot;&quot; id=&quot;{B0BD73D0-7F5D-4272-BFA0-B43AB65069CC}&quot; /&gt;\" /><code id=\"fld_4252A3F257F64F2985F2C6B14E3EF5A0_86473463D14843CFB0C7771DD2F35AE4_en_1_227c3f0a91ec4ba29ab66a858726fd37_564_edit\" type=\"text/sitecore\" chromeType=\"field\" scFieldType=\"general link\" class=\"scpm\" kind=\"open\">{\"commands\":[{\"click\":\"chrome:field:editcontrol({command:\\\"webedit:editlink\\\"})\",\"header\":\"Edit link\",\"icon\":\"/temp/iconcache/networkv2/16x16/link_edit.png\",\"disabledIcon\":\"/temp/link_edit_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"Edits the link destination and appearance\",\"type\":\"\"},{\"click\":\"chrome:field:editcontrol({command:\\\"webedit:clearlink\\\"})\",\"header\":\"Clear Link\",\"icon\":\"/temp/iconcache/networkv2/16x16/link_delete.png\",\"disabledIcon\":\"/temp/link_delete_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"Clears The Link\",\"type\":\"\"},{\"click\":\"chrome:field:editcontrol({command:\\\"webedit:followlink\\\"})\",\"header\":\"Follow Link\",\"icon\":\"/temp/iconcache/applications/16x16/arrow_right_blue.png\",\"disabledIcon\":\"/temp/arrow_right_blue_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"Follow Link\",\"type\":\"sticky\"},{\"click\":\"chrome:common:edititem({command:\\\"webedit:open\\\"})\",\"header\":\"Edit the related item\",\"icon\":\"/temp/iconcache/office/16x16/cubes.png\",\"disabledIcon\":\"/temp/cubes_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"Edit the related item in the Content Editor.\",\"type\":\"common\"},{\"click\":\"chrome:rendering:personalize({command:\\\"webedit:personalize\\\"})\",\"header\":\"Personalize\",\"icon\":\"/temp/iconcache/office/16x16/users_family.png\",\"disabledIcon\":\"/temp/users_family_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"Create or edit personalization for this component.\",\"type\":\"sticky\"},{\"click\":\"chrome:rendering:editvariations({command:\\\"webedit:editvariations\\\"})\",\"header\":\"Edit variations\",\"icon\":\"/temp/iconcache/office/16x16/windows.png\",\"disabledIcon\":\"/temp/windows_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"Edit the variations.\",\"type\":\"sticky\"}],\"contextItemUri\":\"sitecore://master/{4252A3F2-57F6-4F29-85F2-C6B14E3EF5A0}?lang=en&ver=1\",\"custom\":{},\"displayName\":\"GenealLink\",\"expandedDisplayName\":null}</code><a class=\"sample-css-class\" title=\"Sample alternate text\" href=\"/demo?sc_site=Sample\">This is description";
    private const string EditableLastPart = "</a><code class=\"scpm\" type=\"text/sitecore\" chromeType=\"field\" kind=\"close\"></code>";
    private const string Html = "<a class=\"sample-css-class\" href=\"/demo\" title=\"Sample alternate text\">This is description</a>";
    private const string HtmlWithRel = "<a class=\"sample-css-class\" href=\"/demo\" rel=\"noopener noreferrer\" target=\"_blank\" title=\"Sample alternate text\">This is description</a>";
    private const string HtmlWithAnchor = "<a class=\"sample-css-class\" href=\"/demo#anchor\" title=\"Sample alternate text\">This is description</a>";
    private readonly HyperLink _hyperLink = new() { Class = "sample-css-class", Href = "/demo", Target = string.Empty, Text = "This is description", Title = "Sample alternate text" };

    // ReSharper disable once UnusedMember.Global - Used by testing framework
    public static Action<IFixture> AutoSetup => f =>
    {
        TagHelperContext tagHelperContext = new([], new Dictionary<object, object>(), Guid.NewGuid().ToString("N", CultureInfo.InvariantCulture));

        TagHelperOutput tagHelperOutput = new("a", [], (_, _) =>
        {
            DefaultTagHelperContent tagHelperContent = new();
            return Task.FromResult<TagHelperContent>(tagHelperContent);
        });

        f.Register(() => new LinkTagHelper(new EditableChromeRenderer()));

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
        LinkTagHelper sut,
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
    public async Task Process_ScLinkTagWithNullForAttribute_GeneratesEmptyOutput(
        LinkTagHelper sut,
        TagHelperContext tagHelperContext,
        TagHelperOutput tagHelperOutput)
    {
        // Arrange
        tagHelperOutput.TagName = RenderingEngineConstants.SitecoreTagHelpers.LinkHtmlTag;
        sut.For = null!;

        // Act
        await sut.ProcessAsync(tagHelperContext, tagHelperOutput);

        // Assert
        tagHelperOutput.Content.GetContent().Should().Be(string.Empty);
    }

    [Theory]
    [AutoNSubstituteData]
    public void Process_ScTextTagWithValidForAttribute_GeneratesCorrectOutput(
        LinkTagHelper sut,
        TagHelperContext tagHelperContext,
        TagHelperOutput tagHelperOutput)
    {
        // Arrange
        tagHelperOutput.TagName = RenderingEngineConstants.SitecoreTagHelpers.LinkHtmlTag;
        sut.For = GetModelExpression(new HyperLinkField(_hyperLink));

        // Act
        sut.Process(tagHelperContext, tagHelperOutput);

        // Assert
        tagHelperOutput.Content.GetContent().Should().Be(Html);
    }

    [Theory]
    [AutoNSubstituteData]
    public void Process_ScLinkTagWithEmptyValueInForAttribute_GeneratesEmptyOutput(
        LinkTagHelper sut,
        TagHelperContext tagHelperContext,
        TagHelperOutput tagHelperOutput)
    {
        // Arrange
        tagHelperOutput.TagName = RenderingEngineConstants.SitecoreTagHelpers.LinkHtmlTag;
        sut.For = GetModelExpression(new HyperLinkField(new HyperLink()));

        // Act
        sut.Process(tagHelperContext, tagHelperOutput);

        // Assert
        tagHelperOutput.Content.GetContent().Should().Be(string.Empty);
    }

    [Theory]
    [AutoNSubstituteData]
    public void Process_ScLinkTagWithBlankTargetAttribute_AddsRelAttribute(
        LinkTagHelper sut,
        TagHelperContext tagHelperContext,
        TagHelperOutput tagHelperOutput)
    {
        // Arrange
        tagHelperOutput.TagName = RenderingEngineConstants.SitecoreTagHelpers.LinkHtmlTag;
        HyperLinkField testField = new(_hyperLink);
        testField.Value.Target = "_blank";
        sut.For = GetModelExpression(testField);

        // Act
        sut.Process(tagHelperContext, tagHelperOutput);

        // Assert
        tagHelperOutput.Content.GetContent().Should().Be(HtmlWithRel);
    }

    [Theory]
    [AutoNSubstituteData]
    public void Process_ScLinkTagWithAnchor_AddsAnchorToHrefAttribute(
        LinkTagHelper sut,
        TagHelperContext tagHelperContext,
        TagHelperOutput tagHelperOutput)
    {
        // Arrange
        tagHelperOutput.TagName = RenderingEngineConstants.SitecoreTagHelpers.LinkHtmlTag;
        HyperLinkField testField = new(_hyperLink);
        testField.Value.Anchor = "anchor";
        sut.For = GetModelExpression(testField);

        // Act
        sut.Process(tagHelperContext, tagHelperOutput);

        // Assert
        tagHelperOutput.Content.GetContent().Should().Be(HtmlWithAnchor);
    }

    [Theory]
    [AutoNSubstituteData]
    public void Process_ScLinkTagWithEditableFieldAndEditableSetToTrue_GeneratesCorrectOutput(
        LinkTagHelper sut,
        TagHelperContext tagHelperContext,
        TagHelperOutput tagHelperOutput)
    {
        // Arrange
        tagHelperOutput.TagName = RenderingEngineConstants.SitecoreTagHelpers.LinkHtmlTag;
        HyperLinkField testField = new(_hyperLink)
        {
            EditableMarkupFirst = EditableFirstPart,
            EditableMarkupLast = EditableLastPart
        };
        sut.For = GetModelExpression(testField);

        // Act
        sut.Process(tagHelperContext, tagHelperOutput);

        // Assert
        tagHelperOutput.Content.GetContent().Should().Be(Editable);
    }

    [Theory]
    [AutoNSubstituteData]
    public void Process_ScLinkWithEditableFalse_RenderAuthorTextInEE(
        LinkTagHelper sut,
        TagHelperContext tagHelperContext,
        string customLinkText)
    {
        // Arrange
        DefaultTagHelperContent defaultTagHelperContent = new();
        defaultTagHelperContent.Append(customLinkText);
        TagHelperOutput tagHelperOutput = new(RenderingEngineConstants.SitecoreTagHelpers.LinkHtmlTag, [], (_, _) => Task.FromResult<TagHelperContent>(defaultTagHelperContent));

        HyperLinkField testField = new(_hyperLink)
        {
            EditableMarkupFirst = EditableFirstPart,
            EditableMarkupLast = EditableLastPart
        };
        sut.For = GetModelExpression(testField);
        sut.Editable = false;

        // Act
        sut.Process(tagHelperContext, tagHelperOutput);

        // Assert
        tagHelperOutput.Content.GetContent().Should().Contain(customLinkText);
    }

    [Theory]
    [AutoNSubstituteData]
    public void Process_ScLinkWithEditableTrue_RenderCustomTextInEE(
        LinkTagHelper sut,
        TagHelperContext tagHelperContext,
        string customLinkText)
    {
        // Arrange
        DefaultTagHelperContent defaultTagHelperContent = new();
        defaultTagHelperContent.Append(customLinkText);
        TagHelperOutput tagHelperOutput = new(RenderingEngineConstants.SitecoreTagHelpers.LinkHtmlTag, [], (_, _) => Task.FromResult<TagHelperContent>(defaultTagHelperContent));

        HyperLinkField testField = new(_hyperLink)
        {
            EditableMarkupFirst = EditableFirstPart,
            EditableMarkupLast = EditableLastPart
        };
        sut.For = GetModelExpression(testField);
        sut.Editable = true;

        // Act
        sut.Process(tagHelperContext, tagHelperOutput);

        // Assert
        tagHelperOutput.Content.GetContent().Should().Contain(_hyperLink.Text);
    }

    [Theory]
    [AutoNSubstituteData]
    public void Process_ScLinkTagWithEmptyEditableFields_GeneratesCorrectOutput(
        LinkTagHelper sut,
        TagHelperContext tagHelperContext,
        TagHelperOutput tagHelperOutput)
    {
        // Arrange
        tagHelperOutput.TagName = RenderingEngineConstants.SitecoreTagHelpers.LinkHtmlTag;
        HyperLinkField testField = new(_hyperLink)
        {
            EditableMarkupFirst = string.Empty,
            EditableMarkupLast = string.Empty
        };
        sut.For = GetModelExpression(testField);

        // Act
        sut.Process(tagHelperContext, tagHelperOutput);

        // Assert
        tagHelperOutput.Content.GetContent().Should().Be(Html);
    }

    [Theory]
    [AutoNSubstituteData]
    public void Process_ScLinkTagWithEditableFieldAndEditableSetToFalse_GeneratesCorrectOutput(
        LinkTagHelper sut,
        TagHelperContext tagHelperContext,
        TagHelperOutput tagHelperOutput)
    {
        // Arrange
        tagHelperOutput.TagName = RenderingEngineConstants.SitecoreTagHelpers.LinkHtmlTag;
        HyperLinkField testField = new(_hyperLink)
        {
            EditableMarkupFirst = EditableFirstPart,
            EditableMarkupLast = EditableLastPart
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
    public async Task Process_AnchorTagWithNullForAttribute_GeneratesEmptyOutput(
        LinkTagHelper sut,
        TagHelperContext tagHelperContext,
        TagHelperOutput tagHelperOutput)
    {
        // Arrange
        tagHelperOutput.TagName = "a";
        sut.For = null!;

        // Act
        await sut.ProcessAsync(tagHelperContext, tagHelperOutput);

        // Assert
        tagHelperOutput.Content.GetContent().Should().Be(string.Empty);
    }

    [Theory]
    [AutoNSubstituteData]
    public void Process_AnchorTagWithValidForAttribute_GeneratesCorrectOutput(
        LinkTagHelper sut,
        TagHelperContext tagHelperContext,
        TagHelperOutput tagHelperOutput)
    {
        // Arrange
        tagHelperOutput.TagName = "a";
        sut.For = GetModelExpression(new HyperLinkField(_hyperLink));

        // Act
        sut.Process(tagHelperContext, tagHelperOutput);

        // Assert
        tagHelperOutput.Attributes.Should().Contain(a => a.Name == "class");
        tagHelperOutput.Attributes.Should().Contain(a => a.Name == "href");
        tagHelperOutput.Attributes.Should().Contain(a => a.Name == "title");
    }

    [Theory]
    [AutoNSubstituteData]
    public void Process_AnchorTagWithUserAttributes_GeneratesCorrectOutput(
        LinkTagHelper sut,
        TagHelperContext tagHelperContext,
        TagHelperOutput tagHelperOutput)
    {
        // Arrange
        tagHelperOutput.TagName = "a";
        tagHelperOutput.Attributes.Add("class", "test-class");
        sut.For = GetModelExpression(new HyperLinkField(_hyperLink));

        // Act
        sut.Process(tagHelperContext, tagHelperOutput);

        // Assert
        tagHelperOutput.Attributes.Should().Contain(a => a.Name.Equals("class") && a.Value.Equals("test-class"));
    }

    [Theory]
    [AutoNSubstituteData]
    public void Process_AnchorTagWithValidForAttribute_GeneratesOutputWithOuterDivTag(
        RichTextTagHelper sut,
        TagHelperContext tagHelperContext,
        TagHelperOutput tagHelperOutput)
    {
        // Arrange
        tagHelperOutput.TagName = "a";
        sut.For = GetModelExpression(new HyperLinkField(_hyperLink));

        // Act
        sut.Process(tagHelperContext, tagHelperOutput);

        // Assert
        tagHelperOutput.TagName.Should().Be("a");
    }

    [Theory]
    [AutoNSubstituteData]
    public void Process_AnchorTagWithEditableFieldAndEditableSetToTrue_GeneratesCorrectOutput(
        LinkTagHelper sut,
        TagHelperContext tagHelperContext,
        TagHelperOutput tagHelperOutput)
    {
        // Arrange
        tagHelperOutput.TagName = "a";
        HyperLinkField testField = new(_hyperLink)
        {
            EditableMarkupFirst = EditableFirstPart,
            EditableMarkupLast = EditableLastPart
        };
        sut.For = GetModelExpression(testField);

        // Act
        sut.Process(tagHelperContext, tagHelperOutput);

        // Assert
        tagHelperOutput.Content.GetContent().Should().Be(Editable);
    }

    [Theory]
    [AutoNSubstituteData]
    public void Process_AnchorTagWithWithFieldDescription_GeneratesCorrectOutput(
        LinkTagHelper sut,
        TagHelperContext tagHelperContext,
        TagHelperOutput tagHelperOutput)
    {
        // Arrange
        tagHelperOutput.TagName = "a";
        sut.For = GetModelExpression(new HyperLinkField(_hyperLink));

        // Act
        sut.Process(tagHelperContext, tagHelperOutput);

        // Assert
        tagHelperOutput.Content.GetContent().Should().Contain(_hyperLink.Text);
    }

    [Theory]
    [AutoNSubstituteData]
    public void Process_AnchorTagInnerContent_GeneratesCorrectOutput(
        LinkTagHelper sut,
        TagHelperContext tagHelperContext,
        TagHelperOutput tagHelperOutput,
        string testDescription)
    {
        // Arrange
        tagHelperOutput.TagName = "a";
        tagHelperOutput.Content.Append(testDescription);
        sut.For = GetModelExpression(new HyperLinkField(_hyperLink));

        // Act
        sut.Process(tagHelperContext, tagHelperOutput);

        // Assert
        tagHelperOutput.Content.GetContent().Should().Contain(testDescription);
    }

    #region asp-link attribute
    [Theory]
    [AutoNSubstituteData]
    public async Task Process_ScLinkTagWithAspLinkAttributeWithNullForAttribute_GeneratesEmptyOutput(
        LinkTagHelper sut,
        TagHelperContext tagHelperContext,
        TagHelperOutput tagHelperOutput)
    {
        // Arrange
        tagHelperOutput.TagName = RenderingEngineConstants.SitecoreTagHelpers.LinkHtmlTag;
        sut.LinkModel = null;

        // Act
        await sut.ProcessAsync(tagHelperContext, tagHelperOutput);

        // Assert
        tagHelperOutput.Content.GetContent().Should().Be(string.Empty);
    }

    [Theory]
    [AutoNSubstituteData]
    public void Process_ScTextTagWithAspLinkAttributeWithValidForAttribute_GeneratesCorrectOutput(
        LinkTagHelper sut,
        TagHelperContext tagHelperContext,
        TagHelperOutput tagHelperOutput)
    {
        // Arrange
        tagHelperOutput.TagName = RenderingEngineConstants.SitecoreTagHelpers.LinkHtmlTag;
        sut.LinkModel = new HyperLinkField(_hyperLink);

        // Act
        sut.Process(tagHelperContext, tagHelperOutput);

        // Assert
        tagHelperOutput.Content.GetContent().Should().Be(Html);
    }

    [Theory]
    [AutoNSubstituteData]
    public void Process_ScLinkTagWithAspLinkAttributeWithEmptyValueInForAttribute_GeneratesEmptyOutput(
        LinkTagHelper sut,
        TagHelperContext tagHelperContext,
        TagHelperOutput tagHelperOutput)
    {
        // Arrange
        tagHelperOutput.TagName = RenderingEngineConstants.SitecoreTagHelpers.LinkHtmlTag;
        sut.LinkModel = new HyperLinkField(new HyperLink());

        // Act
        sut.Process(tagHelperContext, tagHelperOutput);

        // Assert
        tagHelperOutput.Content.GetContent().Should().Be(string.Empty);
    }

    [Theory]
    [AutoNSubstituteData]
    public void Process_ScLinkTagWithAspLinkAttributeWithBlankTargetAttribute_AddsRelAttribute(
        LinkTagHelper sut,
        TagHelperContext tagHelperContext,
        TagHelperOutput tagHelperOutput)
    {
        // Arrange
        tagHelperOutput.TagName = RenderingEngineConstants.SitecoreTagHelpers.LinkHtmlTag;
        HyperLinkField testField = new(_hyperLink);
        testField.Value.Target = "_blank";
        sut.LinkModel = testField;

        // Act
        sut.Process(tagHelperContext, tagHelperOutput);

        // Assert
        tagHelperOutput.Content.GetContent().Should().Be(HtmlWithRel);
    }

    [Theory]
    [AutoNSubstituteData]
    public void Process_ScLinkTagWithAspLinkAttributeWithEditableFieldAndEditableSetToTrue_GeneratesCorrectOutput(
        LinkTagHelper sut,
        TagHelperContext tagHelperContext,
        TagHelperOutput tagHelperOutput)
    {
        // Arrange
        tagHelperOutput.TagName = RenderingEngineConstants.SitecoreTagHelpers.LinkHtmlTag;
        HyperLinkField testField = new(_hyperLink)
        {
            EditableMarkupFirst = EditableFirstPart,
            EditableMarkupLast = EditableLastPart
        };
        sut.LinkModel = testField;

        // Act
        sut.Process(tagHelperContext, tagHelperOutput);

        // Assert
        tagHelperOutput.Content.GetContent().Should().Be(Editable);
    }

    [Theory]
    [AutoNSubstituteData]
    public void Process_ScLinkWithAspLinkAttributeWithEditableFalse_RenderAuthorTextInEE(
        LinkTagHelper sut,
        TagHelperContext tagHelperContext,
        string customLinkText)
    {
        // Arrange
        DefaultTagHelperContent defaultTagHelperContent = new();
        defaultTagHelperContent.Append(customLinkText);
        TagHelperOutput tagHelperOutput = new(RenderingEngineConstants.SitecoreTagHelpers.LinkHtmlTag, [], (_, _) => Task.FromResult<TagHelperContent>(defaultTagHelperContent));

        HyperLinkField testField = new(_hyperLink)
        {
            EditableMarkupFirst = EditableFirstPart,
            EditableMarkupLast = EditableLastPart
        };
        sut.LinkModel = testField;
        sut.Editable = false;

        // Act
        sut.Process(tagHelperContext, tagHelperOutput);

        // Assert
        tagHelperOutput.Content.GetContent().Should().Contain(customLinkText);
    }

    [Theory]
    [AutoNSubstituteData]
    public void Process_ScLinkWithAspLinkAttributeWithEditableTrue_RenderCustomTextInEE(
        LinkTagHelper sut,
        TagHelperContext tagHelperContext,
        string customLinkText)
    {
        // Arrange
        DefaultTagHelperContent defaultTagHelperContent = new();
        defaultTagHelperContent.Append(customLinkText);
        TagHelperOutput tagHelperOutput = new(RenderingEngineConstants.SitecoreTagHelpers.LinkHtmlTag, [], (_, _) => Task.FromResult<TagHelperContent>(defaultTagHelperContent));

        HyperLinkField testField = new(_hyperLink)
        {
            EditableMarkupFirst = EditableFirstPart,
            EditableMarkupLast = EditableLastPart
        };
        sut.LinkModel = testField;
        sut.Editable = true;

        // Act
        sut.Process(tagHelperContext, tagHelperOutput);

        // Assert
        tagHelperOutput.Content.GetContent().Should().Contain(_hyperLink.Text);
    }

    [Theory]
    [AutoNSubstituteData]
    public void Process_ScLinkTagWithAspLinkAttributeWithEmptyEditableFields_GeneratesCorrectOutput(
        LinkTagHelper sut,
        TagHelperContext tagHelperContext,
        TagHelperOutput tagHelperOutput)
    {
        // Arrange
        tagHelperOutput.TagName = RenderingEngineConstants.SitecoreTagHelpers.LinkHtmlTag;
        HyperLinkField testField = new(_hyperLink)
        {
            EditableMarkupFirst = string.Empty,
            EditableMarkupLast = string.Empty
        };
        sut.LinkModel = testField;

        // Act
        sut.Process(tagHelperContext, tagHelperOutput);

        // Assert
        tagHelperOutput.Content.GetContent().Should().Be(Html);
    }

    [Theory]
    [AutoNSubstituteData]
    public void Process_ScLinkTagWithAspLinkAttributeWithEditableFieldAndEditableSetToFalse_GeneratesCorrectOutput(
        LinkTagHelper sut,
        TagHelperContext tagHelperContext,
        TagHelperOutput tagHelperOutput)
    {
        // Arrange
        tagHelperOutput.TagName = RenderingEngineConstants.SitecoreTagHelpers.LinkHtmlTag;
        HyperLinkField testField = new(_hyperLink)
        {
            EditableMarkupFirst = EditableFirstPart,
            EditableMarkupLast = EditableLastPart
        };
        sut.LinkModel = testField;
        sut.Editable = false;

        // Act
        sut.Process(tagHelperContext, tagHelperOutput);

        // Assert
        tagHelperOutput.Content.GetContent().Should().Be(Html);
    }

    [Theory]
    [AutoNSubstituteData]
    public async Task Process_AnchorTagWithNullAspDateAttribute_GeneratesEmptyOutput(
        LinkTagHelper sut,
        TagHelperContext tagHelperContext,
        TagHelperOutput tagHelperOutput)
    {
        // Arrange
        tagHelperOutput.TagName = "a";
        sut.LinkModel = null;

        // Act
        await sut.ProcessAsync(tagHelperContext, tagHelperOutput);

        // Assert
        tagHelperOutput.Content.GetContent().Should().Be(string.Empty);
    }

    [Theory]
    [AutoNSubstituteData]
    public void Process_AnchorTagWithValidAspLinkAttribute_GeneratesCorrectOutput(
        LinkTagHelper sut,
        TagHelperContext tagHelperContext,
        TagHelperOutput tagHelperOutput)
    {
        // Arrange
        tagHelperOutput.TagName = "a";
        sut.LinkModel = new HyperLinkField(_hyperLink);

        // Act
        sut.Process(tagHelperContext, tagHelperOutput);

        // Assert
        tagHelperOutput.Attributes.Should().Contain(a => a.Name == "class");
        tagHelperOutput.Attributes.Should().Contain(a => a.Name == "href");
        tagHelperOutput.Attributes.Should().Contain(a => a.Name == "title");
    }

    [Theory]
    [AutoNSubstituteData]
    public void Process_AnchorTagWithAspLinkAttributeWithUserAttributes_GeneratesCorrectOutput(
        LinkTagHelper sut,
        TagHelperContext tagHelperContext,
        TagHelperOutput tagHelperOutput)
    {
        // Arrange
        tagHelperOutput.TagName = "a";
        tagHelperOutput.Attributes.Add("class", "test-class");
        sut.LinkModel = new HyperLinkField(_hyperLink);

        // Act
        sut.Process(tagHelperContext, tagHelperOutput);

        // Assert
        tagHelperOutput.Attributes.Should().Contain(a => a.Name.Equals("class") && a.Value.Equals("test-class"));
    }

    [Theory]
    [AutoNSubstituteData]
    public void Process_AnchorTagWithAspLinkAttributeWithEditableFieldAndEditableSetToTrue_GeneratesCorrectOutput(
        LinkTagHelper sut,
        TagHelperContext tagHelperContext,
        TagHelperOutput tagHelperOutput)
    {
        // Arrange
        tagHelperOutput.TagName = "a";
        HyperLinkField testField = new(_hyperLink)
        {
            EditableMarkupFirst = EditableFirstPart,
            EditableMarkupLast = EditableLastPart
        };
        sut.LinkModel = testField;

        // Act
        sut.Process(tagHelperContext, tagHelperOutput);

        // Assert
        tagHelperOutput.Content.GetContent().Should().Be(Editable);
    }

    [Theory]
    [AutoNSubstituteData]
    public void Process_AnchorTagWithAspLinkAttributeWithWithFieldDescription_GeneratesCorrectOutput(
        LinkTagHelper sut,
        TagHelperContext tagHelperContext,
        TagHelperOutput tagHelperOutput)
    {
        // Arrange
        tagHelperOutput.TagName = "a";
        sut.LinkModel = new HyperLinkField(_hyperLink);

        // Act
        sut.Process(tagHelperContext, tagHelperOutput);

        // Assert
        tagHelperOutput.Content.GetContent().Should().Contain(_hyperLink.Text);
    }

    [Theory]
    [AutoNSubstituteData]
    public void Process_AnchorTagWithAspLinkAttributeInnerContent_GeneratesCorrectOutput(
        LinkTagHelper sut,
        TagHelperContext tagHelperContext,
        TagHelperOutput tagHelperOutput,
        string testDescription)
    {
        // Arrange
        tagHelperOutput.TagName = "a";
        tagHelperOutput.Content.Append(testDescription);
        sut.LinkModel = new HyperLinkField(_hyperLink);

        // Act
        sut.Process(tagHelperContext, tagHelperOutput);

        // Assert
        tagHelperOutput.Content.GetContent().Should().Contain(testDescription);
    }

    [Theory]
    [AutoNSubstituteData]
    public void Process_AnchorTagWithAspLinkAttributeWithBlankTargetAttribute_AddsRelAttribute(
        LinkTagHelper sut,
        TagHelperContext tagHelperContext,
        TagHelperOutput tagHelperOutput)
    {
        // Arrange
        tagHelperOutput.TagName = "a";
        HyperLinkField testField = new(_hyperLink);
        testField.Value.Target = "_blank";
        sut.LinkModel = testField;

        // Act
        sut.Process(tagHelperContext, tagHelperOutput);

        // Assert
        tagHelperOutput.Attributes.Should().Contain(a => a.Name.Equals("target") && a.Value.Equals(_hyperLink.Target));
        tagHelperOutput.Attributes.Should().Contain(a => a.Name.Equals("rel") && a.Value.Equals("noopener noreferrer"));
    }

    [Theory]
    [AutoNSubstituteData]
    public void Process_AnchorLinkTagWithAnchor_AddsAnchorToHrefAttribute(
        LinkTagHelper sut,
        TagHelperContext tagHelperContext,
        TagHelperOutput tagHelperOutput)
    {
        // Arrange
        tagHelperOutput.TagName = "a";
        HyperLinkField testField = new(_hyperLink);
        testField.Value.Anchor = "anchor";
        sut.LinkModel = testField;

        // Act
        sut.Process(tagHelperContext, tagHelperOutput);

        // Assert
        tagHelperOutput.Attributes["href"].Value.ToString().Should().EndWith($"#{_hyperLink.Anchor}");
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
        LinkTagHelper sut = new(chromeRenderer);
        EditableChrome openingChrome = Substitute.For<EditableChrome>();
        EditableChrome closingChrome = Substitute.For<EditableChrome>();
        tagHelperOutput.TagName = RenderingEngineConstants.SitecoreTagHelpers.ImageHtmlTag;
        HyperLinkField testField = new(_hyperLink)
        {
            OpeningChrome = openingChrome,
            ClosingChrome = closingChrome
        };
        sut.LinkModel = testField;

        // Act
        sut.Process(tagHelperContext, tagHelperOutput);

        // Assert
        chromeRenderer.Received().Render(openingChrome);
        chromeRenderer.Received().Render(closingChrome);
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