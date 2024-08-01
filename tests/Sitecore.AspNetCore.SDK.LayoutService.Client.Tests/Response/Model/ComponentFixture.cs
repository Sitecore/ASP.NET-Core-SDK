using AutoFixture;
using FluentAssertions;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Response.Model;
using Xunit;

namespace Sitecore.AspNetCore.SDK.LayoutService.Client.Tests.Response.Model;

public class ComponentFixture : FieldsReaderFixture<Component>
{
    // ReSharper disable once UnusedMember.Global - Used by testing framework
    public static Action<IFixture> AutoSetup => f =>
    {
        f.Behaviors.Add(new OmitOnRecursionBehavior());
    };

    [Fact]
    public void Ctor_SetsDefaults()
    {
        // Arrange
        Component sut = new();

        // Act
        bool idIsGuid = Guid.TryParse(sut.Id, out Guid _);

        // Assert
        idIsGuid.Should().BeTrue();
        sut.Name.Should().BeEmpty();
        sut.DataSource.Should().Be("available-in-connected-mode");
        sut.Parameters.Should().BeEmpty();
        sut.Placeholders.Should().BeEmpty();
        sut.Fields.Should().BeEmpty();
    }
}