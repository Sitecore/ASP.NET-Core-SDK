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

public class LinkFieldTagHelperFixture(TestWebApplicationFactory<TestWebApplicationProgram> factory) : IClassFixture<TestWebApplicationFactory<TestWebApplicationProgram>>, IDisposable
{
    private readonly MockHttpMessageHandler _mockClientHandler = new();
    private readonly Uri _layoutServiceUri = new("http://layout.service");

    [Fact]
    public async Task LinkTagHelper_DoesNotResetOtherTagHelperOutput()
    {
        // Arrange
        _mockClientHandler.Responses.Push(new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(Serializer.Serialize(CannedResponses.WithNestedPlaceholder))
        });

        HttpClient client = BuildLinkFieldTagHelperWebApplicationFactory().CreateClient();

        // Act
        string response = await client.GetStringAsync(new Uri("/", UriKind.Relative));

        HtmlDocument doc = new();
        doc.LoadHtml(response);
        HtmlNode? sectionNode = doc.DocumentNode.ChildNodes.First(n => n.HasClass("component-with-links"));
        HtmlNode? secondLink = sectionNode.ChildNodes[3];

        // Assert
        // check scenario that LinkTagHelper does not reset values of nested helpers.
        secondLink.InnerHtml.Should().Contain(TestConstants.TestFieldValue);
    }

    [Fact]
    public async Task LinkTagHelper_GeneratesAnchorTags()
    {
        // Arrange
        _mockClientHandler.Responses.Push(new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(Serializer.Serialize(CannedResponses.WithNestedPlaceholder))
        });

        HttpClient client = BuildLinkFieldTagHelperWebApplicationFactory().CreateClient();

        // Act
        string response = await client.GetStringAsync(new Uri("/", UriKind.Relative));

        HtmlDocument doc = new();
        doc.LoadHtml(response);
        HtmlNode? sectionNode = doc.DocumentNode.ChildNodes.First(n => n.HasClass("component-with-links"));

        // Assert
        // check that there is proper number of 'a' tags generated.
        sectionNode.ChildNodes.Count(n => n.Name.Equals("a", StringComparison.OrdinalIgnoreCase)).Should().Be(7);
    }

    [Fact]
    public async Task LinkTagHelper_PrioritizeUserProvidedLinkText()
    {
        // Arrange
        _mockClientHandler.Responses.Push(new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(Serializer.Serialize(CannedResponses.WithNestedPlaceholder))
        });

        HttpClient client = BuildLinkFieldTagHelperWebApplicationFactory().CreateClient();

        // Act
        string response = await client.GetStringAsync(new Uri("/", UriKind.Relative));

        HtmlDocument doc = new();
        doc.LoadHtml(response);
        HtmlNode? sectionNode = doc.DocumentNode.ChildNodes.First(n => n.HasClass("component-with-links"));

        // Assert
        // check that link will contain user provided link text.
        sectionNode.ChildNodes.First(n => n.Name.Equals("a", StringComparison.OrdinalIgnoreCase)).InnerText.Should().Contain("Sample internal link");
    }

    [Fact]
    public async Task LinkTagHelper_RenderFieldLinkTextIfNoInnerContent()
    {
        // Arrange
        _mockClientHandler.Responses.Push(new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(Serializer.Serialize(CannedResponses.WithNestedPlaceholder))
        });

        HttpClient client = BuildLinkFieldTagHelperWebApplicationFactory().CreateClient();

        // Act
        string response = await client.GetStringAsync(new Uri("/", UriKind.Relative));

        HtmlDocument doc = new();
        doc.LoadHtml(response);
        HtmlNode? sectionNode = doc.DocumentNode.ChildNodes.First(n => n.HasClass("component-with-links"));

        // Assert
        // check that link will contain user provided link text.
        sectionNode.ChildNodes.First(n => n.Name.Equals("a", StringComparison.OrdinalIgnoreCase) && n.HasClass("author-text")).InnerText.Should().Contain("This is field text");
    }

    [Fact]
    public async Task LinkTagHelper_RendersLinkAttributes()
    {
        // Arrange
        _mockClientHandler.Responses.Push(new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(Serializer.Serialize(CannedResponses.WithNestedPlaceholder))
        });

        HttpClient client = BuildLinkFieldTagHelperWebApplicationFactory().CreateClient();

        // Act
        string response = await client.GetStringAsync(new Uri("/", UriKind.Relative));

        HtmlDocument doc = new();
        doc.LoadHtml(response);
        HtmlNode? sectionNode = doc.DocumentNode.ChildNodes.First(n => n.HasClass("component-with-links"));

        // Assert
        // check that link will contain user provided link text.
        HtmlNode? linkNode = sectionNode.ChildNodes.Last(n => n.Name.Equals("a", StringComparison.OrdinalIgnoreCase));

        linkNode.Should().NotBeNull();
        linkNode.Attributes.SingleOrDefault(a => a.Name == "href").Should().NotBeNull();
        linkNode.Attributes.Single(a => a.Name == "href").Value.Should().NotBe(string.Empty);
        linkNode.Attributes.SingleOrDefault(a => a.Name == "target").Should().NotBeNull();
        linkNode.Attributes.Single(a => a.Name == "target").Value.Should().NotBe(string.Empty);
        linkNode.Attributes.SingleOrDefault(a => a.Name == "title").Should().NotBeNull();
        linkNode.Attributes.Single(a => a.Name == "title").Value.Should().NotBe(string.Empty);
        linkNode.Attributes.SingleOrDefault(a => a.Name == "class").Should().NotBeNull();
        linkNode.Attributes.Single(a => a.Name == "class").Value.Should().NotBe(string.Empty);
    }

    [Fact]
    public async Task LinkTagHelper_GeneratesNestedTags()
    {
        // Arrange
        _mockClientHandler.Responses.Push(new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(Serializer.Serialize(CannedResponses.WithNestedPlaceholder))
        });

        HttpClient client = BuildLinkFieldTagHelperWebApplicationFactory().CreateClient();

        // Act
        string response = await client.GetStringAsync(new Uri("/", UriKind.Relative));

        HtmlDocument doc = new();
        doc.LoadHtml(response);
        HtmlNode? sectionNode = doc.DocumentNode.ChildNodes.First(n => n.HasClass("component-with-links"));

        // Assert
        // check that link will contain user provided link text.
        sectionNode.ChildNodes.Last(n => n.Name.Equals("a", StringComparison.OrdinalIgnoreCase)).InnerText.Should().Be(TestConstants.TestFieldValue);
    }

    [Fact]
    public async Task LinkTagHelper_DoesNotTrimUserAttributes()
    {
        // Arrange
        _mockClientHandler.Responses.Push(new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(Serializer.Serialize(CannedResponses.WithNestedPlaceholder))
        });

        HttpClient client = BuildLinkFieldTagHelperWebApplicationFactory().CreateClient();

        // Act
        string response = await client.GetStringAsync(new Uri("/", UriKind.Relative));

        HtmlDocument doc = new();
        doc.LoadHtml(response);
        HtmlNode? sectionNode = doc.DocumentNode.ChildNodes.First(n => n.HasClass("component-with-links"));

        // Assert
        // check that link will contain user provided link text.
        sectionNode.ChildNodes.Last(n => n.Name.Equals("a", StringComparison.OrdinalIgnoreCase)).Attributes.Contains("data-test");
    }

    [Fact]
    public async Task LinkTagHelper_RenderFieldAuthorLinkTextInEEIfEditableTrue()
    {
        // Arrange
        _mockClientHandler.Responses.Push(new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(Serializer.Serialize(CannedResponses.WithNestedPlaceholder))
        });

        HttpClient client = BuildLinkFieldTagHelperWebApplicationFactory().CreateClient();

        // Act
        string response = await client.GetStringAsync(new Uri("/", UriKind.Relative));

        HtmlDocument doc = new();
        doc.LoadHtml(response);
        HtmlNode? sectionNode = doc.DocumentNode.ChildNodes.First(n => n.HasClass("component-with-links"));

        // Assert
        // check that link will contain user provided link text.
        sectionNode.ChildNodes.First(n => n.Name.Equals("a", StringComparison.OrdinalIgnoreCase) && n.HasClass("author-text-ee")).InnerText.Should().Contain("This is field text");
    }

    [Fact]
    public async Task LinkTagHelper_RenderFieldCustomLinkTextInEEIfEditableFalse()
    {
        // Arrange
        _mockClientHandler.Responses.Push(new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(Serializer.Serialize(CannedResponses.WithNestedPlaceholder))
        });

        HttpClient client = BuildLinkFieldTagHelperWebApplicationFactory().CreateClient();

        // Act
        string response = await client.GetStringAsync(new Uri("/", UriKind.Relative));

        HtmlDocument doc = new();
        doc.LoadHtml(response);
        HtmlNode? sectionNode = doc.DocumentNode.ChildNodes.First(n => n.HasClass("component-with-links"));

        // Assert
        // check that link will contain user provided link text.
        sectionNode.ChildNodes.First(n => n.Name.Equals("a", StringComparison.OrdinalIgnoreCase) && n.HasClass("author-text-ee-editable-false")).InnerText.Should().Contain("custom text");
    }

    public void Dispose()
    {
        _mockClientHandler.Dispose();
        GC.SuppressFinalize(this);
    }

    private WebApplicationFactory<TestWebApplicationProgram> BuildLinkFieldTagHelperWebApplicationFactory()
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
                        .AddModelBoundView<ComponentModels.ComponentWithLinks>("Component-With-Links", "ComponentWithLinks")
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