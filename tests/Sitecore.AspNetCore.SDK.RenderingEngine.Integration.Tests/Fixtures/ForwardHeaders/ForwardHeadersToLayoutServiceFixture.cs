using System.Net;
using FluentAssertions;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.TestHost;
using Sitecore.AspNetCore.SDK.AutoFixture.Mocks;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Extensions;
using Sitecore.AspNetCore.SDK.RenderingEngine.Extensions;
using Sitecore.AspNetCore.SDK.TestData;
using Xunit;

// ReSharper disable StringLiteralTypo
namespace Sitecore.AspNetCore.SDK.RenderingEngine.Integration.Tests.Fixtures.ForwardHeaders;

public class ForwardHeadersToLayoutServiceFixture : IDisposable
{
    private const string TestHeaderRhResponse = "testHeaderResponseFromRenderingHost";
    private readonly TestServer _server;
    private readonly MockHttpMessageHandler _mockClientHandler;
    private readonly Uri _layoutServiceUri = new("http://layout.service");

    public ForwardHeadersToLayoutServiceFixture()
    {
        TestServerBuilder testHostBuilder = new();
        _mockClientHandler = new MockHttpMessageHandler();

        _ = testHostBuilder
            .ConfigureServices(builder =>
            {
                builder.Configure<ForwardedHeadersOptions>(options =>
                {
                    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor |
                                               ForwardedHeaders.XForwardedProto;
                });

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
                }).ForwardHeaders(options =>
                {
                    options.HeadersWhitelist.Add("HEADERTOCOPY");
                    options.HeadersWhitelist.Add("Cookie");
                    options.RequestHeadersFilters.Add(
                        (_, result) =>
                        {
                            result.AppendValue("testNonWhitelistedHeader", "testNonWhitelistedHeaderValue");
                            result.AppendValue("headerToModify", "newModifiedHeaderValue");
                            result.AppendValue("cookie", "NewAddedCookie=rku2oxmotbrkwkfxe0cpfrvn; path=/; HttpOnly; SameSite=Lax");
                        });

                    options.ResponseHeadersFilters.Add(
                        (_, result) =>
                        {
                            result.AppendValue(TestHeaderRhResponse, "testHeaderResponseValueFromRenderingHost");
                        });
                });
            })
            .Configure(app =>
            {
                app.UseForwardedHeaders();
                app.UseSitecoreRenderingEngine();
            });

        _server = testHostBuilder.BuildServer(new Uri("http://localhost"));
    }

    [Fact]
    public async Task SitecoreLayoutServiceRequest_FiltersHeaders()
    {
        // Arrange
        _mockClientHandler.Responses.Push(new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(Serializer.Serialize(CannedResponses.WithNestedPlaceholder)),
        });

        HttpClient client = _server.CreateClient();
        HttpRequestMessage request = BrowserWhitelistedHeaders();
        request.Headers.Add("connection", string.Empty);
        request.Headers.Add("keep-alive", "sometestvalueshere");
        request.Headers.Add("public", "sometestvalueshere");
        request.Headers.Add("proxy-authenticate", "sometestvalueshere");
        request.Headers.Add("transfer-encoding", "sometestvalueshere");
        request.Headers.Add("upgrade", "sometestvalueshere");

        // Act
        _ = await client.SendAsync(request);
        HttpRequestMessage lsRequest = _mockClientHandler.Requests.First();

        // Assert
        lsRequest.Headers.Contains("x-forwarded-proto").Should().BeTrue();
        lsRequest.Headers.GetValues("x-forwarded-proto").Should().ContainSingle();
        lsRequest.Headers.GetValues("x-forwarded-proto").First().Should().Be("https");

        lsRequest.Headers.Contains("connection").Should().BeFalse();
        lsRequest.Headers.Contains("keep-alive").Should().BeFalse();
        lsRequest.Headers.Contains("public").Should().BeFalse();
        lsRequest.Headers.Contains("proxy-authenticate").Should().BeFalse();
        lsRequest.Headers.Contains("transfer-encoding").Should().BeFalse();
        lsRequest.Headers.Contains("upgrade").Should().BeFalse();
    }

    [Fact]
    public async Task SitecoreLayoutServiceRequest_FiltersHeaders_CaseSensitive()
    {
        // Arrange
        _mockClientHandler.Responses.Push(new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(Serializer.Serialize(CannedResponses.WithNestedPlaceholder))
        });

        HttpClient client = _server.CreateClient();
        HttpRequestMessage request = new(HttpMethod.Get, new Uri("/", UriKind.Relative));
        request.Headers.Add("COOKIE", "testValue");

        // Act
        _ = await client.SendAsync(request);
        HttpRequestMessage lsRequest = _mockClientHandler.Requests.First();

        // Assert
        lsRequest.Headers.Contains("cookie").Should().BeTrue();
    }

    [Fact]
    public async Task SitecoreLayoutServiceRequest_FiltersHeaders_NonWhitelistedHeaderAdded()
    {
        // Arrange
        _mockClientHandler.Responses.Push(new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(Serializer.Serialize(CannedResponses.WithNestedPlaceholder))
        });

        HttpClient client = _server.CreateClient();
        HttpRequestMessage request = new(HttpMethod.Get, new Uri("/", UriKind.Relative));
        request.Headers.Add("testNonWhitelistedHeader", "testNonWhitelistedHeaderValue");

        // Act
        _ = await client.SendAsync(request);
        HttpRequestMessage lsRequest = _mockClientHandler.Requests.First();

        // Assert
        lsRequest.Headers.Contains("testNonWhitelistedHeader").Should().BeTrue();
    }

    [Fact]
    public async Task SitecoreLayoutServiceRequest_FiltersHeadersNegativeCasesEmptyHeaderValue()
    {
        _mockClientHandler.Responses.Push(new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(Serializer.Serialize(CannedResponses.WithNestedPlaceholder))
        });

        HttpClient client = _server.CreateClient();
        HttpRequestMessage request = BrowserWhitelistedHeaders();
        request.Headers.Add("connection", string.Empty);

        // Act
        _ = await client.SendAsync(request);
        HttpRequestMessage lsRequest = _mockClientHandler.Requests.First();

        // Assert
        lsRequest.Headers.Contains("cookie").Should().BeTrue();
        lsRequest.Headers.Contains("connection").Should().BeFalse();
    }

    [Fact]
    public async Task SitecoreResponse_Headers_CopiedToResponse_AreFilteredAndModified()
    {
        // Arrange
        HttpResponseMessage responseMsg = new()
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(Serializer.Serialize(
                CannedResponses.WithVisitorIdentificationLayoutPlaceholder))
        };

        _mockClientHandler.Responses.Push(responseMsg);

        HttpClient client = _server.CreateClient();
        HttpRequestMessage request = BrowserWhitelistedHeaders();
        request.Headers.Add("headerToModify", "oldHeaderValue");
        request.Headers.Add("HEADERTOCOPY", "sometestvalueshere");

        // Act
        _ = await client.SendAsync(request);
        HttpRequestMessage lsRequest = _mockClientHandler.Requests.First();

        // Asserts
        lsRequest.Headers.GetValues("headerToModify").Should().BeEquivalentTo("newModifiedHeaderValue");
        lsRequest.Headers.Contains("cookie").Should().BeTrue();
        lsRequest.Headers.Contains("HEADERTOCOPY").Should().BeTrue();
    }

    [Fact]
    public async Task SitecoreResponse_Headers_CopiedToResponse_WhitelistedFilteredAndModified()
    {
        // Arrange
        HttpResponseMessage responseMsg = new()
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(Serializer.Serialize(
                CannedResponses.WithVisitorIdentificationLayoutPlaceholder))
        };

        _mockClientHandler.Responses.Push(responseMsg);

        HttpClient client = _server.CreateClient();
        HttpRequestMessage request = BrowserWhitelistedHeaders();
        request.Headers.Add("Cookie", ["ASP.NET_SessionId=rku2oxmotbrkwkfxe0cpfrvn; path=/; HttpOnly; SameSite=Lax", "SC_ANALYTICS_GLOBAL_COOKIE=0f82f53555ce4304a1ee8ae99ab9f9a8|False; expires = Fri, 15 - Mar - 2030 13:15:08 GMT; path =/; HttpOnly"]);

        // Act
        _ = await client.SendAsync(request);
        HttpRequestMessage lsRequest = _mockClientHandler.Requests.First();

        // Asserts
        lsRequest.Headers.GetValues("Cookie").Should().Contain(i => i.Contains("SC_ANALYTICS_GLOBAL_COOKIE=", StringComparison.OrdinalIgnoreCase));
        lsRequest.Headers.GetValues("Cookie").Should().Contain(i => i.Contains("ASP.NET_SessionId=", StringComparison.OrdinalIgnoreCase));
        lsRequest.Headers.GetValues("Cookie").Should().Contain(i => i.Contains("NewAddedCookie=", StringComparison.OrdinalIgnoreCase));
    }

    public void Dispose()
    {
        _server.Dispose();
        _mockClientHandler.Dispose();
        GC.SuppressFinalize(this);
    }

    private static HttpRequestMessage BrowserWhitelistedHeaders()
    {
        HttpRequestMessage request = new(HttpMethod.Get, new Uri("/", UriKind.Relative));

        // whitelisted Headers
        request.Headers.Add("x-forwarded-proto", "https");

        return request;
    }
}