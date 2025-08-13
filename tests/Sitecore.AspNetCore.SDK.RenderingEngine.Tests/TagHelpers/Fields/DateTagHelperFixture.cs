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
using Sitecore.AspNetCore.SDK.RenderingEngine.Rendering;
using Sitecore.AspNetCore.SDK.RenderingEngine.TagHelpers.Fields;
using Xunit;

namespace Sitecore.AspNetCore.SDK.RenderingEngine.Tests.TagHelpers.Fields;

public class DateTagHelperFixture
{
    private const string Editable = "<input id='fld_EB7384E1232B57679FFE4596EED2BD5A_B13D99E479275906BE0796F42BA10579_en_1_c060b68001fb4d7aacaf7f30bdf78dd4_467' class='scFieldValue' name='fld_EB7384E1232B57679FFE4596EED2BD5A_B13D99E479275906BE0796F42BA10579_en_1_c060b68001fb4d7aacaf7f30bdf78dd4_467' type='hidden' value=\"20180314T150000Z\" /><span class=\"scChromeData\">{\"commands\":[{\"click\":\"chrome:field:editcontrol({command:\\\"webedit: editdatetime\\\"})\",\"header\":\"Show calendar\",\"icon\":\"/temp/iconcache/business/16x16/calendar.png\",\"disabledIcon\":\"/temp/calendar_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"Shows the calendar\",\"type\":\"\"},{\"click\":\"chrome:field:editcontrol({command:\\\"webedit: cleardate\\\"})\",\"header\":\"Clear calender\",\"icon\":\"/temp/iconcache/applications/16x16/delete2.png\",\"disabledIcon\":\"/temp/delete2_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"Clear date and time\",\"type\":\"\"},{\"click\":\"chrome:common:edititem({command:\\\"webedit: open\\\"})\",\"header\":\"Edit the related item\",\"icon\":\"/temp/iconcache/office/16x16/cubes.png\",\"disabledIcon\":\"/temp/cubes_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"Edit the related item in the Content Editor.\",\"type\":\"common\"},{\"click\":\"chrome:rendering:personalize({command:\\\"webedit: personalize\\\"})\",\"header\":\"Personalize\",\"icon\":\"/temp/iconcache/office/16x16/users_family.png\",\"disabledIcon\":\"/temp/users_family_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"Create or edit personalization for this component.\",\"type\":\"sticky\"}],\"contextItemUri\":\"sitecore://master/{EB7384E1-232B-5767-9FFE-4596EED2BD5A}?lang=en&ver=1\",\"custom\":{},\"displayName\":\"dateTime\",\"expandedDisplayName\":null}</span><span scFieldType=\"datetime\" scDefaultText=\"[No text in field]\" class=\"scWebEditInput\" id=\"fld_EB7384E1232B57679FFE4596EED2BD5A_B13D99E479275906BE0796F42BA10579_en_1_c060b68001fb4d7aacaf7f30bdf78dd4_467_edit\">3/14/2018 5:00:00 PM</span>";

    private readonly DateTime _date = DateTime.Parse("2012-05-04T00:00:00Z", CultureInfo.InvariantCulture);

    // ReSharper disable once UnusedMember.Global - Used by testing framework
    [ExcludeFromCodeCoverage]
    public static Action<IFixture> AutoSetup => f =>
    {
        TagHelperContext tagHelperContext = new([], new Dictionary<object, object>(), Guid.NewGuid().ToString("N", CultureInfo.InvariantCulture));

        TagHelperOutput tagHelperOutput = new(string.Empty, [], (_, _) =>
        {
            DefaultTagHelperContent tagHelperContent = new();
            return Task.FromResult<TagHelperContent>(tagHelperContent);
        });

        f.Register(() => new DateTagHelper(new EditableChromeRenderer()));

        f.Inject(tagHelperContext);
        f.Inject(tagHelperOutput);
    };

    [Theory]
    [AutoNSubstituteData]
    public void Ctor_InvalidArgs_Throws(GuardClauseAssertion guard)
    {
        guard.VerifyConstructors<DateTagHelper>();
    }

