using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Sitecore.AspNetCore.SDK.RenderingEngine.Attributes;
using Xunit;

namespace Sitecore.AspNetCore.SDK.RenderingEngine.Tests.Attributes;

public class UseSitecoreRenderingAttributeFixture
{
    [Fact]
    public void Ctor_NullType_ThrowsException()
    {
        // Arrange
        Action action = () => _ = new UseSitecoreRenderingAttribute(null!);

        // Act / Assert
        action.Should().Throw<ArgumentNullException>()
            .And.ParamName.Should().Be("configurationType");
    }

    [Fact]
    public void Ctor_WithType_SetsType()
    {
        // Arrange / Act
        UseSitecoreRenderingAttribute sut = new(typeof(Controller));

        // Assert
        sut.ConfigurationType.Should().Be<Controller>();
    }
}