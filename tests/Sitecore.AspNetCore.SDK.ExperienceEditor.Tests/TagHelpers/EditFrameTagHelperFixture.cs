using AutoFixture;
using AutoFixture.Idioms;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;
using NSubstitute;
using Sitecore.AspNetCore.SDK.AutoFixture.Attributes;
using Sitecore.AspNetCore.SDK.AutoFixture.Extensions;
using Sitecore.AspNetCore.SDK.ExperienceEditor.TagHelpers;
using Sitecore.AspNetCore.SDK.ExperienceEditor.TagHelpers.Model;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Response;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Response.Model;
using Sitecore.AspNetCore.SDK.RenderingEngine.Extensions;
using Sitecore.AspNetCore.SDK.RenderingEngine.Rendering;
using Xunit;

namespace Sitecore.AspNetCore.SDK.ExperienceEditor.Tests.TagHelpers;

public class EditFrameTagHelperFixture
{
    private const string OriginalContent = "<span>Content for Edit Frame</span>";

    private static IChromeDataBuilder _chromeDataBuilder = null!;

    private static IChromeDataSerializer _chromeDataSerializer = null!;

    // ReSharper disable once UnusedMember.Global - Used by testing framework
    public static Action<IFixture> AutoSetup => f =>
    {
        _chromeDataBuilder = f.Freeze<IChromeDataBuilder>();
        _chromeDataSerializer = f.Freeze<IChromeDataSerializer>();

        // Configure the View Context - required as property on PlaceholderTagHelper
        ViewContext viewContext = new()
        {
            HttpContext = Substitute.For<HttpContext>()
        };
        FeatureCollection features = new();

        viewContext.HttpContext.Features.Returns(features);
        f.Inject(viewContext);

        // Configure the TagHelperOutput
        TagHelperOutput tagHelperOutput = new("div", [], (result, encoder) =>
        {
            DefaultTagHelperContent tagHelperContent = new();
            _ = tagHelperContent.SetHtmlContent(OriginalContent);
            return Task.FromResult<TagHelperContent>(tagHelperContent);
        });

        f.Inject(tagHelperOutput);
    };

    [Theory]
    [AutoNSubstituteData]
    public void Ctor_InvalidArgs_Throws(GuardClauseAssertion guard)
    {
        guard.VerifyConstructors<EditFrameTagHelper>();
    }

    [Theory]
    [AutoNSubstituteData]
    public async Task ProcessAsync_PageInNormalMode_OutputContentWithoutChanges(
        EditFrameTagHelper sut,
        ViewContext viewContext,
        TagHelperContext tagHelperContext,
        TagHelperOutput tagHelperOutput)
    {
        // Arrange
        SitecoreRenderingContext context = new()
        {
            Response = new SitecoreLayoutResponse([])
            {
                Content = new SitecoreLayoutResponseContent
                {
                    Sitecore = new SitecoreData
                    {
                        Context = new Context
                        {
                            IsEditing = false
                        }
                    }
                }
            }
        };

        viewContext.HttpContext.SetSitecoreRenderingContext(context);
        sut.ViewContext = viewContext;

        // Act
        await sut.ProcessAsync(tagHelperContext, tagHelperOutput);

        // Assert
        tagHelperOutput.TagName.Should().BeEmpty();
        tagHelperOutput.Content.GetContent().Should().Be(string.Empty);
    }

    [Theory]
    [AutoNSubstituteData]
    public async Task ProcessAsync_ViewContextIsNull_ThrowsNullReferenceException(
        EditFrameTagHelper sut,
        TagHelperContext tagHelperContext,
        TagHelperOutput tagHelperOutput)
    {
        // Arrange
        sut.ViewContext = null;
        Func<Task> act =
            () => sut.ProcessAsync(tagHelperContext, tagHelperOutput);

        // Act & Assert
        await act.Should().ThrowAsync<NullReferenceException>().WithMessage("ViewContext parameter cannot be null.");
    }

