using FluentAssertions;
using Sitecore.AspNetCore.SDK.AutoFixture.Attributes;
using Sitecore.AspNetCore.SDK.RenderingEngine.Binding.Attributes;
using Xunit;

namespace Sitecore.AspNetCore.SDK.RenderingEngine.Tests.Binding.Attributes;

public class SitecoreComponentFieldAttributeFixture
{
    [Theory]
    [AutoNSubstituteData]
    public void Attribute_Name_ShouldNotBeNull(SitecoreComponentFieldAttribute sut)
    {
        // Assert
        sut.Name.Should().NotBeNull();
    }

    [Theory]
    [AutoNSubstituteData]
    public void Attribute_BindingSource_ShouldNotBeNull(SitecoreComponentFieldAttribute sut)
    {
        // Assert
        sut.BindingSource.Should().NotBeNull();
    }
}