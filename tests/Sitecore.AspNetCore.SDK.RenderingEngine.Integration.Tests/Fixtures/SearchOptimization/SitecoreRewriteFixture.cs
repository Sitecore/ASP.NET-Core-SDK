using System.Net;
using FluentAssertions;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Caching.Memory;
using NSubstitute;
using Sitecore.AspNetCore.SDK.SearchOptimization.Extensions;
using Sitecore.AspNetCore.SDK.SearchOptimization.Models;
using Sitecore.AspNetCore.SDK.SearchOptimization.Services;
using Xunit;

namespace Sitecore.AspNetCore.SDK.RenderingEngine.Integration.Tests.Fixtures.SearchOptimization;

public class SitecoreRewriteFixture
{
    [Fact]
    public async Task CheckRewritePath_MultipleRulesWithSkipRemaining()
    {
        using IHost host = new HostBuilder()
            .ConfigureWebHost(webHostBuilder =>
            {
                webHostBuilder
                    .UseTestServer()
                    .Configure(app =>
                    {
                        app.UseSitecoreRedirects();
                        app.Run(context => context.Response.WriteAsync(
                            context.Request.Scheme +
                            "://" +
                            context.Request.Host +
                            context.Request.Path +
                            context.Request.QueryString));
                    });
            }).ConfigureServices(services =>
            {
                services.AddSingleton(_ =>
                {
                    IRedirectsService? mockedRedirectService = Substitute.For<IRedirectsService>();
                    mockedRedirectService.GetRedirects(Arg.Any<string>()).Returns([
                        new RedirectInfo
                        {
                            Pattern = "(.*)", Target = "http://example.com/$1", RedirectType = RedirectType.SERVER_TRANSFER, IsQueryStringPreserved = true
                        },
                        new RedirectInfo
                        {
                            Pattern = "(.*)", Target = "http://example.com/42", RedirectType = RedirectType.SERVER_TRANSFER, IsQueryStringPreserved = true
                        }
                    ]);

                    return mockedRedirectService;
                });

                services.AddSingleton(_ =>
                {
                    IMemoryCache? mockedMemoryCache = Substitute.For<IMemoryCache>();
                    return mockedMemoryCache;
                });
            }).Build();

        await host.StartAsync();

        TestServer server = host.GetTestServer();

        string response = await server.CreateClient().GetStringAsync("foo");

        response.Should().Be("http://example.com/foo");
    }

    [Fact]
    public async Task CheckIfEmptyStringRedirectCorrectly()
    {
        using IHost host = new HostBuilder()
            .ConfigureWebHost(webHostBuilder =>
            {
                webHostBuilder
                    .UseTestServer()
                    .Configure(app =>
                    {
                        app.UseSitecoreRedirects();
                    });
            }).ConfigureServices(services =>
            {
                services.AddSingleton(_ =>
                {
                    IRedirectsService? mockedRedirectService = Substitute.For<IRedirectsService>();
                    mockedRedirectService.GetRedirects(Arg.Any<string>()).Returns([
                        new RedirectInfo
                        {
                            Pattern = "(.*)", Target = "$1", RedirectType = RedirectType.REDIRECT_301
                        }
                    ]);

                    return mockedRedirectService;
                });

                services.AddSingleton(_ =>
                {
                    IMemoryCache? mockedMemoryCache = Substitute.For<IMemoryCache>();
                    return mockedMemoryCache;
                });
            }).Build();

        await host.StartAsync();

        TestServer server = host.GetTestServer();

        HttpResponseMessage response = await server.CreateClient().GetAsync(string.Empty);
        response.Headers.Location!.OriginalString.Should().Be("/");
    }

