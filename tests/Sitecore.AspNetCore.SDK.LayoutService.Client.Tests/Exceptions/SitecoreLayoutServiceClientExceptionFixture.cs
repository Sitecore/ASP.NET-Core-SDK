using FluentAssertions;
using Sitecore.AspNetCore.SDK.AutoFixture.Attributes;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Exceptions;
using Xunit;

namespace Sitecore.AspNetCore.SDK.LayoutService.Client.Tests.Exceptions;

public class SitecoreLayoutServiceClientExceptionFixture
{
    private const string DefaultMessage = "An error occurred with the Sitecore layout service.";

    [Theory]
    [AutoNSubstituteData]
    public void SitecoreLayoutServiceClientException_WithMessage_SetsMessage(string message)
    {
        // Act
        SitecoreLayoutServiceClientException sut = new(message);

        // Assert
        sut.Message.Should().Be(message);
    }

    [Theory]
    [AutoNSubstituteData]
    public void SitecoreLayoutServiceClientException_WithMessageAndException_SetsMessageAndInnerException(
        string message,
        Exception exception)
    {
        // Act
        SitecoreLayoutServiceClientException sut = new(message, exception);

        // Assert
        sut.Message.Should().Be(message);
        sut.InnerException.Should().Be(exception);
    }

    [Fact]
    public void SitecoreLayoutServiceClientException_WithNoMessage_UsesDefaultMessage()
    {
        // Act
        SitecoreLayoutServiceClientException sut = new();

        // Assert
        sut.Message.Should().Be(DefaultMessage);
    }

    [Theory]
    [AutoNSubstituteData]
    public void SitecoreLayoutServiceClientException_WithException_SetsInnerException(
        Exception exception)
    {
        // Act
        SitecoreLayoutServiceClientException sut = new(exception);

        // Assert
        sut.Message.Should().Be(DefaultMessage);
        sut.InnerException.Should().Be(exception);
    }
}