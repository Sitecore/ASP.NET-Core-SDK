using AwesomeAssertions;
using Newtonsoft.Json.Linq;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Response;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Response.Model;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Serialization;
using Xunit;

// ReSharper disable StringLiteralTypo
namespace Sitecore.AspNetCore.SDK.LayoutService.Client.Integration.Tests;

public class EditableChromeFixture
{
    public static TheoryData<ISitecoreLayoutSerializer, string> Data => new()
    {
        {
            new JsonLayoutServiceSerializer(),
            "edit-in-horizon-mode"
        },
        {
            new JsonLayoutServiceSerializer(),
            "edit"
        }
    };

    [Theory]
    [MemberData(nameof(Data))]
    public void Component_OpeningChrome_CanBeRead(ISitecoreLayoutSerializer serializer, string editResponseFileName)
    {
        // Arrange
        string json = File.ReadAllText($"./Json/{editResponseFileName}.json");
        dynamic jsonModel = JObject.Parse(json);

        // Act
        SitecoreLayoutResponseContent? result = serializer.Deserialize(json);

        // Assert
        EditableChrome? resultChrome = result?.Sitecore?.Route?.Placeholders["jss-main"].ChromeAt(1);

        dynamic? expectedChrome = jsonModel.sitecore.route.placeholders["jss-main"][1];

        resultChrome.Should().NotBeNull();
        resultChrome!.Name.Should().Be((string)expectedChrome.name);
        resultChrome.Type.Should().Be((string)expectedChrome.type);
        resultChrome.Content.Should().Be((string)expectedChrome.contents);
        resultChrome.Attributes.Should().HaveCount(7);
        resultChrome.Attributes["type"].Should().Be((string)expectedChrome.attributes.type);
        resultChrome.Attributes["chrometype"].Should().Be((string)expectedChrome.attributes.chrometype);
        resultChrome.Attributes["kind"].Should().Be((string)expectedChrome.attributes.kind);
        resultChrome.Attributes["id"].Should().Be((string)expectedChrome.attributes.id);
        resultChrome.Attributes["hintname"].Should().Be((string)expectedChrome.attributes.hintname);
        resultChrome.Attributes["class"].Should().Be((string)expectedChrome.attributes.@class);
        resultChrome.Attributes["data-selectable"].Should().Be((string)expectedChrome.attributes["data-selectable"]);
    }

    [Theory]
    [MemberData(nameof(Data))]
    public void Component_ClosingChrome_CanBeRead(ISitecoreLayoutSerializer serializer, string editResponseFileName)
    {
        // Arrange
        string json = File.ReadAllText($"./Json/{editResponseFileName}.json");
        dynamic jsonModel = JObject.Parse(json);

        // Act
        SitecoreLayoutResponseContent? result = serializer.Deserialize(json);

        // Assert
        EditableChrome? resultChrome = result?.Sitecore?.Route?.Placeholders["jss-main"].ChromeAt(3);

        dynamic? expectedChrome = jsonModel.sitecore.route.placeholders["jss-main"][3];

        resultChrome.Should().NotBeNull();
        resultChrome!.Name.Should().Be((string)expectedChrome.name);
        resultChrome.Type.Should().Be((string)expectedChrome.type);
        resultChrome.Content.Should().Be((string)expectedChrome.contents);
        resultChrome.Attributes.Should().HaveCount(6);
        resultChrome.Attributes["type"].Should().Be((string)expectedChrome.attributes.type);
        resultChrome.Attributes["chrometype"].Should().Be((string)expectedChrome.attributes.chrometype);
        resultChrome.Attributes["kind"].Should().Be((string)expectedChrome.attributes.kind);
        resultChrome.Attributes["id"].Should().Be((string)expectedChrome.attributes.id);
        resultChrome.Attributes["hintkey"].Should().Be((string)expectedChrome.attributes.hintkey);
        resultChrome.Attributes["class"].Should().Be((string)expectedChrome.attributes.@class);
    }

    [Theory]
    [MemberData(nameof(Data))]
    public void Placeholder_OpeningChrome_CanBeRead(ISitecoreLayoutSerializer serializer, string editResponseFileName)
    {
        // Arrange
        string json = File.ReadAllText($"./Json/{editResponseFileName}.json");
        dynamic jsonModel = JObject.Parse(json);

        // Act
        SitecoreLayoutResponseContent? result = serializer.Deserialize(json);

        // Assert
        EditableChrome? resultChrome = result?.Sitecore?.Route?.Placeholders["jss-main"].ChromeAt(0);

        dynamic? expectedChrome = jsonModel.sitecore.route.placeholders["jss-main"][0];

        resultChrome.Should().NotBeNull();
        resultChrome!.Name.Should().Be((string)expectedChrome.name);
        resultChrome.Type.Should().Be((string)expectedChrome.type);
        resultChrome.Content.Should().Be((string)expectedChrome.contents);
        resultChrome.Attributes.Should().HaveCount(7);
        resultChrome.Attributes["type"].Should().Be((string)expectedChrome.attributes.type);
        resultChrome.Attributes["chrometype"].Should().Be((string)expectedChrome.attributes.chrometype);
        resultChrome.Attributes["kind"].Should().Be((string)expectedChrome.attributes.kind);
        resultChrome.Attributes["id"].Should().Be((string)expectedChrome.attributes.id);
        resultChrome.Attributes["key"].Should().Be((string)expectedChrome.attributes.key);
        resultChrome.Attributes["class"].Should().Be((string)expectedChrome.attributes.@class);
        resultChrome.Attributes["data-selectable"].Should().Be((string)expectedChrome.attributes["data-selectable"]);
    }

    [Theory]
    [MemberData(nameof(Data))]
    public void Placeholder_ClosingChrome_CanBeRead(ISitecoreLayoutSerializer serializer, string editResponseFileName)
    {
        // Arrange
        string json = File.ReadAllText($"./Json/{editResponseFileName}.json");
        dynamic jsonModel = JObject.Parse(json);

        // Act
        SitecoreLayoutResponseContent? result = serializer.Deserialize(json);

        // Assert
        EditableChrome? resultChrome = result?.Sitecore?.Route?.Placeholders["jss-main"].ChromeAt(7);

        dynamic? expectedChrome = jsonModel.sitecore.route.placeholders["jss-main"][7];

        resultChrome.Should().NotBeNull();
        resultChrome!.Name.Should().Be((string)expectedChrome.name);
        resultChrome.Type.Should().Be((string)expectedChrome.type);
        resultChrome.Content.Should().Be((string)expectedChrome.contents);
        resultChrome.Attributes.Should().HaveCount(6);
        resultChrome.Attributes["type"].Should().Be((string)expectedChrome.attributes.type);
        resultChrome.Attributes["chrometype"].Should().Be((string)expectedChrome.attributes.chrometype);
        resultChrome.Attributes["kind"].Should().Be((string)expectedChrome.attributes.kind);
        resultChrome.Attributes["id"].Should().Be((string)expectedChrome.attributes.id);
        resultChrome.Attributes["hintname"].Should().Be((string)expectedChrome.attributes.hintname);
        resultChrome.Attributes["class"].Should().Be((string)expectedChrome.attributes.@class);
    }
}