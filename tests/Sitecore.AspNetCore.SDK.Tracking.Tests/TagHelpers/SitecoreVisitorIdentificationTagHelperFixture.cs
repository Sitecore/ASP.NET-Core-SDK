using System.Collections;
using AutoFixture;
using AutoFixture.Idioms;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using NSubstitute;
using Sitecore.AspNetCore.SDK.AutoFixture.Attributes;
using Sitecore.AspNetCore.SDK.AutoFixture.Extensions;
using Sitecore.AspNetCore.SDK.Tracking.VisitorIdentification;
using Sitecore.AspNetCore.SDK.Tracking.VisitorIdentification.Providers;
using Sitecore.AspNetCore.SDK.Tracking.VisitorIdentification.TagHelpers;
using Xunit;

// ReSharper disable StringLiteralTypo
namespace Sitecore.AspNetCore.SDK.Tracking.Tests.TagHelpers;

public class SitecoreVisitorIdentificationTagHelperFixture
{
    // ReSharper disable once UnusedMember.Global - Used by testing framework
    public static Action<IFixture> AutoSetup => f =>
    {
        ViewContext viewContext = new()
        {
            HttpContext = Substitute.For<HttpContext>()
        };

        FeatureCollection features = new();

        viewContext.HttpContext.Features.Returns(features);
        f.Inject(viewContext);

        TagHelperOutput tagHelperOutput = new("test", [], (_, _) =>
        {
            DefaultTagHelperContent tagHelperContent = new();
            tagHelperContent.SetHtmlContent(string.Empty);
            return Task.FromResult<TagHelperContent>(tagHelperContent);
        });

        f.Inject(tagHelperOutput);
    };

    [Theory]
    [AutoNSubstituteData]
    public void Ctor_InvalidArgs_Throws(GuardClauseAssertion guard)
    {
        guard.VerifyConstructors<SitecoreVisitorIdentificationTagHelper>();
    }

    [Theory]
    [AutoNSubstituteData]
    public async Task ProcessAsync_ViRequestNoViewContext_OutputIsEmpty(
        SitecoreVisitorIdentificationTagHelper sut,
        TagHelperContext tagHelperContext,
        TagHelperOutput tagHelperOutput)
    {
        // Arrange

        // Act
        await sut.ProcessAsync(tagHelperContext, tagHelperOutput);

        // Assert
        tagHelperOutput.Content.GetContent().Should().BeEmpty();
    }

    [Theory]
    [AutoNSubstituteData]
    public async Task ProcessAsync_ViRequestOptionsNoUrl_OutputIsEmpty(
        SitecoreVisitorIdentificationTagHelper sut,
        ViewContext viewContext,
        IOptions<SitecoreVisitorIdentificationOptions> stOptions,
        TagHelperContext tagHelperContext,
        TagHelperOutput tagHelperOutput)
    {
        // Arrange
        stOptions.Value.Returns(new SitecoreVisitorIdentificationOptions() { SitecoreInstanceUri = null });

        viewContext.HttpContext.RequestServices.GetService(typeof(IOptions<SitecoreVisitorIdentificationOptions>)).Returns(stOptions);

        // Act
        await sut.ProcessAsync(tagHelperContext, tagHelperOutput);

        // Assert
        tagHelperOutput.Content.GetContent().Should().BeEmpty();
    }

    [Theory]
    [AutoNSubstituteData]
    public async Task ProcessAsync_ViRequestNoGACookieResponseCookieFalse_OutputContainsJS(
        SitecoreVisitorIdentificationTagHelper sut,
        ViewContext viewContext,
        IOptions<SitecoreVisitorIdentificationOptions> stOptions,
        TagHelperContext tagHelperContext,
        TagHelperOutput tagHelperOutput,
        DateTime fakeDateTimeNowUtc)
    {
        // Arrange
        IDateTimeProvider? fakeDateTimeProvider = Substitute.For<IDateTimeProvider>();
        fakeDateTimeProvider.GetUtcNow().Returns(fakeDateTimeNowUtc);

        stOptions.Value.Returns(new SitecoreVisitorIdentificationOptions() { SitecoreInstanceUri = new Uri("https://testurl") });
        viewContext.HttpContext.RequestServices.GetService(typeof(IOptions<SitecoreVisitorIdentificationOptions>)).Returns(stOptions);
        viewContext.HttpContext.RequestServices.GetService(typeof(IDateTimeProvider)).Returns(fakeDateTimeProvider);

        viewContext.HttpContext.Response.Headers.Returns(new HeaderDictionary(new Dictionary<string, StringValues>
        {
            {
                "set-cookie", "SC_ANALYTICS_GLOBAL_COOKIE=0f82f53555ce4304a1ee8ae99ab9f9a8|False; expires = Fri, 15 - Mar - 2030 13:15:08 GMT; path =/; HttpOnly"
            }
        }));

        sut.ViewContext = viewContext;

        // Act
        await sut.ProcessAsync(tagHelperContext, tagHelperOutput);

        // Assert
        tagHelperOutput.Content.GetContent().Should().Be($"<meta name=\"VIcurrentDateTime\" content=\"{fakeDateTimeNowUtc.Ticks}>\"/><meta name=\"VirtualFolder\" content=\"\\\"/><script type='text/javascript' src='/layouts/system/VisitorIdentification.js'></script>");
    }

