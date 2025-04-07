using System.Diagnostics.CodeAnalysis;
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
using Sitecore.AspNetCore.SDK.LayoutService.Client.Response;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Response.Model;
using Sitecore.AspNetCore.SDK.Pages.TagHelpers;
using Sitecore.AspNetCore.SDK.RenderingEngine.Extensions;
using Sitecore.AspNetCore.SDK.RenderingEngine.Interfaces;
using Sitecore.AspNetCore.SDK.RenderingEngine.Rendering;
using Xunit;

namespace Sitecore.AspNetCore.SDK.Pages.Tests.TagHelpers;

public class EditingScriptsTagHelperFixture
{
    [ExcludeFromCodeCoverage]
    public static Action<IFixture> AutoSetup => f =>
    {
        ViewContext viewContext = new()
        {
            HttpContext = Substitute.For<HttpContext>()
        };

        FeatureCollection features = new();
        features[typeof(ISitecoreRenderingContext)] = Substitute.For<ISitecoreRenderingContext>();

        viewContext.HttpContext.Features.Returns(features);
        f.Inject(viewContext);

        TagHelperOutput tagHelperOutput = new("test", [], (_, _) =>
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
        guard.VerifyConstructors<EditingScriptsTagHelper>();
    }

    [Theory]
    [AutoNSubstituteData]
    public async Task ProcessAsync_NoSitecoreContenxt_ExceptionIsThrown(EditingScriptsTagHelper sut, TagHelperContext tagHelperContext, TagHelperOutput tagHelperOutput)
    {
        // Arrange
        ViewContext viewContext = new()
        {
            HttpContext = Substitute.For<HttpContext>()
        };
        FeatureCollection features = new();
        viewContext.HttpContext.Features.Returns(features);
        sut.ViewContext = viewContext;

        // Act
        Func<Task> act = async () => await sut.ProcessAsync(tagHelperContext, tagHelperOutput);

        // Assert
        await act.Should().ThrowAsync<NullReferenceException>();
    }

    [Theory]
    [AutoNSubstituteData]
    public async Task ProcessAsync_NotInEditingMode_OutputIsEmpty(EditingScriptsTagHelper sut, TagHelperContext tagHelperContext, TagHelperOutput tagHelperOutput, ViewContext viewContext)
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
        tagHelperOutput.Content.GetContent().Should().BeEmpty();
    }

    [Theory]
    [AutoNSubstituteData]
    public async Task ProcessAsync_IsInEditingMode_ClientScriptsAreOutput(EditingScriptsTagHelper sut, TagHelperContext tagHelperContext, TagHelperOutput tagHelperOutput, ViewContext viewContext)
    {
        // arrange
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
                    },
                    ContextRawData = "{\"clientScripts\":[\"/assets/js/script1.js\", \"/assets/js/script2.js\", \"/assets/js/script3.js\"]}"
                }
            }
        };

        viewContext.HttpContext.SetSitecoreRenderingContext(context);
        sut.ViewContext = viewContext;

        // act
        await sut.ProcessAsync(tagHelperContext, tagHelperOutput);

        // assert
        tagHelperOutput.Content.GetContent().Should().Contain("<script type=\"text/javascript\" src=\"/assets/js/script1.js\"></script>");
        tagHelperOutput.Content.GetContent().Should().Contain("<script type=\"text/javascript\" src=\"/assets/js/script2.js\"></script>");
        tagHelperOutput.Content.GetContent().Should().Contain("<script type=\"text/javascript\" src=\"/assets/js/script3.js\"></script>");
    }

    [Theory]
    [AutoNSubstituteData]
    public async Task ProcessAsync_IsInEditingMode_ItemDataScriptTagIsOutput(EditingScriptsTagHelper sut, TagHelperContext tagHelperContext, TagHelperOutput tagHelperOutput, ViewContext viewContext)
    {
        // arrange
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
                    },
                    ContextRawData = "{\"clientData\":{\"hrz-canvas-state\": {\"itemId\": \"itemId-1234\",\"itemVersion\": 1,\"siteName\": \"siteName_1234\",\"language\": \"en\",\"deviceId\": \"device_1234\",\"pageMode\": \"NORMAL\",\"variant\": \"variant_1234\"}}}"
                }
            }
        };

        viewContext.HttpContext.SetSitecoreRenderingContext(context);
        sut.ViewContext = viewContext;

        // act
        await sut.ProcessAsync(tagHelperContext, tagHelperOutput);

        // assert
        string expectedItemDataScriptTag = $@"
                            <script id=""hrz-canvas-state"" type=""application/json"">
                                {{
                                    ""itemId"":""itemId-1234"",
                                    ""itemVersion"":1,
                                    ""siteName"":""siteName_1234"",
                                    ""language"":""en"",
                                    ""deviceId"":""device_1234"",
                                    ""pageMode"":""NORMAL"",
                                    ""variant"":""variant_1234""
                                }}
                            </script>
                        ";
        tagHelperOutput.Content.GetContent().Should().Contain(expectedItemDataScriptTag);
    }

    [Theory]
    [AutoNSubstituteData]
    public async Task ProcessAsync_IsInEditingMode_CanvasVerificationTokenIsOutput(EditingScriptsTagHelper sut, TagHelperContext tagHelperContext, TagHelperOutput tagHelperOutput, ViewContext viewContext)
    {
        // arrange
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
                    },
                    ContextRawData = "{\"clientData\":{\"hrz-canvas-verification-token\":\"token_1234\"}}"
                }
            }
        };

        viewContext.HttpContext.SetSitecoreRenderingContext(context);
        sut.ViewContext = viewContext;

        // act
        await sut.ProcessAsync(tagHelperContext, tagHelperOutput);

        // assert
        tagHelperOutput.Content.GetContent().Should().Contain("<script id=\"hrz-canvas-verification-token\" type=\"application/json\">token_1234</script>");
    }
}