using AutoFixture;
using AwesomeAssertions;
using Sitecore.AspNetCore.SDK.AutoFixture.Attributes;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Request;
using Xunit;

namespace Sitecore.AspNetCore.SDK.LayoutService.Client.Tests.Request;

public class SitecoreLayoutRequestExtensionsFixture
{
    // ReSharper disable once UnusedMember.Global - Used by testing framework
    public static Action<IFixture> AutoSetup => f =>
    {
        f.Inject(new SitecoreLayoutRequest());
    };

    [Theory]
    [AutoNSubstituteData]
    public void Entries_Treated_As_Case_Insensitive(SitecoreLayoutRequest sut, string key, string value)
    {
        // Arrange
        sut[key] = value;

        // Act
        sut[key.ToUpperInvariant()] = value;

        // Assert
        sut.Keys.Should().ContainSingle(x => x == key);
        sut.Values.Should().ContainSingle(x => (string)x! == value);
    }

    #region ApiKey

    [Fact]
    public void ApiKey_NullRequest_Throws()
    {
        // Arrange
        Action action = () => SitecoreLayoutRequestExtensions.ApiKey(null!);

        // Assert
        action.Should().Throw<ArgumentNullException>()
            .And.ParamName.Should().Be("request");
    }

    [Theory]
    [AutoNSubstituteData]
    public void ApiKey_MissingEntry_ReturnsNull(SitecoreLayoutRequest sut)
    {
        // Arrange / Act
        string? result = sut.ApiKey();

        // Assert
        result.Should().BeNull();
    }

    [Theory]
    [AutoNSubstituteData]
    public void ApiKey_NullEntry_ReturnsNull(SitecoreLayoutRequest sut)
    {
        // Arrange
        sut[RequestKeys.ApiKey] = null;

        // Act
        string? result = sut.ApiKey();

        // Assert
        result.Should().BeNull();
    }

    [Theory]
    [AutoNSubstituteData]
    public void ApiKey_ValidValueEntry_ReturnsNull(SitecoreLayoutRequest sut, string value)
    {
        // Arrange
        sut[RequestKeys.ApiKey] = value;

        // Act
        string? result = sut.ApiKey();

        // Assert
        result.Should().Be(value);
    }

    [Theory]
    [AutoNSubstituteData]
    public void ApiKey_InvalidValueEntry_ReturnsDefault(SitecoreLayoutRequest sut, Guid value)
    {
        // Arrange
        sut[RequestKeys.ApiKey] = value;
        string? result = sut.ApiKey();

        // Act / Assert
        result.Should().Be(default);
    }

    [Fact]
    public void ApiKey_NullRequestWithValue_Throws()
    {
        // Arrange
        Action action = () => SitecoreLayoutRequestExtensions.ApiKey(null!, null);

        // Assert
        action.Should().Throw<ArgumentNullException>()
            .And.ParamName.Should().Be("request");
    }

    [Theory]
    [AutoNSubstituteData]
    public void ApiKey_WithValue_SetsEntry(SitecoreLayoutRequest sut, string value)
    {
        // Arrange / Act
        sut.ApiKey(value);

        // Assert
        sut[RequestKeys.ApiKey].Should().Be(value);
    }

    [Theory]
    [AutoNSubstituteData]
    public void ApiKey_WithNullValue_SetsEntry(SitecoreLayoutRequest sut)
    {
        // Act
        sut.ApiKey(null);

        // Assert
        bool result = sut.TryGetValue(RequestKeys.ApiKey, out object? _);
        result.Should().BeTrue();
    }

    [Theory]
    [AutoNSubstituteData]
    public void ApiKey_ReturnsRequest(SitecoreLayoutRequest sut)
    {
        // Arrange / Act
        SitecoreLayoutRequest result = sut.ApiKey(null);

        // Assert
        result.Should().BeSameAs(sut);
    }

    #endregion

    #region SiteName

    [Fact]
    public void SiteName_NullRequest_Throws()
    {
        // Arrange
        Action action = () => SitecoreLayoutRequestExtensions.SiteName(null!);

        // Assert
        action.Should().Throw<ArgumentNullException>()
            .And.ParamName.Should().Be("request");
    }

    [Theory]
    [AutoNSubstituteData]
    public void SiteName_MissingEntry_ReturnsNull(SitecoreLayoutRequest sut)
    {
        // Arrange / Act
        string? result = sut.SiteName();

        // Assert
        result.Should().BeNull();
    }

    [Theory]
    [AutoNSubstituteData]
    public void SiteName_NullEntry_ReturnsNull(SitecoreLayoutRequest sut)
    {
        // Arrange
        sut[RequestKeys.SiteName] = null;

        // Act
        string? result = sut.SiteName();

        // Assert
        result.Should().BeNull();
    }

