using FluentAssertions;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Exceptions;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Response.Model;
using Xunit;

namespace Sitecore.AspNetCore.SDK.LayoutService.Client.Tests.Response.Model;

[System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1649:File name should match first type name", Justification = "Generic Type.")]
public abstract class FieldFixture<TField>
    where TField : IField, IFieldReader, new()
{
    private readonly TField _sut = new();

    [Fact]
    public void ReadT_InvalidType_ThrowsException()
    {
        // Arrange
        Action action = () => _sut.Read<Field<object>>();

        // Act / Assert
        action.Should().ThrowExactly<FieldReaderException>();
    }

    [Fact]
    public void ReadT_SameType_ReturnsSelf()
    {
        // Arrange / Act
        TField? result = _sut.Read<TField>();

        // Assert
        result.Should().BeSameAs(_sut);
    }

    [Fact]
    public void TryReadT_InvalidType_ReturnsFalseAndNull()
    {
        // Arrange / Act
        bool result = _sut.TryRead(out Field<object>? instance);

        // Assert
        result.Should().BeFalse();
        instance.Should().BeNull();
    }

    [Fact]
    public void TryReadT_SameType_ReturnsTrueAndSelf()
    {
        // Arrange / Act
        bool result = _sut.TryRead(out TField? instance);

        // Assert
        result.Should().BeTrue();
        instance.Should().BeSameAs(_sut);
    }

    [Fact]
    public void Read_InvalidType_ThrowsException()
    {
        // Arrange
        Action action = () => _sut.Read(typeof(Tuple));

        // Act / Assert
        action.Should().ThrowExactly<FieldReaderException>();
    }

    [Fact]
    public void Read_SameType_ReturnsSelf()
    {
        // Arrange / Act
        object? result = _sut.Read(typeof(TField));

        // Assert
        result.Should().BeSameAs(_sut);
    }

    [Fact]
    public void TryRead_InvalidType_ReturnsFalseAndNull()
    {
        // Arrange / Act
        bool result = _sut.TryRead(typeof(Tuple), out IField? instance);

        // Assert
        result.Should().BeFalse();
        instance.Should().BeNull();
    }

    [Fact]
    public void TryRead_SameType_ReturnsTrueAndSelf()
    {
        // Arrange / Act
        bool result = _sut.TryRead(typeof(TField), out IField? instance);

        // Assert
        result.Should().BeTrue();
        instance.Should().BeSameAs(_sut);
    }
}