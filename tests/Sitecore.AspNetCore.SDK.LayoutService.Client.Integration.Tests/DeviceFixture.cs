using AwesomeAssertions;
using Newtonsoft.Json.Linq;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Response;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Response.Model.Presentation;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Serialization;
using Xunit;

namespace Sitecore.AspNetCore.SDK.LayoutService.Client.Integration.Tests;

public class DeviceFixture
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0028:Simplify collection initialization", Justification = "Can't be done, confuses the compiler types.")]
    public static TheoryData<ISitecoreLayoutSerializer> Serializers => new()
    {
        new JsonLayoutServiceSerializer()
    };

    [Theory]
    [MemberData(nameof(Serializers))]
    public void Device_CanBeRead(ISitecoreLayoutSerializer serializer)
    {
        // Arrange
        string json = File.ReadAllText("./Json/edit.json");
        dynamic jsonModel = JObject.Parse(json);

        // Act
        SitecoreLayoutResponseContent? result = serializer.Deserialize(json);

        // Assert
        Device resultDevice = result!.Sitecore!.Devices[0];

        dynamic? expectedDevice = jsonModel.sitecore.devices[0];

        resultDevice.Should().NotBeNull();
        resultDevice.Id.Should().Be((string)expectedDevice.id);
        resultDevice.LayoutId.Should().Be((string)expectedDevice.layoutId);
        resultDevice.Placeholders.Should().BeEmpty();
        resultDevice.Renderings.Should().HaveCount(3);

        for (int i = 0; i < 3; i++)
        {
            resultDevice.Renderings[i].Id.Should().Be((string)expectedDevice.renderings[i].id);
            resultDevice.Renderings[i].InstanceId.Should().Be((string)expectedDevice.renderings[i].instanceId);
            resultDevice.Renderings[i].PlaceholderKey.Should().Be((string)expectedDevice.renderings[i].placeholderKey);
            resultDevice.Renderings[i].Parameters.Should().BeEmpty();
            resultDevice.Renderings[i].Caching!.Cacheable.Should().Be(null);
            resultDevice.Renderings[i].Caching!.ClearOnIndexUpdate.Should().Be(null);
            resultDevice.Renderings[i].Caching!.VaryByData.Should().Be(null);
            resultDevice.Renderings[i].Caching!.VaryByDevice.Should().Be(null);
            resultDevice.Renderings[i].Caching!.VaryByLogin.Should().Be(null);
            resultDevice.Renderings[i].Caching!.VaryByParameters.Should().Be(null);
            resultDevice.Renderings[i].Caching!.VaryByQueryString.Should().Be(null);
            resultDevice.Renderings[i].Caching!.VaryByUser.Should().Be(null);
            resultDevice.Renderings[i].Personalization!.Conditions.Should().Be(null);
            resultDevice.Renderings[i].Personalization!.MultiVariateTestId.Should().Be(null);
            resultDevice.Renderings[i].Personalization!.PersonalizationTest.Should().Be(null);
            resultDevice.Renderings[i].Personalization!.Rules.Should().Be(null);
        }
    }
}