    [Fact]
    public async Task CheckIfEmptyStringRewriteCorrectly()
    {
        using IHost host = new HostBuilder()
            .ConfigureWebHost(webHostBuilder =>
            {
                webHostBuilder
                    .UseTestServer()
                    .Configure(app =>
                    {
                        app.UseRewriter();
                        app.Run(context => context.Response.WriteAsync(
                            context.Request.Path +
                            context.Request.QueryString));
                    });
            }).ConfigureServices(services =>
            {
                services.AddSingleton(_ =>
                {
                    IRedirectsService? mockedRedirectService = Substitute.For<IRedirectsService>();
                    mockedRedirectService.GetRedirects(Arg.Any<string>()).Returns([
                        new RedirectInfo
                        {
                            Pattern = "(.*)", Target = "$1", RedirectType = RedirectType.SERVER_TRANSFER
                        }
                    ]);

                    return mockedRedirectService;
                });
            }).Build();

        await host.StartAsync();

        TestServer server = host.GetTestServer();

        string response = await server.CreateClient().GetStringAsync(string.Empty);

        response.Should().Be("/");
    }

    [Theory]
    [InlineData("(.*)", "http://example.com/$1", true, "http://example.com", "path", "http://example.com/path")]
    [InlineData("(.*)", "http://example.com", true, "http://example.com/", "", "http://example.com/")]
    [InlineData("path/(.*)", "path?value=$1", true, null, "path/value", "/path?value=value")]
    [InlineData("path/(.*)", "path?param=$1", true, null, "path/value?param1=OtherValue", "/path?param1=OtherValue&param=value")]
    [InlineData("path/(.*)", "http://example.com/pathBase/path?param=$1", true, "http://example.com", "path/value?param1=OtherValue", "http://example.com/pathBase/path?param=value&param1=OtherValue")]
    [InlineData("^/ab[cd]/$", "graphql", true, null, "abc", "/graphql")]
    [InlineData("/bro/", "graphql", true, null, "bro", "/graphql")]
    [InlineData("/bro/(.*)", "graphql", true, null, "bro/add", "/graphql")]
    [InlineData("bro/(.*)", "graphql", true, null, "bro/add", "/graphql")]
    [InlineData("/bro", "graphql", false, null, "bro?sc_test=1", "/graphql")]
    [InlineData("/bro", "graphql", true, null, "bro?sc_test=1", "/graphql?sc_test=1")]
    [InlineData("/bro", "graphql?sc_test2=2", true, null, "bro?sc_test=1", "/graphql?sc_test=1&sc_test2=2")]
    [InlineData("/bro", "graphql?sc_test2=2", false, null, "bro?sc_test=1", "/graphql?sc_test2=2")]
    internal async Task CheckRewritePath(string pattern, string replacement, bool isQueryStringPreserved, string? baseAddress, string requestUrl, string expectedUrl)
    {
        using IHost host = new HostBuilder()
            .ConfigureWebHost(webHostBuilder =>
            {
                webHostBuilder
                    .UseTestServer()
                    .Configure(app =>
                    {
                        app.UseSitecoreRedirects();
                        app.Run(context => context.Response.WriteAsync(
                            baseAddress! +
                            context.Request.Path +
                            context.Request.QueryString));
                    });
            }).ConfigureServices(services =>
            {
                services.AddSingleton(_ =>
                {
                    IRedirectsService? mockedRedirectService = Substitute.For<IRedirectsService>();
                    mockedRedirectService.GetRedirects(Arg.Any<string>()).Returns([
                        new RedirectInfo
                        {
                            Pattern = pattern, Target = replacement, RedirectType = RedirectType.SERVER_TRANSFER, IsQueryStringPreserved = isQueryStringPreserved
                        }
                    ]);

                    return mockedRedirectService;
                });

                services.AddSingleton(_ =>
                {
                    IMemoryCache? mockedMemoryCache = Substitute.For<IMemoryCache>();
                    return mockedMemoryCache;
                });
            }).Build();

        await host.StartAsync();

        TestServer server = host.GetTestServer();

        string response = await server.CreateClient().GetStringAsync(requestUrl);

        response.Should().Be(expectedUrl);
    }

