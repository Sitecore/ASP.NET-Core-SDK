using AwesomeAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Sitecore.AspNetCore.SDK.AutoFixture.Mocks;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Extensions;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Interfaces;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Request;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Response;
using Sitecore.AspNetCore.SDK.RenderingEngine.Extensions;
using Xunit;

// ReSharper disable StringLiteralTypo
namespace Sitecore.AspNetCore.SDK.RenderingEngine.Integration.Tests.Fixtures;

public class RequestDefaultsConfigurationFixture : IDisposable
{
    private readonly MockHttpMessageHandler _clientHandler;
    private readonly WebApplicationFactory<TestWebApplicationProgram> _factory;

    public RequestDefaultsConfigurationFixture()
    {
        _clientHandler = new MockHttpMessageHandler();
        _factory = BuildRequestDefaultsConfigurationWebApplicationFactory();
    }

    [Fact]
    public async Task Request_OnlyGlobalOptionsProvided_FinalRequestUsesGlobalOptions()
    {
        // Arrange
        ISitecoreLayoutClient layoutClient = _factory.Services.GetRequiredService<ISitecoreLayoutClient>();

        SitecoreLayoutRequest request = new SitecoreLayoutRequest()
            .Path("test");

        // Act
        SitecoreLayoutResponse response = await layoutClient.Request(request);

        // Assert
        response.Should().NotBeNull();
        response.Request["key2"].Should().Be("value2");
    }

    [Fact]
    public async Task Request_OnlyHandlerOptionsProvided_FinalRequestUsesHandlerOptions()
    {
        // Arrange
        ISitecoreLayoutClient layoutClient = _factory.Services.GetRequiredService<ISitecoreLayoutClient>();

        SitecoreLayoutRequest request = [];

        // Act
        SitecoreLayoutResponse response = await layoutClient.Request(request);

        // Assert
        response.Should().NotBeNull();
        response.Request["key3"].Should().Be("value4");
    }

    [Fact]
    public async Task Request_OnlyRequestParameterProvided_FinalRequestUsesRequestParameter()
    {
        // Arrange
        ISitecoreLayoutClient layoutClient = _factory.Services.GetRequiredService<ISitecoreLayoutClient>();

        SitecoreLayoutRequest request = new()
        {
            { "somekey", "somevalue" }
        };

        // Act
        SitecoreLayoutResponse response = await layoutClient.Request(request);

        // Assert
        response.Should().NotBeNull();
        response.Request["somekey"].Should().Be("somevalue");
    }

    [Fact]
    public async Task Request_GlobalAndHandlerOptionsProvided_FinalRequestUsesHandlerOptions()
    {
        // Arrange
        ISitecoreLayoutClient layoutClient = _factory.Services.GetRequiredService<ISitecoreLayoutClient>();

        SitecoreLayoutRequest request = [];

        // Act
        SitecoreLayoutResponse response = await layoutClient.Request(request);

        // Assert
        response.Should().NotBeNull();
        response.Request["key1"].Should().Be("value3");
    }

    [Fact]
    public async Task Request_GlobalOptionsAndRequestParametersProvided_FinalRequestUsesRequestParameters()
    {
        // Arrange
        ISitecoreLayoutClient layoutClient = _factory.Services.GetRequiredService<ISitecoreLayoutClient>();

        SitecoreLayoutRequest request = new() { { "key2", "requestvalue" } };

        // Act
        SitecoreLayoutResponse response = await layoutClient.Request(request, "mockwithoutoptions");

        // Assert
        response.Should().NotBeNull();
        response.Request["key2"].Should().Be("requestvalue");
    }

    [Fact]
    public async Task Request_GlobalAndHandlerOptionsAndRequestParametersProvided_FinalRequestUsesRequestParameters()
    {
        // Arrange
        ISitecoreLayoutClient layoutClient = _factory.Services.GetRequiredService<ISitecoreLayoutClient>();

        SitecoreLayoutRequest request = new() { { "key1", "requestvalue" } };

        // Act
        SitecoreLayoutResponse response = await layoutClient.Request(request);

        // Assert
        response.Should().NotBeNull();
        response.Request["key1"].Should().Be("requestvalue");
    }

    [Fact]
    public async Task Request_GlobalAndHandlerAndRequestSetDifferentParameters_FinalRequestUsesAllParameters()
    {
        // Arrange
        ISitecoreLayoutClient layoutClient = _factory.Services.GetRequiredService<ISitecoreLayoutClient>();

        SitecoreLayoutRequest request = new() { { "requestKey", "requestValue" } };

        // Act
        SitecoreLayoutResponse response = await layoutClient.Request(request);

        // Assert
        response.Should().NotBeNull();
        response.Request["key2"].Should().Be("value2");
        response.Request["key3"].Should().Be("value4");
        response.Request["requestKey"].Should().Be("requestValue");
    }

    [Fact]
    public async Task Request_GlobalOptionsProvidedRequestSetsParameterToNull_FinalRequestExcludesParameter()
    {
        // Arrange
        ISitecoreLayoutClient layoutClient = _factory.Services.GetRequiredService<ISitecoreLayoutClient>();

        SitecoreLayoutRequest request = new()
        {
            { "key2", null }
        };

        // Act
        SitecoreLayoutResponse response = await layoutClient.Request(request);

        // Assert
        response.Should().NotBeNull();
        response.Request.Should().NotContainKey("key2");
    }

    [Fact]
    public async Task Request_HandlerOptionsProvidedRequestSetsParameterToNull_FinalRequestExcludesParameter()
    {
        // Arrange
        ISitecoreLayoutClient layoutClient = _factory.Services.GetRequiredService<ISitecoreLayoutClient>();

        SitecoreLayoutRequest request = new()
        {
            { "key3", null }
        };

        // Act
        SitecoreLayoutResponse response = await layoutClient.Request(request);

        // Assert
        response.Should().NotBeNull();
        response.Request.Should().NotContainKey("key3");
    }

    public void Dispose()
    {
        _clientHandler.Dispose();
        _factory.Dispose();
        GC.SuppressFinalize(this);
    }

    private WebApplicationFactory<TestWebApplicationProgram> BuildRequestDefaultsConfigurationWebApplicationFactory()
    {
        WebApplicationFactory<TestWebApplicationProgram> factory = new TestWebApplicationFactory<TestWebApplicationProgram>();

        return factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                ISitecoreLayoutClientBuilder lsc = services
                    .AddSitecoreLayoutService()
                    .WithDefaultRequestOptions(request =>
                    {
                        request["key1"] = "value1";
                        request["key2"] = "value2";
                    });

                lsc.AddHttpHandler("mock", _ => new HttpClient(_clientHandler) { BaseAddress = new Uri("http://layout.service") })
                    .WithRequestOptions(request =>
                    {
                        request["key1"] = "value3";
                        request["key3"] = "value4";
                    })
                    .AsDefaultHandler();

                lsc.AddHttpHandler("mockwithoutoptions", _ => new HttpClient(_clientHandler) { BaseAddress = new Uri("http://layout.service") });

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