    [Theory]
    [AutoNSubstituteData]
    public void SiteName_ValidValueEntry_ReturnsNull(SitecoreLayoutRequest sut, string value)
    {
        // Arrange
        sut[RequestKeys.SiteName] = value;

        // Act
        string? result = sut.SiteName();

        // Assert
        result.Should().Be(value);
    }

    [Theory]
    [AutoNSubstituteData]
    public void SiteName_InvalidValueEntry_ReturnsDefault(SitecoreLayoutRequest sut, Guid value)
    {
        // Arrange
        sut[RequestKeys.SiteName] = value;
        string? result = sut.SiteName();

        // Act / Assert
        result.Should().Be(default);
    }

    [Fact]
    public void SiteName_NullRequestWithValue_Throws()
    {
        // Arrange
        Action action = () => SitecoreLayoutRequestExtensions.SiteName(null!, null);

        // Assert
        action.Should().Throw<ArgumentNullException>()
            .And.ParamName.Should().Be("request");
    }

    [Theory]
    [AutoNSubstituteData]
    public void SiteName_WithValue_SetsEntry(SitecoreLayoutRequest sut, string value)
    {
        // Arrange / Act
        sut.SiteName(value);

        // Assert
        sut[RequestKeys.SiteName].Should().Be(value);
    }

    [Theory]
    [AutoNSubstituteData]
    public void SiteName_WithNullValue_SetsEntry(SitecoreLayoutRequest sut)
    {
        // Act
        sut.SiteName(null);

        // Assert
        bool result = sut.TryGetValue(RequestKeys.SiteName, out object? _);
        result.Should().BeTrue();
    }

    [Theory]
    [AutoNSubstituteData]
    public void SiteName_ReturnsRequest(SitecoreLayoutRequest sut)
    {
        // Arrange / Act
        SitecoreLayoutRequest result = sut.SiteName(null);

        // Assert
        result.Should().BeSameAs(sut);
    }

    #endregion

    #region Language

    [Fact]
    public void Language_NullRequest_Throws()
    {
        // Arrange
        Action action = () => SitecoreLayoutRequestExtensions.Language(null!);

        // Assert
        action.Should().Throw<ArgumentNullException>()
            .And.ParamName.Should().Be("request");
    }

    [Theory]
    [AutoNSubstituteData]
    public void Language_MissingEntry_ReturnsNull(SitecoreLayoutRequest sut)
    {
        // Arrange / Act
        string? result = sut.Language();

        // Assert
        result.Should().BeNull();
    }

    [Theory]
    [AutoNSubstituteData]
    public void Language_NullEntry_ReturnsNull(SitecoreLayoutRequest sut)
    {
        // Arrange
        sut[RequestKeys.Language] = null;

        // Act
        string? result = sut.Language();

        // Assert
        result.Should().BeNull();
    }

    [Theory]
    [AutoNSubstituteData]
    public void Language_ValidValueEntry_ReturnsNull(SitecoreLayoutRequest sut, string value)
    {
        // Arrange
        sut[RequestKeys.Language] = value;

        // Act
        string? result = sut.Language();

        // Assert
        result.Should().Be(value);
    }

    [Theory]
    [AutoNSubstituteData]
    public void Language_InvalidValueEntry_ReturnsDefault(SitecoreLayoutRequest sut, Guid value)
    {
        // Arrange
        sut[RequestKeys.Language] = value;
        string? result = sut.Language();

        // Act / Assert
        result.Should().Be(default);
    }

    [Fact]
    public void Language_NullRequestWithValue_Throws()
    {
        // Arrange
        Action action = () => SitecoreLayoutRequestExtensions.Language(null!, null);

        // Assert
        action.Should().Throw<ArgumentNullException>()
            .And.ParamName.Should().Be("request");
    }

    [Theory]
    [AutoNSubstituteData]
    public void Language_WithValue_SetsEntry(SitecoreLayoutRequest sut, string value)
    {
        // Arrange / Act
        sut.Language(value);

        // Assert
        sut[RequestKeys.Language].Should().Be(value);
    }

    [Theory]
    [AutoNSubstituteData]
    public void Language_WithNullValue_SetsEntry(SitecoreLayoutRequest sut)
    {
        // Act
        sut.Language(null);

        // Assert
        bool result = sut.TryGetValue(RequestKeys.Language, out object? _);
        result.Should().BeTrue();
    }

