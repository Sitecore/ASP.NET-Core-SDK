using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using NSubstitute;
using Sitecore.AspNetCore.SDK.AutoFixture.Attributes;
using Sitecore.AspNetCore.SDK.RenderingEngine.Extensions;
using Xunit;

namespace Sitecore.AspNetCore.SDK.RenderingEngine.Tests.Extensions;

public class HttpRequestExtensionsFixture
{
    [Theory]
    [AutoNSubstituteData]
    public void GetValueFromQueryOrCookies_WithNullKey_Throws(HttpRequest sut)
    {
        Assert.Throws<ArgumentNullException>(() => sut.GetValueFromQueryOrCookies(null!));
    }

    [Theory]
    [AutoNSubstituteData]
    public void GetValueFromQueryOrCookies_WhenQueryContainsValue_Returns(HttpRequest sut, string key, string value)
    {
        sut.Query[key].Returns((StringValues)value);

        string? result = sut.GetValueFromQueryOrCookies(key);

        result.Should().Be(value);
    }

    [Theory]
    [AutoNSubstituteData]
    public void GetValueFromQueryOrCookies_WhenCookieContainsValue_Returns(HttpRequest sut, string key, string value)
    {
        sut.Query[key].Returns(StringValues.Empty);
        sut.Cookies[key].Returns(value);

        string? result = sut.GetValueFromQueryOrCookies(key);

        result.Should().Be(value);
    }

    [Theory]
    [AutoNSubstituteData]
    public void GetValueFromQueryOrCookies_WhenQueryOrCookieDonNotContainValue_ReturnsNull(HttpRequest sut, string key)
    {
        sut.Query[key].Returns(StringValues.Empty);
        sut.Cookies[key].Returns(string.Empty);

        string? result = sut.GetValueFromQueryOrCookies(key);

        result.Should().BeNull();
    }

    [Theory]
    [AutoNSubstituteData]
    public void TryGetValueFromQueryOrCookies_WithNullKey_Throws(HttpRequest sut)
    {
        Assert.Throws<ArgumentNullException>(() => sut.TryGetValueFromQueryOrCookies(null!, out string? _));
    }

    [Theory]
    [AutoNSubstituteData]
    public void TryGetValueFromQueryOrCookies_WhenQueryContainsValue_ReturnsTrue(HttpRequest sut, string key, string requestValue)
    {
        sut.Query[key].Returns((StringValues)requestValue);

        bool result = sut.TryGetValueFromQueryOrCookies(key, out string? value);

        result.Should().BeTrue();
        value.Should().Be(requestValue);
    }

    [Theory]
    [AutoNSubstituteData]
    public void TryGetValueFromQueryOrCookies_WhenCookieContainsValue_ReturnsTrue(HttpRequest sut, string key, string requestValue)
    {
        sut.Query[key].Returns(StringValues.Empty);
        sut.Cookies[key].Returns(requestValue);

        bool result = sut.TryGetValueFromQueryOrCookies(key, out string? value);

        result.Should().BeTrue();
        value.Should().Be(requestValue);
    }

    [Theory]
    [AutoNSubstituteData]
    public void TryGetValueFromQueryOrCookies_WhenQueryOrCookieDonNotContainValue_ReturnsFalse(HttpRequest sut, string key)
    {
        sut.Query[key].Returns(StringValues.Empty);
        sut.Cookies[key].Returns(string.Empty);

        bool result = sut.TryGetValueFromQueryOrCookies(key, out string? value);

        result.Should().BeFalse();
        value.Should().BeNull();
    }
}