    [Theory]
    [InlineData("(.*)", "http://example.com/$1", RedirectType.REDIRECT_301, true, null, "path", "http://example.com/path")]
    [InlineData("(.*)", "http://example.com", RedirectType.REDIRECT_301, true, null, "", "http://example.com/")]
    [InlineData("path/(.*)", "path?value=$1", RedirectType.REDIRECT_301, true, null, "path/value", "/path?value=value")]
    [InlineData("path/(.*)", "path?param=$1", RedirectType.REDIRECT_301, true, null, "path/value?param1=OtherValue", "/path?param1=OtherValue&param=value")]
    [InlineData("path/(.*)", "http://example.com/pathBase/path?param=$1", RedirectType.REDIRECT_301, true, "http://example.com/pathBase", "path/value?param1=OtherValue", "http://example.com/pathBase/path?param1=OtherValue&param=value")]
    [InlineData("path/(.*)", "http://hoψst.com/pÂthBase/path?parãm=$1", RedirectType.REDIRECT_301, true, "http://example.com/pathBase", "path/value?päram1=OtherValüe", "http://xn--host-cpd.com/p%C3%82thBase/path?p%C3%A4ram1=OtherVal%C3%BCe&parãm=value")]
    [InlineData("(.*)", "http://example.com/$1", RedirectType.REDIRECT_302, true, null, "path", "http://example.com/path")]
    [InlineData("^/ab[cd]/$", "graphql", RedirectType.REDIRECT_301, true, null, "abc", "/graphql")]
    [InlineData("/bro/", "graphql", RedirectType.REDIRECT_301, true, null, "bro", "/graphql")]
    [InlineData("/bro/(.*)", "graphql", RedirectType.REDIRECT_301, true, null, "bro/add", "/graphql")]
    [InlineData("bro/(.*)", "graphql", RedirectType.REDIRECT_301, true, null, "bro/add", "/graphql")]
    [InlineData("/bro", "graphql", RedirectType.REDIRECT_301, false, null, "bro?sc_test=1", "/graphql")]
    [InlineData("/bro", "graphql", RedirectType.REDIRECT_301, true, null, "bro?sc_test=1", "/graphql?sc_test=1")]
    [InlineData("/bro", "graphql?sc_test2=2", RedirectType.REDIRECT_301, true, null, "bro?sc_test=1", "/graphql?sc_test=1&sc_test2=2")]
    [InlineData("/bro", "graphql?sc_test2=2", RedirectType.REDIRECT_301, false, null, "bro?sc_test=1", "/graphql?sc_test2=2")]
    internal async Task CheckRedirectPath(string pattern, string replacement, RedirectType redirectType, bool isQueryStringPreserved, string? baseAddress, string requestUrl, string expectedUrl)
    {
        using IHost host = new HostBuilder()
            .ConfigureWebHost(webHostBuilder =>
            {
                webHostBuilder
                    .UseTestServer()
                    .Configure(app =>
                    {
                        app.UseSitecoreRedirects();
                    });
            }).ConfigureServices(services =>
            {
                services.AddSingleton(_ =>
                {
                    IRedirectsService? mockedRedirectService = Substitute.For<IRedirectsService>();
                    mockedRedirectService.GetRedirects(Arg.Any<string>()).Returns([
                        new RedirectInfo
                        {
                            Pattern = pattern, Target = replacement, RedirectType = redirectType, IsQueryStringPreserved = isQueryStringPreserved
                        }
                    ]);

                    return mockedRedirectService;
                });

                services.AddSingleton(_ =>
                {
                    IMemoryCache? mockedMemoryCache = Substitute.For<IMemoryCache>();
                    return mockedMemoryCache;
                });
            }).Build();

        await host.StartAsync();

        TestServer server = host.GetTestServer();
        if (!string.IsNullOrEmpty(baseAddress))
        {
            server.BaseAddress = new Uri(baseAddress);
        }

        HttpResponseMessage response = await server.CreateClient().GetAsync(requestUrl);

        HttpStatusCode expectedRedirectCode = redirectType == RedirectType.REDIRECT_301 ? HttpStatusCode.MovedPermanently : HttpStatusCode.Redirect;
        response.StatusCode.Should().Be(expectedRedirectCode);
        response.Headers.Location!.OriginalString.Should().Be(expectedUrl);
    }
}