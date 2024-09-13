using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text.Encodings.Web;
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

public class TextFieldTagHelperFixture
{
    private const string TestEditableMarkup = "<input id='fld_585596CA7903500B8DF20357DD6E3FAC_34DDE037197355A0ADDEBBC7BF700D7F_en_1_b6270e9a40b34bf9bfeaafd8999a003b_283' class='scFieldValue' name='fld_585596CA7903500B8DF20357DD6E3FAC_34DDE037197355A0ADDEBBC7BF700D7F_en_1_b6270e9a40b34bf9bfeaafd8999a003b_283' type='hidden' value=\"JSS Styleguide\" /><span class=\"scChromeData\">{\"commands\":[{\"click\":\"chrome:common:edititem({command:\\\"webedit:open\\\"})\",\"header\":\"Edit the related item\",\"icon\":\"/temp/iconcache/office/16x16/cubes.png\",\"disabledIcon\":\"/temp/cubes_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"Edit the related item in the Content Editor.\",\"type\":\"common\"},{\"click\":\"chrome:rendering:personalize({command:\\\"webedit:personalize\\\"})\",\"header\":\"Personalize\",\"icon\":\"/temp/iconcache/office/16x16/users_family.png\",\"disabledIcon\":\"/temp/users_family_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"Create or edit personalization for this component.\",\"type\":\"sticky\"}],\"contextItemUri\":\"sitecore://master/{585596CA-7903-500B-8DF2-0357DD6E3FAC}?lang=en&ver=1\",\"custom\":{},\"displayName\":\"heading\",\"expandedDisplayName\":null}</span><span id=\"fld_585596CA7903500B8DF20357DD6E3FAC_34DDE037197355A0ADDEBBC7BF700D7F_en_1_b6270e9a40b34bf9bfeaafd8999a003b_283_edit\" sc_parameters=\"prevent-line-break=true\" contenteditable=\"true\" class=\"scWebEditInput\" scFieldType=\"single-line text\" scDefaultText=\"[No text in field]\">JSS Styleguide</span>";
    private const string TestText = "This is the test text";
    private const string TestHtml = "<p>This is the test text</p>";
    private static readonly string TestMultilineText = $"<p>This is the test text {Environment.NewLine} with line endings.</p>";

    // ReSharper disable once UnusedMember.Global - Used by testing framework
    [ExcludeFromCodeCoverage]
    public static Action<IFixture> AutoSetup => f =>
    {
        TagHelperContext tagHelperContext = new(
            [],
            new Dictionary<object, object>(),
            Guid.NewGuid().ToString("N", CultureInfo.InvariantCulture));

        TagHelperOutput tagHelperOutput = new("span", [], (_, _) =>
        {
            DefaultTagHelperContent tagHelperContent = new();
            return Task.FromResult<TagHelperContent>(tagHelperContent);
        });

        f.Register(() => new TextFieldTagHelper());

        f.Inject(tagHelperContext);
        f.Inject(tagHelperOutput);
    };

    [Theory]
    [AutoNSubstituteData]
    public void Ctor_InvalidArgs_Throws(GuardClauseAssertion guard)
    {
        guard.VerifyConstructors<TextFieldTagHelper>();
    }

