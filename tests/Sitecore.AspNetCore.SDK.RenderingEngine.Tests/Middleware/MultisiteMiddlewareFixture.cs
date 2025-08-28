using AwesomeAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using NSubstitute;
using Sitecore.AspNetCore.SDK.AutoFixture.Attributes;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Request;
using Sitecore.AspNetCore.SDK.RenderingEngine.Configuration;
using Sitecore.AspNetCore.SDK.RenderingEngine.Extensions;
using Sitecore.AspNetCore.SDK.RenderingEngine.Middleware;
using Sitecore.AspNetCore.SDK.RenderingEngine.Services;
using Xunit;

namespace Sitecore.AspNetCore.SDK.RenderingEngine.Tests.Middleware;

public class MultisiteMiddlewareFixture
{
    private readonly RequestDelegate _next;
    private readonly IOptions<RenderingEngineOptions> _renderingEngineOptions;
    private readonly ISiteResolver _siteResolver;
    private readonly IMemoryCache _memoryCache;
    private readonly IOptions<MultisiteOptions> _multisiteOptions;

    public MultisiteMiddlewareFixture()
    {
        _next = Substitute.For<RequestDelegate>();
        _renderingEngineOptions = Substitute.For<IOptions<RenderingEngineOptions>>();
        _renderingEngineOptions.Value.Returns(new RenderingEngineOptions());
        _siteResolver = Substitute.For<ISiteResolver>();
        _memoryCache = Substitute.For<IMemoryCache>();
        _multisiteOptions = Substitute.For<IOptions<MultisiteOptions>>();
        _multisiteOptions.Value.Returns(new MultisiteOptions());
    }

    [Theory]
    [AutoNSubstituteData]
    internal async Task Invoke_SetSiteNameFromSiteResolving(HttpRequest httpRequest, string expectedSiteName)
    {
        // Arrange
        httpRequest.HttpContext.Request.Returns(httpRequest);
        HttpContext httpContext = httpRequest.HttpContext;
        MultisiteMiddleware sut = new(_next, _siteResolver, _memoryCache, _renderingEngineOptions, _multisiteOptions);
        _siteResolver.GetByHost(httpRequest.Host.Value).Returns(expectedSiteName);
        httpRequest.Query.TryGetValue(Arg.Any<string>(), out _).Returns(false);

        // Act
        await sut.Invoke(httpRequest.HttpContext);
        httpContext.TryGetResolvedSiteName(out string? resolvedSiteName);
        SitecoreLayoutRequest sitecoreLayoutRequest = [];
        _renderingEngineOptions.Value.RequestMappings.LastOrDefault()?.Invoke(httpRequest, sitecoreLayoutRequest);

        // Assert
        resolvedSiteName.Should().Be(expectedSiteName);
        await _next.Received(1).Invoke(httpContext);
        sitecoreLayoutRequest.SiteName().Should().Be(expectedSiteName);
        await _siteResolver.Received(1).GetByHost(httpRequest.Host.Value);
    }

    [Theory]
    [AutoNSubstituteData]
    internal async Task Invoke_SetSiteNameFromQueryString(HttpRequest httpRequest, string expectedSiteName)
    {
        // Arrange
        httpRequest.HttpContext.Request.Returns(httpRequest);
        HttpContext httpContext = httpRequest.HttpContext;
        MultisiteMiddleware sut = new(_next, _siteResolver, _memoryCache, _renderingEngineOptions, _multisiteOptions);

        httpRequest.QueryString = new QueryString($"?sc_site={expectedSiteName}");

        Dictionary<string, StringValues> queryDictionary = new()
        {
            {
                "sc_site", expectedSiteName
            }
        };
        httpRequest.Query = new QueryCollection(queryDictionary);

        // Act
        await sut.Invoke(httpContext);
        httpContext.TryGetResolvedSiteName(out string? resolvedSiteName);
        SitecoreLayoutRequest sitecoreLayoutRequest = [];
        _renderingEngineOptions.Value.RequestMappings.LastOrDefault()?.Invoke(httpRequest, sitecoreLayoutRequest);

        // Assert
        resolvedSiteName.Should().Be(expectedSiteName);
        await _next.Received(1).Invoke(httpContext);
        sitecoreLayoutRequest.SiteName().Should().Be(expectedSiteName);
        await _siteResolver.Received(0).GetByHost(httpRequest.Host.Value);
    }

    [Theory]
    [AutoNSubstituteData]
    internal async Task Invoke_DoesNotSetSiteNameIfItIsNotResolved(HttpContext httpContext, string defaultSiteName)
    {
        // Arrange
        httpContext.Request.HttpContext.Returns(httpContext);
        MultisiteMiddleware sut = new(_next, _siteResolver, _memoryCache, _renderingEngineOptions, _multisiteOptions);
        httpContext.Request.Query.TryGetValue(Arg.Any<string>(), out _).Returns(false);

        // Act
        await sut.Invoke(httpContext);
        httpContext.TryGetResolvedSiteName(out string? resolvedSiteName);
        SitecoreLayoutRequest sitecoreLayoutRequest = [];
        sitecoreLayoutRequest.SiteName(defaultSiteName);
        _renderingEngineOptions.Value.RequestMappings.LastOrDefault()?.Invoke(httpContext.Request, sitecoreLayoutRequest);

        // Assert
        resolvedSiteName.Should().Be(null);
        sitecoreLayoutRequest.SiteName().Should().Be(defaultSiteName);
        await _next.Received(1).Invoke(httpContext);
        await _siteResolver.Received(1).GetByHost(httpContext.Request.Host.Value);
    }
}