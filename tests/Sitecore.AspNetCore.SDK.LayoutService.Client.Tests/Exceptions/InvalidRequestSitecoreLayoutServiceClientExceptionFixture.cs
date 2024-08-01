using FluentAssertions;
using Sitecore.AspNetCore.SDK.AutoFixture.Attributes;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Exceptions;
using Xunit;

namespace Sitecore.AspNetCore.SDK.LayoutService.Client.Tests.Exceptions;

public class InvalidRequestSitecoreLayoutServiceClientExceptionFixture
{
    private const string DefaultMessage = "An invalid request was sent to the Sitecore layout service.";

    [Theory]
    [AutoNSubstituteData]
    public void InvalidRequestSitecoreLayoutServiceClientException_WithMessage_SetsMessage(string message)
    {
        // Act
        InvalidRequestSitecoreLayoutServiceClientException sut = new(message);

        // Assert
        sut.Message.Should().Be(message);
    }

    [Theory]
    [AutoNSubstituteData]
    public void InvalidRequestSitecoreLayoutServiceClientException_WithMessageAndException_SetsMessageAndInnerException(
        string message,
        Exception exception)
    {
        // Act
        InvalidRequestSitecoreLayoutServiceClientException sut = new(message, exception);

        // Assert
        sut.Message.Should().Be(message);
        sut.InnerException.Should().Be(exception);
    }

    [Fact]
    public void InvalidRequestSitecoreLayoutServiceClientException_WithNoMessage_UsesDefaultMessage()
    {
        // Act
        InvalidRequestSitecoreLayoutServiceClientException sut = new();

        // Assert
        sut.Message.Should().Be(DefaultMessage);
    }

    [Theory]
    [AutoNSubstituteData]
    public void InvalidRequestSitecoreLayoutServiceClientException_WithException_SetsInnerException(
        Exception exception)
    {
        // Act
        InvalidRequestSitecoreLayoutServiceClientException sut = new(exception);

        // Assert
        sut.Message.Should().Be(DefaultMessage);
        sut.InnerException.Should().Be(exception);
    }
}