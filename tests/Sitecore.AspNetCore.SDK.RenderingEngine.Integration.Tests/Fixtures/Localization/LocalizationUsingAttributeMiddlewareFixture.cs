using System.Globalization;
using System.Net;
using System.Net.Http.Headers;
using AwesomeAssertions;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.TestHost;
using Sitecore.AspNetCore.SDK.AutoFixture.Mocks;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Extensions;
using Sitecore.AspNetCore.SDK.RenderingEngine.Extensions;
using Sitecore.AspNetCore.SDK.TestData;
using Xunit;

namespace Sitecore.AspNetCore.SDK.RenderingEngine.Integration.Tests.Fixtures.Localization;

public class LocalizationUsingAttributeMiddlewareFixture : IDisposable
{
    private readonly TestServer _server;
    private readonly MockHttpMessageHandler _mockClientHandler;
    private readonly Uri _layoutServiceUri = new("http://layout.service");

    public LocalizationUsingAttributeMiddlewareFixture()
    {
        TestServerBuilder testHostBuilder = new();
        _mockClientHandler = new MockHttpMessageHandler();
        testHostBuilder
            .ConfigureServices(builder =>
            {
                builder.AddLocalization(options => options.ResourcesPath = "Resources");
                builder
                    .AddSitecoreLayoutService()
                    .AddHttpHandler("mock", _ => new HttpClient(_mockClientHandler) { BaseAddress = _layoutServiceUri })
                    .AsDefaultHandler();
                builder.AddSitecoreRenderingEngine(options =>
                {
                    options
                        .AddModelBoundView<ComponentModels.Component4>("Component-4", "Component4")
                        .AddDefaultComponentRenderer();
                });
            })
            .Configure(app =>
            {
                app.UseRouting();
                app.UseRequestLocalization(options =>
                {
                    List<CultureInfo> supportedCultures = [new("en"), new("uk-UA"), new("da-DK")];

                    options.DefaultRequestCulture = new RequestCulture(culture: "en", uiCulture: "en");
                    options.SupportedCultures = supportedCultures;
                    options.SupportedUICultures = supportedCultures;
                    options.UseSitecoreRequestLocalization();
                });
                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapSitecoreLocalizedRoute("Localized", "UseLocalizeWithAttribute", "UsingAttribute");
                    endpoints.MapDefaultControllerRoute();
                });
            });

        _server = testHostBuilder.BuildServer(new Uri("http://localhost"));
    }

    [Theory]
    [InlineData("br", "en")]
    [InlineData("en", "en")]
    [InlineData("uk-UA", "uk-UA")]
    public async Task LocalizationRouteProvider_SetsCorrectRequestsLanguage_FromTheRoute(string routeLanguage, string mappedLanguage)
    {
        // Arrange
        _mockClientHandler.Responses.Push(new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(Serializer.Serialize(CannedResponses.WithNestedPlaceholder))
        });

        HttpClient client = _server.CreateClient();

        // Act
        await client.GetStringAsync(new Uri($"/{routeLanguage}/UsingAttribute/UseLocalizeWithAttribute", UriKind.Relative));

        // Assert
        _mockClientHandler.Requests.Single().RequestUri!.AbsoluteUri.Should().Contain($"sc_lang={mappedLanguage}");
    }

    [Theory]
    [InlineData("br", "en")]
    [InlineData("en", "en")]
    [InlineData("uk-UA", "uk-UA")]
    [InlineData(null, "en")]
    public async Task LocalizationRouteProvider_SetsCorrectRequestsLanguage_FromAcceptLanguageHeader(string? acceptLanguageHeader, string mappedLanguage)
    {
        // Arrange
        _mockClientHandler.Responses.Push(new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(Serializer.Serialize(CannedResponses.WithNestedPlaceholder))
        });

        HttpClient client = _server.CreateClient();
        client.DefaultRequestHeaders.AcceptLanguage.Clear();

        if (!string.IsNullOrWhiteSpace(acceptLanguageHeader))
        {
            client.DefaultRequestHeaders.AcceptLanguage.Add(new StringWithQualityHeaderValue(acceptLanguageHeader));
        }

        // Act
        await client.GetStringAsync(new Uri("/UsingAttribute/UseLocalizeWithAttribute", UriKind.Relative));

        // Assert
        _mockClientHandler.Requests.Single().RequestUri!.AbsoluteUri.Should().Contain($"sc_lang={mappedLanguage}");
    }

    [Theory]
    [InlineData("uk-UA", "da-DK", "uk-UA")]
    [InlineData("br", "da-DK", "da-DK")]
    [InlineData("br", null, "en")]
    [InlineData("uk-UA", null, "uk-UA")]
    [InlineData(null, null, "en")]
    public async Task LocalizationRouteProvider_SetsCorrectRequestsLanguage_RouteLanguageDominatesOverAcceptLanguageAttribute(string? routeLanguage, string? acceptLanguageHeader, string? mappedLanguage)
    {
        // Arrange
        _mockClientHandler.Responses.Push(new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(Serializer.Serialize(CannedResponses.WithNestedPlaceholder))
        });

        HttpClient client = _server.CreateClient();
        client.DefaultRequestHeaders.AcceptLanguage.Clear();

        if (!string.IsNullOrWhiteSpace(acceptLanguageHeader))
        {
            client.DefaultRequestHeaders.AcceptLanguage.Add(new StringWithQualityHeaderValue(acceptLanguageHeader));
        }

        // Act
        await client.GetStringAsync(new Uri($"{routeLanguage}/UsingAttribute/UseLocalizeWithAttribute", UriKind.Relative));

        // Assert
        _mockClientHandler.Requests.Single().RequestUri!.AbsoluteUri.Should().Contain($"sc_lang={mappedLanguage}");
    }

    public void Dispose()
    {
        _mockClientHandler.Dispose();
        _server.Dispose();
        GC.SuppressFinalize(this);
    }
}