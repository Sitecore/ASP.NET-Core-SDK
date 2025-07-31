using System.Net;
using AutoFixture;
using AwesomeAssertions;
using Sitecore.AspNetCore.SDK.AutoFixture.Attributes;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Extensions;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Request;
using Xunit;
using SitecoreLayoutRequestExtensions = Sitecore.AspNetCore.SDK.LayoutService.Client.Extensions.SitecoreLayoutRequestExtensions;

// ReSharper disable StringLiteralTypo
namespace Sitecore.AspNetCore.SDK.LayoutService.Client.Tests.Extensions.Http;

public class SitecoreLayoutRequestHttpExtensionsFixture
{
    // ReSharper disable once UnusedMember.Global - Used by testing framework
    public static Action<IFixture> AutoSetup => f =>
    {
        f.Inject(new SitecoreLayoutRequest());
    };

    [Theory]
    [AutoNSubstituteData]
    public void BuildDefaultSitecoreLayoutRequestUri_IsGuarded(SitecoreLayoutRequest request, Uri baseAddress)
    {
        // Arrange
        Func<Uri> allNull =
            () => SitecoreLayoutRequestExtensions.BuildDefaultSitecoreLayoutRequestUri(null!, null!);
        Func<Uri> uriNull =
            () => request.BuildDefaultSitecoreLayoutRequestUri(null!);
        Func<Uri> requestNull =
            () => SitecoreLayoutRequestExtensions.BuildDefaultSitecoreLayoutRequestUri(null!, baseAddress);

        // Assert
        allNull.Should().Throw<ArgumentNullException>();
        uriNull.Should().Throw<ArgumentNullException>();
        requestNull.Should().Throw<ArgumentNullException>();
    }

    [Theory]
    [AutoNSubstituteData]
    public void BuildUri_IsGuarded(SitecoreLayoutRequest request, Uri baseAddress, List<string> uriArgs)
    {
        // Arrange
        Func<Uri> requestNull =
            () => SitecoreLayoutRequestExtensions.BuildUri(null!, null!, null!);
        Func<Uri> allNull =
            () => request.BuildUri(null!, null!);
        Func<Uri> queryParamsNull =
            () => request.BuildUri(baseAddress, null!);
        Func<Uri> uriNull =
            () => request.BuildUri(null!, uriArgs);

        // Assert
        requestNull.Should().Throw<ArgumentNullException>();
        allNull.Should().Throw<ArgumentNullException>();
        queryParamsNull.Should().Throw<ArgumentNullException>();
        uriNull.Should().Throw<ArgumentNullException>();
    }

    [Theory]
    [AutoNSubstituteData]
    public void BuildDefaultSitecoreLayoutRequestUri_WithValidRequestContainingAllScEntries_ReturnsCorrectUrl(SitecoreLayoutRequest sut, Uri baseAddress)
    {
        // Arrange
        sut.ApiKey("apikey");
        sut.Language("en");
        sut.Path("home");
        sut.SiteName("site");
        sut.Mode("edit");
        sut.PreviewDate("121212");

        // Act
        Uri uri = sut.BuildDefaultSitecoreLayoutRequestUri(baseAddress);

        // Assert
        uri.Should().Be($"{baseAddress}?sc_apikey=apikey&sc_lang=en&item=home&sc_site=site&sc_mode=edit&sc_date=121212");
    }

    [Theory]
    [AutoNSubstituteData]
    public void BuildDefaultSitecoreLayoutRequestUri_WithValidRequestContainingVariousEntries_ReturnsCorrectUrl(SitecoreLayoutRequest sut, Uri baseAddress)
    {
        // Arrange
        sut.ApiKey("apikey");
        sut.Language("en");
        sut.Add("test1", "testvalue1");
        sut.Add("test2", "testvalue2");

        // Act
        Uri uri = sut.BuildDefaultSitecoreLayoutRequestUri(baseAddress);

        // Assert
        uri.Should().Be($"{baseAddress}?sc_apikey=apikey&sc_lang=en");
    }

    [Theory]
    [AutoNSubstituteData]
    public void BuildDefaultSitecoreLayoutRequestUri_WithValidRequestContainingVariousEntriesAndAdditionalParams_ReturnsCorrectUrl(SitecoreLayoutRequest sut, Uri baseAddress)
    {
        // Arrange
        sut.ApiKey("apikey");
        sut.Language("en");
        sut.Add("test1", "testvalue1");
        sut.Add("test2", "testvalue2");

        // Act
        Uri uri = sut.BuildDefaultSitecoreLayoutRequestUri(baseAddress, ["test1", "test2"]);

        // Assert
        uri.Should().Be($"{baseAddress}?sc_apikey=apikey&sc_lang=en&test1=testvalue1&test2=testvalue2");
    }

    [Theory]
    [AutoNSubstituteData]
    public void BuildUri_WithInvalidRequestEntries_ReturnsCorrectUrl(SitecoreLayoutRequest sut, Uri baseAddress)
    {
        // Arrange
        sut.ApiKey(null);
        sut.Language(string.Empty);
        sut.Path(string.Empty);
        sut.Add(RequestKeys.SiteName, new Cookie());
        List<string> defaultKeys =
            [RequestKeys.SiteName, RequestKeys.Path, RequestKeys.Language, RequestKeys.ApiKey];

        // Act
        Uri uri = sut.BuildUri(baseAddress, defaultKeys);

        // Assert
        uri.Should().Be($"{baseAddress}");
    }

    [Theory]
    [AutoNSubstituteData]
    public void BuildUri_WithValidRequestContainingScUnencodedEntries_ReturnsCorrectEncodedUrl(SitecoreLayoutRequest sut, Uri baseAddress)
    {
        // Arrange
        sut.ApiKey("<script type=\"text/javascript\">alert(\"hello\");</script>");
        List<string> defaultKeys =
            [RequestKeys.SiteName, RequestKeys.Path, RequestKeys.Language, RequestKeys.ApiKey];

        // Act
        Uri uri = sut.BuildUri(baseAddress, defaultKeys);

        // Assert
        uri.Should().Be($"{baseAddress}?sc_apikey=<script+type%3D\"text%2Fjavascript\">alert(\"hello\")%3B<%2Fscript>");
    }
}