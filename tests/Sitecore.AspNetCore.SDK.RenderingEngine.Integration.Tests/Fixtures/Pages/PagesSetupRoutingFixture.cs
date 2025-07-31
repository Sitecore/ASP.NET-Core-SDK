using System.Net;
using AwesomeAssertions;
using Sitecore.AspNetCore.SDK.TestData;
using Xunit;

namespace Sitecore.AspNetCore.SDK.RenderingEngine.Integration.Tests.Fixtures.Pages;

public class PagesSetupRoutingFixture(TestWebApplicationFactory<TestPagesProgram> factory) : IClassFixture<TestWebApplicationFactory<TestPagesProgram>>
{
    private readonly TestWebApplicationFactory<TestPagesProgram> _factory = factory;

    [Fact]
    public async Task ConfigRoute_MissingSecret_ReturnsBadRequest()
    {
        // Arrange
        HttpClient client = _factory.CreateClient();
        string url = $"{TestConstants.ConfigRoute}?secret=";

        // Act
        HttpResponseMessage? response = await client.GetAsync(url);

        // Assert
        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task ConfigRoute_InvalidSecret_ReturnsBadRequest()
    {
        // Arrange
        HttpClient client = _factory.CreateClient();
        string url = $"{TestConstants.ConfigRoute}?secret=invalid_secret_value";

        // Act
        HttpResponseMessage? response = await client.GetAsync(url);

        // Assert
        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task ConfigRoute_InvalidRequestOrigin_ReturnsBadRequest()
    {
        // Arrange
        HttpClient client = _factory.CreateClient();
        string url = $"{TestConstants.ConfigRoute}?secret={TestConstants.JssEditingSecret}";
        client.DefaultRequestHeaders.Add("Origin", "http://invalid_origin_domain.com");

        // Act
        HttpResponseMessage? response = await client.GetAsync(url);

        // Assert
        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task ConfigRoute_ValidCall_ReturnsCorrectObject()
    {
        // Arrange
        HttpClient client = _factory.CreateClient();
        string url = $"{TestConstants.ConfigRoute}?secret={TestConstants.JssEditingSecret}";
        client.DefaultRequestHeaders.Add("Origin", "https://pages.sitecorecloud.io");

        // Act
        HttpResponseMessage? response = await client.GetAsync(url);

        // Assert
        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Content.Headers.ContentType.Should().NotBeNull();
        response.Content.Headers.ContentType?.MediaType.Should().Be("application/json");
        response.Headers.NonValidated["Content-Security-Policy"].Should().BeEquivalentTo("frame-ancestors 'self'  https://pages.sitecorecloud.io");
        response.Headers.NonValidated["Access-Control-Allow-Origin"].Should().BeEquivalentTo("https://pages.sitecorecloud.io");
        response.Headers.NonValidated["Access-Control-Allow-Methods"].Should().BeEquivalentTo("GET, POST, OPTIONS, PUT, PATCH, DELETE");
    }

    [Fact]
    public async Task RenderRoute_MissingSecret_ReturnsBadRequest()
    {
        // Arrange
        HttpClient client = _factory.CreateClient();
        string url = $"{TestConstants.RenderRoute}?secret=";

        // Act
        HttpResponseMessage? response = await client.GetAsync(url);

        // Assert
        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task RenderRoute_InvalidSecret_ReturnsBadRequest()
    {
        // Arrange
        HttpClient client = _factory.CreateClient();
        string url = $"{TestConstants.RenderRoute}?secret=invalid_secret_value";

        // Act
        HttpResponseMessage? response = await client.GetAsync(url);

        // Assert
        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task RenderRoute_ValidCall_ReturnsCorrectResponse()
    {
        // Arrange
        HttpClient client = _factory.CreateClient();
        Guid itemId = Guid.NewGuid();
        string language = "en";
        string layoutKind = "final";
        string mode = "edit";
        string route = TestConstants.RenderRoute; // The controller needs to return a valid route in its RedirectResponse, so we're reusing the same route here instead of creating a fake one.
        string site = "siteA";
        string version = "1";
        string tenantId = "tenant1234";
        string url = $"{TestConstants.RenderRoute}?secret={TestConstants.JssEditingSecret}&sc_itemid={itemId}&sc_lang={language}&sc_layoutKind={layoutKind}&mode={mode}&sc_site={site}&sc_version={version}&tenant_id={tenantId}&route={route}";

        // Act
        HttpResponseMessage? response = await client.GetAsync(url);

        // Assert
        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.Redirect);
    }
}