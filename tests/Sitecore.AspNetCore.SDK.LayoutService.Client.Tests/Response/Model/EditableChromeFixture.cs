using FluentAssertions;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Response.Model;
using Xunit;

namespace Sitecore.AspNetCore.SDK.LayoutService.Client.Tests.Response.Model;

public class EditableChromeFixture
{
    [Fact]
    public void Ctor_SetsDefaults()
    {
        // Act
        EditableChrome sut = new();

        // Assert
        sut.Name.Should().Be("code");
        sut.Type.Should().Be("text/sitecore");
        sut.Content.Should().BeEmpty();
        sut.Attributes.Should().BeEmpty();
    }
}