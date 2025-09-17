using System.Net;
using AwesomeAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
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
    private readonly WebApplicationFactory<TestWebApplicationProgram> _factory;

    public RequestExtensionsFixture()
    {
        _clientHandler = new MockHttpMessageHandler();
        _factory = BuildRequestExtensionsWebApplicationFactory();
    }

    [Fact]
    public async Task Properties_WithPath_AddedToRequest()
    {
        _clientHandler.Responses.Push(new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK
        });

        ISitecoreLayoutClient layoutService = _factory.Services.GetRequiredService<ISitecoreLayoutClient>();

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

        ISitecoreLayoutClient layoutService = _factory.Services.GetRequiredService<ISitecoreLayoutClient>();

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

        ISitecoreLayoutClient layoutService = _factory.Services.GetRequiredService<ISitecoreLayoutClient>();

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

        ISitecoreLayoutClient layoutService = _factory.Services.GetRequiredService<ISitecoreLayoutClient>();

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
        _factory.Dispose();
        GC.SuppressFinalize(this);
    }

    private WebApplicationFactory<TestWebApplicationProgram> BuildRequestExtensionsWebApplicationFactory()
    {
        WebApplicationFactory<TestWebApplicationProgram> factory = new TestWebApplicationFactory<TestWebApplicationProgram>();

        return factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                services
                    .AddSitecoreLayoutService()
                    .AddHttpHandler("mock", _ => new HttpClient(_clientHandler) { BaseAddress = new Uri("http://layout.service") })
                    .AsDefaultHandler();

                services.AddSitecoreRenderingEngine(options => options.MapToRequest((http, sc) => { sc.Path(http.Path); }));
            });

            builder.Configure(app =>
            {
                app.UseRouting();
                app.UseSitecoreRenderingEngine();
                app.UseEndpoints(endpoints => { endpoints.MapDefaultControllerRoute(); });
            });
        });
    }
}