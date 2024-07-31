using FluentAssertions;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Response.Model;
using Xunit;

namespace Sitecore.AspNetCore.SDK.LayoutService.Client.Tests.Response.Model;

public class SiteFixture
{
    [Fact]
    public void Ctor_PropertiesAreDefaults()
    {
        // Arrange
        Site sut = new();

        // Act / Assert
        sut.Name.Should().Be(default);
    }
}