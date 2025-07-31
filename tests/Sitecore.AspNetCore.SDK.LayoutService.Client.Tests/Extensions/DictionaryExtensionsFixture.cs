using AwesomeAssertions;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Extensions;
using Xunit;

namespace Sitecore.AspNetCore.SDK.LayoutService.Client.Tests.Extensions;

public class DictionaryExtensionsFixture
{
    [Fact]
    public void ToDebugString_DictionaryIsNull_ThrowsArgumentNullException()
    {
        // Arrange
        Func<string> act =
            () => ((Dictionary<string, string>)null!).ToDebugString();

        // Act & Assert
        act.Should().Throw<ArgumentNullException>().WithMessage("Value cannot be null. (Parameter 'dictionary')");
    }

    [Fact]
    public void ToDebugString_EmptyDictionary_ReturnBrackets()
    {
        // Act
        Dictionary<string, string> testsDictionary = [];
        string result = testsDictionary.ToDebugString();

        // Assert
        result.Should().Be("{}");
    }

    [Fact]
    public void ToDebugString_NotEmptyDictionary_ReturnString()
    {
        // Act
        Dictionary<string, string> testsDictionary = new()
        {
            { "key1", "value1" },
            { "key2", "value2" }
        };

        string result = testsDictionary.ToDebugString();

        // Assert
        result.Should().Be("{key1=value1,key2=value2}");
    }
}