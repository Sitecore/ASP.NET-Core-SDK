using FluentAssertions;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Response.Model;
using Xunit;

namespace Sitecore.AspNetCore.SDK.LayoutService.Client.Tests.Response.Model;

public class PlaceholderFixture
{
    [Fact]
    public void Ctor_SetsDefaults()
    {
        // Act
        Placeholder sut = [];

        // Assert
        sut.Should().BeEmpty();
    }
}