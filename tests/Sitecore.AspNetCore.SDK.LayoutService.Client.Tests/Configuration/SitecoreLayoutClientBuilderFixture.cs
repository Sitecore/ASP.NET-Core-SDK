using AutoFixture.Idioms;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Sitecore.AspNetCore.SDK.AutoFixture.Attributes;
using Sitecore.AspNetCore.SDK.AutoFixture.Extensions;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Configuration;
using Xunit;

namespace Sitecore.AspNetCore.SDK.LayoutService.Client.Tests.Configuration;

public class SitecoreLayoutClientBuilderFixture
{
    [Theory]
    [AutoNSubstituteData]
    public void Ctor_IsGuarded(GuardClauseAssertion guard)
    {
        // Act / Assert
        guard.VerifyConstructors<SitecoreLayoutClientBuilder>();
    }

    [Theory]
    [AutoNSubstituteData]
    public void Ctor_SetsServices(IServiceCollection services)
    {
        // Arrange / Act
        SitecoreLayoutClientBuilder sut = new(services);

        // Assert
        sut.Services.Should().BeSameAs(services);
    }
}