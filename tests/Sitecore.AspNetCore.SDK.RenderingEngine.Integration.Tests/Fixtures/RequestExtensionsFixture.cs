using System.Net;
using FluentAssertions;
using Microsoft.AspNetCore.TestHost;
using Sitecore.AspNetCore.SDK.AutoFixture.Mocks;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Extensions;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Interfaces;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Request;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Response;
using Sitecore.AspNetCore.SDK.RenderingEngine.Extensions;
using Xunit;

namespace Sitecore.AspNetCore.SDK.RenderingEngine.Integration.Tests.Fixtures;

public class RequestExtensionsFixture : IDisposable
{
    private readonly MockHttpMessageHandler _clientHandler;
    private readonly TestServer _server;

    public RequestExtensionsFixture()
    {
        TestServerBuilder testHostBuilder = new();
        _clientHandler = new MockHttpMessageHandler();
        testHostBuilder
            .ConfigureServices(builder =>
            {
                builder
                    .AddSitecoreLayoutService()
                    .AddHttpHandler("mock", _ => new HttpClient(_clientHandler) { BaseAddress = new Uri("http://layout.service") })
                    .AsDefaultHandler();
            })
            .Configure(app =>
            {
                app.UseSitecoreRenderingEngine();
            });

        _server = testHostBuilder.BuildServer(new Uri("http://localhost"));
    }

    [Fact]
    public async Task Properties_WithPath_AddedToRequest()
    {
        _clientHandler.Responses.Push(new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK
        });

        ISitecoreLayoutClient layoutService = _server.Services.GetRequiredService<ISitecoreLayoutClient>();

        SitecoreLayoutRequest request = new SitecoreLayoutRequest()
            .Path("UsingGlobalMiddleware");

        SitecoreLayoutResponse result = await layoutService.Request(request);

        result.Request.Should().ContainKey("item");
        result.Request["item"].Should().Be("UsingGlobalMiddleware");
    }

    [Fact]
    public async Task Properties_WithLanguage_AddedToRequest()
    {
        _clientHandler.Responses.Push(new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK
        });

        ISitecoreLayoutClient layoutService = _server.Services.GetRequiredService<ISitecoreLayoutClient>();

        SitecoreLayoutRequest request = new SitecoreLayoutRequest()
            .Path("UsingGlobalMiddleware")
            .Language("en");

        SitecoreLayoutResponse result = await layoutService.Request(request);

        result.Request.Should().ContainKey("sc_lang");
        result.Request["sc_lang"].Should().Be("en");
    }

    [Fact]
    public async Task Properties_WithApiKey_AddedToRequest()
    {
        _clientHandler.Responses.Push(new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK
        });

        ISitecoreLayoutClient layoutService = _server.Services.GetRequiredService<ISitecoreLayoutClient>();

        SitecoreLayoutRequest request = new SitecoreLayoutRequest()
            .Path("UsingGlobalMiddleware")
            .ApiKey("123");

        SitecoreLayoutResponse result = await layoutService.Request(request);

        result.Request.Should().ContainKey("sc_apikey");
        result.Request["sc_apikey"].Should().Be("123");
    }

    [Fact]
    public async Task Properties_WithSiteName_AddedToRequest()
    {
        _clientHandler.Responses.Push(new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK
        });

        ISitecoreLayoutClient layoutService = _server.Services.GetRequiredService<ISitecoreLayoutClient>();

        SitecoreLayoutRequest request = new SitecoreLayoutRequest()
            .Path("UsingGlobalMiddleware")
            .SiteName("name");

        SitecoreLayoutResponse result = await layoutService.Request(request);

        result.Request.Should().ContainKey("sc_site");
        result.Request["sc_site"].Should().Be("name");
    }

    public void Dispose()
    {
        _clientHandler.Dispose();
        _server.Dispose();
        GC.SuppressFinalize(this);
    }
}