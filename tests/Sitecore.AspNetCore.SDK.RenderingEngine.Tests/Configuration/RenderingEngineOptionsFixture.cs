using AutoFixture;
using AwesomeAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using NSubstitute;
using Sitecore.AspNetCore.SDK.AutoFixture.Attributes;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Request;
using Sitecore.AspNetCore.SDK.RenderingEngine.Configuration;
using Xunit;

namespace Sitecore.AspNetCore.SDK.RenderingEngine.Tests.Configuration;

public class RenderingEngineOptionsFixture
{
    private readonly RenderingEngineOptions _sut = new();

    // ReSharper disable once UnusedMember.Global - Used by testing framework
    public static Action<IFixture> AutoSetup => f =>
    {
        f.Register<string, PathString>(s => new PathString("/" + s));
    };

    [Fact]
    public void Ctor_RendererRegistry_IsEmpty()
    {
        _sut.RendererRegistry.Should().BeEmpty();
    }

    [Theory]
    [AutoNSubstituteData]
    public void Ctor_MapToRequest_SetsPathByDefault(HttpRequest http)
    {
        // Arrange
        SitecoreLayoutRequest scRequest = [];

        // Act
        _sut.RequestMappings.Single().Invoke(http, scRequest);

        // Assert
        scRequest.Path().Should().Be(http.Path);
    }

    [Theory]
    [AutoNSubstituteData]
    public void Ctor_MapToRequest_AcceptsCulture(HttpRequest http)
    {
        // Arrange
        PathString path = http.Path;
        http.Path = "/da" + http.Path;
        http.RouteValues.Add("culture", "da");
        http.RouteValues.Add(RenderingEngineConstants.RouteValues.SitecoreRoute, path);

        SitecoreLayoutRequest scRequest = [];

        // Act
        _sut.RequestMappings.Single().Invoke(http, scRequest);

        // Assert
        scRequest.Path().Should().Be(path);
    }

    [Theory]
    [AutoNSubstituteData]
    public void Ctor_MapToRequest_AcceptCulture_From_RequestCultureFeature(HttpRequest http)
    {
        // Arrange
        string requestedCulture = "da";
        http.Path = http.Path;

        SitecoreLayoutRequest scRequest = [];
        IRequestCultureFeature? requestCultureFeature = Substitute.For<IRequestCultureFeature>();
        requestCultureFeature.RequestCulture.Returns(new RequestCulture(requestedCulture));
        http.HttpContext.Features.Get<IRequestCultureFeature>().Returns(requestCultureFeature);

        // Act
        _sut.RequestMappings.Single().Invoke(http, scRequest);

        // Assert
        scRequest.Language().Should().Be(requestedCulture);
    }
}