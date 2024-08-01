using System.ComponentModel.Design;
using AutoFixture;
using AutoFixture.Idioms;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using NSubstitute;
using Sitecore.AspNetCore.SDK.AutoFixture.Attributes;
using Sitecore.AspNetCore.SDK.AutoFixture.Extensions;
using Sitecore.AspNetCore.SDK.RenderingEngine.Interfaces;
using Sitecore.AspNetCore.SDK.RenderingEngine.Rendering;
using Sitecore.AspNetCore.SDK.RenderingEngine.Tests.Mocks;
using Xunit;

namespace Sitecore.AspNetCore.SDK.RenderingEngine.Tests.Rendering;

public class ViewComponentComponentRendererFixture
{
    private const string Locator = "testLocator";

    // ReSharper disable once UnusedMember.Global - Used by testing framework
    public static Action<IFixture> AutoSetup => f =>
    {
        IHtmlHelper? htmlHelper = Substitute.For<IHtmlHelper>();
        f.Inject(htmlHelper);

        f.Inject(new ViewContext());
        ICustomViewComponentHelper? viewComponentHelper = f.Create<ICustomViewComponentHelper>();
        f.Inject<IViewComponentHelper>(viewComponentHelper);
        f.Inject<IViewContextAware>(viewComponentHelper);

        IServiceProvider? serviceProvider = f.Freeze<IServiceProvider>();
        serviceProvider.GetService(typeof(IViewComponentHelper)).Returns(viewComponentHelper);

        f.Freeze<ISitecoreRenderingContext>();

        ViewComponentComponentRenderer viewComponentRenderer = new(Locator);
        f.Inject(viewComponentRenderer);
    };

    [Fact]
    public void Describe_LocatorIsNotNullOrEmpty_DescriptorCanCreateComponentRenderer()
    {
        // Arrange
        ServiceContainer services = new();

        // Act
        ComponentRendererDescriptor descriptor = ViewComponentComponentRenderer.Describe(_ => true, Locator);

        // Assert
        descriptor.Should().NotBeNull();

        IComponentRenderer renderer = descriptor.GetOrCreate(services);
        renderer.Should().NotBeNull();
        renderer.Should().BeOfType(typeof(ViewComponentComponentRenderer));
    }

    [Theory]
    [AutoNSubstituteData]
    public void Render_IsGuarded(GuardClauseAssertion guard)
    {
        guard.VerifyMethod<ViewComponentComponentRenderer>("Render");
    }

    [Theory]
    [AutoNSubstituteData]
    public void Constructor_IsGuarded(GuardClauseAssertion guard)
    {
        guard.VerifyConstructors<ViewComponentComponentRenderer>();
    }

    [Theory]
    [AutoNSubstituteData]
    public void Render_ViewComponent_PassedTheLocator(
        IViewComponentHelper viewComponentHelper,
        ISitecoreRenderingContext renderingContext,
        ViewContext viewContext,
        ViewComponentComponentRenderer sut)
    {
        // act
        _ = sut.Render(renderingContext, viewContext);

        // assert
        Received.InOrder(() => _ = viewComponentHelper.InvokeAsync(Locator, null));
    }
}