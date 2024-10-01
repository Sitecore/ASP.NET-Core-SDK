using System.Net;
using FluentAssertions;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.TestHost;
using Sitecore.AspNetCore.SDK.AutoFixture.Mocks;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Extensions;
using Sitecore.AspNetCore.SDK.RenderingEngine.Extensions;
using Sitecore.AspNetCore.SDK.RenderingEngine.Integration.Tests.Fixtures.Mocks;
using Sitecore.AspNetCore.SDK.Tracking;
using Sitecore.AspNetCore.SDK.Tracking.VisitorIdentification;
using Xunit;

// ReSharper disable StringLiteralTypo
namespace Sitecore.AspNetCore.SDK.RenderingEngine.Integration.Tests.Fixtures.Tracking;

public class TrackingProxyFixture : IDisposable
{
    private readonly TestServer _server;
    private readonly MockHttpMessageHandler _mockClientHandler = new();
    private readonly Uri _layoutServiceUri = new("http://layout.service");
    private readonly Uri _cmInstanceUri = new("http://layout.service");

    public TrackingProxyFixture()
    {
        TestServerBuilder testHostBuilder = new();
        _mockClientHandler.Responses.Push(new HttpResponseMessage(HttpStatusCode.OK));

        _ = testHostBuilder
            .ConfigureServices(builder =>
            {
                builder.Configure<ForwardedHeadersOptions>(options =>
                {
                    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
                });

                builder
                    .AddSitecoreLayoutService()
                    .AddHttpHandler("mock", _ => new HttpClient(_mockClientHandler) { BaseAddress = _layoutServiceUri })
                    .AsDefaultHandler();

                builder.AddSitecoreRenderingEngine(options =>
                    {
                        options.AddDefaultComponentRenderer();
                    })
                    .WithTracking();

                builder.AddSitecoreVisitorIdentification(options =>
                {
                    options.SitecoreInstanceUri = _cmInstanceUri;
                });

                builder.AddSingleton<IHttpClientFactory>(_ =>
                {
                    return new CustomHttpClientFactory(
                        () =>
                            new HttpClient(_mockClientHandler));
                });
            })
            .Configure(app =>
            {
                app.UseForwardedHeaders();
                app.UseSitecoreVisitorIdentification();
                app.UseSitecoreRenderingEngine();
            });

        _server = testHostBuilder.BuildServer(new Uri("http://localhost"));
    }

    [Fact]
    public async Task SitecoreRequests_ToLayouts_MustBeProxied()
    {
        // Arrange
        HttpClient client = _server.CreateClient();
        HttpRequestMessage request = new(HttpMethod.Get, new Uri("/layouts/System/VisitorIdentification.js", UriKind.Relative));
        request.Headers.Add("Cookie", ["ASP.NET_SessionId=rku2oxmotbrkwkfxe0cpfrvn; path=/; HttpOnly; SameSite=Lax", "SC_ANALYTICS_GLOBAL_COOKIE=0f82f53555ce4304a1ee8ae99ab9f9a8|False; expires = Fri, 15 - Mar - 2030 13:15:08 GMT; path =/; HttpOnly"]);
        request.Headers.Add("X-Forwarded-For", "172.217.16.14");

        // Act
        await client.SendAsync(request);

        // Asserts
        _mockClientHandler.Requests.Should().ContainSingle("A call to rendering middleware is not expected.");
        _mockClientHandler.Requests[0].RequestUri!.Host.Should().Be(_cmInstanceUri.Host);
        _mockClientHandler.Requests[0].RequestUri!.Scheme.Should().Be(_cmInstanceUri.Scheme);
        _mockClientHandler.Requests[0].RequestUri!.PathAndQuery.Should().Be("/layouts/System/VisitorIdentification.js");
        _mockClientHandler.Requests[0].Headers.Should().Contain(h => h.Key.Equals("Cookie"));
        _mockClientHandler.Requests[0].Headers.GetValues("x-forwarded-for").First().ToUpperInvariant().Should().Be("172.217.16.14");
        _mockClientHandler.Requests[0].Headers.GetValues("x-forwarded-host").First().ToUpperInvariant().Should().Be("LOCALHOST");
        _mockClientHandler.Requests[0].Headers.GetValues("x-forwarded-proto").First().ToUpperInvariant().Should().Be("HTTP");
    }

    public void Dispose()
    {
        _server.Dispose();
        _mockClientHandler.Dispose();
        GC.SuppressFinalize(this);
    }
}