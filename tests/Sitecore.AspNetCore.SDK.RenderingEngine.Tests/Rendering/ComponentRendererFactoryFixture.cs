using AutoFixture;
using AutoFixture.Idioms;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Sitecore.AspNetCore.SDK.AutoFixture.Attributes;
using Sitecore.AspNetCore.SDK.AutoFixture.Extensions;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Response.Model;
using Sitecore.AspNetCore.SDK.RenderingEngine.Configuration;
using Sitecore.AspNetCore.SDK.RenderingEngine.Rendering;
using Xunit;

namespace Sitecore.AspNetCore.SDK.RenderingEngine.Tests.Rendering;

public class ComponentRendererFactoryFixture
{
    private const string TestComponentName = "TestComponent";

    // ReSharper disable once UnusedMember.Global - Used by testing framework
    public static Action<IFixture> AutoSetup => f =>
    {
        IComponentRenderer? componentRenderer = f.Freeze<IComponentRenderer>();
        List<IComponentRenderer> componentRenderers = [componentRenderer];
        f.Inject<IEnumerable<IComponentRenderer>>(componentRenderers);

        ServiceCollection services = [];
        ServiceProvider serviceProvider = services.BuildServiceProvider();

        RenderingEngineOptions renderingEngineOptions = new()
        {
            RendererRegistry = new SortedList<int, ComponentRendererDescriptor>
            {
                { 0, new ComponentRendererDescriptor(name => name == TestComponentName, _ => componentRenderer, TestComponentName) }
            }
        };

        IOptions<RenderingEngineOptions> options = Options.Create(renderingEngineOptions);
        ComponentRendererFactory componentRendererFactory = new(options, serviceProvider);

        f.Inject(componentRendererFactory);

        Component component = new() { Name = TestComponentName };
        f.Inject(component);
    };

    [Theory]
    [AutoNSubstituteData]
    public void Ctor_InvalidArgs_Throws(GuardClauseAssertion guard)
    {
        guard.VerifyConstructors<ComponentRendererFactory>();
    }

    [Theory]
    [AutoNSubstituteData]
    public void GetInstance_NullComponent_Throws(ComponentRendererFactory sut)
    {
        // Arrange
        Component component = null!;
        Func<IComponentRenderer> act =
            () => sut.GetRenderer(component);

        // Act & assert
        act.Should().Throw<ArgumentNullException>().WithMessage("Value cannot be null. (Parameter 'component')");
    }

    [Theory]
    [AutoNSubstituteData]
    public void GetInstance_EmptyComponentName_Throws(ComponentRendererFactory sut)
    {
        // Arrange
        Component component = new() { Name = string.Empty };
        Func<IComponentRenderer> act =
            () => sut.GetRenderer(component);

        // Act & assert
        act.Should().Throw<ArgumentException>().WithParameterName("componentName");
    }

    [Theory]
    [AutoNSubstituteData]
    public void GetInstance_RendererTypeDoesNotExist_Throws(
        ComponentRendererFactory sut)
    {
        // Arrange
        Component component = new() { Name = "testComponent" };
        Func<IComponentRenderer> act =
            () => sut.GetRenderer(component);

        // Act & assert
        act.Should().Throw<InvalidOperationException>().WithMessage("The component renderer descriptor for testComponent is null. Please ensure that correct Sitecore component-to-view mappings are defined as part of the AddSitecoreRenderingEngine options in Startup.ConfigureServices.");
    }

    [Theory]
    [AutoNSubstituteData]
    public void GetInstance_RendererExists_ReturnsValidValue(
        IComponentRenderer componentRenderer,
        Component component,
        ComponentRendererFactory sut)
    {
        // Act
        IComponentRenderer result = sut.GetRenderer(component);

        // Assert
        result.Should().Be(componentRenderer);
    }
}