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
using Sitecore.AspNetCore.SDK.RenderingEngine.TagHelpers.Fields;
using Xunit;

namespace Sitecore.AspNetCore.SDK.RenderingEngine.Tests.TagHelpers.Fields;

public class NumberTagHelperFixture
{
    private const double Number1 = 9.99;

    private const double Number2 = 5.40;

    private const string Editable = "<input id='fld_30B7EF86921458E890726B0CEFE157CD_E495286A0C745FC3BC7D849BB5AA087B_en_1_a514562dbc4c4b5b91a88f1c8033a1b4_304' class='scFieldValue' name='fld_30B7EF86921458E890726B0CEFE157CD_E495286A0C745FC3BC7D849BB5AA087B_en_1_a514562dbc4c4b5b91a88f1c8033a1b4_304' type='hidden' value=\"9.99\" /><span class=\"scChromeData\">{\"commands\":[{\"click\":\"chrome:field:editcontrol({command:\\\"webedit: editnumber\\\"})\",\"header\":\"Edit number\",\"icon\":\"/temp/iconcache/wordprocessing/16x16/word_count.png\",\"disabledIcon\":\"/temp/word_count_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"Edit number\",\"type\":\"\"},{\"click\":\"chrome:common:edititem({command:\\\"webedit: open\\\"})\",\"header\":\"Edit the related item\",\"icon\":\"/temp/iconcache/office/16x16/cubes.png\",\"disabledIcon\":\"/temp/cubes_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"Edit the related item in the Content Editor.\",\"type\":\"common\"},{\"click\":\"chrome:rendering:personalize({command:\\\"webedit: personalize\\\"})\",\"header\":\"Personalize\",\"icon\":\"/temp/iconcache/office/16x16/users_family.png\",\"disabledIcon\":\"/temp/users_family_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"Create or edit personalization for this component.\",\"type\":\"sticky\"},{\"click\":\"chrome:rendering:editvariations({command:\\\"webedit: editvariations\\\"})\",\"header\":\"Edit variations\",\"icon\":\"/temp/iconcache/office/16x16/windows.png\",\"disabledIcon\":\"/temp/windows_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"Edit the variations.\",\"type\":\"sticky\"}],\"contextItemUri\":\"sitecore://master/{30B7EF86-9214-58E8-9072-6B0CEFE157CD}?lang=en&ver=1\",\"custom\":{},\"displayName\":\"sample\",\"expandedDisplayName\":null}</span><span scFieldType=\"number\" scDefaultText=\"[No text in field]\" contenteditable=\"true\" class=\"scWebEditInput\" id=\"fld_30B7EF86921458E890726B0CEFE157CD_E495286A0C745FC3BC7D849BB5AA087B_en_1_a514562dbc4c4b5b91a88f1c8033a1b4_304_edit\">9.99</span>";

    // ReSharper disable once UnusedMember.Global - Used by testing framework
    public static Action<IFixture> AutoSetup => f =>
    {
        TagHelperContext tagHelperContext = new([], new Dictionary<object, object>(), Guid.NewGuid().ToString("N", CultureInfo.InvariantCulture));

        TagHelperOutput tagHelperOutput = new(string.Empty, [], (_, _) =>
        {
            DefaultTagHelperContent tagHelperContent = new();
            return Task.FromResult<TagHelperContent>(tagHelperContent);
        });

        f.Register(() => new NumberTagHelper());

        f.Inject(tagHelperContext);
        f.Inject(tagHelperOutput);
    };

    [Theory]
    [AutoNSubstituteData]
    public void Ctor_InvalidArgs_Throws(GuardClauseAssertion guard)
    {
        guard.VerifyConstructors<NumberTagHelper>();
    }

