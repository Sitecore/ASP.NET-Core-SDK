using AwesomeAssertions;
using Sitecore.AspNetCore.SDK.AutoFixture.Attributes;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Exceptions;
using Xunit;

namespace Sitecore.AspNetCore.SDK.LayoutService.Client.Tests.Exceptions;

public class ItemNotFoundSitecoreLayoutServiceClientExceptionFixture
{
    private const string DefaultMessage = "The Sitecore layout service returned an item not found response.";

    [Theory]
    [AutoNSubstituteData]
    public void ItemNotFoundSitecoreLayoutServiceClientException_WithMessage_SetsMessage(string message)
    {
        // Act
        ItemNotFoundSitecoreLayoutServiceClientException sut = new(message);

        // Assert
        sut.Message.Should().Be(message);
    }

    [Theory]
    [AutoNSubstituteData]
    public void ItemNotFoundSitecoreLayoutServiceClientException_WithMessageAndException_SetsMessageAndInnerException(
        string message,
        Exception exception)
    {
        // Act
        ItemNotFoundSitecoreLayoutServiceClientException sut = new(message, exception);

        // Assert
        sut.Message.Should().Be(message);
        sut.InnerException.Should().Be(exception);
    }

    [Fact]
    public void ItemNotFoundSitecoreLayoutServiceClientException_WithNoMessage_UsesDefaultMessage()
    {
        // Act
        ItemNotFoundSitecoreLayoutServiceClientException sut = new();

        // Assert
        sut.Message.Should().Be(DefaultMessage);
    }

    [Theory]
    [AutoNSubstituteData]
    public void ItemNotFoundSitecoreLayoutServiceClientException_WithException_SetsInnerException(
        Exception exception)
    {
        // Act
        ItemNotFoundSitecoreLayoutServiceClientException sut = new(exception);

        // Assert
        sut.Message.Should().Be(DefaultMessage);
        sut.InnerException.Should().Be(exception);
    }
}