using AutoFixture;
using FluentAssertions;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Response.Model;
using Xunit;

namespace Sitecore.AspNetCore.SDK.LayoutService.Client.Tests.Response.Model;

public class RouteFixture : FieldsReaderFixture<Route>
{
    // ReSharper disable once UnusedMember.Global - Used by testing framework
    public static Action<IFixture> AutoSetup => f =>
    {
        f.Behaviors.Add(new OmitOnRecursionBehavior());
    };

    [Fact]
    public void Ctor_SetsDefaults()
    {
        // Arrange / Act
        Route sut = new();

        // Assert
        sut.DatabaseName.Should().BeNull();
        sut.DeviceId.Should().BeNull();
        sut.ItemId.Should().BeNull();
        sut.ItemLanguage.Should().BeNull();
        sut.ItemVersion.Should().BeNull();
        sut.LayoutId.Should().BeNull();
        sut.TemplateId.Should().BeNull();
        sut.TemplateName.Should().BeNull();
        sut.Name.Should().BeNull();
        sut.DisplayName.Should().BeNull();
        sut.Placeholders.Should().BeEmpty();
        sut.Fields.Should().BeEmpty();
    }
}