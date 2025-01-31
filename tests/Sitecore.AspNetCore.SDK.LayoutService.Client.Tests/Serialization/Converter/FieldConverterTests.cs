using System.Text.Json;
using FluentAssertions;
using Sitecore.AspNetCore.SDK.AutoFixture.Attributes;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Response.Model;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Response.Model.Fields;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Response.Model.Properties;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Serialization;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Serialization.Converter;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Serialization.Fields;
using Xunit;

namespace Sitecore.AspNetCore.SDK.LayoutService.Client.Tests.Serialization.Converter;

public class FieldConverterTests
{
    private readonly FieldConverter _sut = new();

    public static TheoryData<IFieldReader, string> Fields => new()
    {
        { new TextField("Test"), """{"editable":"","Id":"","value":"Test"}""" },
        { new RichTextField("Test Noencoding", false), """{"editable":"","Id":"","value":"Test Noencoding"}""" },
        { new RichTextField("Test%20Encoded"), """{"editable":"","Id":"","value":"Test Encoded"}""" },
        { new CheckboxField(true), """{"editable":"","Id":"","value":true}""" },
        { new CheckboxField(false), """{"editable":"","Id":""}""" },
        { new ImageField(new Image { Alt = "Alt Text", Border = 1, Class = "styleclass", HSpace = 1, Height = 100, Src = "https://image.com/test.jpg", Title = "Title", VSpace = 1, Width = 100 }), """{"editable":"","Id":"","value":{"src":"https://image.com/test.jpg","alt":"Alt Text","height":"100","width":"100","title":"Title","hSpace":"1","vSpace":"1","border":"1","class":"styleclass"}}""" }
    };

    [Fact]
    public void WriteJson_NullValues_ThrowsNullReferenceException()
    {
        // Act
        Action action = () => _sut.Write(null!, null!, null!);

        // Assert
        action.Should().Throw<ArgumentNullException>();
    }

    [Theory]
    [MemberData(nameof(Fields))]
    public void WriteJson_TextField_Success(IFieldReader field, string result)
    {
        // Arrange
        using MemoryStream stream = new();
        using Utf8JsonWriter writer = new(stream);

        // Act
        _sut.Write(writer, field, JsonLayoutServiceSerializer.GetDefaultSerializerOptions());

        // Assert
        stream.Position = 0;
        using StreamReader reader = new(stream);
        string value = reader.ReadToEnd();
        value.Should().Be(result);
    }

    [Fact]
    public void CanConvert_TypeIsField_ReturnsTrue()
    {
        _sut.CanConvert(typeof(IFieldReader)).Should().BeTrue();
    }

    [Fact]
    public void CanConvert_TypeIsNotField_ReturnsFalse()
    {
        _sut.CanConvert(typeof(string)).Should().BeFalse();
    }

    [Theory]
    [AutoNSubstituteData]
    public void ReadJson_Guarded(
        Type objectType,
        JsonSerializerOptions options)
    {
        // Act
        Action nullType = () => Read(null!, options);
        Action nullOptions = () => Read(objectType, null!);

        // Assert
        nullType.Should().Throw<ArgumentNullException>();
        nullOptions.Should().Throw<ArgumentNullException>();
        return;

        void Read(Type type, JsonSerializerOptions jsonOptions)
        {
            Utf8JsonReader reader = default;
            _sut.Read(ref reader, type, jsonOptions);
        }
    }

    [Theory]
    [AutoNSubstituteData]
    public void ReadJson_ReaderTokenTypeIsStartArray_ReturnsJsonSerializedFieldInstance(JsonSerializerOptions options)
    {
        // Arrange
        byte[] bytes = "[]"u8.ToArray();
        Utf8JsonReader reader = new(bytes);

        // Act
        IFieldReader actualValue = _sut.Read(ref reader, typeof(IFieldReader), options);

        // Assert
        actualValue.Should().BeOfType<JsonSerializedField>();
    }

    [Theory]
    [AutoNSubstituteData]
    public void ReadJson_ReaderTokenTypeIsStartObject_ReturnsJsonSerializedFieldInstance(JsonSerializerOptions options)
    {
        // Arrange
        byte[] bytes = "{}"u8.ToArray();
        Utf8JsonReader reader = new(bytes);

        // Act
        IFieldReader actualValue = _sut.Read(ref reader, typeof(IFieldReader), options);

        // Assert
        actualValue.Should().BeOfType<JsonSerializedField>();
    }

    [Theory]
    [AutoNSubstituteData]
    public void ReadJson_NonSupportedJson_ShouldThrowJsonException(JsonSerializerOptions options)
    {
        // Act
        Action invalidJson = Read;

        // Assert
        invalidJson.Should().Throw<JsonException>()
            .WithMessage($"Expected an array or object when deserializing a {typeof(IFieldReader)}. Found String");
        return;

        void Read()
        {
            byte[] bytes = "\"true\""u8.ToArray();
            Utf8JsonReader reader = new(bytes);
            _sut.Read(ref reader, typeof(IFieldReader), options);
        }
    }
}