    [Theory]
    [AutoNSubstituteData]
    public void Language_ReturnsRequest(SitecoreLayoutRequest sut)
    {
        // Arrange / Act
        SitecoreLayoutRequest result = sut.Language(null);

        // Assert
        result.Should().BeSameAs(sut);
    }

    #endregion

    #region PreviewDate

    [Fact]
    public void PreviewDate_NullRequest_Throws()
    {
        // Arrange
        Action action = () => SitecoreLayoutRequestExtensions.PreviewDate(null!);

        // Assert
        action.Should().Throw<ArgumentNullException>()
            .And.ParamName.Should().Be("request");
    }

    [Theory]
    [AutoNSubstituteData]
    public void PreviewDate_MissingEntry_ReturnsNull(SitecoreLayoutRequest sut)
    {
        // Arrange / Act
        string? result = sut.PreviewDate();

        // Assert
        result.Should().BeNull();
    }

    [Theory]
    [AutoNSubstituteData]
    public void PreviewDate_NullEntry_ReturnsNull(SitecoreLayoutRequest sut)
    {
        // Arrange
        sut[RequestKeys.PreviewDate] = null;

        // Act
        string? result = sut.PreviewDate();

        // Assert
        result.Should().BeNull();
    }

    [Theory]
    [AutoNSubstituteData]
    public void PreviewDate_ValidValueEntry_ReturnsNull(SitecoreLayoutRequest sut, string value)
    {
        // Arrange
        sut[RequestKeys.PreviewDate] = value;

        // Act
        string? result = sut.PreviewDate();

        // Assert
        result.Should().Be(value);
    }

    [Theory]
    [AutoNSubstituteData]
    public void PreviewDate_InvalidValueEntry_ReturnsDefault(SitecoreLayoutRequest sut, Guid value)
    {
        // Arrange
        sut[RequestKeys.PreviewDate] = value;
        string? result = sut.PreviewDate();

        // Act / Assert
        result.Should().Be(default);
    }

    [Fact]
    public void PreviewDate_NullRequestWithValue_Throws()
    {
        // Arrange
        Action action = () => SitecoreLayoutRequestExtensions.PreviewDate(null!, null);

        // Assert
        action.Should().Throw<ArgumentNullException>()
            .And.ParamName.Should().Be("request");
    }

    [Theory]
    [AutoNSubstituteData]
    public void PreviewDate_WithValue_SetsEntry(SitecoreLayoutRequest sut, string value)
    {
        // Arrange / Act
        sut.PreviewDate(value);

        // Assert
        sut[RequestKeys.PreviewDate].Should().Be(value);
    }

    [Theory]
    [AutoNSubstituteData]
    public void PreviewDate_WithNullValue_SetsEntry(SitecoreLayoutRequest sut)
    {
        // Act
        sut.PreviewDate(null);

        // Assert
        bool result = sut.TryGetValue(RequestKeys.PreviewDate, out object? _);
        result.Should().BeTrue();
    }

    [Theory]
    [AutoNSubstituteData]
    public void PreviewDate_ReturnsRequest(SitecoreLayoutRequest sut)
    {
        // Arrange / Act
        SitecoreLayoutRequest result = sut.PreviewDate(null);

        // Assert
        result.Should().BeSameAs(sut);
    }

    #endregion

    #region Mode

    [Fact]
    public void Mode_NullRequest_Throws()
    {
        // Arrange
        Action action = () => SitecoreLayoutRequestExtensions.Mode(null!);

        // Assert
        action.Should().Throw<ArgumentNullException>()
            .And.ParamName.Should().Be("request");
    }

    [Theory]
    [AutoNSubstituteData]
    public void Mode_MissingEntry_ReturnsNull(SitecoreLayoutRequest sut)
    {
        // Arrange / Act
        string? result = sut.Mode();

        // Assert
        result.Should().BeNull();
    }

    [Theory]
    [AutoNSubstituteData]
    public void Mode_NullEntry_ReturnsNull(SitecoreLayoutRequest sut)
    {
        // Arrange
        sut[RequestKeys.Mode] = null;

        // Act
        string? result = sut.Mode();

        // Assert
        result.Should().BeNull();
    }

    [Theory]
    [AutoNSubstituteData]
    public void Mode_ValidValueEntry_ReturnsNull(SitecoreLayoutRequest sut, string value)
    {
        // Arrange
        sut[RequestKeys.Mode] = value;

        // Act
        string? result = sut.Mode();

        // Assert
        result.Should().Be(value);
    }

    [Theory]
    [AutoNSubstituteData]
    public void Mode_InvalidValueEntry_ReturnsDefault(SitecoreLayoutRequest sut, Guid value)
    {
        // Arrange
        sut[RequestKeys.Mode] = value;
        string? result = sut.Mode();

        // Act / Assert
        result.Should().Be(default);
    }

