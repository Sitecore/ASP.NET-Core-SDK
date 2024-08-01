using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using NSubstitute;
using Sitecore.AspNetCore.SDK.AutoFixture.Attributes;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Response;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Response.Model;
using Sitecore.AspNetCore.SDK.RenderingEngine.Extensions;
using Sitecore.AspNetCore.SDK.RenderingEngine.Interfaces;
using Sitecore.AspNetCore.SDK.RenderingEngine.Rendering;
using Sitecore.AspNetCore.SDK.TestData;
using Xunit;

namespace Sitecore.AspNetCore.SDK.RenderingEngine.Tests.Extensions;

public class RazorPageExtensionsFixture
{
    // ReSharper disable once UnusedMember.Global - Used by testing framework
    public static Action<IFixture> AutoSetup => f =>
    {
        f.Behaviors.Add(new OmitOnRecursionBehavior());
        ViewContext viewContext = new()
        {
            HttpContext = Substitute.For<HttpContext>()
        };

        FeatureCollection features = new()
        {
            [typeof(ISitecoreRenderingContext)] = new SitecoreRenderingContext
            {
                Component = new Component(),
                Response = new SitecoreLayoutResponse([])
                {
                    Content = CannedResponses.StyleGuide1,
                }
            }
        };

        viewContext.HttpContext.Features.Returns(features);

        IRazorPage? page = Substitute.For<IRazorPage>();
        page.ViewContext.Returns(viewContext);
        f.Inject(page);
    };

    [Fact]
    public void RazorPageExtensions_SitecoreRoute_Guarded()
    {
        Func<Route?> act =
            () => RazorPageExtensions.SitecoreRoute(null!);

        act.Should().Throw<ArgumentNullException>();
    }

    [Theory]
    [AutoNSubstituteData]
    public void SitecoreRoute__WithRazorPage_ReturnsRoute(IRazorPage page)
    {
        // Act
        Route? route = page.SitecoreRoute();

        // Assert
        route.Should().BeEquivalentTo(CannedResponses.StyleGuide1.Sitecore!.Route);
    }

    [Fact]
    public void RazorPageExtensions_SitecoreContext_Guarded()
    {
        Func<Context?> act =
            () => RazorPageExtensions.SitecoreContext(null!);

        act.Should().Throw<ArgumentNullException>();
    }

    [Theory]
    [AutoNSubstituteData]
    public void SitecoreContext__WithRazorPage_ReturnsContext(IRazorPage page)
    {
        // Act
        Context? route = page.SitecoreContext();

        // Assert
        route.Should().BeEquivalentTo(CannedResponses.StyleGuide1.Sitecore!.Context);
    }

    [Fact]
    public void RazorPageExtensions_SitecoreComponent_Guarded()
    {
        Func<Component?> act =
            () => RazorPageExtensions.SitecoreComponent(null!);

        act.Should().Throw<ArgumentNullException>();
    }

    [Theory]
    [AutoNSubstituteData]
    public void SitecoreComponent_WithRazorPage_ReturnsComponent(IRazorPage page)
    {
        // Act
        Component? route = page.SitecoreComponent();

        // Assert
        route.Should().BeOfType(typeof(Component));
    }
}