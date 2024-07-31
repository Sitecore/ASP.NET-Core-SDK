using FluentAssertions;
using Sitecore.AspNetCore.SDK.AutoFixture.Attributes;
using Sitecore.AspNetCore.SDK.RenderingEngine.Binding.Attributes;
using Xunit;

namespace Sitecore.AspNetCore.SDK.RenderingEngine.Tests.Binding.Attributes;

public class SitecoreComponentPropertyAttributeFixture
{
    [Theory]
    [AutoNSubstituteData]
    public void Attribute_Name_ShouldNotBeNull(SitecoreComponentPropertyAttribute sut)
    {
        // Assert
        sut.Name.Should().NotBeNull();
    }

    [Theory]
    [AutoNSubstituteData]
    public void Attribute_BindingSource_ShouldNotBeNull(SitecoreComponentPropertyAttribute sut)
    {
        // Assert
        sut.BindingSource.Should().NotBeNull();
    }
}