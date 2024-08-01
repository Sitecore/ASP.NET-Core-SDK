using FluentAssertions;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Response.Model.Fields;
using Xunit;

namespace Sitecore.AspNetCore.SDK.LayoutService.Client.Tests.Response.Model.Fields;

public class ContentListFieldFixture : FieldFixture<ContentListField>
{
    [Fact]
    public void Ctor_SetsDefaults()
    {
        // Arrange / Act
        ContentListField sut = [];

        // Assert
        sut.Should().BeEmpty();
    }
}