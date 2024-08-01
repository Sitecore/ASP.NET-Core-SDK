using FluentAssertions;
using Sitecore.AspNetCore.SDK.AutoFixture.Attributes;
using Sitecore.AspNetCore.SDK.RenderingEngine.Binding.Attributes;
using Xunit;

namespace Sitecore.AspNetCore.SDK.RenderingEngine.Tests.Binding.Attributes;

public class SitecoreComponentParameterAttributeFixture
{
    [Theory]
    [AutoNSubstituteData]
    public void Attribute_Name_ShouldNotBeNull(SitecoreComponentParameterAttribute sut)
    {
        // Assert
        sut.Name.Should().NotBeNull();
    }

    [Theory]
    [AutoNSubstituteData]
    public void Attribute_BindingSource_ShouldNotBeNull(SitecoreComponentParameterAttribute sut)
    {
        // Assert
        sut.BindingSource.Should().NotBeNull();
    }
}