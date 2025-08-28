using System.Reflection;
using AwesomeAssertions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NSubstitute;
using Sitecore.AspNetCore.SDK.AutoFixture.Attributes;
using Sitecore.AspNetCore.SDK.RenderingEngine.Configuration;
using Sitecore.AspNetCore.SDK.RenderingEngine.Extensions;
using Sitecore.AspNetCore.SDK.RenderingEngine.Middleware;
using Sitecore.AspNetCore.SDK.RenderingEngine.Services;
using Xunit;

namespace Sitecore.AspNetCore.SDK.RenderingEngine.Tests.Extensions;

public class MultisiteAppConfigurationExtensionsFixture
{
    [Fact]
    public void AddMultisite_NullServicesProperties_ThrowsExceptions()
    {
        // Arrange
        Func<IServiceCollection> act =
            () => MultisiteAppConfigurationExtensions.AddMultisite(null!);

        // Act & Assert
        act.Should().Throw<ArgumentNullException>().WithParameterName("services");
    }

    [Theory]
    [AutoNSubstituteData]
    public void AddSitecoreRedirects_RegisterProperServicesAndConfiguration(Action<MultisiteOptions> multisiteOptions)
    {
        // Arrange
        IServiceCollection serviceCollection = new ServiceCollection();

        // Act
        serviceCollection.AddMultisite(multisiteOptions);

        // Assert
        // NOTE https://stackoverflow.com/questions/57123686/how-to-verify-addsingleton-with-special-type-is-received-using-nsubstitute-frame
        serviceCollection.Should().HaveCount(8);
        serviceCollection[5].ImplementationInstance.Should().BeEquivalentTo(new ConfigureNamedOptions<MultisiteOptions>(string.Empty, multisiteOptions));
        serviceCollection[6].ServiceType.Should().Be(typeof(ISiteCollectionService));
        serviceCollection[6].ImplementationType.Should().Be(typeof(GraphQLSiteCollectionService));
        serviceCollection[7].ServiceType.Should().Be(typeof(ISiteResolver));
        serviceCollection[7].ImplementationType.Should().Be(typeof(SiteResolver));
    }

    [Fact]
    public void UseMultisite_NullApplicationBuilder_ThrowsExceptions()
    {
        Func<IApplicationBuilder> act =
            () => MultisiteAppConfigurationExtensions.UseMultisite(null!);

        act.Should().Throw<ArgumentNullException>().WithParameterName("app");
    }

    [Theory]
    [AutoNSubstituteData]
    public void UseMultisite_UseMultisiteMiddleware(IApplicationBuilder applicationBuilder)
    {
        // Act
        applicationBuilder.ApplicationServices.GetService(typeof(ISiteCollectionService)).Returns(Substitute.For<ISiteCollectionService>());
        applicationBuilder.ApplicationServices.GetService(typeof(ISiteResolver)).Returns(Substitute.For<ISiteResolver>());
        applicationBuilder.UseMultisite();

        // Assert
        bool received = applicationBuilder.ReceivedCalls().Any(c => c.GetArguments().OfType<Delegate>().Any(d =>
            d.Target?.GetType().GetField("_middleware", BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(d.Target).As<Type>().FullName == typeof(MultisiteMiddleware).FullName));
        received.Should().BeTrue();
    }
}