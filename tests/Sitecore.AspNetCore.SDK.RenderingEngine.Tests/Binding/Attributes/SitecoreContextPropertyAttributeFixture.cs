using FluentAssertions;
using Sitecore.AspNetCore.SDK.AutoFixture.Attributes;
using Sitecore.AspNetCore.SDK.RenderingEngine.Binding.Attributes;
using Xunit;

namespace Sitecore.AspNetCore.SDK.RenderingEngine.Tests.Binding.Attributes;

public class SitecoreContextPropertyAttributeFixture
{
    [Theory]
    [AutoNSubstituteData]
    public void Attribute_Name_ShouldNotBeNull(SitecoreContextPropertyAttribute sut)
    {
        // Assert
        sut.Name.Should().NotBeNull();
    }

    [Theory]
    [AutoNSubstituteData]
    public void Attribute_BindingSource_ShouldNotBeNull(SitecoreContextPropertyAttribute sut)
    {
        // Assert
        sut.BindingSource.Should().NotBeNull();
    }
}