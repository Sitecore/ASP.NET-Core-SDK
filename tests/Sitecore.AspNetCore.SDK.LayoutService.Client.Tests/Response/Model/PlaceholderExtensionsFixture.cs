using FluentAssertions;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Response.Model;
using Xunit;

namespace Sitecore.AspNetCore.SDK.LayoutService.Client.Tests.Response.Model;

public class PlaceholderExtensionsFixture
{
    #region ComponentAt
    [Fact]
    public void ComponentAt_WithNullPlaceholder_Throws()
    {
        // Arrange
        Action action = () => PlaceholderExtensions.ComponentAt(null!, 0);

        // Assert
        action.Should().Throw<ArgumentNullException>()
            .And.ParamName.Should().Be("placeholder");
    }

    [Fact]
    public void ComponentAt_WithInvalidIndex_Throws()
    {
        // Arrange
        Action action = () => new Placeholder().ComponentAt(0);

        // Assert
        action.Should().Throw<ArgumentOutOfRangeException>()
            .And.ParamName.Should().Be("index");
    }

    [Fact]
    public void ComponentAt_WithDifferentFeatureAtSpecifiedIndex_ReturnsNull()
    {
        // Arrange
        Placeholder placeholder = [new EditableChrome()];

        Component? component = placeholder.ComponentAt(0);

        // Assert
        component.Should().BeNull();
    }

    [Fact]
    public void ComponentAt_WithValidParameters_ReturnsCorrectComponent()
    {
        // Arrange
        Placeholder placeholder = [new Component()];

        Component? component = placeholder.ComponentAt(0);

        // Assert
        component.Should().NotBeNull();
    }
    #endregion

    #region ChromeAt
    [Fact]
    public void ChromeAt_WithNullPlaceholder_Throws()
    {
        // Arrange
        Action action = () => PlaceholderExtensions.ChromeAt(null!, 0);

        // Assert
        action.Should().Throw<ArgumentNullException>()
            .And.ParamName.Should().Be("placeholder");
    }

    [Fact]
    public void ChromeAt_WithInvalidIndex_Throws()
    {
        // Arrange
        Action action = () => new Placeholder().ChromeAt(0);

        // Assert
        action.Should().Throw<ArgumentOutOfRangeException>()
            .And.ParamName.Should().Be("index");
    }

    [Fact]
    public void ChromeAt_WithDifferentFeatureAtSpecifiedIndex_ReturnsNull()
    {
        // Arrange
        Placeholder placeholder = [new Component()];

        EditableChrome? chrome = placeholder.ChromeAt(0);

        // Assert
        chrome.Should().BeNull();
    }

    [Fact]
    public void ChromeAt_WithValidParameters_ReturnsCorrectChrome()
    {
        // Arrange
        Placeholder placeholder = [new EditableChrome()];

        EditableChrome? chrome = placeholder.ChromeAt(0);

        // Assert
        chrome.Should().NotBeNull();
    }
    #endregion
}