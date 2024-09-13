using System.ComponentModel.Design;
using System.Diagnostics.CodeAnalysis;
using AutoFixture;
using AutoFixture.Idioms;
using FluentAssertions;
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

public class PartialViewComponentRendererFixture
{
    private const string Locator = "testLocator";

    // ReSharper disable once UnusedMember.Global - Used by testing framework
    [ExcludeFromCodeCoverage]
    public static Action<IFixture> AutoSetup => f =>
    {
        f.Inject(new ViewContext());
        ICustomHtmlHelper? htmlHelper = f.Create<ICustomHtmlHelper>();
        f.Inject<IHtmlHelper>(htmlHelper);
        f.Inject<IViewContextAware>(htmlHelper);

        IServiceProvider? serviceProvider = f.Freeze<IServiceProvider>();
        serviceProvider.GetService(typeof(IHtmlHelper)).Returns(htmlHelper);

        f.Freeze<ISitecoreRenderingContext>();
    };

    [Fact]
    public void Describe_LocatorIsNotNullOrEmpty_DescriptorCanCreateComponentRenderer()
    {
        // Arrange
        IServiceProvider services = new ServiceContainer();

        // Act
        ComponentRendererDescriptor descriptor = PartialViewComponentRenderer.Describe(_ => true, Locator);

        // Assert
        descriptor.Should().NotBeNull();

        IComponentRenderer renderer = descriptor.GetOrCreate(services);
        renderer.Should().NotBeNull();
        renderer.Should().BeOfType(typeof(PartialViewComponentRenderer));
    }

    [Theory]
    [AutoNSubstituteData]
    public void Constructor_IsGuarded(GuardClauseAssertion guard)
    {
        guard.VerifyConstructors<PartialViewComponentRenderer>();
    }

    [Theory]
    [AutoNSubstituteData]
    public async Task Render_PartialView_PassedTheLocator(
        IHtmlHelper htmlHelper,
        string locator,
        ViewContext viewContext)
    {
        ISitecoreRenderingContext? renderingContext = Substitute.For<ISitecoreRenderingContext>();
        RenderingHelpers helpers = new(null!, htmlHelper);
        renderingContext.RenderingHelpers.Returns(helpers);
        PartialViewComponentRenderer sut = new(locator);

        // act
        _ = await sut.Render(renderingContext, viewContext);

        // assert
        Received.InOrder(() => _ = htmlHelper.PartialAsync(locator, Arg.Any<object>(), null));
    }
}