using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using AutoFixture;
using AutoFixture.Idioms;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Options;
using NSubstitute;
using Sitecore.AspNetCore.SDK.AutoFixture.Attributes;
using Sitecore.AspNetCore.SDK.AutoFixture.Extensions;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Response;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Response.Model;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Response.Model.Fields;
using Sitecore.AspNetCore.SDK.RenderingEngine.Configuration;
using Sitecore.AspNetCore.SDK.RenderingEngine.Extensions;
using Sitecore.AspNetCore.SDK.RenderingEngine.Interfaces;
using Sitecore.AspNetCore.SDK.RenderingEngine.Rendering;
using Sitecore.AspNetCore.SDK.RenderingEngine.TagHelpers;
using Sitecore.AspNetCore.SDK.RenderingEngine.Tests.Mocks;
using Xunit;

// ReSharper disable StringLiteralTypo
namespace Sitecore.AspNetCore.SDK.RenderingEngine.Tests.TagHelpers;

public class PlaceholderTagHelperFixture
{
    private const string TestComponentName = "TestComponent";

    private const string PlaceHolderWithNoComponentsName = "empty-placeholder";

    private const string PlaceHolderWithComponentsName = "populated-placeholder";

    private static IComponentRendererFactory _componentRendererFactory = null!;

    // ReSharper disable once UnusedMember.Global - Used by testing framework
    [ExcludeFromCodeCoverage]
    public static Action<IFixture> AutoSetup => f =>
    {
        // prevent nested recursion issues
        f.Behaviors.Add(new OmitOnRecursionBehavior());

        Component component = new()
        {
            Name = TestComponentName
        };
        f.Inject(component);

        TestComponentRenderer componentRenderer = new();
        _componentRendererFactory = f.Freeze<IComponentRendererFactory>();
        _componentRendererFactory
            .GetRenderer(Arg.Any<Component>())
            .Returns(componentRenderer);

        f.Inject<IEditableChromeRenderer>(new EditableChromeRenderer());

        // Configure the options - Required in ctor of PlaceholderTagHelper
        IOptions<RenderingEngineOptions>? options = f.Freeze<IOptions<RenderingEngineOptions>>();
        RenderingEngineOptions innerOptions = new()
        {
            RendererRegistry = new SortedList<int, ComponentRendererDescriptor>
            {
                {
                    0, new ComponentRendererDescriptor(name => name == TestComponentName, _ => componentRenderer, TestComponentName)
                }
            }
        };
        options.Value.Returns(innerOptions);

        // Configure the View Context - required as property on PlaceholderTagHelper
        ViewContext viewContext = new()
        {
            HttpContext = Substitute.For<HttpContext>()
        };

        FeatureCollection features = new();

        viewContext.HttpContext.Features.Returns(features);
        f.Inject(viewContext);

        // Configure the TagHelperOutput - Required when Placeholder render logic is invoked
        TagHelperOutput tagHelperOutput = new("placeholder", [], (_, _) =>
        {
            DefaultTagHelperContent tagHelperContent = new();
            tagHelperContent.SetHtmlContent(string.Empty);
            return Task.FromResult<TagHelperContent>(tagHelperContent);
        });

        f.Inject(tagHelperOutput);
    };

    [Theory]
    [AutoNSubstituteData]
    public void Ctor_InvalidArgs_Throws(GuardClauseAssertion guard)
    {
        guard.VerifyConstructors<PlaceholderTagHelper>();
    }

    [Theory]
    [AutoNSubstituteData]
    public async Task ProcessAsync_PlaceholderNameIsNotSet_OutputContainsHtmlComment(
        PlaceholderTagHelper sut,
        TagHelperContext tagHelperContext,
        TagHelperOutput tagHelperOutput)
    {
        // Arrange
        sut.Name = null;

        // Act
        await sut.ProcessAsync(tagHelperContext, tagHelperOutput);

        // Assert
        tagHelperOutput.Content.GetContent().Should().Be("<!-- Placeholder name was not defined. -->");
    }