    [Theory]
    [AutoNSubstituteData]
    public void Process_IsGuarded(
        DateTagHelper sut,
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
    public async Task Process_ScDateTagWithNullForAttribute_GeneratesEmptyOutput(
        DateTagHelper sut,
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
    public void Process_ScDateTagWithValidForAttribute_GeneratesCorrectOutput(
        DateTagHelper sut,
        TagHelperContext tagHelperContext,
        TagHelperOutput tagHelperOutput)
    {
        // Arrange
        tagHelperOutput.TagName = RenderingEngineConstants.SitecoreTagHelpers.DateHtmlTag;
        sut.For = GetModelExpression(new DateField(_date));

        // Act
        sut.Process(tagHelperContext, tagHelperOutput);

        // Assert
        tagHelperOutput.Content.GetContent().Should().Be(_date.ToString(CultureInfo.CurrentCulture));
    }

    [Theory]
    [AutoNSubstituteData]
    public void Process_ScDateTagWithEmptyValueInForAttribute_GeneratesEmptyOutput(
        DateTagHelper sut,
        TagHelperContext tagHelperContext,
        TagHelperOutput tagHelperOutput)
    {
        // Arrange
        tagHelperOutput.TagName = RenderingEngineConstants.SitecoreTagHelpers.DateHtmlTag;
        sut.For = GetModelExpression(new DateField());

        // Act
        sut.Process(tagHelperContext, tagHelperOutput);

        // Assert
        tagHelperOutput.Content.GetContent().Should().Be(string.Empty);
    }

    [Theory]
    [InlineData("en-US")]
    [InlineData("da-DK")]
    [InlineData("uk-UA")]
    public void Process_ScDateTagWithCustomFormat_GeneratesCustomDateFormatOutput(string cultureName)
    {
        // Arrange
        CultureInfo originalCulture = CultureInfo.CurrentCulture;
        CultureInfo originalUiCulture = CultureInfo.CurrentUICulture;
        CultureInfo testCulture = new CultureInfo(cultureName);

        try
        {
            CultureInfo.CurrentCulture = testCulture;
            CultureInfo.CurrentUICulture = testCulture;

            const string dateFormat = "MM/dd/yyyy H:mm";
            DateTagHelper sut = new DateTagHelper(new EditableChromeRenderer());
            TagHelperContext tagHelperContext = new TagHelperContext([], new Dictionary<object, object>(), Guid.NewGuid().ToString("N", CultureInfo.InvariantCulture));
            TagHelperOutput tagHelperOutput = new TagHelperOutput(string.Empty, [], (_, _) =>
            {
                DefaultTagHelperContent tagHelperContent = new();
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

            tagHelperOutput.TagName = RenderingEngineConstants.SitecoreTagHelpers.DateHtmlTag;
            sut.DateFormat = dateFormat;
            sut.For = GetModelExpression(new DateField(_date));

            // Act
            sut.Process(tagHelperContext, tagHelperOutput);

            // Assert - Expect culture-specific formatting based on current culture
            string expected = _date.ToString(dateFormat, testCulture);
            tagHelperOutput.Content.GetContent().Should().Be(expected);
        }
        finally
        {
            CultureInfo.CurrentCulture = originalCulture;
            CultureInfo.CurrentUICulture = originalUiCulture;
        }
    }

    [Theory]
    [AutoNSubstituteData]
    public void Process_ScDateTagWithCustomCulture_GeneratesCustomDateFormatOutput(
        DateTagHelper sut,
        TagHelperContext tagHelperContext,
        TagHelperOutput tagHelperOutput)
    {
        // Arrange
        const string culture = "ua-ua";
        tagHelperOutput.TagName = RenderingEngineConstants.SitecoreTagHelpers.DateHtmlTag;
        sut.Culture = culture;
        sut.For = GetModelExpression(new DateField(_date));

        // Act
        sut.Process(tagHelperContext, tagHelperOutput);

        // Assert
        tagHelperOutput.Content.GetContent().Should().Be(_date.ToString(CultureInfo.CreateSpecificCulture(culture)));
    }

    [Theory]
    [AutoNSubstituteData]
    public void Process_AspForWorksForDateForRandomTags_GeneratesEmptyOutput(
        DateTagHelper sut,
        TagHelperContext tagHelperContext,
        TagHelperOutput tagHelperOutput)
    {
        // Arrange
        tagHelperOutput.TagName = "span";
        sut.For = GetModelExpression(new DateField(_date));

        // Act
        sut.Process(tagHelperContext, tagHelperOutput);

        // Assert
        tagHelperOutput.Content.GetContent().Should().Be(_date.ToString(CultureInfo.CurrentCulture));
    }

    [Theory]
    [AutoNSubstituteData]
    public void Process_ScDateTagWithEditableFieldAndEditableSetToTrue_GeneratesCorrectOutput(
        DateTagHelper sut,
        TagHelperContext tagHelperContext,
        TagHelperOutput tagHelperOutput)
    {
        // Arrange
        tagHelperOutput.TagName = RenderingEngineConstants.SitecoreTagHelpers.DateHtmlTag;
        DateField testField = new(_date)
        {
            EditableMarkup = Editable
        };
        sut.DateModel = testField;

        // Act
        sut.Process(tagHelperContext, tagHelperOutput);

        // Assert
        tagHelperOutput.Content.GetContent().Should().Be(Editable);
    }

    #region asp-date attribute
    [Theory]
    [AutoNSubstituteData]
    public async Task Process_ScDateTagWithAspDataAttributeWithNullForAttribute_GeneratesEmptyOutput(
        DateTagHelper sut,
        TagHelperContext tagHelperContext,
        TagHelperOutput tagHelperOutput)
    {
        // Arrange
        tagHelperOutput.TagName = RenderingEngineConstants.SitecoreTagHelpers.DateHtmlTag;
        sut.DateModel = null;

        // Act
        await sut.ProcessAsync(tagHelperContext, tagHelperOutput);

        // Assert
        tagHelperOutput.Content.GetContent().Should().Be(string.Empty);
    }

    [Theory]
    [AutoNSubstituteData]
    public void Process_ScDateTagWithAspDataAttributeWithValidForAttribute_GeneratesCorrectOutput(
        DateTagHelper sut,
        TagHelperContext tagHelperContext,
        TagHelperOutput tagHelperOutput)
    {
        // Arrange
        tagHelperOutput.TagName = RenderingEngineConstants.SitecoreTagHelpers.DateHtmlTag;
        sut.DateModel = new DateField(_date);

        // Act
        sut.Process(tagHelperContext, tagHelperOutput);

        // Assert
        tagHelperOutput.Content.GetContent().Should().Be(_date.ToString(CultureInfo.CurrentCulture));
    }

    [Theory]
    [AutoNSubstituteData]
    public void Process_ScDateTagWithAspDataAttributeWithEmptyValueInForAttribute_GeneratesEmptyOutput(
        DateTagHelper sut,
        TagHelperContext tagHelperContext,
        TagHelperOutput tagHelperOutput)
    {
        // Arrange
        tagHelperOutput.TagName = RenderingEngineConstants.SitecoreTagHelpers.DateHtmlTag;
        sut.DateModel = new DateField();

        // Act
        sut.Process(tagHelperContext, tagHelperOutput);

        // Assert
        tagHelperOutput.Content.GetContent().Should().Be(string.Empty);
    }

    [Theory]
    [InlineData("en-US")]
    [InlineData("da-DK")]
    [InlineData("uk-UA")]
    public void Process_ScDateTagWithAspDataAttributeWithCustomFormat_GeneratesCustomDateFormatOutput(string cultureName)
    {
        // Arrange
        CultureInfo originalCulture = CultureInfo.CurrentCulture;
        CultureInfo originalUiCulture = CultureInfo.CurrentUICulture;
        CultureInfo testCulture = new CultureInfo(cultureName);

        try
        {
            CultureInfo.CurrentCulture = testCulture;
            CultureInfo.CurrentUICulture = testCulture;

            string dateFormat = "MM/dd/yyyy H:mm";
            DateTagHelper sut = new DateTagHelper(new EditableChromeRenderer());
            TagHelperContext tagHelperContext = new TagHelperContext([], new Dictionary<object, object>(), Guid.NewGuid().ToString("N", CultureInfo.InvariantCulture));
            TagHelperOutput tagHelperOutput = new TagHelperOutput(string.Empty, [], (_, _) =>
            {
                DefaultTagHelperContent tagHelperContent = new();
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

            tagHelperOutput.TagName = RenderingEngineConstants.SitecoreTagHelpers.DateHtmlTag;
            sut.DateFormat = dateFormat;
            sut.DateModel = new DateField(_date);

            // Act
            sut.Process(tagHelperContext, tagHelperOutput);

            // Assert - Expect culture-specific formatting based on current culture
            string expected = _date.ToString(dateFormat, testCulture);
            tagHelperOutput.Content.GetContent().Should().Be(expected);
        }
        finally
        {
            CultureInfo.CurrentCulture = originalCulture;
            CultureInfo.CurrentUICulture = originalUiCulture;
        }
    }

    [Theory]
    [AutoNSubstituteData]
    public void Process_ScDateWithAspDataAttributeTagWithCustomCulture_GeneratesCustomDateFormatOutput(
        DateTagHelper sut,
        TagHelperContext tagHelperContext,
        TagHelperOutput tagHelperOutput)
    {
        // Arrange
        string culture = "ua-ua";
        tagHelperOutput.TagName = RenderingEngineConstants.SitecoreTagHelpers.DateHtmlTag;
        sut.Culture = culture;
        sut.DateModel = new DateField(_date);

        // Act
        sut.Process(tagHelperContext, tagHelperOutput);

        // Assert
        tagHelperOutput.Content.GetContent().Should().Be(_date.ToString(CultureInfo.CreateSpecificCulture(culture)));
    }

    [Theory]
    [AutoNSubstituteData]
    public void Process_AspDateWorksForDateForRandomTags_GeneratesEmptyOutput(
        DateTagHelper sut,
        TagHelperContext tagHelperContext,
        TagHelperOutput tagHelperOutput)
    {
        // Arrange
        tagHelperOutput.TagName = "span";
        sut.DateModel = new DateField(_date);

        // Act
        sut.Process(tagHelperContext, tagHelperOutput);

        // Assert
        tagHelperOutput.Content.GetContent().Should().Be(_date.ToString(CultureInfo.CurrentCulture));
    }

    [Theory]
    [AutoNSubstituteData]
    public void Process_ScDateWithAspDataAttributeTagWithEditableFieldAndEditableSetToTrue_GeneratesCorrectOutput(
        DateTagHelper sut,
        TagHelperContext tagHelperContext,
        TagHelperOutput tagHelperOutput)
    {
        // Arrange
        tagHelperOutput.TagName = RenderingEngineConstants.SitecoreTagHelpers.DateHtmlTag;
        DateField testField = new(_date)
        {
            EditableMarkup = Editable
        };
        sut.DateModel = testField;

        // Act
        sut.Process(tagHelperContext, tagHelperOutput);

        // Assert
        tagHelperOutput.Content.GetContent().Should().Be(Editable);
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
        DateTagHelper sut = new(chromeRenderer);
        EditableChrome openingChrome = Substitute.For<EditableChrome>();
        EditableChrome closingChrome = Substitute.For<EditableChrome>();
        tagHelperOutput.TagName = RenderingEngineConstants.SitecoreTagHelpers.DateHtmlTag;
        DateField testField = new(_date)
        {
            EditableMarkup = Editable,
            OpeningChrome = openingChrome,
            ClosingChrome = closingChrome
        };
        sut.DateModel = testField;

        // Act
        sut.Process(tagHelperContext, tagHelperOutput);

        // Assert
        tagHelperOutput.Content.GetContent().Should().Contain(Editable);
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