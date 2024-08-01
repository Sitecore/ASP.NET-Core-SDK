using FluentAssertions;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Response.Model.Presentation;
using Xunit;

namespace Sitecore.AspNetCore.SDK.LayoutService.Client.Tests.Response.Model.Presentation;

public class DeviceFixture
{
    [Fact]
    public void Ctor_SetsDefaults()
    {
        // Arrange & act
        Device sut = new();

        // Assert
        sut.Id.Should().BeNull();
        sut.LayoutId.Should().BeNull();
        sut.Placeholders.Should().BeEmpty();
        sut.Renderings.Should().BeEmpty();
    }
}