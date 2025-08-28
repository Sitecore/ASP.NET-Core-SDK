using AwesomeAssertions;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Response.Model.Fields;
using Xunit;

namespace Sitecore.AspNetCore.SDK.LayoutService.Client.Tests.Response.Model.Fields;

public class CheckBoxFieldFixture : FieldFixture<CheckboxField>
{
    [Fact]
    public void Ctor_SetsDefaults()
    {
        // Arrange / Act
        CheckboxField instance = new();

        // Assert
        instance.Value.Should().Be(default);
        instance.EditableMarkup.Should().BeEmpty();
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Ctor_WithValue_SetsValue(bool value)
    {
        // Arrange / Act
        CheckboxField instance = new(value);

        // Assert
        instance.Value.Should().Be(value);
        instance.EditableMarkup.Should().BeEmpty();
    }
}