using AwesomeAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
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
    private readonly WebApplicationFactory<TestWebApplicationProgram> _factory;

    public SitecoreLayoutClientBuilderExtensionsFixture()
    {
        _messageHandler = new MockHttpMessageHandler();
        _factory = BuildSitecoreLayoutClientBuilderWebApplicationFactory();
    }

    [Fact]
    public void DefaultHandler_SetsSitecoreLayoutServiceOptions()
    {
        // Act
        IOptions<SitecoreLayoutClientOptions> layoutService = _factory.Services.GetRequiredService<IOptions<SitecoreLayoutClientOptions>>();

        // Assert
        layoutService.Value.DefaultHandler.Should().Be("otherMock");
    }

    public void Dispose()
    {
        _messageHandler.Dispose();
        _factory.Dispose();
        GC.SuppressFinalize(this);
    }

    private WebApplicationFactory<TestWebApplicationProgram> BuildSitecoreLayoutClientBuilderWebApplicationFactory()
    {
        WebApplicationFactory<TestWebApplicationProgram> factory = new TestWebApplicationFactory<TestWebApplicationProgram>();

        return factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                ISitecoreLayoutClientBuilder lsc = services
                    .AddSitecoreLayoutService();

                lsc.AddHttpHandler("mock", _ => new HttpClient(_messageHandler) { BaseAddress = new Uri("http://layout.service") });

                lsc.AddHttpHandler("otherMock", _ => new HttpClient(_messageHandler) { BaseAddress = new Uri("http://layout.service") })
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