using AwesomeAssertions;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Configuration;
using Xunit;

namespace Sitecore.AspNetCore.SDK.LayoutService.Client.Tests.Configuration;

public class SitecoreLayoutRequestOptionsFixture
{
    [Fact]
    public void Ctor_SetsDefaults()
    {
        // Arrange / Act
        SitecoreLayoutRequestOptions sut = new();

        // Assert
        sut.RequestDefaults.Should().NotBeNull();
        sut.RequestDefaults.Should().BeEmpty();
    }
}