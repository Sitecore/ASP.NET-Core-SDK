using FluentAssertions;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Request;
using Xunit;

namespace Sitecore.AspNetCore.SDK.LayoutService.Client.Tests.Request;

public class SitecoreLayoutRequestFixture
{
    private readonly SitecoreLayoutRequest _sut = new();

    [Fact]
    public void Ctor_IsEmptyDictionary()
    {
        // Assert
        _sut.Should().BeEmpty();
    }
}