using AwesomeAssertions;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Response.Model;
using Xunit;

namespace Sitecore.AspNetCore.SDK.LayoutService.Client.Tests.Response.Model;

public class ContextFixture
{
    [Fact]
    public void Ctor_SetDefaults()
    {
        // Arrange / Act
        Context sut = new();

        // Assert
        sut.IsEditing.Should().Be(default);
        sut.Site.Should().Be(default);
        sut.PageState.Should().BeNull();
        sut.Language.Should().Be(string.Empty);
    }
}