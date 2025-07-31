using AwesomeAssertions;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Options;
using Sitecore.AspNetCore.SDK.AutoFixture.Mocks;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Configuration;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Extensions;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Interfaces;
using Sitecore.AspNetCore.SDK.RenderingEngine.Extensions;
using Xunit;

namespace Sitecore.AspNetCore.SDK.RenderingEngine.Integration.Tests.Fixtures;

public class SitecoreLayoutClientBuilderExtensionsFixture : IDisposable
{
    private readonly MockHttpMessageHandler _messageHandler;
    private readonly TestServer _server;

    public SitecoreLayoutClientBuilderExtensionsFixture()
    {
        TestServerBuilder testHostBuilder = new();
        _messageHandler = new MockHttpMessageHandler();
        testHostBuilder
            .ConfigureServices(builder =>
            {
                ISitecoreLayoutClientBuilder lsc = builder
                    .AddSitecoreLayoutService();

                lsc.AddHttpHandler("mock", _ => new HttpClient(_messageHandler) { BaseAddress = new Uri("http://layout.service") });

                lsc.AddHttpHandler("otherMock", _ => new HttpClient(_messageHandler) { BaseAddress = new Uri("http://layout.service") })
                    .AsDefaultHandler();
            })
            .Configure(app =>
            {
                app.UseSitecoreRenderingEngine();
            });

        _server = testHostBuilder.BuildServer(new Uri("http://localhost"));
    }

    [Fact]
    public void DefaultHandler_SetsSitecoreLayoutServiceOptions()
    {
        // Act
        IOptions<SitecoreLayoutClientOptions> layoutService = _server.Services.GetRequiredService<IOptions<SitecoreLayoutClientOptions>>();

        // Assert
        layoutService.Value.DefaultHandler.Should().Be("otherMock");
    }

    public void Dispose()
    {
        _messageHandler.Dispose();
        _server.Dispose();
        GC.SuppressFinalize(this);
    }
}