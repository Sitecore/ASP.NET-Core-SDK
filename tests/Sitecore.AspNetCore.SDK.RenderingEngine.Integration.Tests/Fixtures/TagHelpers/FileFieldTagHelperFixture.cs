using System.Net;
using AwesomeAssertions;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc.Testing;
using Sitecore.AspNetCore.SDK.AutoFixture.Mocks;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Extensions;
using Sitecore.AspNetCore.SDK.RenderingEngine.Extensions;
using Sitecore.AspNetCore.SDK.TestData;
using Xunit;

namespace Sitecore.AspNetCore.SDK.RenderingEngine.Integration.Tests.Fixtures.TagHelpers;

public class FileFieldTagHelperFixture(TestWebApplicationFactory<TestWebApplicationProgram> factory) : IClassFixture<TestWebApplicationFactory<TestWebApplicationProgram>>, IDisposable
{
    private readonly MockHttpMessageHandler _mockClientHandler = new();
    private readonly Uri _layoutServiceUri = new("http://layout.service");

    [Fact]
    public async Task FileTagHelper_RendersAttributeFromModel()
    {
        // Arrange
        _mockClientHandler.Responses.Push(new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(Serializer.Serialize(CannedResponses.WithNestedPlaceholder))
        });

        HttpClient client = BuildFileFieldTagHelperWebApplicationFactory().CreateClient();

        // Act
        string response = await client.GetStringAsync(new Uri("/", UriKind.Relative));

        HtmlDocument doc = new();
        doc.LoadHtml(response);
        HtmlNode? sectionNode = doc.DocumentNode.ChildNodes.First(n => n.HasClass("component-with-files"));
        HtmlNode? downloadLink1 = sectionNode.ChildNodes[1];
        HtmlNode? downloadLink2 = sectionNode.ChildNodes[3];
        HtmlNode? downloadLink3 = sectionNode.ChildNodes[5];
        HtmlNode? downloadLink4 = sectionNode.ChildNodes[7];

        // Assert
        downloadLink1.InnerHtml.Should().Contain("Download link text");
        downloadLink3.Attributes.Should().HaveCount(3);
        downloadLink1.Attributes["type"].Value.Should().Be("application/pdf");
        downloadLink1.Attributes["title"].Value.Should().Be("Download link description");
        downloadLink1.Attributes["href"].Value.Should().Be("/doc.pdf");

        downloadLink2.InnerHtml.Should().Contain("Download link text");
        downloadLink3.Attributes.Should().HaveCount(3);
        downloadLink2.Attributes["type"].Value.Should().Be("application/pdf");
        downloadLink2.Attributes["title"].Value.Should().Be("Download link description");
        downloadLink2.Attributes["href"].Value.Should().Be("/doc.pdf");

        // Assert
        downloadLink3.InnerHtml.Should().Contain("Download link text");
        downloadLink3.Attributes.Should().HaveCount(3);
        downloadLink3.Attributes["type"].Value.Should().Be("application/pdf");
        downloadLink3.Attributes["title"].Value.Should().Be("Download link description");
        downloadLink3.Attributes["href"].Value.Should().Be("/doc.pdf");

