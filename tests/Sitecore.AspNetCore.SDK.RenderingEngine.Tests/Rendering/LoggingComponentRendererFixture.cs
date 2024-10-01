using System.ComponentModel.Design;
using System.Diagnostics.CodeAnalysis;
using AutoFixture;
using AutoFixture.Idioms;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Sitecore.AspNetCore.SDK.AutoFixture.Attributes;
using Sitecore.AspNetCore.SDK.AutoFixture.Extensions;
using Sitecore.AspNetCore.SDK.RenderingEngine.Interfaces;
using Sitecore.AspNetCore.SDK.RenderingEngine.Rendering;
using Xunit;

namespace Sitecore.AspNetCore.SDK.RenderingEngine.Tests.Rendering;

public class LoggingComponentRendererFixture
{
    // ReSharper disable once UnusedMember.Global - Used by testing framework
    [ExcludeFromCodeCoverage]
    public static Action<IFixture> AutoSetup => f =>
    {
        ILogger<LoggingComponentRenderer>? logger = Substitute.For<ILogger<LoggingComponentRenderer>>();
        f.Inject(logger);

        f.Inject(new ViewContext());

        IServiceProvider? serviceProvider = f.Freeze<IServiceProvider>();
        serviceProvider.GetService(typeof(ILogger<LoggingComponentRenderer>)).Returns(logger);

        f.Freeze<ISitecoreRenderingContext>();

        LoggingComponentRenderer loggingComponentRenderer = new(logger);
        f.Inject(loggingComponentRenderer);
    };

    [Fact]
    public void Describe_LocatorIsNotNullOrEmpty_DescriptorCanCreateComponentRenderer()
    {
        // Arrange
        ServiceContainer services = new();
        services.AddService(typeof(ILogger<LoggingComponentRenderer>), Substitute.For<ILogger<LoggingComponentRenderer>>());

        // Act
        ComponentRendererDescriptor descriptor = LoggingComponentRenderer.Describe(_ => true);

        // Assert
        descriptor.Should().NotBeNull();

        IComponentRenderer renderer = descriptor.GetOrCreate(services);
        renderer.Should().NotBeNull();
        renderer.Should().BeOfType(typeof(LoggingComponentRenderer));
    }

    [Theory]
    [AutoNSubstituteData]
    public void Render_IsGuarded(GuardClauseAssertion guard)
    {
        guard.VerifyMethod<LoggingComponentRenderer>("Render");
    }

    [Theory]
    [AutoNSubstituteData]
    public void Constructor_IsGuarded(GuardClauseAssertion guard)
    {
        guard.VerifyConstructors<LoggingComponentRenderer>();
    }
}