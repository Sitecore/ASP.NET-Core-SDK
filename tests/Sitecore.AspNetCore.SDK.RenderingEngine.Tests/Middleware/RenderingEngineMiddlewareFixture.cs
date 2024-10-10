using AutoFixture;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using NSubstitute;
using Sitecore.AspNetCore.SDK.AutoFixture.Attributes;
using Sitecore.AspNetCore.SDK.AutoFixture.Extensions;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Interfaces;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Request;
using Sitecore.AspNetCore.SDK.RenderingEngine.Configuration;
using Sitecore.AspNetCore.SDK.RenderingEngine.Extensions;
using Sitecore.AspNetCore.SDK.RenderingEngine.Interfaces;
using Sitecore.AspNetCore.SDK.RenderingEngine.Middleware;
using Sitecore.AspNetCore.SDK.RenderingEngine.Rendering;
using Xunit;

namespace Sitecore.AspNetCore.SDK.RenderingEngine.Tests.Middleware;

public class RenderingEngineMiddlewareFixture
{
    // ReSharper disable once UnusedMember.Global - Used by testing framework
    public static Action<IFixture> AutoSetup => f =>
    {
        f.Behaviors.Add(new OmitOnRecursionBehavior());
        f.Inject(new BindingInfo());
    };

    [Theory]
    [AutoNSubstituteData]
    public void Ctor_Guarded(GuardClauseAssertion guard)
    {
        guard.VerifyConstructors<RenderingEngineMiddleware>();
    }

    [Theory]
    [AutoNSubstituteData]
    public async Task Invoke_Guarded(
        RenderingEngineMiddleware sut,
        IViewComponentHelper componentHelper,
        IHtmlHelper htmlHelper)
    {
        Func<Task> act =
            () => sut.Invoke(null!, componentHelper, htmlHelper);

        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Theory]
    [AutoNSubstituteData]
    public async Task Invoke_NextMiddlewareCalled(
        [Frozen] RequestDelegate next,
        HttpContext httpContext,
        RenderingEngineMiddleware sut,
        IViewComponentHelper componentHelper,
        IHtmlHelper htmlHelper)
    {
        // act
        await sut.Invoke(httpContext, componentHelper, htmlHelper);

        // assert
        Received.InOrder(() => next.Invoke(httpContext));
    }

    [Theory]
    [AutoNSubstituteData]
    public async Task Invoke_ContextAddedToHttpContext(
        [Frozen] ISitecoreLayoutClient layoutClient,
        [Frozen] ISitecoreLayoutRequestMapper requestMapper,
        HttpContext httpContext,
        RenderingEngineMiddleware sut,
        IViewComponentHelper componentHelper,
        IHtmlHelper htmlHelper)
    {
        bool contextWasSet = false;
        httpContext.Features.Get<ISitecoreRenderingContext>().Returns((ISitecoreRenderingContext)null!);
        httpContext.Features.When(x => x.Set(Arg.Any<SitecoreRenderingContext>())).Do(_ => contextWasSet = true);

        // act
        await sut.Invoke(httpContext, componentHelper, htmlHelper);

        // assert
        requestMapper.Received(1).Map(httpContext.Request);
        Received.InOrder(() => layoutClient.Request(Arg.Any<SitecoreLayoutRequest>()));
        contextWasSet.Should().BeTrue();
    }

    [Theory]
    [AutoNSubstituteData]
    public async Task Invoke_ExecutedPostRenderingActions(
        [Frozen] IOptions<RenderingEngineOptions> options,
        HttpContext httpContext,
        RenderingEngineMiddleware sut,
        IViewComponentHelper componentHelper,
        IHtmlHelper htmlHelper)
    {
        // arrange
        httpContext.Features.Get<ISitecoreRenderingContext>().Returns((ISitecoreRenderingContext)null!);

        int executed = 0;

        options.Value.AddPostRenderingAction(_ => { executed++; });
        options.Value.AddPostRenderingAction(_ => { executed++; });

        httpContext.Features.Get<IOptions<RenderingEngineOptions>>().Returns(options);

        // act
        await sut.Invoke(httpContext, componentHelper, htmlHelper);

        // assert
        executed.Should().Be(2);
    }
}