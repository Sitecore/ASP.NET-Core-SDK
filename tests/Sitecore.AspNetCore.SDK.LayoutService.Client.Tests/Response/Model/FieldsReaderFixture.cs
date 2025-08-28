using AwesomeAssertions;
using Sitecore.AspNetCore.SDK.AutoFixture.Attributes;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Exceptions;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Response.Model;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Response.Model.Fields;
using Xunit;

namespace Sitecore.AspNetCore.SDK.LayoutService.Client.Tests.Response.Model;

public abstract class FieldsReaderFixture<TFieldsReader>
    where TFieldsReader : FieldsReader, new()
{
    private readonly TFieldsReader _sut = new();

    [Theory]
    [AutoNSubstituteData]
    public void ReadFieldT_MissingField_ReturnsNull(string name)
    {
        // Arrange
        _sut.Fields.Clear();

        // Act
        Field<string>? result = _sut.ReadField<Field<string>>(name);

        // Assert
        result.Should().BeNull();
    }

    [Theory]
    [MemberAutoNSubstituteData(nameof(Keys))]
    public void ReadFieldT_WithField_MatchesNameInsensitive(string fieldName, string readName)
    {
        // Arrange
        Field<string> field = new() { Value = string.Empty };
        _sut.Fields[fieldName] = field;

        // Act
        Field<string>? result = _sut.ReadField<Field<string>>(readName);

        // Assert
        result.Should().Be(field);
    }

    [Theory]
    [AutoNSubstituteData]
    public void TryReadFieldT_MissingField_ReturnsFalse(string name)
    {
        // Arrange
        _sut.Fields.Clear();

        // Act
        bool result = _sut.TryReadField(name, out Field<string>? field);

        // Assert
        result.Should().BeFalse();
        field.Should().BeNull();
    }

    [Theory]
    [MemberAutoNSubstituteData(nameof(Keys))]
    public void TryReadFieldT_WithField_MatchesNameInsensitive(string fieldName, string readName)
    {
        // Arrange
        Field<string> fieldInstance = new() { Value = string.Empty };
        _sut.Fields[fieldName] = fieldInstance;

        // Act
        bool result = _sut.TryReadField(readName, out Field<string>? field);

        // Assert
        result.Should().BeTrue();
        field.Should().Be(fieldInstance);
    }

    [Theory]
    [AutoNSubstituteData]
    public void ReadField_MissingField_ReturnsNull(string name)
    {
        // Arrange
        _sut.Fields.Clear();

        // Act
        object? result = _sut.ReadField(typeof(Field<string>), name);

        // Assert
        result.Should().BeNull();
    }

    [Theory]
    [AutoNSubstituteData]
    public void ReadField_InvalidType_ThrowsException(string name)
    {
        // Arrange
        _sut.Fields[name] = new Field<string> { Value = string.Empty };
        Action action = () => _sut.ReadField(typeof(string), name);

        // Act / Assert
        action.Should().ThrowExactly<FieldReaderException>();
    }

    [Theory]
    [MemberAutoNSubstituteData(nameof(Keys))]
    public void ReadField_WithField_MatchesNameInsensitive(string fieldName, string readName)
    {
        // Arrange
        Field<string> field = new() { Value = string.Empty };
        _sut.Fields[fieldName] = field;

        // Act
        object? result = _sut.ReadField(typeof(Field<string>), readName);

        // Assert
        result.Should().Be(field);
    }

    [Theory]
    [AutoNSubstituteData]
    public void TryReadField_MissingField_ReturnsFalse(string name)
    {
        // Arrange
        _sut.Fields.Clear();

        // Act
        bool result = _sut.TryReadField(typeof(Field<string>), name, out object? field);

        // Assert
        result.Should().BeFalse();
        field.Should().BeNull();
    }

    [Theory]
    [AutoNSubstituteData]
    public void TryReadField_InvalidType_ReturnsFalse(string name)
    {
        // Arrange
        _sut.Fields.Clear();

        // Act
        bool result = _sut.TryReadField(typeof(string), name, out object? field);

        // Assert
        result.Should().BeFalse();
        field.Should().BeNull();
    }

    [Theory]
    [MemberAutoNSubstituteData(nameof(Keys))]
    public void TryReadField_WithField_MatchesNameInsensitive(string fieldName, string readName)
    {
        // Arrange
        Field<string> fieldInstance = new() { Value = string.Empty };
        _sut.Fields[fieldName] = fieldInstance;

        // Act
        bool result = _sut.TryReadField(typeof(Field<string>), readName, out object? field);

        // Assert
        result.Should().BeTrue();
        field.Should().Be(fieldInstance);
    }

    [Fact]
    public void ReadFieldsT_NoFields_ReturnsDefaultInstance()
    {
        // Arrange
        _sut.Fields.Clear();

        // Act
        SimpleTestModel? result = _sut.ReadFields<SimpleTestModel>();

        // Act / Assert
        result.Should().NotBeNull();
    }

    [Fact]
    public void ReadFieldsT_WithFields_MatchesNameInsensitive()
    {
        // Arrange
        _sut.Fields["TEXT"] = new TextField { Value = "testing" };
        _sut.Fields["number"] = new NumberField { Value = 500 };

        // Act
        SimpleTestModel? result = _sut.ReadFields<SimpleTestModel>();

        // Act / Assert
        result.Should().NotBeNull();
        result!.Text!.Value.Should().Be("testing");
        result.Number!.Value.Should().Be(500);
    }

    [Fact]
    public void TryReadFieldsT_MissingField_ReturnsFalse()
    {
        // Arrange
        _sut.Fields.Clear();

        // Act
        bool result = _sut.TryReadFields(out SimpleTestModel? instance);

        // Assert
        result.Should().BeTrue();
        instance.Should().NotBeNull();
    }

    [Fact]
    public void TryReadFieldsT_WithField_MatchesNameInsensitive()
    {
        // Arrange
        _sut.Fields["TEXT"] = new TextField { Value = "testing" };
        _sut.Fields["number"] = new NumberField { Value = 500 };

        // Act
        bool result = _sut.TryReadFields(out SimpleTestModel? instance);

        // Assert
        result.Should().BeTrue();
        instance!.Text!.Value.Should().Be("testing");
        instance.Number!.Value.Should().Be(500);
    }

    [Fact]
    public void ReadFields_MissingField_ReturnsInstance()
    {
        // Arrange
        _sut.Fields.Clear();

        // Act
        object? result = _sut.ReadFields(typeof(SimpleTestModel));

        // Assert
        result.Should().NotBeNull();
    }

    [Fact]
    public void ReadFields_WithField_MatchesNameInsensitive()
    {
        // Arrange
        _sut.Fields["TEXT"] = new TextField { Value = "testing" };
        _sut.Fields["number"] = new NumberField { Value = 500 };

        // Act
        object? result = _sut.ReadFields(typeof(SimpleTestModel));

        // Assert
        result.Should().NotBeNull();
        SimpleTestModel? instance = result as SimpleTestModel;
        instance!.Text!.Value.Should().Be("testing");
        instance.Number!.Value.Should().Be(500);
    }

    [Fact]
    public void TryReadFields_MissingFields_ReturnsTrueWithInstance()
    {
        // Arrange
        _sut.Fields.Clear();

        // Act
        bool result = _sut.TryReadFields(typeof(SimpleTestModel), out object? instance);

        // Assert
        result.Should().BeTrue();
        instance.Should().NotBeNull();
    }

    [Fact]
    public void TryReadFields_WithFields_ReturnsTrueWithPopulatedInstance()
    {
        // Arrange
        _sut.Fields["TEXT"] = new TextField { Value = "testing" };
        _sut.Fields["number"] = new NumberField { Value = 500 };

        // Act
        bool result = _sut.TryReadFields(typeof(SimpleTestModel), out object? instance);

        // Assert
        result.Should().BeTrue();
        SimpleTestModel? typedInstance = instance as SimpleTestModel;
        typedInstance!.Text!.Value.Should().Be("testing");
        typedInstance.Number!.Value.Should().Be(500);
    }

    private static IEnumerable<object[]> Keys()
    {
        yield return ["this", "THIS"];
        yield return ["THIS", "this"];
        yield return ["This", "ThIs"];
    }
}