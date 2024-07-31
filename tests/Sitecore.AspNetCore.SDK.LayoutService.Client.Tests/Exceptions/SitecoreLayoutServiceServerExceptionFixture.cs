using FluentAssertions;
using Sitecore.AspNetCore.SDK.AutoFixture.Attributes;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Exceptions;
using Xunit;

namespace Sitecore.AspNetCore.SDK.LayoutService.Client.Tests.Exceptions;

public class SitecoreLayoutServiceServerExceptionFixture
{
    private const string DefaultMessage = "The Sitecore layout service returned a server error.";

    [Theory]
    [AutoNSubstituteData]
    public void SitecoreLayoutServiceServerException_WithMessage_SetsMessage(string message)
    {
        // Act
        SitecoreLayoutServiceServerException sut = new(message);

        // Assert
        sut.Message.Should().Be(message);
    }

    [Theory]
    [AutoNSubstituteData]
    public void SitecoreLayoutServiceServerException_WithMessageAndException_SetsMessageAndInnerException(
        string message,
        Exception exception)
    {
        // Act
        SitecoreLayoutServiceServerException sut = new(message, exception);

        // Assert
        sut.Message.Should().Be(message);
        sut.InnerException.Should().Be(exception);
    }

    [Fact]
    public void SitecoreLayoutServiceServerException_WithNoMessage_UsesDefaultMessage()
    {
        // Act
        SitecoreLayoutServiceServerException sut = new();

        // Assert
        sut.Message.Should().Be(DefaultMessage);
    }

    [Theory]
    [AutoNSubstituteData]
    public void SitecoreLayoutServiceServerException_WithException_SetsInnerException(
        Exception exception)
    {
        // Act
        SitecoreLayoutServiceServerException sut = new(exception);

        // Assert
        sut.Message.Should().Be(DefaultMessage);
        sut.InnerException.Should().Be(exception);
    }
}