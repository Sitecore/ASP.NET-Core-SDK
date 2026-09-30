using System.Net;
using AwesomeAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Sitecore.AspNetCore.SDK.AutoFixture.Mocks;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Extensions;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Interfaces;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Request;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Response;
using Sitecore.AspNetCore.SDK.RenderingEngine.Extensions;
using Xunit;

namespace Sitecore.AspNetCore.SDK.RenderingEngine.Integration.Tests.Fixtures;

public class RequestHeadersValidationFixture : IClassFixture<TestWebApplicationFactory<TestWebApplicationProgram>>, IDisposable
{
    private readonly TestWebApplicationFactory<TestWebApplicationProgram> _factory;
    private MockHttpMessageHandler _clientHandler = new();
    private WebApplicationFactory<TestWebApplicationProgram> _appFactory = null!;

    public RequestHeadersValidationFixture(TestWebApplicationFactory<TestWebApplicationProgram> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Request_WithNonValidatedHeaders_HeadersAreProperlyValidated()
    {
        // Arrange
        _appFactory = BuildRequestHeadersWebApplicationFactory(["User-Agent"]);
        ISitecoreLayoutClient layoutClient = _appFactory.Services.GetRequiredService<ISitecoreLayoutClient>();

        SitecoreLayoutRequest request = new SitecoreLayoutRequest()
            .Path("test");

        // Act
        SitecoreLayoutResponse response = await layoutClient.Request(request);

        // Assert
        response.Should().NotBeNull();
        response.Errors.FirstOrDefault(error => error.InnerException!.Message == "The format of value 'site;core' is invalid.").Should().Be(null);
        object? headerKeys = response.Request["sc_request_headers_key"];

        Dictionary<string, string[]>? userAgentHeader = headerKeys as Dictionary<string, string[]>;
        userAgentHeader!["User-Agent"][0].Should().Be("site;core");
    }

    [Fact]
    public async Task Request_WithoutNonValidatedHeaders_ErrorThrown()
    {
        // Arrange
        _appFactory = BuildRequestHeadersWebApplicationFactory(Array.Empty<string>());
        ISitecoreLayoutClient layoutClient = _appFactory.Services.GetRequiredService<ISitecoreLayoutClient>();

        SitecoreLayoutRequest request = new SitecoreLayoutRequest()
            .Path("test");

        // Act
        SitecoreLayoutResponse response = await layoutClient.Request(request);

        // Assert
        response.Should().NotBeNull();
        response.Errors.FirstOrDefault(error => error.InnerException!.Message == "The format of value 'site;core' is invalid.").Should().NotBe(null);
    }

    public void Dispose()
    {
        _clientHandler.Dispose();
        _appFactory?.Dispose();
        GC.SuppressFinalize(this);
    }

    private WebApplicationFactory<TestWebApplicationProgram> BuildRequestHeadersWebApplicationFactory(string[] nonValidatedHeaders)
    {
        _clientHandler = new MockHttpMessageHandler();
        Dictionary<string, string[]> headers = new()
        {
            { "User-Agent", ["site;core"] }
        };

        return _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                ISitecoreLayoutClientBuilder lsc = services.AddSitecoreLayoutService();

                lsc.AddHttpHandler("mock", _ => new HttpClient(_clientHandler) { BaseAddress = new Uri("http://layout.service") }, nonValidatedHeaders)
                    .WithRequestOptions(request =>
                    {
                        request["sc_request_headers_key"] = headers;
                        request["key3"] = "value4";
                    })
                    .AsDefaultHandler();

                services.AddSitecoreRenderingEngine();
            });

            builder.Configure(app =>
            {
                app.UseSitecoreRenderingEngine();
            });
        });
    }
}