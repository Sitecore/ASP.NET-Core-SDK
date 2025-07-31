using System.Net;
using AwesomeAssertions;
using Microsoft.AspNetCore.TestHost;
using Sitecore.AspNetCore.SDK.AutoFixture.Mocks;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Extensions;
using Sitecore.AspNetCore.SDK.RenderingEngine.Extensions;
using Sitecore.AspNetCore.SDK.TestData;
using Sitecore.AspNetCore.SDK.Tracking;
using Sitecore.AspNetCore.SDK.Tracking.VisitorIdentification;
using Xunit;

// ReSharper disable StringLiteralTypo
namespace Sitecore.AspNetCore.SDK.RenderingEngine.Integration.Tests.Fixtures.Tracking;

public class AttributeBasedTrackingFixture : IDisposable
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

    private readonly MockHttpMessageHandler _mockClientHandler;

    private readonly Uri _layoutServiceUri = new("http://layout.service");

    private readonly Uri _cmInstanceUri = new("http://layout.service");

    public AttributeBasedTrackingFixture()
    {
        TestServerBuilder testHostBuilder = new();
        _mockClientHandler = new MockHttpMessageHandler();

        _ = testHostBuilder
            .ConfigureServices(builder =>
            {
                builder
                    .AddSitecoreLayoutService()
                    .AddHttpHandler("mock", _ => new HttpClient(_mockClientHandler)
                    {
                        BaseAddress = _layoutServiceUri
                    })
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
                app.UseSitecoreVisitorIdentification();
            });

        _server = testHostBuilder.BuildServer(new Uri("http://localhost"));
    }

    [Fact]
    public async Task SitecoreLayoutServiceResponseMetadata_ProxyCookies()
    {
        // Arrange
        _mockClientHandler.Responses.Push(new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(Serializer.Serialize(CannedResponses.WithVisitorIdentificationLayoutPlaceholder)),
            Headers =
            {
                { "Set-Cookie", AspSessionId },
                { "Set-Cookie", AnalyticsCookie }
            }
        });

        HttpClient client = _server.CreateClient();

        // Act
        HttpResponseMessage response = await client.GetAsync(new Uri("/AttributeBased", UriKind.Relative));

        response.Headers.GetValues("Set-Cookie").Should().HaveCount(2);
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
            Content = new StringContent(Serializer.Serialize(CannedResponses.WithVisitorIdentificationLayoutPlaceholder)),
        });

        HttpClient client = _server.CreateClient();
        HttpRequestMessage request = new(HttpMethod.Get, new Uri("/AttributeBased", UriKind.Relative));
        request.Headers.Add("Cookie", ["ASP.NET_SessionId=rku2oxmotbrkwkfxe0cpfrvn; path=/; HttpOnly; SameSite=Lax", "SC_ANALYTICS_GLOBAL_COOKIE=0f82f53555ce4304a1ee8ae99ab9f9a8|False; expires = Fri, 15 - Mar - 2030 13:15:08 GMT; path =/; HttpOnly"]);

        // Act
        await client.SendAsync(request);

        HttpRequestMessage lsRequest = _mockClientHandler.Requests.First();

        lsRequest.Headers.GetValues("Cookie").Should().HaveCount(2);
        lsRequest.Headers.GetValues("Cookie").Should().Contain(i => i.Contains("ASP.NET_SessionId=", StringComparison.OrdinalIgnoreCase));
        lsRequest.Headers.GetValues("Cookie").Should().Contain(i => i.Contains("SC_ANALYTICS_GLOBAL_COOKIE=", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public async Task SitecoreRequests_ToLayouts_MustIncludeVisitorIdentificationJs()
    {
        // Arrange
        _mockClientHandler.Responses.Push(new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(Serializer.Serialize(CannedResponses.WithVisitorIdentificationLayoutPlaceholder)),
        });

        HttpClient client = _server.CreateClient();
        HttpRequestMessage request = new(HttpMethod.Get, new Uri("/AttributeBased", UriKind.Relative));

        // Act
        HttpResponseMessage response = await client.SendAsync(request);

        // Asserts
        string content = await response.Content.ReadAsStringAsync();

        content.Should().Contain("<script");
    }

    public void Dispose()
    {
        _server.Dispose();
        _mockClientHandler.Dispose();
        GC.SuppressFinalize(this);
    }
}