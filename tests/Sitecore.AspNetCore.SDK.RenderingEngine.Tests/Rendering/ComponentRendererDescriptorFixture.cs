using System.Diagnostics.CodeAnalysis;
using AutoFixture;
using FluentAssertions;
using NSubstitute;
using Sitecore.AspNetCore.SDK.AutoFixture.Attributes;
using Sitecore.AspNetCore.SDK.RenderingEngine.Rendering;
using Xunit;

namespace Sitecore.AspNetCore.SDK.RenderingEngine.Tests.Rendering;

public class ComponentRendererDescriptorFixture
{
    // ReSharper disable once UnusedMember.Global - Used by testing framework
    [ExcludeFromCodeCoverage]
    public static Action<IFixture> AutoSetup()
    {
        return f =>
        {
            IComponentRenderer? renderer = Substitute.For<IComponentRenderer>();
            f.Inject(renderer);

            IServiceProvider? services = Substitute.For<IServiceProvider>();
            services.GetService(typeof(IComponentRenderer)).Returns(renderer);
            f.Inject(services);
        };
    }

    [Fact]
    public void Ctor_IsGuarded()
    {
        // Arrange
        Func<ComponentRendererDescriptor> act =
            () => new ComponentRendererDescriptor(null!, null!);

        // Act & Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Theory]
    [AutoNSubstituteData]
    public void GetOrCreate_ServiceProviderContainsRendererType_ReturnsRendererType(IServiceProvider services, IComponentRenderer renderer)
    {
        // Arrange
        ComponentRendererDescriptor sut = new(name => name == "Test", _ => renderer);

        // Act
        IComponentRenderer result = sut.GetOrCreate(services);

        // Assert
        result.Should().NotBeNull();
    }
}