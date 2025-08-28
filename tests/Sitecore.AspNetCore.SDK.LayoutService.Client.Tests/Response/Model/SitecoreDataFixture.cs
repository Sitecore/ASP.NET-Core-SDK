using AwesomeAssertions;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Response.Model;
using Xunit;

namespace Sitecore.AspNetCore.SDK.LayoutService.Client.Tests.Response.Model;

public class SitecoreDataFixture
{
    [Fact]
    public void Ctor_PropertiesAreDefaults()
    {
        // Arrange
        SitecoreData sut = new();

        // Act / Assert
        sut.Context.Should().Be(default);
        sut.Route.Should().Be(default);
        sut.Devices.Should().BeEmpty();
    }
}