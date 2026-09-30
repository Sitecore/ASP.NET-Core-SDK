using System.Net;
using AwesomeAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Sitecore.AspNetCore.SDK.AutoFixture.Mocks;
using Sitecore.AspNetCore.SDK.RenderingEngine.Integration.Tests.Fixtures.Mocks;
using Sitecore.AspNetCore.SDK.SearchOptimization.Extensions;
using Xunit;

namespace Sitecore.AspNetCore.SDK.RenderingEngine.Integration.Tests.Fixtures.SearchOptimization;

public class SitemapProxyFixture : IClassFixture<TestWebApplicationFactory<TestWebApplicationProgram>>, IDisposable
{
    private readonly MockHttpMessageHandler _mockClientHandler = new();
    private readonly Uri _cdInstanceUri = new("http://cd");
    private readonly WebApplicationFactory<TestWebApplicationProgram> _factory;

    public SitemapProxyFixture(TestWebApplicationFactory<TestWebApplicationProgram> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            _mockClientHandler.Responses.Push(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK
            });

            builder.ConfigureServices(services =>
            {
                services.AddSingleton<IHttpClientFactory>(_ =>
                {
                    return new CustomHttpClientFactory(
                        () => new HttpClient(_mockClientHandler));
                });

                services.AddSitemap(c => c.Url = _cdInstanceUri);
            });

            builder.Configure(app =>
            {
                app.UseSitemap();
            });
        });

        // Accessing _factory.Server forces the TestServer to start. The variable is unused; this is intentional.
        _ = _factory.Server;
    }

    [Fact]
    public async Task SitemapRequest_MustBeProxied()
    {
        // Arrange
        HttpClient client = _factory.CreateClient();
        HttpRequestMessage request = new(HttpMethod.Get, new Uri("/sitemap.xml", UriKind.Relative));

        // Act
        await client.SendAsync(request);

        // Asserts
        _mockClientHandler.Requests.Should().ContainSingle();
        _mockClientHandler.Requests[0].RequestUri!.Host.Should().Be(_cdInstanceUri.Host);
        _mockClientHandler.Requests[0].RequestUri!.Scheme.Should().Be(_cdInstanceUri.Scheme);
        _mockClientHandler.Requests[0].RequestUri!.PathAndQuery.Should().Be("/sitemap.xml");
    }

    public void Dispose()
    {
        _mockClientHandler.Dispose();
        _factory.Dispose();
        GC.SuppressFinalize(this);
    }
}