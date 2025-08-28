using AutoFixture;
using AwesomeAssertions;
using Microsoft.AspNetCore.Http;
using NSubstitute;
using Sitecore.AspNetCore.SDK.AutoFixture.Attributes;
using Sitecore.AspNetCore.SDK.RenderingEngine.Extensions;
using Sitecore.AspNetCore.SDK.RenderingEngine.Interfaces;
using Sitecore.AspNetCore.SDK.RenderingEngine.Rendering;
using Xunit;

namespace Sitecore.AspNetCore.SDK.RenderingEngine.Tests.Extensions;

public class HttpContextExtensionsFixture
{
    // ReSharper disable once UnusedMember.Global - Used by testing framework
    public static Action<IFixture> AutoSetup()
    {
        return f =>
        {
            f.Inject<SitecoreRenderingContext>(null!);
        };
    }

    [Fact]
    public void GetSitecoreLayoutFeature_Guarded()
    {
        Func<ISitecoreRenderingContext?> act =
            () => HttpContextExtensions.GetSitecoreRenderingContext(null!);

        act.Should().Throw<ArgumentNullException>();
    }

    [Theory]
    [AutoNSubstituteData]
    public void GetSitecoreLayoutFeature_ReturnsTheRightFeature(HttpContext context)
    {
        // act
        _ = context.GetSitecoreRenderingContext();

        // assert
        context.Features.Received(1).Get<ISitecoreRenderingContext>();
    }
}