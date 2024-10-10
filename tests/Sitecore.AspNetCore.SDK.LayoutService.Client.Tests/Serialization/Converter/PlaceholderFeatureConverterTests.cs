using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using AutoFixture;
using FluentAssertions;
using Sitecore.AspNetCore.SDK.AutoFixture.Attributes;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Response.Model;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Serialization.Converter;
using Xunit;

namespace Sitecore.AspNetCore.SDK.LayoutService.Client.Tests.Serialization.Converter;

public class PlaceholderFeatureConverterTests
{
    private readonly PlaceholderFeatureConverter _sut = new(new PlaceholderParser(new FieldParser()));

    // ReSharper disable once UnusedMember.Global - Used by testing framework
    public static Action<IFixture> AutoSetup => f =>
    {
        f.Inject(new Placeholder());

        JsonSerializerOptions options = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            NumberHandling = JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString,
            Converters = { new FieldConverter(), new PlaceholderFeatureConverter(new PlaceholderParser(new FieldParser())) }
        };
        f.Inject(options);
    };

    [Fact]
    public void WriteJson_ThrowsNullReferenceException()
    {
        // Act
        Action action = () => _sut.Write(null!, null!, null!);

        // Assert
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void CanConvert_TypeIsPlaceholder_ReturnsTrue()
    {
        _sut.CanConvert(typeof(Placeholder)).Should().BeTrue();
    }

    [Fact]
    public void CanConvert_TypeIsNotPlaceholder_ReturnsFalse()
    {
        _sut.CanConvert(typeof(string)).Should().BeFalse();
    }

    [Theory]
    [AutoNSubstituteData]
    public void Read_Guarded(
        Type objectType,
        JsonSerializerOptions options)
    {
        // Act
        Action nullType = () => Read(null!, options);
        Action nullOptions = () => Read(objectType, null!);

        // Assert
        nullType.Should().Throw<ArgumentNullException>();
        nullOptions.Should().Throw<ArgumentNullException>();

        void Read(Type type, JsonSerializerOptions jsonOptions)
        {
            Utf8JsonReader reader = default;
            _sut.Read(ref reader, type, jsonOptions);
        }
    }

    [Theory]
    [AutoNSubstituteData]
    public void Read_OnlyComponents_ReturnsComponents(JsonSerializerOptions options)
    {
        // Arrange
        string json = File.ReadAllText("./Json/onlyComponents.json");
        byte[] bytes = [.. Encoding.UTF8.GetBytes(json)];
        Utf8JsonReader reader = new(bytes);
        reader.Read();

        // Act
        Placeholder result = _sut.Read(ref reader, typeof(Component), options);

        // Assert
        result.Should().HaveCount(2);
        Component? component1 = result.ComponentAt(0);
        component1!.Id.Should().Be("e02ddb9b-a062-5e50-924a-1940d7e053ce");
        component1.Name.Should().Be("ContentBlock");
        component1.DataSource.Should().Be("{585596CA-7903-500B-8DF2-0357DD6E3FAC}");
        component1.Fields["heading"].Read<Field<string>>()!.Value.Should().Be("Example heading");
        component1.Fields["content"].Read<Field<string>>()!.Value.Should().Be("Example content");
        Component? component2 = result.ComponentAt(1);
        component2!.Id.Should().Be("34a6553c-81de-5cd3-989e-853f6cb6df8c");
        component2.Name.Should().Be("Styleguide-Layout");
        component2.DataSource.Should().Be(string.Empty);
    }

    [Theory]
    [AutoNSubstituteData]
    public void Read_OnlyEditChromes_ReturnsEditChromes(JsonSerializerOptions options)
    {
        // Arrange
        string json = File.ReadAllText("./Json/onlyEditChromes.json");
        byte[] bytes = [.. Encoding.UTF8.GetBytes(json)];
        Utf8JsonReader reader = new(bytes);
        reader.Read();

        // Act
        Placeholder result = _sut.Read(ref reader, typeof(Placeholder), options);

        // Assert
        result.Should().HaveCount(4);
        EditableChrome? chrome1 = result.ChromeAt(0);
        chrome1.Should().NotBeNull();
        chrome1!.Name.Should().Be("code");
        chrome1.Type.Should().Be("text/sitecore");
        chrome1.Content.Should().Be("{\"commands\":[{\"click\":\"chrome:placeholder:addControl\",\"header\":\"Add to here\",\"icon\":\"/temp/iconcache/office/16x16/add.png\",\"disabledIcon\":\"/temp/add_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"Add a new rendering to the '{0}' placeholder.\",\"type\":\"\"},{\"click\":\"chrome:placeholder:editSettings\",\"header\":\"\",\"icon\":\"/temp/iconcache/office/16x16/window_gear.png\",\"disabledIcon\":\"/temp/window_gear_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"Edit the placeholder settings.\",\"type\":\"\"}],\"contextItemUri\":\"sitecore://master/{616E2DAA-BB71-5117-82B1-B360EF600213}?lang=en&ver=1\",\"custom\":{\"allowedRenderings\":[\"1DE91AADC1465D8983FA31A8FD63EBB3\",\"4E3C94B3A9D25478B7548D87283D8AA6\",\"26D9B310A5365D6B975442DB6BE1D381\",\"27EA18D87B6456108919947077956819\"],\"editable\":\"true\"},\"displayName\":\"Main\",\"expandedDisplayName\":null}");
        chrome1.Attributes.Should().HaveCount(7);
        chrome1.Attributes["type"].Should().Be("text/sitecore");
        chrome1.Attributes["chrometype"].Should().Be("placeholder");
        chrome1.Attributes["kind"].Should().Be("open");
        chrome1.Attributes["id"].Should().Be("jss_main");
        chrome1.Attributes["key"].Should().Be("jss-main");
        chrome1.Attributes["class"].Should().Be("scpm");
        chrome1.Attributes["data-selectable"].Should().Be("true");
    }

    [Theory]
    [AutoNSubstituteData]
    public void Read_MixedComponentsEditChromes_ReturnsComponentsAndEditChromes(JsonSerializerOptions options)
    {
        // Arrange
        string json = File.ReadAllText("./Json/mixedComponentsEditChromes.json");
        byte[] bytes = [.. Encoding.UTF8.GetBytes(json)];
        Utf8JsonReader reader = new(bytes);
        reader.Read();

        // Act
        Placeholder result = _sut.Read(ref reader, typeof(Placeholder), options);

        // Assert
        result.Should().HaveCount(6);

        EditableChrome? chrome1 = result.ChromeAt(0);
        chrome1.Should().NotBeNull();
        chrome1!.Name.Should().Be("code");
        chrome1.Type.Should().Be("text/sitecore");
        chrome1.Content.Should().Be("{\"commands\":[{\"click\":\"chrome:placeholder:addControl\",\"header\":\"Add to here\",\"icon\":\"/temp/iconcache/office/16x16/add.png\",\"disabledIcon\":\"/temp/add_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"Add a new rendering to the '{0}' placeholder.\",\"type\":\"\"},{\"click\":\"chrome:placeholder:editSettings\",\"header\":\"\",\"icon\":\"/temp/iconcache/office/16x16/window_gear.png\",\"disabledIcon\":\"/temp/window_gear_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"Edit the placeholder settings.\",\"type\":\"\"}],\"contextItemUri\":\"sitecore://master/{616E2DAA-BB71-5117-82B1-B360EF600213}?lang=en&ver=1\",\"custom\":{\"allowedRenderings\":[\"1DE91AADC1465D8983FA31A8FD63EBB3\",\"4E3C94B3A9D25478B7548D87283D8AA6\",\"26D9B310A5365D6B975442DB6BE1D381\",\"27EA18D87B6456108919947077956819\"],\"editable\":\"true\"},\"displayName\":\"Main\",\"expandedDisplayName\":null}");
        chrome1.Attributes.Should().HaveCount(7);
        chrome1.Attributes["type"].Should().Be("text/sitecore");
        chrome1.Attributes["chrometype"].Should().Be("placeholder");
        chrome1.Attributes["kind"].Should().Be("open");
        chrome1.Attributes["id"].Should().Be("jss_main");
        chrome1.Attributes["key"].Should().Be("jss-main");
        chrome1.Attributes["class"].Should().Be("scpm");
        chrome1.Attributes["data-selectable"].Should().Be("true");

        Component? component1 = result.ComponentAt(2);
        component1!.Id.Should().Be("e02ddb9b-a062-5e50-924a-1940d7e053ce");
        component1.Name.Should().Be("ContentBlock");
        component1.DataSource.Should().Be("{585596CA-7903-500B-8DF2-0357DD6E3FAC}");
        component1.Fields["heading"].Read<Field<string>>()!.Value.Should().Be("Example heading");
        component1.Fields["content"].Read<Field<string>>()!.Value.Should().Be("Example content");
        Component? component2 = result.ComponentAt(4);
        component2!.Id.Should().Be("34a6553c-81de-5cd3-989e-853f6cb6df8c");
        component2.Name.Should().Be("Styleguide-Layout");
        component2.DataSource.Should().Be(string.Empty);
    }

    [Theory]
    [AutoNSubstituteData]
    public void Read_IncorrectJson_ThrowsJsonException(JsonSerializerOptions options)
    {
        // Arrange
        void Read(Type type, JsonSerializerOptions jsonOptions)
        {
            const string json = "[true, true, false]";
            byte[] bytes = [.. Encoding.UTF8.GetBytes(json)];
            Utf8JsonReader reader = new(bytes);
            reader.Read();
            _sut.Read(ref reader, type, jsonOptions);
        }

        // Act
        Action result = () => Read(typeof(Placeholder), options);

        // Assert
        result.Should().Throw<JsonException>();
    }

    [Theory]
    [AutoNSubstituteData]
    public void Read_IncorrectParams_ThrowsJsonException(JsonSerializerOptions options)
    {
        // Arrange
        void Read(Type type, JsonSerializerOptions jsonOptions)
        {
            const string json = "[{\"params\": true}]";
            byte[] bytes = [.. Encoding.UTF8.GetBytes(json)];
            Utf8JsonReader reader = new(bytes);
            reader.Read();
            _sut.Read(ref reader, type, jsonOptions);
        }

        // Act
        Action result = () => Read(typeof(Placeholder), options);

        // Assert
        result.Should().Throw<JsonException>();
    }

    [Theory]
    [AutoNSubstituteData]
    public void Read_IncorrectPlaceholders_ThrowsJsonException(JsonSerializerOptions options)
    {
        // Arrange
        void Read(Type type, JsonSerializerOptions jsonOptions)
        {
            const string json = "[{\"placeholders\": true}]";
            byte[] bytes = [.. Encoding.UTF8.GetBytes(json)];
            Utf8JsonReader reader = new(bytes);
            reader.Read();
            _sut.Read(ref reader, type, jsonOptions);
        }

        // Act
        Action result = () => Read(typeof(Placeholder), options);

        // Assert
        result.Should().Throw<JsonException>();
    }

    [Theory]
    [AutoNSubstituteData]
    public void Read_NotArray_ThrowsJsonException(JsonSerializerOptions options)
    {
        // Arrange
        void Read(Type type, JsonSerializerOptions jsonOptions)
        {
            const string json = "{}";
            byte[] bytes = [.. Encoding.UTF8.GetBytes(json)];
            Utf8JsonReader reader = new(bytes);
            reader.Read();
            _sut.Read(ref reader, type, jsonOptions);
        }

        // Act
        Action result = () => Read(typeof(Placeholder), options);

        // Assert
        result.Should().Throw<JsonException>();
    }
}