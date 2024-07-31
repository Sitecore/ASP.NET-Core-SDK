using FluentAssertions;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Configuration;
using Xunit;

namespace Sitecore.AspNetCore.SDK.LayoutService.Client.Tests.Configuration;

public class HttpLayoutRequestHandlerOptionsFixture
{
    [Fact]
    public void Ctor_SetsDefaults()
    {
        // Arrange / Act
        HttpLayoutRequestHandlerOptions sut = new();

        // Assert
        sut.RequestMap.Should().NotBeNull();
        sut.RequestMap.Should().BeEmpty();
    }
}