    [Theory]
    [AutoNSubstituteData]
    public async Task ProcessAsync_ViRequestGACookieFalseResponseCookieFalse_OutputContainsJS(
        SitecoreVisitorIdentificationTagHelper sut,
        ViewContext viewContext,
        IOptions<SitecoreVisitorIdentificationOptions> stOptions,
        TagHelperContext tagHelperContext,
        TagHelperOutput tagHelperOutput,
        DateTime fakeDateTimeNowUtc)
    {
        // Arrange
        IDateTimeProvider? fakeDateTimeProvider = Substitute.For<IDateTimeProvider>();
        fakeDateTimeProvider.GetUtcNow().Returns(fakeDateTimeNowUtc);
        stOptions.Value.Returns(new SitecoreVisitorIdentificationOptions { SitecoreInstanceUri = new Uri("https://testurl") });
        viewContext.HttpContext.RequestServices.GetService(typeof(IOptions<SitecoreVisitorIdentificationOptions>)).Returns(stOptions);
        viewContext.HttpContext.RequestServices.GetService(typeof(IDateTimeProvider)).Returns(fakeDateTimeProvider);

        viewContext.HttpContext.Request.Cookies.Returns(new RequestCookies(new Dictionary<string, string>
        {
            {
                "SC_ANALYTICS_GLOBAL_COOKIE", "0f82f53555ce4304a1ee8ae99ab9f9a8|False; expires = Fri, 15 - Mar - 2030 13:15:08 GMT; path =/; HttpOnly"
            }
        }));

        viewContext.HttpContext.Response.Headers.Returns(new HeaderDictionary(new Dictionary<string, StringValues>
        {
            {
                "set-cookie", "SC_ANALYTICS_GLOBAL_COOKIE=0f82f53555ce4304a1ee8ae99ab9f9a8|False; expires = Fri, 15 - Mar - 2030 13:15:08 GMT; path =/; HttpOnly"
            }
        }));

        sut.ViewContext = viewContext;

        // Act
        await sut.ProcessAsync(tagHelperContext, tagHelperOutput);

        // Assert
        tagHelperOutput.Content.GetContent().Should().Be($"<meta name=\"VIcurrentDateTime\" content=\"{fakeDateTimeNowUtc.Ticks}>\"/><meta name=\"VirtualFolder\" content=\"\\\"/><script type='text/javascript' src='/layouts/system/VisitorIdentification.js'></script>");
    }

    [Theory]
    [AutoNSubstituteData]
    public async Task ProcessAsync_ViRequestGACookieFalseResponseCookieTrue_OutputDoesNOTContainJS(
        SitecoreVisitorIdentificationTagHelper sut,
        ViewContext viewContext,
        IOptions<SitecoreVisitorIdentificationOptions> stOptions,
        TagHelperContext tagHelperContext,
        TagHelperOutput tagHelperOutput,
        DateTime fakeDateTimeNowUtc)
    {
        // Arrange
        IDateTimeProvider? fakeDateTimeProvider = Substitute.For<IDateTimeProvider>();
        fakeDateTimeProvider.GetUtcNow().Returns(fakeDateTimeNowUtc);
        stOptions.Value.Returns(new SitecoreVisitorIdentificationOptions { SitecoreInstanceUri = new Uri("https://testurl") });
        viewContext.HttpContext.RequestServices.GetService(typeof(IOptions<SitecoreVisitorIdentificationOptions>)).Returns(stOptions);
        viewContext.HttpContext.RequestServices.GetService(typeof(IDateTimeProvider)).Returns(fakeDateTimeProvider);

        viewContext.HttpContext.Request.Cookies.Returns(new RequestCookies(new Dictionary<string, string>
        {
            {
                "SC_ANALYTICS_GLOBAL_COOKIE", "0f82f53555ce4304a1ee8ae99ab9f9a8|False; expires = Fri, 15 - Mar - 2030 13:15:08 GMT; path =/; HttpOnly"
            }
        }));

        viewContext.HttpContext.Response.Headers.SetCookie.Returns(new StringValues(
            "SC_ANALYTICS_GLOBAL_COOKIE=0f82f53555ce4304a1ee8ae99ab9f9a8|True; expires = Fri, 15 - Mar - 2030 13:15:08 GMT; path =/; HttpOnly"));

        sut.ViewContext = viewContext;

        // Act
        await sut.ProcessAsync(tagHelperContext, tagHelperOutput);

        // Assert
        tagHelperOutput.Content.GetContent().Should().NotBe($"<meta name=\"VIcurrentDateTime\" content=\"{fakeDateTimeNowUtc.Ticks}>\"/><meta name=\"VirtualFolder\" content=\"\\\"/><script type='text/javascript' src='/layouts/system/VisitorIdentification.js'></script>");
    }

