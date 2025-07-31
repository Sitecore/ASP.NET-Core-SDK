using AwesomeAssertions;
using Sitecore.AspNetCore.SDK.RenderingEngine.Extensions;
using Xunit;

namespace Sitecore.AspNetCore.SDK.RenderingEngine.Tests.Extensions;

public class EncodingExtensionsFixture
{
    [Fact]
    public void EncodeControlCharacters_WithEmptyValue_ReturnsEmptyString()
    {
        // Arrange & Act
        string result = string.Empty.EncodeControlCharacters();

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void EncodeControlCharacters_WithNullValue_ReturnsEmptyString()
    {
        // Arrange & Act
        string result = EncodingExtensions.EncodeControlCharacters(null!);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void EncodeControlCharacters_WithSpecialCharactersInValue_ReturnsUnencodedValue()
    {
        // Arrange
        const string value = "!\"£$%^&*()_-=@~#><,./?";

        // Act
        string result = value.EncodeControlCharacters();

        // Assert
        result.Should().Be(value);
    }

    [Fact]
    public void EncodeControlCharacters_WithUnencodedLineBreakInValue_ReturnsValueWithEncodedLineBreak()
    {
        // Arrange
        const string encodedLineBreak = "%0d%0a";
        const string value = "\r\ntest";

        // Act
        string result = value.EncodeControlCharacters();

        // Assert
        result.Should().Be(encodedLineBreak + "test");
    }

    [Fact]
    public void EncodeControlCharacters_WithEncodedLineBreakInValue_ReturnsValueWithEncodedLineBreak()
    {
        // Arrange
        const string encodedLineBreak = "%0d%0a";
        const string value = encodedLineBreak + "test";

        // Act
        string result = value.EncodeControlCharacters();

        // Assert
        result.Should().Be(value);
    }
}