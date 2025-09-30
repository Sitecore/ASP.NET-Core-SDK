using System.Net;
using AwesomeAssertions;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Sitecore.AspNetCore.SDK.AutoFixture.Mocks;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Extensions;
using Sitecore.AspNetCore.SDK.RenderingEngine.Extensions;
using Sitecore.AspNetCore.SDK.TestData;
using Xunit;

// ReSharper disable StringLiteralTypo
namespace Sitecore.AspNetCore.SDK.RenderingEngine.Integration.Tests.Fixtures.TagHelpers;

public class ImageFieldTagHelperFixture : IClassFixture<TestWebApplicationFactory<TestWebApplicationProgram>>, IDisposable
{
    private readonly MockHttpMessageHandler _mockClientHandler = new();
    private readonly Uri _layoutServiceUri = new("http://layout.service");
    private readonly WebApplicationFactory<TestWebApplicationProgram> _factory;

    public ImageFieldTagHelperFixture(TestWebApplicationFactory<TestWebApplicationProgram> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                services
                    .AddSitecoreLayoutService()
                    .AddHttpHandler("mock", _ => new HttpClient(_mockClientHandler) { BaseAddress = _layoutServiceUri })
                    .AsDefaultHandler();

                services.AddSitecoreRenderingEngine(options =>
                {
                    options
                        .AddModelBoundView<ComponentModels.ComponentWithImages>("Component-With-Images", "ComponentWithImages")
                        .AddViewComponent("Component-1", "Component1")
                        .AddModelBoundView<ComponentModels.Component2>("Component-2", "Component2")
                        .AddDefaultComponentRenderer();
                });

                services.AddControllersWithViews();
            });

            builder.Configure(app =>
            {
                app.UseRouting();
                app.UseSitecoreRenderingEngine();
                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapDefaultControllerRoute();
                });
            });
        });

        // Accessing _factory.Server forces the TestServer to start. The variable is unused; this is intentional.
        _ = _factory.Server;
    }

    [Fact]
    public async Task ImgTagHelper_GeneratedProperImageWithCustomAttributes()
    {
        // Arrange
        _mockClientHandler.Responses.Push(new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(Serializer.Serialize(CannedResponses.PageWithPreview))
        });

        HttpClient client = _factory.CreateClient();

        // Act
        string response = await client.GetStringAsync(new Uri("/", UriKind.Relative));

        HtmlDocument doc = new();
        doc.LoadHtml(response);
        HtmlNode? sectionNode = doc.DocumentNode.ChildNodes.First(n => n.HasClass("component-with-images"));

        // Assert
        // check scenario that ImageTagHelper render proper image tag with custom attributes.
        sectionNode.ChildNodes[5].OuterHtml.Should().Contain(TestConstants.SecondImageTestValue);
    }

    [Fact]
    public async Task ImgTagHelper_GeneratesImageTags()
    {
        // Arrange
        _mockClientHandler.Responses.Push(new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(Serializer.Serialize(CannedResponses.PageWithPreview))
        });

        HttpClient client = _factory.CreateClient();

        // Act
        string response = await client.GetStringAsync(new Uri("/", UriKind.Relative));

        HtmlDocument doc = new();
        doc.LoadHtml(response);
        HtmlNode? sectionNode = doc.DocumentNode.ChildNodes.First(n => n.HasClass("component-with-images"));

        // Assert
        // check that there is proper number of 'img' tags generated.
        sectionNode.ChildNodes.Count(n => n.Name.Equals("img", StringComparison.OrdinalIgnoreCase)).Should().Be(2);
    }

    [Fact]
    public async Task ImgTagHelper_GeneratedProperHtmlWithoutTagName()
    {
        // Arrange
        _mockClientHandler.Responses.Push(new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(Serializer.Serialize(CannedResponses.PageWithPreview))
        });

        HttpClient client = _factory.CreateClient();

        // Act
        string response = await client.GetStringAsync(new Uri("/", UriKind.Relative));

        HtmlDocument doc = new();
        doc.LoadHtml(response);
        HtmlNode? sectionNode = doc.DocumentNode.ChildNodes.First(n => n.HasClass("component-with-images"));

        // Assert
        // check that link will contain user provided link text.
        sectionNode.ChildNodes[1].OuterHtml.Should().Contain(TestConstants.ImageFieldValue);
    }

    [Fact]
    public async Task ImgTagHelper_GeneratesProperImageUrlIncludingImageParams()
    {
        // Arrange
        _mockClientHandler.Responses.Push(new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(Serializer.Serialize(CannedResponses.PageWithPreview))
        });

        HttpClient client = _factory.CreateClient();

        // Act
        string response = await client.GetStringAsync(new Uri("/", UriKind.Relative));

        HtmlDocument doc = new();
        doc.LoadHtml(response);
        HtmlNode? sectionNode = doc.DocumentNode.ChildNodes.First(n => n.HasClass("component-with-images"));
        HtmlNode? lastImage = sectionNode.ChildNodes.Last(n => n.Name.Equals("img", StringComparison.OrdinalIgnoreCase));

        // Assert
        // check that image url contains mw and mh parameters
        lastImage.Attributes.Should().Contain(a => a.Name == "src");
        lastImage.Attributes["src"].Value.Should().Contain("mw=100&amp;mh=50");
    }

    [Fact]
    public async Task ImgTagHelper_GeneratesProperEditableImageMarkupWithCustomProperties()
    {
        // Arrange
        _mockClientHandler.Responses.Push(new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(Serializer.Serialize(CannedResponses.EditablePage))
        });

        HttpClient client = _factory.CreateClient();

        // Act
        string response = await client.GetStringAsync(new Uri("/", UriKind.Relative));

        HtmlDocument doc = new();
        doc.LoadHtml(response);
        HtmlNode? sectionNode = doc.DocumentNode.ChildNodes.First(n => n.HasClass("component-1")).ChildNodes.First(n => n.HasClass("component-2"));

        // Assert
        // check that editable markup contains all custom params
        sectionNode.InnerHtml.Should().Contain("height=\"50\"");
        sectionNode.InnerHtml.Should().Contain("width=\"94\"");
        sectionNode.InnerHtml.Should().Contain("class=\"image1\"");
        sectionNode.InnerHtml.Should().Contain("alt=\"customAlt\"");
        sectionNode.InnerHtml.Should().Contain("src=\"/sitecore/shell/-/jssmedia/styleguide/data/media/img/sc_logo.png?mw=100&mh=50\"");
    }

    public void Dispose()
    {
        _mockClientHandler.Dispose();
        _factory.Dispose();
        GC.SuppressFinalize(this);
    }
}