using AutoFixture;
using AwesomeAssertions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NSubstitute;
using Sitecore.AspNetCore.SDK.AutoFixture.Attributes;
using Sitecore.AspNetCore.SDK.Tracking.VisitorIdentification;
using Xunit;

namespace Sitecore.AspNetCore.SDK.Tracking.Tests.Extensions;

public class VisitorIdentificationConfigurationExtensionsFixture
{
    // ReSharper disable once UnusedMember.Global - Used by testing framework
    public static Action<IFixture> AutoSetup => f =>
    {
        IServiceProvider? services = Substitute.For<IServiceProvider>();

        f.Inject(services);
    };

    [Fact]
    public void WithVisitorIdentification_NullServices_Throws()
    {
        // Arrange
        Action action = () => VisitorIdentificationAppConfigurationExtensions.AddSitecoreVisitorIdentification(null!);

        // Act / Assert
        action.Should().Throw<ArgumentNullException>()
            .And.ParamName.Should().Be("services");
    }

    [Fact]
    public void WithVisitorIdentification_NullOptions_NotThrows()
    {
        // Arrange
        IServiceCollection serviceCollection = new ServiceCollection();
        Action action = () => serviceCollection.AddSitecoreVisitorIdentification();

        // Act / Assert
        action.Should().NotThrow();
    }

    [Fact]
    public void UseSitecoreTracking_NullApp_Throws()
    {
        // Arrange
        Action action = () => VisitorIdentificationAppConfigurationExtensions.UseSitecoreVisitorIdentification(null!);

        // Act / Assert
        action.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("app");
    }

    [Theory]
    [AutoNSubstituteData]
    public void UseSitecoreTracking_NullChecksRenderingEngineOptionsUri_DosNotThrow(IApplicationBuilder app, IOptions<SitecoreVisitorIdentificationOptions> trOptions)
    {
        // Arrange
        app.ApplicationServices.GetService(typeof(IOptions<SitecoreVisitorIdentificationOptions>)).Returns(trOptions);

        // Act
        // Assert
        Action();
        return;

        void Action() => app.UseSitecoreVisitorIdentification();
    }
}