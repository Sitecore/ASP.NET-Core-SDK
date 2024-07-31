using FluentAssertions;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Configuration;
using Xunit;

namespace Sitecore.AspNetCore.SDK.LayoutService.Client.Tests.Configuration;

public class SitecoreLayoutServiceOptionsFixture
{
    [Fact]
    public void Ctor_SetsDefaults()
    {
        // Arrange / Act
        SitecoreLayoutClientOptions sut = new();

        // Assert
        sut.DefaultHandler.Should().BeNull();
        sut.HandlerRegistry.Should().BeEmpty();
    }
}