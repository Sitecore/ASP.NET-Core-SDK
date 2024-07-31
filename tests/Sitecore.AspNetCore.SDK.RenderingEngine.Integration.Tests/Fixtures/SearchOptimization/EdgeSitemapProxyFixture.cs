using System.Net;
using FluentAssertions;
using GraphQL;
using GraphQL.Client.Abstractions;
using Microsoft.AspNetCore.TestHost;
using NSubstitute;
using Sitecore.AspNetCore.SDK.RenderingEngine.Integration.Tests.Fixtures.Mocks;
using Sitecore.AspNetCore.SDK.SearchOptimization.Extensions;
using Sitecore.AspNetCore.SDK.SearchOptimization.Models;
using Sitecore.AspNetCore.SDK.SearchOptimization.Services;
using Xunit;

namespace Sitecore.AspNetCore.SDK.RenderingEngine.Integration.Tests.Fixtures.SearchOptimization;

public class EdgeSitemapProxyFixture : IDisposable
{
    private readonly TestServer _server;
    private readonly HttpLayoutClientMessageHandler _mockClientHandler = new();
    private readonly ISitemapService _mockSitemapService = Substitute.For<ISitemapService>();
    private readonly Uri _edgeSitemapUrl = new("https://xmcloud-test.com/sitemap.xml");

    public EdgeSitemapProxyFixture()
    {
        _mockClientHandler.Responses.Push(new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK
        });

        TestServerBuilder testHostBuilder = new();
        _ = testHostBuilder
            .ConfigureServices(builder =>
            {
                builder.AddSingleton(_mockSitemapService);
                builder.AddSingleton<IHttpClientFactory>(_ =>
                {
                    return new CustomHttpClientFactory(
                        () =>
                            new HttpClient(_mockClientHandler));
                });

                IGraphQLClient? mockedGraphQlClient = Substitute.For<IGraphQLClient>();
                mockedGraphQlClient.SendQueryAsync<SiteInfoResultModel>(Arg.Any<GraphQLRequest>()).Returns(new GraphQLResponse<SiteInfoResultModel>
                {
                    Data = new SiteInfoResultModel
                    {
                        Site = new Site
                        {
                            SiteInfo = new SiteInfo
                            {
                                Sitemap =
                                [
                                    _edgeSitemapUrl.ToString()
                                ]
                            }
                        }
                    }
                });

                builder.AddSingleton(mockedGraphQlClient);
                builder.AddEdgeSitemap();
            })
            .Configure(app =>
            {
                app.UseSitemap();
            });

        _server = testHostBuilder.BuildServer(new Uri("http://localhost"));
    }

    [Fact]
    public async Task EdgeSitemap_MustBeProxied()
    {
        // Arrange
        HttpClient client = _server.CreateClient();
        HttpRequestMessage request = new(HttpMethod.Get, new Uri("/sitemap.xml", UriKind.Relative));
        _mockSitemapService.GetSitemapUrl(Arg.Any<string>(), Arg.Any<string>())
            .Returns(_edgeSitemapUrl.AbsoluteUri);

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
        _server.Dispose();
        _mockClientHandler.Dispose();
        GC.SuppressFinalize(this);
    }
}