    [Fact]
    public void Mode_NullRequestWithValue_Throws()
    {
        // Arrange
        Action action = () => SitecoreLayoutRequestExtensions.Mode(null!, null);

        // Assert
        action.Should().Throw<ArgumentNullException>()
            .And.ParamName.Should().Be("request");
    }

    [Theory]
    [AutoNSubstituteData]
    public void Mode_WithValue_SetsEntry(SitecoreLayoutRequest sut, string value)
    {
        // Arrange / Act
        sut.Mode(value);

        // Assert
        sut[RequestKeys.Mode].Should().Be(value);
    }

    [Theory]
    [AutoNSubstituteData]
    public void Mode_WithNullValue_SetsEntry(SitecoreLayoutRequest sut)
    {
        // Act
        sut.Mode(null);

        // Assert
        bool result = sut.TryGetValue(RequestKeys.Mode, out object? _);
        result.Should().BeTrue();
    }

    [Theory]
    [AutoNSubstituteData]
    public void Mode_ReturnsRequest(SitecoreLayoutRequest sut)
    {
        // Arrange / Act
        SitecoreLayoutRequest result = sut.Mode(null);

        // Assert
        result.Should().BeSameAs(sut);
    }

    #endregion

    #region AuthenticationHeader

    [Fact]
    public void AuthenticationHeader_NullRequest_Throws()
    {
        // Arrange
        Action action = () => SitecoreLayoutRequestExtensions.AuthenticationHeader(null!);

        // Assert
        action.Should().Throw<ArgumentNullException>()
            .And.ParamName.Should().Be("request");
    }

    [Theory]
    [AutoNSubstituteData]
    public void AuthenticationHeader_MissingEntry_ReturnsNull(SitecoreLayoutRequest sut)
    {
        // Arrange / Act
        string? result = sut.AuthenticationHeader();

        // Assert
        result.Should().BeNull();
    }

    [Theory]
    [AutoNSubstituteData]
    public void AuthenticationHeader_NullEntry_ReturnsNull(SitecoreLayoutRequest sut)
    {
        // Arrange
        sut[RequestKeys.Path] = null;

        // Act
        string? result = sut.AuthenticationHeader();

        // Assert
        result.Should().BeNull();
    }

    [Theory]
    [AutoNSubstituteData]
    public void AuthenticationHeader_ValidValueEntry_ReturnsNull(SitecoreLayoutRequest sut, string value)
    {
        // Arrange
        sut[RequestKeys.AuthHeaderKey] = value;

        // Act
        string? result = sut.AuthenticationHeader();

        // Assert
        result.Should().Be(value);
    }

    [Theory]
    [AutoNSubstituteData]
    public void AuthenticationHeader_InvalidValueEntry_ReturnsDefault(SitecoreLayoutRequest sut, Guid value)
    {
        // Arrange
        sut[RequestKeys.AuthHeaderKey] = value;
        string? result = sut.AuthenticationHeader();

        // Act / Assert
        result.Should().Be(default);
    }

    [Fact]
    public void AuthenticationHeader_NullRequestWithValue_Throws()
    {
        // Arrange
        Action action = () => SitecoreLayoutRequestExtensions.AuthenticationHeader(null!, null);

        // Assert
        action.Should().Throw<ArgumentNullException>()
            .And.ParamName.Should().Be("request");
    }

    [Theory]
    [AutoNSubstituteData]
    public void AuthenticationHeader_WithValue_SetsEntry(SitecoreLayoutRequest sut, string value)
    {
        // Arrange / Act
        sut.AuthenticationHeader(value);

        // Assert
        sut[RequestKeys.AuthHeaderKey].Should().Be(value);
    }

    [Theory]
    [AutoNSubstituteData]
    public void AuthenticationHeader_WithNullValue_SetsEntry(SitecoreLayoutRequest sut)
    {
        // Act
        sut.AuthenticationHeader(null);

        // Assert
        bool result = sut.TryGetValue(RequestKeys.AuthHeaderKey, out object? _);
        result.Should().BeTrue();
    }

    [Theory]
    [AutoNSubstituteData]
    public void AuthenticationHeader_ReturnsRequest(SitecoreLayoutRequest sut)
    {
        // Arrange / Act
        SitecoreLayoutRequest result = sut.AuthenticationHeader(null);

        // Assert
        result.Should().BeSameAs(sut);
    }

    #endregion

    #region Path