        downloadLink4.InnerHtml.Should().Contain("Download link text");
        downloadLink3.Attributes.Should().HaveCount(3);
        downloadLink4.Attributes["type"].Value.Should().Be("application/pdf");
        downloadLink4.Attributes["title"].Value.Should().Be("Download link description");
        downloadLink4.Attributes["href"].Value.Should().Be("/doc.pdf");
    }

    [Fact]
    public async Task FileTagHelper_RendersCustomTagsAttributes()
    {
        // Arrange
        _mockClientHandler.Responses.Push(new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(Serializer.Serialize(CannedResponses.WithNestedPlaceholder))
        });

        HttpClient client = BuildFileFieldTagHelperWebApplicationFactory().CreateClient();

        // Act
        string response = await client.GetStringAsync(new Uri("/", UriKind.Relative));

        HtmlDocument doc = new();
        doc.LoadHtml(response);
        HtmlNode? sectionNode = doc.DocumentNode.ChildNodes.First(n => n.HasClass("component-with-files"));
        HtmlNode? downloadLink1 = sectionNode.ChildNodes[9];
        HtmlNode? downloadLink2 = sectionNode.ChildNodes[11];
        HtmlNode? downloadLink3 = sectionNode.ChildNodes[13];
        HtmlNode? downloadLink4 = sectionNode.ChildNodes[15];

        // Assert
        downloadLink1.InnerHtml.Should().Contain("Download link text");
        downloadLink1.Attributes.Should().HaveCount(6);
        downloadLink1.Attributes["type"].Value.Should().Be("application/pdf");
        downloadLink1.Attributes["title"].Value.Should().Be("Download link description");
        downloadLink1.Attributes["href"].Value.Should().Be("/doc.pdf");
        downloadLink1.Attributes["class"].Value.Should().Be("test-class");
        downloadLink1.Attributes["target"].Value.Should().Be("_blank");
        downloadLink1.Attributes["custom-attribute"].Value.Should().Be("test-custom-attribute");

        downloadLink2.InnerHtml.Should().Contain("Download link text");
        downloadLink2.Attributes.Should().HaveCount(6);
        downloadLink2.Attributes["type"].Value.Should().Be("application/pdf");
        downloadLink2.Attributes["title"].Value.Should().Be("Download link description");
        downloadLink2.Attributes["href"].Value.Should().Be("/doc.pdf");
        downloadLink2.Attributes["class"].Value.Should().Be("test-class");
        downloadLink2.Attributes["target"].Value.Should().Be("_blank");
        downloadLink2.Attributes["custom-attribute"].Value.Should().Be("test-custom-attribute");

        downloadLink3.InnerHtml.Should().Contain("Download link text");
        downloadLink3.Attributes.Should().HaveCount(6);
        downloadLink3.Attributes["type"].Value.Should().Be("application/pdf");
        downloadLink3.Attributes["title"].Value.Should().Be("Download link description");
        downloadLink3.Attributes["href"].Value.Should().Be("/doc.pdf");
        downloadLink3.Attributes["class"].Value.Should().Be("test-class");
        downloadLink3.Attributes["target"].Value.Should().Be("_blank");
        downloadLink3.Attributes["custom-attribute"].Value.Should().Be("test-custom-attribute");

        downloadLink4.InnerHtml.Should().Contain("Download link text");
        downloadLink4.Attributes.Should().HaveCount(6);
        downloadLink4.Attributes["type"].Value.Should().Be("application/pdf");
        downloadLink4.Attributes["title"].Value.Should().Be("Download link description");
        downloadLink4.Attributes["href"].Value.Should().Be("/doc.pdf");
        downloadLink4.Attributes["class"].Value.Should().Be("test-class");
        downloadLink4.Attributes["target"].Value.Should().Be("_blank");
        downloadLink4.Attributes["custom-attribute"].Value.Should().Be("test-custom-attribute");
    }

    [Fact]
    public async Task FileTagHelper_OverridesModelAttributes()
    {
        // Arrange
        _mockClientHandler.Responses.Push(new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(Serializer.Serialize(CannedResponses.WithNestedPlaceholder))
        });

        HttpClient client = BuildFileFieldTagHelperWebApplicationFactory().CreateClient();

        // Act
        string response = await client.GetStringAsync(new Uri("/", UriKind.Relative));

        HtmlDocument doc = new();
        doc.LoadHtml(response);
        HtmlNode? sectionNode = doc.DocumentNode.ChildNodes.First(n => n.HasClass("component-with-files"));
        HtmlNode? downloadLink1 = sectionNode.ChildNodes[17];
        HtmlNode? downloadLink2 = sectionNode.ChildNodes[19];
        HtmlNode? downloadLink3 = sectionNode.ChildNodes[21];
        HtmlNode? downloadLink4 = sectionNode.ChildNodes[23];

        // Assert
        downloadLink1.InnerHtml.Should().Contain("Custom Download Link");
        downloadLink1.Attributes.Should().HaveCount(3);
        downloadLink1.Attributes["type"].Value.Should().Be("application/custom");
        downloadLink1.Attributes["title"].Value.Should().Be("custom-title");
        downloadLink1.Attributes["href"].Value.Should().Be("/customLink");

        downloadLink2.InnerHtml.Should().Contain("Custom Download Link");
        downloadLink2.Attributes.Should().HaveCount(3);
        downloadLink2.Attributes["type"].Value.Should().Be("application/custom");
        downloadLink2.Attributes["title"].Value.Should().Be("custom-title");
        downloadLink2.Attributes["href"].Value.Should().Be("/customLink");

        downloadLink3.InnerHtml.Should().Contain("Custom Download Link");
        downloadLink3.Attributes.Should().HaveCount(3);
        downloadLink3.Attributes["type"].Value.Should().Be("application/custom");
        downloadLink3.Attributes["title"].Value.Should().Be("custom-title");
        downloadLink3.Attributes["href"].Value.Should().Be("/customLink");

        downloadLink4.InnerHtml.Should().Contain("Custom Download Link");
        downloadLink4.Attributes.Should().HaveCount(3);
        downloadLink4.Attributes["type"].Value.Should().Be("application/custom");
        downloadLink4.Attributes["title"].Value.Should().Be("custom-title");
        downloadLink4.Attributes["href"].Value.Should().Be("/customLink");
    }

    [Fact]
    public async Task FileTagHelper_RendersInnerHtml()
    {
        // Arrange
        _mockClientHandler.Responses.Push(new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(Serializer.Serialize(CannedResponses.WithNestedPlaceholder))
        });

        HttpClient client = BuildFileFieldTagHelperWebApplicationFactory().CreateClient();

        // Act
        string response = await client.GetStringAsync(new Uri("/", UriKind.Relative));

        HtmlDocument doc = new();
        doc.LoadHtml(response);
        HtmlNode? sectionNode = doc.DocumentNode.ChildNodes.First(n => n.HasClass("component-with-files"));
        HtmlNode? downloadLink1 = sectionNode.ChildNodes[25];
        HtmlNode? downloadLink2 = sectionNode.ChildNodes[27];
        HtmlNode? downloadLink3 = sectionNode.ChildNodes[29];
        HtmlNode? downloadLink4 = sectionNode.ChildNodes[31];

        // Assert
        downloadLink1.InnerHtml.Should().Contain("<h1>Inner html</h1>");
        downloadLink2.InnerHtml.Should().Contain("<h1>Inner html</h1>");
        downloadLink3.InnerHtml.Should().Contain("<h1>Inner html</h1>");
        downloadLink4.InnerHtml.Should().Contain("<h1>Inner html</h1>");
    }

    public void Dispose()
    {
        _mockClientHandler.Dispose();
        GC.SuppressFinalize(this);
    }

    private WebApplicationFactory<TestWebApplicationProgram> BuildFileFieldTagHelperWebApplicationFactory()
    {
        return factory.WithWebHostBuilder(builder =>
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
                        .AddModelBoundView<ComponentModels.ComponentWithFiles>("Component-With-Files", "ComponentWithFiles")
                        .AddDefaultComponentRenderer();
                });
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
    }
}