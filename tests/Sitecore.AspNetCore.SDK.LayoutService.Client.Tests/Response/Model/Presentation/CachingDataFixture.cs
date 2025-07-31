using AwesomeAssertions;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Response.Model.Presentation;
using Xunit;

namespace Sitecore.AspNetCore.SDK.LayoutService.Client.Tests.Response.Model.Presentation;

public class CachingDataFixture
{
    [Fact]
    public void Ctor_SetsDefaults()
    {
        // Arrange & act
        CachingData sut = new();

        // Assert
        sut.Cacheable.Should().BeNull();
        sut.ClearOnIndexUpdate.Should().BeNull();
        sut.VaryByData.Should().BeNull();
        sut.VaryByDevice.Should().BeNull();
        sut.VaryByLogin.Should().BeNull();
        sut.VaryByParameters.Should().BeNull();
        sut.VaryByQueryString.Should().BeNull();
        sut.VaryByUser.Should().BeNull();
    }
}