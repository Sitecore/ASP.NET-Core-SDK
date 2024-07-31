using FluentAssertions;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Response.Model.Presentation;
using Xunit;

namespace Sitecore.AspNetCore.SDK.LayoutService.Client.Tests.Response.Model.Presentation;

public class RenderingFixture
{
    [Fact]
    public void Ctor_SetsDefaults()
    {
        // Arrange & act
        Rendering sut = new();

        // Assert
        sut.Id.Should().BeNull();
        sut.InstanceId.Should().BeNull();
        sut.PlaceholderKey.Should().BeEmpty();
        sut.DataSource.Should().BeNull();
        sut.Parameters.Should().BeEmpty();
        sut.Caching.Should().BeNull();
        sut.Personalization.Should().BeNull();
    }
}