    [Theory]
    [AutoNSubstituteData]
    public void Process_IsGuarded(
        TextFieldTagHelper sut,
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
    public void Process_TextField_ReturnsEncodedHtml(TextFieldTagHelper sut, TagHelperContext tagHelperContext, TagHelperOutput tagHelperOutput)
    {
        // Arrange
        sut.For = GetModelExpression(new TextField(TestHtml));

        // Act
        sut.Process(tagHelperContext, tagHelperOutput);

        // Assert
        tagHelperOutput.Content.GetContent().Should().Be(HtmlEncoder.Default.Encode(TestHtml));
    }

    [Theory]
    [MemberAutoNSubstituteData(nameof(GetModelExpressionTestData), MemberType = typeof(TextFieldTagHelperFixture))]
    public void Process_OnlyTextField_MustBeProcessed(ModelExpression model, string expectedOutput, TextFieldTagHelper sut, TagHelperContext tagHelperContext, TagHelperOutput tagHelperOutput)
    {
        // Arrange
        sut.For = model;

        // Act
        sut.Process(tagHelperContext, tagHelperOutput);

        // Assert
        tagHelperOutput.Content.GetContent().Should().Be(expectedOutput);
    }

    [Theory]
    [AutoNSubstituteData]
    public void Process_TextField_ShouldReplaceLineEndingsByDefault(TextFieldTagHelper sut, TagHelperContext tagHelperContext, TagHelperOutput tagHelperOutput)
    {
        // Arrange
        sut.For = GetModelExpression(new TextField(TestMultilineText));

        // Act
        sut.Process(tagHelperContext, tagHelperOutput);

        // Assert
        tagHelperOutput.Content.GetContent().Should().Be("<p>This is the test text <br /> with line endings.</p>");
    }

    [Theory]
    [AutoNSubstituteData]
    public void Process_TextField_ShouldNotReplaceLineEndingsIfConvertNewLinesIsFalse(TextFieldTagHelper sut, TagHelperContext tagHelperContext, TagHelperOutput tagHelperOutput)
    {
        // Arrange
        sut.ConvertNewLines = false;
        sut.For = GetModelExpression(new TextField(TestMultilineText));

        // Act
        sut.Process(tagHelperContext, tagHelperOutput);

        // Assert
        tagHelperOutput.Content.GetContent().Should().Be(HtmlEncoder.Default.Encode(TestMultilineText));
    }

    [Theory]
    [AutoNSubstituteData]
    public void Process_EditableTextFieldAndEditableSetToTrue_GeneratesCorrectOutput(TextFieldTagHelper sut, TagHelperContext tagHelperContext, TagHelperOutput tagHelperOutput)
    {
        // Arrange
        TextField testField = new(TestText)
        {
            EditableMarkup = TestEditableMarkup
        };
        sut.For = GetModelExpression(testField);

        // Act
        sut.Process(tagHelperContext, tagHelperOutput);

        // Assert
        tagHelperOutput.Content.GetContent().Should().Be(TestEditableMarkup);
    }

    [Theory]
    [AutoNSubstituteData]
    public void Process_EmptyEditableTextFieldAndEditableSetToTrue_GeneratesCorrectOutput(TextFieldTagHelper sut, TagHelperContext tagHelperContext, TagHelperOutput tagHelperOutput)
    {
        // Arrange
        TextField testField = new(TestText)
        {
            EditableMarkup = string.Empty
        };
        sut.For = GetModelExpression(testField);

        // Act
        sut.Process(tagHelperContext, tagHelperOutput);

        // Assert
        tagHelperOutput.Content.GetContent().Should().Be(TestText);
    }

    [Theory]
    [AutoNSubstituteData]
    public void Process_EditableFieldAndEditableSetToFalse_GeneratesCorrectOutput(TextFieldTagHelper sut, TagHelperContext tagHelperContext, TagHelperOutput tagHelperOutput)
    {
        // Arrange
        TextField testField = new(TestText)
        {
            EditableMarkup = TestEditableMarkup
        };
        sut.For = GetModelExpression(testField);
        sut.Editable = false;

        // Act
        sut.Process(tagHelperContext, tagHelperOutput);

        // Assert
        tagHelperOutput.Content.GetContent().Should().Be(TestText);
    }

    private static IEnumerable<object[]> GetModelExpressionTestData()
    {
        yield return [null!, string.Empty];
        yield return [GetModelExpression(new TextField(TestHtml)), HtmlEncoder.Default.Encode(TestHtml)];
        yield return [GetModelExpression(new RichTextField(TestHtml)), string.Empty];
        yield return [GetModelExpression(new DateField(DateTime.UtcNow)), string.Empty];
        yield return [GetModelExpression(new CheckboxField(false)), string.Empty];
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