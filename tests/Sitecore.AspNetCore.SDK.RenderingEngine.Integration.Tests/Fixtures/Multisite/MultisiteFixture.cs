using System.Net;
using AutoFixture.Xunit2;
using AwesomeAssertions;
using GraphQL;
using GraphQL.Client.Abstractions;
using Microsoft.AspNetCore.Mvc.Testing;
using NSubstitute;
using Sitecore.AspNetCore.SDK.AutoFixture.Mocks;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Extensions;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Request;
using Sitecore.AspNetCore.SDK.RenderingEngine.Extensions;
using Sitecore.AspNetCore.SDK.RenderingEngine.Middleware.Models;
using Xunit;

// ReSharper disable StringLiteralTypo
namespace Sitecore.AspNetCore.SDK.RenderingEngine.Integration.Tests.Fixtures.Multisite;

public class MultisiteFixture : IClassFixture<TestWebApplicationFactory<TestWebApplicationProgram>>, IDisposable
{
    private const string DefaultSiteName = "defaultSiteName";
    private readonly MockHttpMessageHandler _mockClientHandler = new();
    private readonly Uri _layoutServiceUri = new("http://layout.service");

    private readonly WebApplicationFactory<TestWebApplicationProgram> _factory;

    public MultisiteFixture(TestWebApplicationFactory<TestWebApplicationProgram> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                IGraphQLClient? mockedGraphQLClient = Substitute.For<IGraphQLClient>();
                mockedGraphQLClient
                    .SendQueryAsync<SiteInfoCollectionResult>(Arg.Any<GraphQLRequest>())
                    .Returns(new GraphQLResponse<SiteInfoCollectionResult>
                    {
                        Data = new SiteInfoCollectionResult
                        {
                            Site = new Site
                            {
                                SiteInfoCollection = new[]
                                {
                                    new SiteInfo { HostName = "host1", Name = "siteForHost1" },
                                    new SiteInfo { HostName = "host2", Name = "siteForHost2" },
                                    new SiteInfo { HostName = "foo.bar", Name = "fooSite" },
                                    new SiteInfo { HostName = "*.test.com", Name = "wildcardSite" },
                                    new SiteInfo { HostName = "concrete.test.com", Name = "concrete" },
                                    new SiteInfo { HostName = "multiHostname1.test.com | multiHostname2.test.com ", Name = "multiHostNameTestSite" }
                                }
                            }
                        }
                    });

                services
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

                services.AddSitecoreRenderingEngine(options =>
                {
                    options.AddDefaultPartialView("_ComponentNotFound");
                });

                services.AddSingleton(mockedGraphQLClient);
                services.AddMultisite();
            });

            builder.Configure(app =>
            {
                app.UseRouting();
                app.UseMultisite();
                app.UseSitecoreRenderingEngine();
                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapFallbackToController("Index", "Multisite");
                });
            });
        });

        // provide a default per-fixture response so startup/concurrent requests don't consume per-test responses
        _mockClientHandler.Responses.Push(new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK
        });

        _ = _factory.Server;
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
        HttpClient client = _factory.CreateClient();
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
        HttpClient client = _factory.CreateClient();
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
        HttpClient client = _factory.CreateClient();
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
        HttpClient client = _factory.CreateClient();

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
        _mockClientHandler.Dispose();
        _factory.Dispose();
        GC.SuppressFinalize(this);
    }
}