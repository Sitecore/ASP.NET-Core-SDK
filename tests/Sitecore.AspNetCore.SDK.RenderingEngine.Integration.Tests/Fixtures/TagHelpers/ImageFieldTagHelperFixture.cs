using System.Net;
using FluentAssertions;
using HtmlAgilityPack;
using Microsoft.AspNetCore.TestHost;
using Sitecore.AspNetCore.SDK.AutoFixture.Mocks;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Extensions;
using Sitecore.AspNetCore.SDK.RenderingEngine.Extensions;
using Sitecore.AspNetCore.SDK.TestData;
using Xunit;

// ReSharper disable StringLiteralTypo
namespace Sitecore.AspNetCore.SDK.RenderingEngine.Integration.Tests.Fixtures.TagHelpers;

public class ImageFieldTagHelperFixture : IDisposable
{
    private readonly TestServer _server;
    private readonly MockHttpMessageHandler _mockClientHandler;
    private readonly Uri _layoutServiceUri = new("http://layout.service");

    public ImageFieldTagHelperFixture()
    {
        TestServerBuilder testHostBuilder = new();
        _mockClientHandler = new MockHttpMessageHandler();
        testHostBuilder
            .ConfigureServices(builder =>
            {
                builder
                    .AddSitecoreLayoutService()
                    .AddHttpHandler("mock", _ => new HttpClient(_mockClientHandler) { BaseAddress = _layoutServiceUri })
                    .AsDefaultHandler();
                builder.AddSitecoreRenderingEngine(options =>
                {
                    options
                        .AddModelBoundView<ComponentModels.ComponentWithImages>("Component-With-Images", "ComponentWithImages")
                        .AddViewComponent("Component-1", "Component1")
                        .AddModelBoundView<ComponentModels.Component2>("Component-2", "Component2")
                        .AddDefaultComponentRenderer();
                });
            })
            .Configure(app =>
            {
                app.UseRouting();
                app.UseSitecoreRenderingEngine();
                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapDefaultControllerRoute();
                });
            });

        _server = testHostBuilder.BuildServer(new Uri("http://localhost"));
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

        HttpClient client = _server.CreateClient();

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

        HttpClient client = _server.CreateClient();

        // Act
        string response = await client.GetStringAsync(new Uri("/", UriKind.Relative));

        HtmlDocument doc = new();
        doc.LoadHtml(response);
        HtmlNode? sectionNode = doc.DocumentNode.ChildNodes.First(n => n.HasClass("component-with-images"));

        // Assert
        // check that there is proper number of 'img' tags generated.
        sectionNode.ChildNodes.Count(n => n.Name.Equals("img", StringComparison.OrdinalIgnoreCase)).Should().Be(4);
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

        HttpClient client = _server.CreateClient();

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

        HttpClient client = _server.CreateClient();

        // Act
        string response = await client.GetStringAsync(new Uri("/", UriKind.Relative));

        HtmlDocument doc = new();
        doc.LoadHtml(response);
        HtmlNode? sectionNode = doc.DocumentNode.ChildNodes.First(n => n.HasClass("component-with-images"));
        HtmlNode? secondImage = sectionNode.Descendants("img").ElementAtOrDefault(1);

        // Assert
        // check that image url contains mw and mh parameters
        secondImage.Should().NotBeNull();
        secondImage?.Attributes.Should().Contain(a => a.Name == "src");
        secondImage?.Attributes["src"].Value.Should().Contain("mw=100&amp;mh=50");
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

        HttpClient client = _server.CreateClient();

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

    [Fact]
    public async Task ImgTagHelper_GeneratesProperSrcSetAttribute()
    {
        // Arrange
        _mockClientHandler.Responses.Push(new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(Serializer.Serialize(CannedResponses.PageWithPreview))
        });

        HttpClient client = _server.CreateClient();

        // Act
        string response = await client.GetStringAsync(new Uri("/", UriKind.Relative));
        HtmlDocument doc = new HtmlDocument();
        doc.LoadHtml(response);
        HtmlNode? sectionNode = doc.DocumentNode.ChildNodes.First(n => n.HasClass("component-with-images"));

        // Assert
        // Third image for <sc-img /> (index 2)
        HtmlNode? thirdImg = sectionNode.Descendants("img").ElementAt(2);
        thirdImg.Should().NotBeNull();
        thirdImg.Attributes.Should().Contain(a => a.Name == "srcset");
        thirdImg.Attributes.Should().Contain(a => a.Name == "sizes");
        thirdImg.Attributes["srcset"].Value.Should().Contain("site/third.png?mw=400 400w");
        thirdImg.Attributes["srcset"].Value.Should().Contain("site/third.png?mw=200 200w");
        thirdImg.Attributes["sizes"].Value.Should().Be("(min-width: 400px) 400px, 200px");

        // Fourth image for <img /> (index 3)
        HtmlNode? fourthImg = sectionNode.Descendants("img").ElementAt(3);
        fourthImg.Should().NotBeNull();
        fourthImg.Attributes.Should().Contain(a => a.Name == "srcset");
        fourthImg.Attributes.Should().Contain(a => a.Name == "sizes");
        fourthImg.Attributes["srcset"].Value.Should().Contain("site/fourth.png?mw=800 800w");
        fourthImg.Attributes["srcset"].Value.Should().Contain("site/fourth.png?mw=400 400w");
        fourthImg.Attributes["sizes"].Value.Should().Be("(min-width: 800px) 800px, 400px");
    }

    [Fact]
    public async Task ImgTagHelper_SrcSetAttributeContainsCorrectUrlsAndSizes()
    {
        // Arrange
        _mockClientHandler.Responses.Push(new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(Serializer.Serialize(CannedResponses.PageWithPreview))
        });

        HttpClient client = _server.CreateClient();

        // Act
        string response = await client.GetStringAsync(new Uri("/", UriKind.Relative));
        HtmlDocument doc = new HtmlDocument();
        doc.LoadHtml(response);
        HtmlNode? sectionNode = doc.DocumentNode.ChildNodes.First(n => n.HasClass("component-with-images"));

        // Assert
        // Third image for <sc-img /> (index 2)
        HtmlNode? thirdImg = sectionNode.Descendants("img").ElementAt(2);
        thirdImg.Should().NotBeNull();
        thirdImg.Attributes["srcset"].Value.Should().Contain("site/third.png?mw=400 400w");
        thirdImg.Attributes["srcset"].Value.Should().Contain("site/third.png?mw=200 200w");
        thirdImg.Attributes["sizes"].Value.Should().Be("(min-width: 400px) 400px, 200px");

        // Fourth image for <img /> (index 3)
        HtmlNode? fourthImg = sectionNode.Descendants("img").ElementAt(3);
        fourthImg.Should().NotBeNull();
        fourthImg.Attributes["srcset"].Value.Should().Contain("site/fourth.png?mw=800 800w");
        fourthImg.Attributes["srcset"].Value.Should().Contain("site/fourth.png?mw=400 400w");
        fourthImg.Attributes["sizes"].Value.Should().Be("(min-width: 800px) 800px, 400px");
    }

    public void Dispose()
    {
        _mockClientHandler.Dispose();
        _server.Dispose();
        GC.SuppressFinalize(this);
    }
}