    [Theory]
    [AutoNSubstituteData]
    public async Task ProcessAsync_ViRequestGACookieTrueResponseGACookieFalse_OutputContainsJS(
        SitecoreVisitorIdentificationTagHelper sut,
        ViewContext viewContext,
        IOptions<SitecoreVisitorIdentificationOptions> stOptions,
        TagHelperContext tagHelperContext,
        TagHelperOutput tagHelperOutput,
        DateTime fakeDateTimeNowUtc)
    {
        // Arrange
        IDateTimeProvider? fakeDateTimeProvider = Substitute.For<IDateTimeProvider>();
        fakeDateTimeProvider.GetUtcNow().Returns(fakeDateTimeNowUtc);
        stOptions.Value.Returns(new SitecoreVisitorIdentificationOptions { SitecoreInstanceUri = new Uri("https://testurl") });
        viewContext.HttpContext.RequestServices.GetService(typeof(IOptions<SitecoreVisitorIdentificationOptions>)).Returns(stOptions);
        viewContext.HttpContext.RequestServices.GetService(typeof(IDateTimeProvider)).Returns(fakeDateTimeProvider);

        viewContext.HttpContext.Request.Cookies.Returns(new RequestCookies(new Dictionary<string, string>
        {
            {
                "SC_ANALYTICS_GLOBAL_COOKIE", "0f82f53555ce4304a1ee8ae99ab9f9a8|True; expires = Fri, 15 - Mar - 2030 13:15:08 GMT; path =/; HttpOnly"
            }
        }));

        viewContext.HttpContext.Response.Headers.SetCookie.Returns(new StringValues("SC_ANALYTICS_GLOBAL_COOKIE=0f82f53555ce4304a1ee8ae99ab9f9a8|False; expires = Fri, 15 - Mar - 2030 13:15:08 GMT; path =/; HttpOnly"));

        sut.ViewContext = viewContext;

        // Act
        await sut.ProcessAsync(tagHelperContext, tagHelperOutput);

        // Assert
        tagHelperOutput.Content.GetContent().Should().Be($"<meta name=\"VIcurrentDateTime\" content=\"{fakeDateTimeNowUtc.Ticks}>\"/><meta name=\"VirtualFolder\" content=\"\\\"/><script type='text/javascript' src='/layouts/system/VisitorIdentification.js'></script>");
    }

    [Theory]
    [AutoNSubstituteData]
    public async Task ProcessAsync_ViRequestGACookieTrueNoResponseGACookie_OutputDosNotContainJS(
        SitecoreVisitorIdentificationTagHelper sut,
        ViewContext viewContext,
        IOptions<SitecoreVisitorIdentificationOptions> stOptions,
        TagHelperContext tagHelperContext,
        TagHelperOutput tagHelperOutput,
        DateTime fakeDateTimeNowUtc)
    {
        // Arrange
        IDateTimeProvider? fakeDateTimeProvider = Substitute.For<IDateTimeProvider>();
        fakeDateTimeProvider.GetUtcNow().Returns(fakeDateTimeNowUtc);
        stOptions.Value.Returns(new SitecoreVisitorIdentificationOptions { SitecoreInstanceUri = new Uri("https://testurl") });
        viewContext.HttpContext.RequestServices.GetService(typeof(IOptions<SitecoreVisitorIdentificationOptions>)).Returns(stOptions);
        viewContext.HttpContext.RequestServices.GetService(typeof(IDateTimeProvider)).Returns(fakeDateTimeProvider);

        viewContext.HttpContext.Request.Cookies.Returns(new RequestCookies(new Dictionary<string, string>
        {
            {
                "SC_ANALYTICS_GLOBAL_COOKIE", "0f82f53555ce4304a1ee8ae99ab9f9a8|True; expires = Fri, 15 - Mar - 2030 13:15:08 GMT; path =/; HttpOnly"
            }
        }));

        sut.ViewContext = viewContext;

        // Act
        await sut.ProcessAsync(tagHelperContext, tagHelperOutput);

        // Assert
        tagHelperOutput.Content.GetContent().Should().NotBe($"<meta name=\"VIcurrentDateTime\" content=\"{fakeDateTimeNowUtc.Ticks}>\"/><meta name=\"VirtualFolder\" content=\"\\\"/><script type='text/javascript' src='/layouts/system/VisitorIdentification.js'></script>");
    }

    private class RequestCookies(Dictionary<string, string> cookies)
        : Dictionary<string, string>(cookies), IRequestCookieCollection
    {
        public new ICollection<string> Keys => base.Keys;

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}