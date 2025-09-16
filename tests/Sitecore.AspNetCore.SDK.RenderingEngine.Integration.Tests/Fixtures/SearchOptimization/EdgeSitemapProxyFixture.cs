using System.Net;
using AwesomeAssertions;
using GraphQL;
using GraphQL.Client.Abstractions;
using Microsoft.AspNetCore.Mvc.Testing;
using NSubstitute;
using Sitecore.AspNetCore.SDK.AutoFixture.Mocks;
using Sitecore.AspNetCore.SDK.RenderingEngine.Integration.Tests.Fixtures.Mocks;
using Sitecore.AspNetCore.SDK.SearchOptimization.Extensions;
using Sitecore.AspNetCore.SDK.SearchOptimization.Models;
using Sitecore.AspNetCore.SDK.SearchOptimization.Services;
using Xunit;

namespace Sitecore.AspNetCore.SDK.RenderingEngine.Integration.Tests.Fixtures.SearchOptimization;

public class EdgeSitemapProxyFixture(TestWebApplicationFactory<TestWebApplicationProgram> factory) : IClassFixture<TestWebApplicationFactory<TestWebApplicationProgram>>, IDisposable
{
    private readonly MockHttpMessageHandler _mockClientHandler = new();
    private readonly ISitemapService _mockSitemapService = Substitute.For<ISitemapService>();
    private readonly Uri _edgeSitemapUrl = new("https://xmcloud-test.com/sitemap.xml");

    [Fact]
    public async Task EdgeSitemap_MustBeProxied()
    {
        // Arrange
        HttpClient client = BuildEdgeSitemapWebApplicationFactory().CreateClient();
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
        _mockClientHandler.Dispose();
        GC.SuppressFinalize(this);
    }

    private WebApplicationFactory<TestWebApplicationProgram> BuildEdgeSitemapWebApplicationFactory()
    {
        _mockClientHandler.Responses.Push(new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK
        });

        return factory.WithWebHostBuilder(builder =>
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
    }
}