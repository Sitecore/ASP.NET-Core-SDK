using AutoFixture;
using AwesomeAssertions;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Response.Model;
using Xunit;

namespace Sitecore.AspNetCore.SDK.LayoutService.Client.Tests.Response.Model;

public class FieldOfTValueFixture
{
    // ReSharper disable once UnusedMember.Global - Used by testing framework
    public static Action<IFixture> AutoSetup => f =>
    {
        f.Behaviors.Add(new OmitOnRecursionBehavior());
    };

    [Fact]
    public void FieldOfPrimitive_Fields_ReturnsValue()
    {
        // Arrange
        Field<string> sut = new() { Value = string.Empty };

        // Act
        string result = sut.Value;

        // Assert
        result.Should().Be(string.Empty);
    }

    [Fact]
    public void FieldOfClass_Fields_ReturnsDefault()
    {
        // Arrange
        Field<SimpleTestModel> sut = new() { Value = default! };

        // Act
        SimpleTestModel result = sut.Value;

        // Assert
        result.Should().Be(default);
    }

    [Fact]
    public void Read_Returns_CurrentInstance()
    {
        // Arrange
        Field<SimpleTestModel> sut = new() { Value = new SimpleTestModel() };

        // Act
        Field<SimpleTestModel>? result = sut.Read<Field<SimpleTestModel>>();

        // Assert
        result.Should().BeSameAs(sut);
    }
}