    [Theory]
    [AutoNSubstituteData]
    public void Process_IsGuarded(
        NumberTagHelper sut,
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
    public async Task Process_ScNumberTagWithNullForAttribute_GeneratesEmptyOutput(
        NumberTagHelper sut,
        TagHelperContext tagHelperContext,
        TagHelperOutput tagHelperOutput)
    {
        // Arrange
        tagHelperOutput.TagName = RenderingEngineConstants.SitecoreTagHelpers.DateHtmlTag;
        sut.For = null!;

        // Act
        await sut.ProcessAsync(tagHelperContext, tagHelperOutput);

        // Assert
        tagHelperOutput.Content.GetContent().Should().Be(string.Empty);
    }

    [Theory]
    [AutoNSubstituteData]
    public void Process_ScNumberTagWithValidForAttribute_GeneratesCorrectOutput(
        NumberTagHelper sut,
        TagHelperContext tagHelperContext,
        TagHelperOutput tagHelperOutput)
    {
        // Arrange
        tagHelperOutput.TagName = RenderingEngineConstants.SitecoreTagHelpers.NumberHtmlTag;
        sut.For = GetModelExpression(new NumberField(Number1));

        // Act
        sut.Process(tagHelperContext, tagHelperOutput);

        // Assert
        tagHelperOutput.Content.GetContent().Should().Be(Number1.ToString(CultureInfo.CurrentCulture));
    }

    [Theory]
    [AutoNSubstituteData]
    public void Process_ScNumberTagWithCustomFormat_GeneratesCustomDateFormatOutput(
        NumberTagHelper sut,
        TagHelperContext tagHelperContext,
        TagHelperOutput tagHelperOutput)
    {
        // Arrange
        tagHelperOutput.TagName = RenderingEngineConstants.SitecoreTagHelpers.NumberHtmlTag;
        sut.NumberFormat = "C";
        sut.For = GetModelExpression(new NumberField(Number1));

        // Act
        sut.Process(tagHelperContext, tagHelperOutput);

        // Assert
        tagHelperOutput.Content.GetContent().Should().Be(Number1.ToString(sut.NumberFormat, CultureInfo.CurrentCulture));
    }

    [Theory]
    [AutoNSubstituteData]
    public void Process_AspForWorksForNumberForRandomTags_GeneratesCorrectOutput(
        NumberTagHelper sut,
        TagHelperContext tagHelperContext,
        TagHelperOutput tagHelperOutput)
    {
        // Arrange
        tagHelperOutput.TagName = "span";
        sut.For = GetModelExpression(new NumberField(Number2));

        // Act
        sut.Process(tagHelperContext, tagHelperOutput);

        // Assert
        tagHelperOutput.Content.GetContent().Should().Be(Number2.ToString(CultureInfo.CurrentCulture));
    }

    [Theory]
    [AutoNSubstituteData]
    public void Process_ScNumberTagWithEditableFieldAndEditableSetToTrue_GeneratesCorrectOutput(
        NumberTagHelper sut,
        TagHelperContext tagHelperContext,
        TagHelperOutput tagHelperOutput)
    {
        // Arrange
        tagHelperOutput.TagName = RenderingEngineConstants.SitecoreTagHelpers.NumberHtmlTag;
        NumberField testField = new(Number2)
        {
            EditableMarkup = Editable
        };
        sut.For = GetModelExpression(testField);

        // Act
        sut.Process(tagHelperContext, tagHelperOutput);

        // Assert
        tagHelperOutput.Content.GetContent().Should().Be(Editable);
    }

    [Theory]
    [AutoNSubstituteData]
    public void Process_ScNumberTagWithCultureInfo_GeneratesCorrectOutput(
        NumberTagHelper sut,
        TagHelperContext tagHelperContext,
        TagHelperOutput tagHelperOutput)
    {
        // Arrange
        tagHelperOutput.TagName = RenderingEngineConstants.SitecoreTagHelpers.NumberHtmlTag;
        sut.NumberFormat = "C";
        sut.Culture = "ua-ua";
        sut.For = GetModelExpression(new NumberField(Number1));

        // Act
        sut.Process(tagHelperContext, tagHelperOutput);

        // Assert
        tagHelperOutput.Content.GetContent().Should().Be(Number1.ToString(sut.NumberFormat, CultureInfo.CreateSpecificCulture(sut.Culture)));
    }

    #region asp-number attribute
    [Theory]
    [AutoNSubstituteData]
    public async Task Process_ScNumberTagWithAspNumberAttributeWithNullForAttribute_GeneratesEmptyOutput(
        NumberTagHelper sut,
        TagHelperContext tagHelperContext,
        TagHelperOutput tagHelperOutput)
    {
        // Arrange
        tagHelperOutput.TagName = RenderingEngineConstants.SitecoreTagHelpers.DateHtmlTag;
        sut.NumberModel = null;

        // Act
        await sut.ProcessAsync(tagHelperContext, tagHelperOutput);

        // Assert
        tagHelperOutput.Content.GetContent().Should().Be(string.Empty);
    }

    [Theory]
    [AutoNSubstituteData]
    public void Process_ScNumberTagWithAspNumberAttributeWithValidForAttribute_GeneratesCorrectOutput(
        NumberTagHelper sut,
        TagHelperContext tagHelperContext,
        TagHelperOutput tagHelperOutput)
    {
        // Arrange
        tagHelperOutput.TagName = RenderingEngineConstants.SitecoreTagHelpers.NumberHtmlTag;
        sut.NumberModel = new NumberField(Number1);

        // Act
        sut.Process(tagHelperContext, tagHelperOutput);

        // Assert
        tagHelperOutput.Content.GetContent().Should().Be(Number1.ToString(CultureInfo.CurrentCulture));
    }

    [Theory]
    [AutoNSubstituteData]
    public void Process_ScNumberTagWithAspNumberAttributeWithCustomFormat_GeneratesCustomDateFormatOutput(
        NumberTagHelper sut,
        TagHelperContext tagHelperContext,
        TagHelperOutput tagHelperOutput)
    {
        // Arrange
        tagHelperOutput.TagName = RenderingEngineConstants.SitecoreTagHelpers.NumberHtmlTag;
        sut.NumberFormat = "C";
        sut.NumberModel = new NumberField(Number1);

        // Act
        sut.Process(tagHelperContext, tagHelperOutput);

        // Assert
        tagHelperOutput.Content.GetContent().Should().Be(Number1.ToString(sut.NumberFormat, CultureInfo.CurrentCulture));
    }

    [Theory]
    [AutoNSubstituteData]
    public void Process_AspNumberWorksForNumberForRandomTags_GeneratesCorrectOutput(
        NumberTagHelper sut,
        TagHelperContext tagHelperContext,
        TagHelperOutput tagHelperOutput)
    {
        // Arrange
        tagHelperOutput.TagName = "span";
        sut.NumberModel = new NumberField(Number2);

        // Act
        sut.Process(tagHelperContext, tagHelperOutput);

        // Assert
        tagHelperOutput.Content.GetContent().Should().Be(Number2.ToString(CultureInfo.CurrentCulture));
    }

    [Theory]
    [AutoNSubstituteData]
    public void Process_ScNumberTagWithAspNumberAttributeWithEditableFieldAndEditableSetToTrue_GeneratesCorrectOutput(
        NumberTagHelper sut,
        TagHelperContext tagHelperContext,
        TagHelperOutput tagHelperOutput)
    {
        // Arrange
        tagHelperOutput.TagName = RenderingEngineConstants.SitecoreTagHelpers.NumberHtmlTag;
        NumberField testField = new(Number2)
        {
            EditableMarkup = Editable
        };
        sut.NumberModel = testField;

        // Act
        sut.Process(tagHelperContext, tagHelperOutput);

        // Assert
        tagHelperOutput.Content.GetContent().Should().Be(Editable);
    }

    [Theory]
    [AutoNSubstituteData]
    public void Process_ScNumberWithAspNumberAttributeTagWithCultureInfo_GeneratesCorrectOutput(
        NumberTagHelper sut,
        TagHelperContext tagHelperContext,
        TagHelperOutput tagHelperOutput)
    {
        // Arrange
        tagHelperOutput.TagName = RenderingEngineConstants.SitecoreTagHelpers.NumberHtmlTag;
        sut.NumberFormat = "C";
        sut.Culture = "ua-ua";
        sut.NumberModel = new NumberField(Number1);

        // Act
        sut.Process(tagHelperContext, tagHelperOutput);

        // Assert
        tagHelperOutput.Content.GetContent().Should().Be(Number1.ToString(sut.NumberFormat, CultureInfo.CreateSpecificCulture(sut.Culture)));
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