using AutoFixture.Idioms;
using FluentAssertions;
using Sitecore.AspNetCore.SDK.AutoFixture.Attributes;
using Sitecore.AspNetCore.SDK.AutoFixture.Extensions;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Response;
using Xunit;

namespace Sitecore.AspNetCore.SDK.LayoutService.Client.Tests.Response;

public class SitecoreLayoutResponseFixture
{
    [Theory]
    [AutoNSubstituteData]
    public void Ctor_IsGuarded(GuardClauseAssertion guard)
    {
        // Act / Assert
        guard.VerifyConstructors<SitecoreLayoutResponse>();
    }

    [Fact]
    public void Ctor_WithErrors_SetsDefaults()
    {
        // Arrange / Act
        SitecoreLayoutResponse sut = new([], []);

        // Assert
        sut.Metadata.Should().BeNull();
        sut.Content.Should().Be(default);
        sut.HasErrors.Should().BeFalse();
        sut.Errors.Should().NotBeNull();
        sut.Errors.Should().BeEmpty();
        sut.Request.Should().BeEmpty();
    }

    [Fact]
    public void Ctor_NoErrors_SetsDefaults()
    {
        // Arrange / Act
        SitecoreLayoutResponse sut = new([]);

        // Assert
        sut.Metadata.Should().BeNull();
        sut.Content.Should().Be(default);
        sut.HasErrors.Should().BeFalse();
        sut.Errors.Should().BeEmpty();
        sut.Request.Should().BeEmpty();
    }
}