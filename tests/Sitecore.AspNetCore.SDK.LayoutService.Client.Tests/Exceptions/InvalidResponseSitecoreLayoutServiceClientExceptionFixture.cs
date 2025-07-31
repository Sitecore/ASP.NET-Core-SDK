using AwesomeAssertions;
using Sitecore.AspNetCore.SDK.AutoFixture.Attributes;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Exceptions;
using Xunit;

namespace Sitecore.AspNetCore.SDK.LayoutService.Client.Tests.Exceptions;

public class InvalidResponseSitecoreLayoutServiceClientExceptionFixture
{
    private const string DefaultMessage = "The Sitecore layout service returned a response in an invalid format.";

    [Theory]
    [AutoNSubstituteData]
    public void InvalidResponseSitecoreLayoutServiceClientException_WithMessage_SetsMessage(string message)
    {
        // Act
        InvalidResponseSitecoreLayoutServiceClientException sut = new(message);

        // Assert
        sut.Message.Should().Be(message);
    }

    [Theory]
    [AutoNSubstituteData]
    public void InvalidResponseSitecoreLayoutServiceClientException_WithMessageAndException_SetsMessageAndInnerException(
        string message,
        Exception exception)
    {
        // Act
        InvalidResponseSitecoreLayoutServiceClientException sut = new(message, exception);

        // Assert
        sut.Message.Should().Be(message);
        sut.InnerException.Should().Be(exception);
    }

    [Fact]
    public void InvalidResponseSitecoreLayoutServiceClientException_WithNoMessage_UsesDefaultMessage()
    {
        // Act
        InvalidResponseSitecoreLayoutServiceClientException sut = new();

        // Assert
        sut.Message.Should().Be(DefaultMessage);
    }

    [Theory]
    [AutoNSubstituteData]
    public void InvalidResponseSitecoreLayoutServiceClientException_WithException_SetsInnerException(
        Exception exception)
    {
        // Act
        InvalidResponseSitecoreLayoutServiceClientException sut = new(exception);

        // Assert
        sut.Message.Should().Be(DefaultMessage);
        sut.InnerException.Should().Be(exception);
    }
}