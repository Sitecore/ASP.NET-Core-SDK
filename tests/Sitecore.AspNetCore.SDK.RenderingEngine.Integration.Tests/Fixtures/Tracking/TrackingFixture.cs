using System.Net;
using FluentAssertions;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.TestHost;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Extensions;
using Sitecore.AspNetCore.SDK.RenderingEngine.Extensions;
using Sitecore.AspNetCore.SDK.TestData;
using Sitecore.AspNetCore.SDK.Tracking;
using Sitecore.AspNetCore.SDK.Tracking.VisitorIdentification;
using Xunit;

// ReSharper disable StringLiteralTypo
namespace Sitecore.AspNetCore.SDK.RenderingEngine.Integration.Tests.Fixtures.Tracking;

public class TrackingFixture : IDisposable
{
    private static readonly string[] AspSessionId =
    [
        "ASP.NET_SessionId=rku2oxmotbrkwkfxe0cpfrvn; path=/; HttpOnly; SameSite=Lax"
    ];

    private static readonly string[] AnalyticsCookie =
    [
        "SC_ANALYTICS_GLOBAL_COOKIE=0f82f53555ce4304a1ee8ae99ab9f9a8|False; expires = Fri, 15 - Mar - 2030 13:15:08 GMT; path =/; HttpOnly"
    ];

    private readonly TestServer _server;

    private readonly HttpLayoutClientMessageHandler _mockClientHandler;

    private readonly Uri _layoutServiceUri = new("http://layout.service");

    private readonly Uri _cmInstanceUri = new("http://layout.service");