    [Theory]
    [AutoNSubstituteData]
    public async Task ProcessAsync_PageInEditModeAndWithoutAttributes_OutputContainsEditFrameMarkup(
        EditFrameTagHelper sut,
        ViewContext viewContext,
        TagHelperContext tagHelperContext,
        TagHelperOutput tagHelperOutput)
    {
        // Arrange
        const string chromeDataPart = "Hello, it is Chrome Data";
        _chromeDataSerializer.Serialize(Arg.Any<Dictionary<string, object?>>()).Returns(chromeDataPart);
        SitecoreRenderingContext context = new()
        {
            Response = new SitecoreLayoutResponse([])
            {
                Content = new SitecoreLayoutResponseContent
                {
                    Sitecore = new SitecoreData
                    {
                        Context = new Context
                        {
                            IsEditing = true
                        }
                    }
                }
            }
        };

        viewContext.HttpContext.SetSitecoreRenderingContext(context);
        sut.ViewContext = viewContext;
        sut.Source = null;
        sut.Tooltip = null;
        sut.Parameters = null;
        sut.Buttons = null;
        sut.CssClass = null;
        sut.Title = null;

        // Act
        await sut.ProcessAsync(tagHelperContext, tagHelperOutput);

        // Assert
        tagHelperOutput.Content.GetContent().Should().Be($"<div class=\"scLooseFrameZone\"><span class=\"scChromeData\">{chromeDataPart}</span>{OriginalContent}</div>");
    }

    [Theory]
    [AutoNSubstituteData]
    public async Task ProcessAsync_PageInEditModeAndWithSourceAttribute_OutputContainsEditFrameMarkup(
        EditFrameTagHelper sut,
        ViewContext viewContext,
        TagHelperContext tagHelperContext,
        TagHelperOutput tagHelperOutput)
    {
        // Arrange
        EditFrameDataSource source = new()
        {
            DatabaseName = "master",
            Language = "en",
            ItemId = Guid.NewGuid().ToString(),
        };
        const string chromeDataPart = "Hello, it is Chrome Data";
        _chromeDataSerializer.Serialize(Arg.Any<Dictionary<string, object?>>()).Returns(chromeDataPart);
        SitecoreRenderingContext context = new()
        {
            Response = new SitecoreLayoutResponse([])
            {
                Content = new SitecoreLayoutResponseContent
                {
                    Sitecore = new SitecoreData
                    {
                        Context = new Context
                        {
                            IsEditing = true
                        }
                    }
                }
            }
        };

        viewContext.HttpContext.SetSitecoreRenderingContext(context);
        sut.ViewContext = viewContext;
        sut.Source = source;
        sut.Tooltip = null;
        sut.Parameters = null;
        sut.Buttons = null;
        sut.CssClass = null;
        sut.Title = null;

        // Act
        await sut.ProcessAsync(tagHelperContext, tagHelperOutput);

        // Assert
        tagHelperOutput.Content.GetContent().Should().Be($"<div class=\"scLooseFrameZone\" sc_item=\"sitecore://{source.DatabaseName}/{source.ItemId}?lang={source.Language}\"><span class=\"scChromeData\">{chromeDataPart}</span>{OriginalContent}</div>");
    }

    [Theory]
    [AutoNSubstituteData]
    public async Task ProcessAsync_PageInEditModeAndWithButtonsAttribute_CommandBuilderShouldBeCalled(
        EditFrameTagHelper sut,
        ViewContext viewContext,
        TagHelperContext tagHelperContext,
        TagHelperOutput tagHelperOutput)
    {
        // Arrange
        List<EditButtonBase> buttons =
        [
            new WebEditButton(),
            new DividerEditButton()
        ];
        const string chromeDataPart = "Hello, it is Chrome Data";
        _chromeDataSerializer.Serialize(Arg.Any<Dictionary<string, object?>>()).Returns(chromeDataPart);
        SitecoreRenderingContext context = new()
        {
            Response = new SitecoreLayoutResponse([])
            {
                Content = new SitecoreLayoutResponseContent
                {
                    Sitecore = new SitecoreData
                    {
                        Context = new Context
                        {
                            IsEditing = true
                        }
                    }
                }
            }
        };

        viewContext.HttpContext.SetSitecoreRenderingContext(context);
        sut.ViewContext = viewContext;
        sut.Source = null;
        sut.Tooltip = null;
        sut.Parameters = null;
        sut.Buttons = buttons;
        sut.CssClass = null;
        sut.Title = null;

        // Act
        await sut.ProcessAsync(tagHelperContext, tagHelperOutput);

        // Assert
        foreach (EditButtonBase button in buttons)
        {
            _chromeDataBuilder.Received(1).MapButtonToCommand(button, null, null);
        }
    }
}