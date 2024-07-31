using FluentAssertions;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Response.Model.Presentation;
using Xunit;

namespace Sitecore.AspNetCore.SDK.LayoutService.Client.Tests.Response.Model.Presentation;

public class PlaceholderDataFixture
{
    [Fact]
    public void Ctor_SetsDefaults()
    {
        // Arrange & act
        PlaceholderData sut = new();

        // Assert
        sut.Key.Should().BeNull();
        sut.InstanceId.Should().BeNull();
        sut.MetadataId.Should().BeNull();
    }
}