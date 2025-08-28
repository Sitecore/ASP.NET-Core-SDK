using System.Net;
using AutoFixture.Xunit2;
using AwesomeAssertions;
using GraphQL;
using GraphQL.Client.Abstractions;
using Microsoft.AspNetCore.TestHost;
using NSubstitute;
using Sitecore.AspNetCore.SDK.AutoFixture.Mocks;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Extensions;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Request;
using Sitecore.AspNetCore.SDK.RenderingEngine.Extensions;
using Sitecore.AspNetCore.SDK.RenderingEngine.Middleware.Models;
using Xunit;

// ReSharper disable StringLiteralTypo
namespace Sitecore.AspNetCore.SDK.RenderingEngine.Integration.Tests.Fixtures.Multisite;

public class MultisiteFixture : IDisposable
{
    private const string DefaultSiteName = "defaultSiteName";
    private readonly TestServer _server;
    private readonly MockHttpMessageHandler _mockClientHandler;
    private readonly Uri _layoutServiceUri = new("http://layout.service");

    public MultisiteFixture()
    {
        TestServerBuilder testHostBuilder = new();
        _mockClientHandler = new MockHttpMessageHandler();
        testHostBuilder
            .ConfigureServices(builder =>
            {
                builder
                    .AddSitecoreLayoutService().WithDefaultRequestOptions(request =>
                    {
                        request
                            .SiteName(DefaultSiteName);
                        if (!request.ContainsKey(RequestKeys.Language))
                        {
                            request.Language("en");
                        }
                    })
                    .AddHttpHandler("mock", _ => new HttpClient(_mockClientHandler) { BaseAddress = _layoutServiceUri })
                    .AsDefaultHandler();

                IGraphQLClient? mockedGraphQLClient = Substitute.For<IGraphQLClient>();
                mockedGraphQLClient.SendQueryAsync<SiteInfoCollectionResult>(Arg.Any<GraphQLRequest>()).Returns(new GraphQLResponse<SiteInfoCollectionResult>
                {
                    Data = new SiteInfoCollectionResult
                    {
                        Site = new Site
                        {
                            SiteInfoCollection =
                            [
                                new SiteInfo { HostName = "host1", Name = "siteForHost1" },
                                new SiteInfo { HostName = "host2", Name = "siteForHost2" },
                                new SiteInfo { HostName = "foo.bar", Name = "fooSite" },
                                new SiteInfo { HostName = "*.test.com", Name = "wildcardSite" },
                                new SiteInfo { HostName = "concrete.test.com", Name = "concrete" },
                                new SiteInfo { HostName = "multiHostname1.test.com | multiHostname2.test.com ", Name = "multiHostNameTestSite" }
                            ]
                        }
                    }
                });

                builder.AddSitecoreRenderingEngine(options =>
                {
                    options.AddDefaultPartialView("_ComponentNotFound");
                });

                builder.AddSingleton(mockedGraphQLClient);
                builder.AddMultisite();
            })
            .Configure(app =>
            {
                app.UseRouting();
                app.UseMultisite();
                app.UseSitecoreRenderingEngine();
                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapFallbackToController("Index", "Multisite");
                });
            });

        _mockClientHandler.Responses.Push(new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK
        });
        _server = testHostBuilder.BuildServer(new Uri("http://localhost"));
    }

    [Theory]
    [InlineData("host1", "siteForHost1")]
    [InlineData("host2", "siteForHost2")]
    [InlineData("foo.bar", "fooSite")]
    [InlineData("new.test.com", "wildcardSite")]
    [InlineData("new.new.test.com", "wildcardSite")]
    [InlineData("concrete.test.com", "concrete")]
    [InlineData("multiHostname1.test.com", "multiHostNameTestSite")]
    [InlineData("multiHostname2.test.com", "multiHostNameTestSite")]
    public async Task Multisite_Should_Resolve_SiteName_ByHostName(string hostname, string expectedSiteName)
    {
        // Arrange
        HttpClient client = _server.CreateClient();
        client.BaseAddress = new Uri($"http://{hostname}");

        // Act
        HttpResponseMessage response = await client.GetAsync("/multisite");
        string responseString = await response.Content.ReadAsStringAsync();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        responseString.Should().Be($"\"{expectedSiteName}\"");
    }

    [Fact]
    public async Task Multisite_Should_Resolve_SiteName_ByQueryParam()
    {
        // Arrange
        HttpClient client = _server.CreateClient();
        const string expectedSiteName = "siteNameFromQueryString";

        // Act
        HttpResponseMessage response = await client.GetAsync($"/multisite?sc_site={expectedSiteName}");
        string responseString = await response.Content.ReadAsStringAsync();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        responseString.Should().Be($"\"{expectedSiteName}\"");
    }

    [Theory]
    [AutoData]
    public async Task Multisite_Should_FallBacks_To_DefaultSite_If_Site_Is_NotResolved(string hostname)
    {
        // Arrange
        HttpClient client = _server.CreateClient();
        client.BaseAddress = new Uri($"http://{hostname}");

        // Act
        HttpResponseMessage response = await client.GetAsync("/multisite");
        string responseString = await response.Content.ReadAsStringAsync();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        responseString.Should().Be($"\"{DefaultSiteName}\"");
    }

    [Theory]
    [InlineData("host1", "fakesite", "siteForHost1")]
    public async Task Multisite_Should_FallBacks_To_DefaultSite_If_Site_Is_NotResolved_OnSecondRequest(string hostnameFirstRequest, string hostnameSecondRequest, string resolvedFirsSite)
    {
        // Arrange
        HttpClient client = _server.CreateClient();

        HttpRequestMessage msg = new()
        {
            RequestUri = new Uri($"http://{hostnameFirstRequest}/multisite"),
        };

        HttpResponseMessage responseFirst = await client.SendAsync(msg);

        // Act
        string responseStringFirst = await responseFirst.Content.ReadAsStringAsync();

        msg.RequestUri = new Uri($"http://{hostnameSecondRequest}/multisite");
        HttpResponseMessage responseSecond = await client.GetAsync("/multisite");
        string responseStringSecond = await responseSecond.Content.ReadAsStringAsync();

        // Assert
        responseFirst.StatusCode.Should().Be(HttpStatusCode.OK);
        responseStringFirst.Should().Be($"\"{resolvedFirsSite}\"");
        responseSecond.StatusCode.Should().Be(HttpStatusCode.OK);
        responseStringSecond.Should().Be($"\"{DefaultSiteName}\"");
    }

    public void Dispose()
    {
        _server.Dispose();
        _mockClientHandler.Dispose();
        GC.SuppressFinalize(this);
    }
}