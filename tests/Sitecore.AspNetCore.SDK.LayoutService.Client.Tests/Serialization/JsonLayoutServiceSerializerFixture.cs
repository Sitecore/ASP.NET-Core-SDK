using FluentAssertions;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Response;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Serialization;
using Xunit;

namespace Sitecore.AspNetCore.SDK.LayoutService.Client.Tests.Serialization;

public class JsonLayoutServiceSerializerFixture
{
    private readonly JsonLayoutServiceSerializer _sut = new();

    [Fact]
    public void Deserialize_NullArgument_Throws()
    {
        // Arrange
        Action action = () => _sut.Deserialize(null!);

        // Act & Assert
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Deserialize_EmptyArgument_ReturnSitecoreAndContextRawDataNull()
    {
        // Act
        SitecoreLayoutResponseContent? result = _sut.Deserialize("{}");

        // Assert
        result.Should().NotBeNull();
        result!.Sitecore.Should().BeNull();
        result.ContextRawData.Should().BeEmpty();
    }

    [Fact]
    public void Deserialize_CorrectArgument_ReturnSitecoreLayoutResponseContent()
    {
        // Arrange
        string json = File.ReadAllText("./Json/layoutResponse.json");

        // Act
        SitecoreLayoutResponseContent? result = _sut.Deserialize(json);

        // Assert
        result.Should().NotBeNull();
        result!.Sitecore.Should().NotBeNull();
        result.ContextRawData.Should().NotBeNullOrWhiteSpace();
    }
}