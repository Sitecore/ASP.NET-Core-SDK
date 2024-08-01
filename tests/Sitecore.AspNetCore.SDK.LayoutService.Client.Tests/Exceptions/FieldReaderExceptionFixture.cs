using FluentAssertions;
using Sitecore.AspNetCore.SDK.AutoFixture.Attributes;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Exceptions;
using Xunit;

namespace Sitecore.AspNetCore.SDK.LayoutService.Client.Tests.Exceptions;

public class FieldReaderExceptionFixture
{
    private const string DefaultMessage = "The Field could not be read as the type {0}";

    [Theory]
    [AutoNSubstituteData]
    public void Ctor_WithMessage_SetsMessage(string message)
    {
        // Act
        FieldReaderException sut = new(message);

        // Assert
        sut.Message.Should().Be(message);
    }

    [Theory]
    [AutoNSubstituteData]
    public void Ctor_WithMessageAndException_SetsMessageAndInnerException(
        string message,
        Exception exception)
    {
        // Act
        FieldReaderException sut = new(message, exception);

        // Assert
        sut.Message.Should().Be(message);
        sut.InnerException.Should().Be(exception);
    }

    [Fact]
    public void Ctor_WithType_UsesDefaultMessage()
    {
        // Act
        Type type = typeof(int);
        FieldReaderException sut = new(type);

        // Assert
        sut.Message.Should().Be(string.Format(System.Globalization.CultureInfo.CurrentCulture, DefaultMessage, type));
    }

    [Theory]
    [AutoNSubstituteData]
    public void Ctor_WithException_SetsInnerException(
        Exception exception)
    {
        // Act
        Type type = typeof(int);
        FieldReaderException sut = new(type, exception);

        // Assert
        sut.Message.Should().Be(string.Format(System.Globalization.CultureInfo.CurrentCulture, DefaultMessage, type));
        sut.InnerException.Should().Be(exception);
    }
}