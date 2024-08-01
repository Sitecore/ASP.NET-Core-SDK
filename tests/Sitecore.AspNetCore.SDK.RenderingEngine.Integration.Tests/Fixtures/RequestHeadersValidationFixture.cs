using FluentAssertions;
using Microsoft.AspNetCore.TestHost;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Extensions;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Interfaces;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Request;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Response;
using Sitecore.AspNetCore.SDK.RenderingEngine.Extensions;
using Xunit;

namespace Sitecore.AspNetCore.SDK.RenderingEngine.Integration.Tests.Fixtures;

public class RequestHeadersValidationFixture : IDisposable
{
    private HttpLayoutClientMessageHandler _clientHandler = new();
    private TestServer _server = null!;

    [Fact]
    public async Task Request_WithNonValidatedHeaders_HeadersAreProperlyValidated()
    {
        // Arrange
        ConfigureServices(["User-Agent"]);
        ISitecoreLayoutClient layoutClient = _server.Services.GetRequiredService<ISitecoreLayoutClient>();

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
        ConfigureServices([]);
        ISitecoreLayoutClient layoutClient = _server.Services.GetRequiredService<ISitecoreLayoutClient>();

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
        _server.Dispose();
        GC.SuppressFinalize(this);
    }

    private void ConfigureServices(string[] nonValidatedHeaders)
    {
        TestServerBuilder testHostBuilder = new();
        _clientHandler = new HttpLayoutClientMessageHandler();
        Dictionary<string, string[]> headers = new()
        {
            { "User-Agent", ["site;core"] }
        };

        testHostBuilder
            .ConfigureServices(builder =>
            {
                ISitecoreLayoutClientBuilder lsc = builder
                    .AddSitecoreLayoutService();

                lsc.AddHttpHandler("mock", _ => new HttpClient(_clientHandler) { BaseAddress = new Uri("http://layout.service") }, nonValidatedHeaders)
                    .WithRequestOptions(request =>
                    {
                        request["sc_request_headers_key"] = headers;
                        request["key3"] = "value4";
                    })
                    .AsDefaultHandler();

                builder
                    .AddSitecoreRenderingEngine();
            })
            .Configure(app =>
            {
                app.UseSitecoreRenderingEngine();
            });

        _server = testHostBuilder.BuildServer(new Uri("http://localhost"));
    }
}