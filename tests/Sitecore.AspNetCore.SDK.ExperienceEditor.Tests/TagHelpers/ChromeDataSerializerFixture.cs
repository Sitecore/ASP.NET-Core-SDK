using FluentAssertions;
using Sitecore.AspNetCore.SDK.ExperienceEditor.TagHelpers;
using Xunit;

namespace Sitecore.AspNetCore.SDK.ExperienceEditor.Tests.TagHelpers;

public class ChromeDataSerializerFixture
{
    private readonly ChromeDataSerializer _serializer = new();

    [Fact]
    public void Serialize_EmptyData_ShouldReturnEmptyJsonObject()
    {
        // Arrange
        Dictionary<string, object?> data = [];

        // Act
        string result = _serializer.Serialize(data);

        // Assert
        result.Should().Be("{}");
    }

    [Fact]
    public void Serialize_WithValidData_ShouldReturnJson()
    {
        // Arrange
        Dictionary<string, object?> data = new()
        {
            ["class"] = "my-super-class",
            ["displayName"] = "my-super-name",
            ["itsNull"] = null
        };

        // Act
        string result = _serializer.Serialize(data);

        // Assert
        result.Should().Be("{\"class\":\"my-super-class\",\"displayName\":\"my-super-name\",\"itsNull\":null}");
    }
}