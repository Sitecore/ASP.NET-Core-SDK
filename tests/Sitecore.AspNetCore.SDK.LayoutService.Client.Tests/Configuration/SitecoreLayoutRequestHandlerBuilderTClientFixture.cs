using AutoFixture.Idioms;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Sitecore.AspNetCore.SDK.AutoFixture.Attributes;
using Sitecore.AspNetCore.SDK.AutoFixture.Extensions;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Configuration;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Interfaces;
using Xunit;

namespace Sitecore.AspNetCore.SDK.LayoutService.Client.Tests.Configuration;

public class SitecoreLayoutRequestHandlerBuilderTClientFixture
{
    [Theory]
    [AutoNSubstituteData]
    public void Ctor_IsGuarded(GuardClauseAssertion guard)
    {
        // Act / Assert
        guard.VerifyConstructors<SitecoreLayoutRequestHandlerBuilder<ILayoutRequestHandler>>();
    }

    [Theory]
    [MemberAutoNSubstituteData(nameof(EmptyStrings))]
    public void Ctor_InvalidHandlerName_SetsProperties(string handlerName, IServiceCollection services)
    {
        // Arrange
        Action action = () => _ = new SitecoreLayoutRequestHandlerBuilder<ILayoutRequestHandler>(handlerName, services);

        // Assert
        action.Should().Throw<ArgumentException>()
            .And.ParamName.Should().Be("handlerName");
    }

    [Theory]
    [AutoNSubstituteData]
    public void Ctor_ValidValues_SetsProperties(IServiceCollection services, string handlerName)
    {
        // Arrange / Act
        SitecoreLayoutRequestHandlerBuilder<ILayoutRequestHandler> sut = new(handlerName, services);

        // Assert
        sut.Services.Should().BeSameAs(services);
        sut.HandlerName.Should().Be(handlerName);
    }

    private static IEnumerable<object[]> EmptyStrings()
    {
        yield return [null!];
        yield return [string.Empty];
        yield return ["\t\t   "];
    }
}