    [Fact]
    public void Path_NullRequest_Throws()
    {
        // Arrange
        Action action = () => SitecoreLayoutRequestExtensions.Path(null!);

        // Assert
        action.Should().Throw<ArgumentNullException>()
            .And.ParamName.Should().Be("request");
    }

    [Theory]
    [AutoNSubstituteData]
    public void Path_MissingEntry_ReturnsNull(SitecoreLayoutRequest sut)
    {
        // Arrange / Act
        string? result = sut.Path();

        // Assert
        result.Should().BeNull();
    }

    [Theory]
    [AutoNSubstituteData]
    public void Path_NullEntry_ReturnsNull(SitecoreLayoutRequest sut)
    {
        // Arrange
        sut[RequestKeys.Path] = null;

        // Act
        string? result = sut.Path();

        // Assert
        result.Should().BeNull();
    }

    [Theory]
    [AutoNSubstituteData]
    public void Path_ValidValueEntry_ReturnsNull(SitecoreLayoutRequest sut, string value)
    {
        // Arrange
        sut[RequestKeys.Path] = value;

        // Act
        string? result = sut.Path();

        // Assert
        result.Should().Be(value);
    }

    [Theory]
    [AutoNSubstituteData]
    public void Path_InvalidValueEntry_ReturnsDefault(SitecoreLayoutRequest sut, Guid value)
    {
        // Arrange
        sut[RequestKeys.Path] = value;
        string? result = sut.Path();

        // Act / Assert
        result.Should().Be(default);
    }

    [Fact]
    public void Path_NullRequestWithValue_Throws()
    {
        // Arrange
        Action action = () => SitecoreLayoutRequestExtensions.Path(null!, null);

        // Assert
        action.Should().Throw<ArgumentNullException>()
            .And.ParamName.Should().Be("request");
    }

    [Theory]
    [AutoNSubstituteData]
    public void Path_WithValue_SetsEntry(SitecoreLayoutRequest sut, string value)
    {
        // Arrange / Act
        sut.Path(value);

        // Assert
        sut[RequestKeys.Path].Should().Be(value);
    }

    [Theory]
    [AutoNSubstituteData]
    public void Path_WithNullValue_SetsEntry(SitecoreLayoutRequest sut)
    {
        // Act
        sut.Path(null);

        // Assert
        bool result = sut.TryGetValue(RequestKeys.Path, out object? _);
        result.Should().BeTrue();
    }

    [Theory]
    [AutoNSubstituteData]
    public void Path_ReturnsRequest(SitecoreLayoutRequest sut)
    {
        // Arrange / Act
        SitecoreLayoutRequest result = sut.Path(null);

        // Assert
        result.Should().BeSameAs(sut);
    }

    #endregion

    #region UpdateRequest

    [Fact]
    public void UpdateRequest_NullRequest_Throws()
    {
        // Arrange
        Action action = () => SitecoreLayoutRequestExtensions.UpdateRequest(null!, null);

        // Assert
        action.Should().Throw<ArgumentNullException>()
            .And.ParamName.Should().Be("request");
    }

    [Theory]
    [AutoNSubstituteData]
    public void UpdateRequest_NullRequestFallback_ReturnsOriginalRequest(SitecoreLayoutRequest sut)
    {
        // Act
        SitecoreLayoutRequest updatedRequest = sut.UpdateRequest(null);

        // Assert
        updatedRequest.Should().NotBeNull();
        updatedRequest.Should().BeEquivalentTo(sut);
    }

    [Theory]
    [AutoNSubstituteData]
    public void UpdateRequest_ValidRequestFallback_EntryInOriginalRequestWins(SitecoreLayoutRequest sut, Dictionary<string, object> requestFallback)
    {
        // Arrange
        sut.SiteName("sitename");
        requestFallback.Add(RequestKeys.SiteName, "fallbacksitename");

        // Act
        SitecoreLayoutRequest updatedRequest = sut.UpdateRequest(requestFallback!);

        // Assert
        updatedRequest.Should().NotBeNull();
        updatedRequest.SiteName().Should().Be(sut.SiteName());
    }

    [Theory]
    [AutoNSubstituteData]
    public void UpdateRequest_ValidRequestFallback_NullEntryInOriginalRequestWins(SitecoreLayoutRequest sut, Dictionary<string, object> requestFallback)
    {
        // Arrange
        sut.Add(RequestKeys.SiteName, null);
        requestFallback.Add(RequestKeys.SiteName, "fallbacksitename");

        // Act
        SitecoreLayoutRequest updatedRequest = sut.UpdateRequest(requestFallback!);

        // Assert
        updatedRequest.Should().NotBeNull();
        updatedRequest.Should().NotContainKey(RequestKeys.SiteName);
    }
    #endregion
}