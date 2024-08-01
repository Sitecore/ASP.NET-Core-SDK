using FluentAssertions;
using Newtonsoft.Json.Linq;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Response;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Response.Model;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Serialization;
using Xunit;

namespace Sitecore.AspNetCore.SDK.LayoutService.Client.Integration.Tests;

public class ContextFixture
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0028:Simplify collection initialization", Justification = "Can't be done, confuses the compiler types.")]
    public static TheoryData<ISitecoreLayoutSerializer> Serializers => new()
    {
        new JsonLayoutServiceSerializer()
    };

    [Theory]
    [MemberData(nameof(Serializers))]
    public void Context_CanBeRead(ISitecoreLayoutSerializer serializer)
    {
        // Arrange
        string json = File.ReadAllText("./Json/edit.json");
        dynamic jsonModel = JObject.Parse(json);

        // Act
        SitecoreLayoutResponseContent? result = serializer.Deserialize(json);

        // Assert
        Context? resultContext = result?.Sitecore?.Context;

        dynamic? expectedContext = jsonModel.sitecore.context;

        resultContext.Should().NotBeNull();
        resultContext!.IsEditing.Should().Be((bool)expectedContext.pageEditing);

        resultContext.Site.Should().NotBeNull();
        resultContext.Site!.Name.Should().Be((string)expectedContext.site.name);
        resultContext.PageState.Should().Be((PageState)expectedContext.pageState);
        resultContext.Language.Should().Be((string)expectedContext.language);
    }
}