    public TrackingFixture()
    {
        TestServerBuilder testHostBuilder = new();
        _mockClientHandler = new HttpLayoutClientMessageHandler();

        _ = testHostBuilder
            .ConfigureServices(builder =>
            {
                builder.Configure<ForwardedHeadersOptions>(options =>
                {
                    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor;
                });

                builder
                    .AddSitecoreLayoutService()
                    .AddHttpHandler("mock", _ => new HttpClient(_mockClientHandler) { BaseAddress = _layoutServiceUri })
                    .AsDefaultHandler();

                builder.AddSitecoreRenderingEngine(options =>
                    {
                        options
                            .AddDefaultComponentRenderer();
                    })
                    .WithTracking();

                builder.AddSitecoreVisitorIdentification(o => o.SitecoreInstanceUri = _cmInstanceUri);
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
    public async Task SitecoreResponse_WithRobotDetection_MustIncludeVisitorIdentificationJs()
    {
        // Arrange
        _mockClientHandler.Responses.Push(new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(Serializer.Serialize(CannedResponses.WithVisitorIdentificationLayoutPlaceholder))
        });

        HttpClient client = _server.CreateClient();
        HttpRequestMessage request = new(HttpMethod.Get, new Uri("/", UriKind.Relative));

        // Act
        HttpResponseMessage response = await client.SendAsync(request);

        // Asserts
        string content = await response.Content.ReadAsStringAsync();

        content.Should().Contain("<meta name=\"VIcurrentDateTime\" content=\"");
        content.Should().Contain("<meta name=\"VirtualFolder\" content=\"\\\"/><script type='text/javascript' src='/layouts/system/VisitorIdentification.js'></script>");
    }

    [Fact]
    public async Task SitecoreRenderingHostResponseMetadata_ProxyCookiesWithLSResponse()
    {
        // Arrange
        _mockClientHandler.Responses.Push(new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(Serializer.Serialize(CannedResponses.WithNestedPlaceholder)),
            Headers =
            {
                { "Set-Cookie", AspSessionId },
                { "Set-Cookie", AnalyticsCookie }
            }
        });

        HttpClient client = _server.CreateClient();

        // Act
        HttpRequestMessage browserRequest = new(HttpMethod.Get, new Uri("/", UriKind.Relative));
        browserRequest.Headers.Add("cookie", ["ASP.NET_SessionId=rku2oxmotbrkwkfxe0cpfrvn; path=/; HttpOnly; SameSite=Lax", "SC_ANALYTICS_GLOBAL_COOKIE=0f82f53555ce4304a1ee8ae99ab9f9a8|False; expires = Fri, 15 - Mar - 2030 13:15:08 GMT; path =/; HttpOnly"]);
        browserRequest.Headers.Add("user-agent", "testUserAgentValue");
        browserRequest.Headers.Add("referer", "testRefererValue");
        browserRequest.Headers.Add("x-forwarded-proto", "https");
        browserRequest.Headers.Add("x-original-proto", "https");
        HttpResponseMessage response = await client.SendAsync(browserRequest);

        // Assert
        response.Headers.GetValues("Set-Cookie").Should().Contain(i => i.StartsWith("ASP.NET_SessionId=", StringComparison.OrdinalIgnoreCase));
        response.Headers.GetValues("Set-Cookie").Should().Contain(i => i.StartsWith("SC_ANALYTICS_GLOBAL_COOKIE=", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public async Task SitecoreLayoutServer_ProxyCookiesFromRequest()
    {
        // Arrange
        _mockClientHandler.Responses.Push(new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(Serializer.Serialize(CannedResponses.WithNestedPlaceholder))
        });

        HttpClient client = _server.CreateClient();
        HttpRequestMessage browserRequest = new(HttpMethod.Get, new Uri("/", UriKind.Relative));
        browserRequest.Headers.Add("Cookie", ["ASP.NET_SessionId=rku2oxmotbrkwkfxe0cpfrvn; path=/; HttpOnly; SameSite=Lax", "SC_ANALYTICS_GLOBAL_COOKIE=0f82f53555ce4304a1ee8ae99ab9f9a8|False; expires = Fri, 15 - Mar - 2030 13:15:08 GMT; path =/; HttpOnly"]);

        // Act
        await client.SendAsync(browserRequest);

        HttpRequestMessage lsRequest = _mockClientHandler.Requests.First();

        // Assert
        lsRequest.Headers.GetValues("Cookie").Should().HaveCount(2);
        lsRequest.Headers.GetValues("Cookie").Should().Contain(i => i.Contains("ASP.NET_SessionId=", StringComparison.OrdinalIgnoreCase));
        lsRequest.Headers.GetValues("Cookie").Should().Contain(i => i.Contains("SC_ANALYTICS_GLOBAL_COOKIE=", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public async Task SitecoreLayoutServiceRequest_IPAddress_MustBeResolvedCorrectly()
    {
        // Arrange
        _mockClientHandler.Responses.Push(new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(Serializer.Serialize(CannedResponses.WithNestedPlaceholder))
        });

        HttpClient client = _server.CreateClient();
        HttpRequestMessage request = new(HttpMethod.Get, new Uri("/", UriKind.Relative));
        request.Headers.Add("X-Forwarded-For", "192.168.1.0, 172.217.16.14");

        // Act
        await client.SendAsync(request);
        HttpRequestMessage lsRequest = _mockClientHandler.Requests.First();

        lsRequest.Headers.GetValues("X-Forwarded-For").Should().ContainSingle();
        lsRequest.Headers.Contains("X-Forwarded-For").Should().BeTrue();
        lsRequest.Headers.GetValues("X-Forwarded-For").Should().BeEquivalentTo("172.217.16.14");
    }

    [Fact]
    public async Task SitecoreLayoutServiceResponseMetadata_ProxyCookies()
    {
        // Arrange
        _mockClientHandler.Responses.Push(new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(Serializer.Serialize(CannedResponses.WithNestedPlaceholder)),
            Headers =
            {
                { "Set-Cookie", AspSessionId },
                { "Set-Cookie", AnalyticsCookie }
            }
        });

        HttpClient client = _server.CreateClient();

        // Act
        HttpResponseMessage response = await client.GetAsync(new Uri("/", UriKind.Relative));

        // Assert
        response.Headers.GetValues("Set-Cookie").Should().HaveCount(2);
        response.Headers.GetValues("Set-Cookie").Should().Contain(i => i.StartsWith("ASP.NET_SessionId=", StringComparison.OrdinalIgnoreCase));
        response.Headers.GetValues("Set-Cookie").Should().Contain(i => i.StartsWith("SC_ANALYTICS_GLOBAL_COOKIE=", StringComparison.OrdinalIgnoreCase));
    }

    public void Dispose()
    {
        _server.Dispose();
        _mockClientHandler.Dispose();
        GC.SuppressFinalize(this);
    }
}