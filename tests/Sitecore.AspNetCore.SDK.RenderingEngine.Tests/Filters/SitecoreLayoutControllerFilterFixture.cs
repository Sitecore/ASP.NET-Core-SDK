using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Sitecore.AspNetCore.SDK.AutoFixture.Attributes;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Response;
using Sitecore.AspNetCore.SDK.RenderingEngine.Extensions;
using Sitecore.AspNetCore.SDK.RenderingEngine.Filters;
using Sitecore.AspNetCore.SDK.RenderingEngine.Interfaces;
using Sitecore.AspNetCore.SDK.RenderingEngine.Rendering;
using Sitecore.AspNetCore.SDK.TestData;
using Xunit;

namespace Sitecore.AspNetCore.SDK.RenderingEngine.Tests.Filters;

public class SitecoreLayoutControllerFilterFixture
{
    // ReSharper disable once UnusedMember.Global - Used by testing framework
    public static Action<IFixture> AutoSetup => f =>
    {
        SitecoreRenderingContext sitecoreLayoutFeature = new()
        {
            Response = new SitecoreLayoutResponse([]) { Content = CannedResponses.StyleGuide1 }
        };

        FeatureCollection featureCollection = new();
        featureCollection.Set<ISitecoreRenderingContext>(sitecoreLayoutFeature);

        f.Inject(new BindingInfo());
        f.Inject<HttpContext>(new DefaultHttpContext(featureCollection));
    };

    [Theory]
    [AutoNSubstituteData]
    public void ASitecoreLayoutControllerFilter_GuardedAsync(SitecoreLayoutContextControllerFilter sut)
    {
        Action act =
            () => sut.OnActionExecuting(null!);

        act.Should().Throw<ArgumentNullException>();
    }

    [Theory]
    [AutoNSubstituteData]
    public void ApplicationBuilderExtensions_ReturnsTheRightFeature(
        SitecoreLayoutContextControllerFilter sut,
        ActionExecutingContext context,
        HttpContext httpContext)
    {
        // act
        sut.OnActionExecuting(context);

        // assert
        httpContext.GetSitecoreRenderingContext()!.Controller.Should().Be(context.Controller as ControllerBase);
    }
}