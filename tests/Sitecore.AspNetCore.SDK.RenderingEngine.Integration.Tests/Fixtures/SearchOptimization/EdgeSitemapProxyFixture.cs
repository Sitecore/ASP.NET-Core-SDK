using System.Net;
using AwesomeAssertions;
using GraphQL;
using GraphQL.Client.Abstractions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using NSubstitute;
using Sitecore.AspNetCore.SDK.AutoFixture.Mocks;
using Sitecore.AspNetCore.SDK.RenderingEngine.Integration.Tests.Fixtures.Mocks;
using Sitecore.AspNetCore.SDK.SearchOptimization.Extensions;
using Sitecore.AspNetCore.SDK.SearchOptimization.Models;
using Sitecore.AspNetCore.SDK.SearchOptimization.Services;
using Xunit;

namespace Sitecore.AspNetCore.SDK.RenderingEngine.Integration.Tests.Fixtures.SearchOptimization;

public class EdgeSitemapProxyFixture : IClassFixture<TestWebApplicationFactory<TestWebApplicationProgram>>, IDisposable
{
    private readonly MockHttpMessageHandler _mockClientHandler = new();
    private readonly ISitemapService _mockSitemapService = Substitute.For<ISitemapService>();
    private readonly Uri _edgeSitemapUrl = new("https://xmcloud-test.com/sitemap.xml");
    private readonly WebApplicationFactory<TestWebApplicationProgram> _factory;

    public EdgeSitemapProxyFixture(TestWebApplicationFactory<TestWebApplicationProgram> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                services.AddSingleton(_mockSitemapService);

                services.AddSingleton<IHttpClientFactory>(_ =>
                {
                    return new CustomHttpClientFactory(
                        () => new HttpClient(_mockClientHandler));
                });

                IGraphQLClient? mockedGraphQLClient = Substitute.For<IGraphQLClient>();
                mockedGraphQLClient
                    .SendQueryAsync<SiteInfoResultModel>(Arg.Any<GraphQLRequest>())
                    .Returns(new GraphQLResponse<SiteInfoResultModel>
                    {
                        Data = new SiteInfoResultModel
                        {
                            Site = new Site
                            {
                                SiteInfo = new SiteInfo
                                {
                                    Sitemap = new[] { _edgeSitemapUrl.ToString() }
                                }
                            }
                        }
                    });

                services.AddSingleton(mockedGraphQLClient);
                services.AddEdgeSitemap();
            });

            builder.Configure(app =>
            {
                app.UseSitemap();
            });
        });

        // provide a default per-fixture response so concurrent/startup requests don't consume the per-test response
        _mockClientHandler.Responses.Push(new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK
        });

        // Accessing _factory.Server forces the TestServer to start. The variable is unused; this is intentional.
        TestServer startedServer = _factory.Server;
    }

    [Fact]
    public async Task EdgeSitemap_MustBeProxied()
    {
        // Arrange - push per-test response
        _mockClientHandler.Responses.Push(new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK
        });

        _mockSitemapService.GetSitemapUrl(Arg.Any<string>(), Arg.Any<string>())
            .Returns(_edgeSitemapUrl.AbsoluteUri);

        HttpClient client = _factory.CreateClient();
        HttpRequestMessage request = new(HttpMethod.Get, new Uri("/sitemap.xml", UriKind.Relative));

        // Act
        await client.SendAsync(request);

        // Asserts
        _mockClientHandler.Requests.Should().ContainSingle();
        _mockClientHandler.Requests[0].RequestUri!.Host.Should().Be(_edgeSitemapUrl.Host);
        _mockClientHandler.Requests[0].RequestUri!.Scheme.Should().Be(_edgeSitemapUrl.Scheme);
        _mockClientHandler.Requests[0].RequestUri!.PathAndQuery.Should().Be("/sitemap.xml");
    }

    public void Dispose()
    {
        _mockClientHandler.Dispose();
        _factory.Dispose();
        GC.SuppressFinalize(this);
    }
}