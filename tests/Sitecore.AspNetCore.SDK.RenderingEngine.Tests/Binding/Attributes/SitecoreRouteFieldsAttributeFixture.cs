using FluentAssertions;
using Sitecore.AspNetCore.SDK.AutoFixture.Attributes;
using Sitecore.AspNetCore.SDK.RenderingEngine.Binding.Attributes;
using Xunit;

namespace Sitecore.AspNetCore.SDK.RenderingEngine.Tests.Binding.Attributes;

public class SitecoreRouteFieldsAttributeFixture
{
    [Theory]
    [AutoNSubstituteData]
    public void Attribute_BindingSource_ShouldNotBeNull(SitecoreRouteFieldsAttribute sut)
    {
        // Assert
        sut.BindingSource.Should().NotBeNull();
    }
}