using FluentAssertions;
using NSubstitute;
using Sitecore.AspNetCore.SDK.RenderingEngine.Middleware.Models;
using Sitecore.AspNetCore.SDK.RenderingEngine.Services;
using Xunit;

// ReSharper disable StringLiteralTypo - Casing and spelling must be incorrect for testing
namespace Sitecore.AspNetCore.SDK.RenderingEngine.Tests.Services;

public class SiteResolverFixture
{
    private readonly ISiteCollectionService _siteCollectionService = Substitute.For<ISiteCollectionService>();

    [Theory]
    [InlineData("like.site.com", "site2")]
    [InlineData("longsitehost2.com", "site3")]
    [InlineData("ordeR.eu.site.com", "site3")]
    [InlineData("i.site.com", "site4")]
    public async Task GetByHost_ResolvesByHost_WithMultipleHostNames_AndWhitespaces(string hostName, string expectedSiteName)
    {
        // Arrange
        List<SiteInfo> siteCollection =
        [
            new SiteInfo { Name = "site1", HostName = "*.eu.site.com" },
            new SiteInfo { Name = "site2", HostName = "*.SITE.com | longsitehost.com" },
            new SiteInfo { Name = "site3", HostName = "ordeR.eu.site.com  | longsitehost2.com" },
            new SiteInfo { Name = "site4", HostName = "i.site.com" },
        ];

        _siteCollectionService.GetSitesCollection().Returns<SiteInfo?[]?>([.. siteCollection]);
        SiteResolver siteResolver = new(_siteCollectionService);

        // Act
        string? result = await siteResolver.GetByHost(hostName);

        // Assert
        result.Should().Be(expectedSiteName);
    }

    [Fact]
    public async Task GetByHost_ShouldReturnSiteWhenWildcardIsProvided()
    {
        // Arrange
        List<SiteInfo> siteCollection =
        [
            new SiteInfo { Name = "bar", HostName = "bar.net" },
            new SiteInfo { Name = "wildcard", HostName = "*" }
        ];

        _siteCollectionService.GetSitesCollection().Returns<SiteInfo?[]?>([.. siteCollection]);
        SiteResolver siteResolver = new(_siteCollectionService);

        // Act
        string? result = await siteResolver.GetByHost("foo.com");

        // Assert
        result.Should().Be("wildcard");
    }

    [Fact]
    public async Task GetByHost_ShouldPreferMostSpecificMatch()
    {
        // Arrange
        List<SiteInfo> siteCollection =
        [
            new SiteInfo { Name = "foo", HostName = "*" },
            new SiteInfo { Name = "bar", HostName = "*.app.net" },
            new SiteInfo { Name = "i-bar", HostName = "i.app.net" },
            new SiteInfo { Name = "baz", HostName = "baz.app.net" }
        ];

        _siteCollectionService.GetSitesCollection().Returns<SiteInfo?[]?>([.. siteCollection]);
        SiteResolver siteResolver = new(_siteCollectionService);

        // Act
        string? site1 = await siteResolver.GetByHost("foo.net");
        string? site2 = await siteResolver.GetByHost("bar.app.net");
        string? site3 = await siteResolver.GetByHost("i.app.net");
        string? site4 = await siteResolver.GetByHost("Baz.app.net");

        // Assert
        site1.Should().Be("foo");
        site2.Should().Be("bar");
        site3.Should().Be("i-bar");
        site4.Should().Be("baz");
    }

    [Fact]
    public async Task GetByHost_ShouldPreferFirstSiteMatchForSameHostName()
    {
        // Arrange
        List<SiteInfo> siteCollection =
        [
            new SiteInfo { Name = "foo", HostName = "*" },
            new SiteInfo { Name = "bar", HostName = "Bar.net" },
            new SiteInfo { Name = "foo-never", HostName = "*" },
            new SiteInfo { Name = "bar-never", HostName = "bar.net" }
        ];

        _siteCollectionService.GetSitesCollection().Returns<SiteInfo?[]?>([.. siteCollection]);
        SiteResolver siteResolver = new(_siteCollectionService);

        // Act
        string? site1 = await siteResolver.GetByHost("foo.net");
        string? site2 = await siteResolver.GetByHost("bar.net");

        // Assert
        site1.Should().Be("foo");
        site2.Should().Be("bar");
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public async Task GetByHost_ThrowsArgumentException_IfHostIsNullOrEmpty(string? hostName)
    {
        // Arrange
        SiteResolver siteResolver = new(_siteCollectionService);
        Func<Task<string?>> act =
            () => siteResolver.GetByHost(hostName!);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>().WithParameterName(nameof(hostName));
    }
}