    [Theory]
    [AutoNSubstituteData]
    public async Task ProcessAsync_PlaceholderNameNotInLayoutServiceResponse_OutputContainsHtmlComment(
        PlaceholderTagHelper sut,
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
                        Context = new Context(),
                        Route = new Route
                        {
                            Placeholders =
                            {
                                [PlaceHolderWithComponentsName] =
                                [
                                    new Component
                                    {
                                        Id = Guid.NewGuid().ToString("N", CultureInfo.InvariantCulture),
                                        Name = string.Empty,
                                        Fields =
                                        {
                                            ["TestField1"] = new TextField("TestFieldValue1")
                                        }
                                    }
                                ]
                            }
                        }
                    }
                }
            }
        };

        viewContext.HttpContext.SetSitecoreRenderingContext(context);
        sut.Name = "unknown";
        sut.ViewContext = viewContext;

        // Act
        await sut.ProcessAsync(tagHelperContext, tagHelperOutput);

        // Assert
        tagHelperOutput.Content.GetContent().Should().Be("<!-- Placeholder was not defined. -->");
    }

    [Theory]
    [AutoNSubstituteData]
    public async Task ProcessAsync_RenderingContextIsNull_ThrowsNullReferenceException(
        PlaceholderTagHelper sut,
        ViewContext viewContext,
        TagHelperContext tagHelperContext,
        TagHelperOutput tagHelperOutput)
    {
        // Arrange
        viewContext.HttpContext.Features[typeof(ISitecoreRenderingContext)] = null;
        Func<Task> act =
            () => sut.ProcessAsync(tagHelperContext, tagHelperOutput);

        // Act & Assert
        await act.Should().ThrowAsync<NullReferenceException>().WithMessage("SitecoreLayout cannot be null.");
    }

    [Theory]
    [AutoNSubstituteData]
    public async Task ProcessAsync_ViewContextIsNull_ThrowsNullReferenceException(
        PlaceholderTagHelper sut,
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
    public async Task ProcessAsync_PlaceholderNameInLayoutServiceResponseAndPlaceholderIsEmpty_OutputContainsHtmlComment(
        PlaceholderTagHelper sut,
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
                        Context = new Context(),
                        Route = new Route
                        {
                            Placeholders =
                            {
                                [PlaceHolderWithNoComponentsName] = []
                            }
                        }
                    }
                }
            }
        };

        viewContext.HttpContext.SetSitecoreRenderingContext(context);
        sut.Name = PlaceHolderWithNoComponentsName;
        sut.ViewContext = viewContext;

        // Act
        await sut.ProcessAsync(tagHelperContext, tagHelperOutput);

        // Assert
        tagHelperOutput.Content.GetContent().Should().Be($"<!-- Placeholder '{PlaceHolderWithNoComponentsName}' was empty. -->");
    }

    [Theory]
    [AutoNSubstituteData]
    public async Task ProcessAsync_PlaceholderNameInLayoutServiceResponseAndPlaceholderIsNotEmpty_OutputContainsComponentHtml(
        PlaceholderTagHelper sut,
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
                        Context = new Context(),
                        Route = new Route
                        {
                            Placeholders =
                            {
                                [PlaceHolderWithComponentsName] =
                                [
                                    new Component
                                    {
                                        Id = Guid.NewGuid().ToString("N", CultureInfo.InvariantCulture),
                                        Name = TestComponentName,
                                        Fields =
                                        {
                                            ["TestField1"] = new TextField("TestFieldValue1")
                                        }
                                    }
                                ]
                            }
                        }
                    }
                }
            }
        };

        viewContext.HttpContext.SetSitecoreRenderingContext(context);
        sut.Name = PlaceHolderWithComponentsName;
        sut.ViewContext = viewContext;

        // Act
        await sut.ProcessAsync(tagHelperContext, tagHelperOutput);

        // Assert
        tagHelperOutput.Content.GetContent().Should().Be(TestComponentRenderer.HtmlContent);
    }

    [Theory]
    [AutoNSubstituteData]
    public async Task ProcessAsync_PlaceholderContainsComponentWithoutName_OutputContainsComponentHtml(
        PlaceholderTagHelper sut,
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
                        Context = new Context(),
                        Route = new Route
                        {
                            Placeholders =
                            {
                                [PlaceHolderWithComponentsName] =
                                [
                                    new Component
                                    {
                                        Id = Guid.NewGuid().ToString("N", CultureInfo.InvariantCulture),
                                        Name = string.Empty,
                                        Fields =
                                        {
                                            ["TestField1"] = new TextField("TestFieldValue1")
                                        }
                                    }
                                ]
                            }
                        }
                    }
                }
            }
        };

        viewContext.HttpContext.SetSitecoreRenderingContext(context);
        sut.Name = PlaceHolderWithComponentsName;
        sut.ViewContext = viewContext;

        // Act
        await sut.ProcessAsync(tagHelperContext, tagHelperOutput);

        // Assert
        tagHelperOutput.Content.GetContent().Should().Be(TestComponentRenderer.HtmlContent);
    }

    [Theory]
    [AutoNSubstituteData]
    public async Task ProcessAsync_PlaceholderContainsComponentAndChrome_OutputContainsComponentAndChromeHtml(
        PlaceholderTagHelper sut,
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
                        Context = new Context(),
                        Route = new Route
                        {
                            Placeholders =
                            {
                                [PlaceHolderWithComponentsName] =
                                [
                                    new EditableChrome
                                    {
                                        Content = TestComponentRenderer.ChromeContent,
                                        Attributes = new Dictionary<string, string>
                                        {
                                            {
                                                "type", "text/sitecore"
                                            },
                                            {
                                                "chrometype", "rendering"
                                            },
                                            {
                                                "kind", "open"
                                            }
                                        }
                                    },

                                    new Component
                                    {
                                        Id = Guid.NewGuid().ToString("N", CultureInfo.InvariantCulture),
                                        Name = string.Empty,
                                        Fields =
                                        {
                                            ["TestField1"] = new TextField("TestFieldValue1")
                                        }
                                    },

                                    new EditableChrome
                                    {
                                        Attributes = new Dictionary<string, string>
                                        {
                                            {
                                                "type", "text/sitecore"
                                            },
                                            {
                                                "chrometype", "rendering"
                                            },
                                            {
                                                "kind", "close"
                                            }
                                        }
                                    }
                                ]
                            }
                        }
                    }
                }
            }
        };

        viewContext.HttpContext.SetSitecoreRenderingContext(context);
        sut.Name = PlaceHolderWithComponentsName;
        sut.ViewContext = viewContext;

        // Act
        await sut.ProcessAsync(tagHelperContext, tagHelperOutput);

        // Assert
        tagHelperOutput.Content.GetContent().Should().Be(
            $"<code type='text/sitecore' chrometype='rendering' kind='open'>{TestComponentRenderer.ChromeContent}</code>{TestComponentRenderer.HtmlContent}<code type='text/sitecore' chrometype='rendering' kind='close'></code>");
    }

    [Theory]
    [AutoNSubstituteData]
    public async Task ProcessAsync_PlaceholderContainsUnknownPlaceholderFeature_OutputIsEmpty(
        PlaceholderTagHelper sut,
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
                        Context = new Context(),
                        Route = new Route
                        {
                            Placeholders =
                            {
                                [PlaceHolderWithComponentsName] =
                                [
                                    new TestPlaceholderFeature
                                    {
                                        Content = TestComponentRenderer.HtmlContent
                                    }
                                ]
                            }
                        }
                    }
                }
            }
        };

        viewContext.HttpContext.SetSitecoreRenderingContext(context);
        sut.Name = PlaceHolderWithComponentsName;
        sut.ViewContext = viewContext;

        // Act
        await sut.ProcessAsync(tagHelperContext, tagHelperOutput);

        // Assert
        tagHelperOutput.Content.GetContent().Should().BeEmpty();
    }

    [Theory]
    [AutoNSubstituteData]
    public async Task ProcessAsync_PlaceholderContainsUnknownPlaceholderFeature_IsInEditingMode_OutputIsEditingWrapperTag(
        PlaceholderTagHelper sut,
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
                        Context = new Context { IsEditing = true },
                        Route = new Route
                        {
                            Placeholders =
                            {
                                [PlaceHolderWithComponentsName] =
                                [
                                    new TestPlaceholderFeature
                                    {
                                        Content = TestComponentRenderer.HtmlContent
                                    }
                                ]
                            }
                        }
                    }
                }
            }
        };

        viewContext.HttpContext.SetSitecoreRenderingContext(context);
        sut.Name = PlaceHolderWithComponentsName;
        sut.ViewContext = viewContext;

        // Act
        await sut.ProcessAsync(tagHelperContext, tagHelperOutput);

        // Assert
        tagHelperOutput.Content.GetContent().Should().Be("<div class=\"sc-empty-placeholder\"></div>");
    }

    [Theory]
    [AutoNSubstituteData]
    public async Task ProcessAsync_PlaceholderNameInLayoutServiceResponseAndPlaceholderIsNotEmpty_ContextComponentDoNotChange(
        PlaceholderTagHelper sut,
        ViewContext viewContext,
        TagHelperContext tagHelperContext,
        TagHelperOutput tagHelperOutput)
    {
        // Arrange
        Component component = new();
        SitecoreRenderingContext context = new()
        {
            Component = component,
            Response = new SitecoreLayoutResponse([])
            {
                Content = new SitecoreLayoutResponseContent
                {
                    Sitecore = new SitecoreData
                    {
                        Context = new Context(),
                        Route = new Route
                        {
                            Placeholders =
                            {
                                [PlaceHolderWithComponentsName] =
                                [
                                    new Component
                                    {
                                        Id = Guid.NewGuid().ToString("N", CultureInfo.InvariantCulture),
                                        Name = TestComponentName,
                                        Fields =
                                        {
                                            ["TestField1"] = new TextField("TestFieldValue1")
                                        }
                                    }
                                ]
                            }
                        }
                    }
                }
            }
        };

        viewContext.HttpContext.SetSitecoreRenderingContext(context);
        sut.Name = PlaceHolderWithComponentsName;
        sut.ViewContext = viewContext;

        // Act
        await sut.ProcessAsync(tagHelperContext, tagHelperOutput);

        // Assert
        context.Component.Should().Be(component);
    }

    [Theory]
    [AutoNSubstituteData]
    public async Task ProcessAsync_PlaceholderNameInLayoutServiceResponseAndPlaceholderIsNotEmpty_GetRendererWithRightComponent(
        PlaceholderTagHelper sut,
        ViewContext viewContext,
        TagHelperContext tagHelperContext,
        TagHelperOutput tagHelperOutput)
    {
        // Arrange
        Component component = new();
        Component placeholderFeature = new()
        {
            Id = Guid.NewGuid().ToString("N", CultureInfo.InvariantCulture),
            Name = TestComponentName,
            Fields =
            {
                ["TestField1"] = new TextField("TestFieldValue1")
            }
        };

        SitecoreRenderingContext context = new()
        {
            Component = component,
            Response = new SitecoreLayoutResponse([])
            {
                Content = new SitecoreLayoutResponseContent
                {
                    Sitecore = new SitecoreData
                    {
                        Context = new Context(),
                        Route = new Route
                        {
                            Placeholders =
                            {
                                [PlaceHolderWithComponentsName] = [placeholderFeature]
                            }
                        }
                    }
                }
            }
        };

        viewContext.HttpContext.SetSitecoreRenderingContext(context);
        sut.Name = PlaceHolderWithComponentsName;
        sut.ViewContext = viewContext;

        // Act
        await sut.ProcessAsync(tagHelperContext, tagHelperOutput);

        // Assert
        _componentRendererFactory.Received().GetRenderer(placeholderFeature);
    }
}