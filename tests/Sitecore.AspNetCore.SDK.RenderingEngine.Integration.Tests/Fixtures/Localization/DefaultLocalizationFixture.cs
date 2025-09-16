using System.Net;
using AwesomeAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Sitecore.AspNetCore.SDK.AutoFixture.Mocks;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Extensions;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Request;
using Sitecore.AspNetCore.SDK.RenderingEngine.Extensions;
using Sitecore.AspNetCore.SDK.TestData;
using Xunit;

namespace Sitecore.AspNetCore.SDK.RenderingEngine.Integration.Tests.Fixtures.Localization;

public class DefaultLocalizationFixture(TestWebApplicationFactory<TestWebApplicationProgram> factory) : IClassFixture<TestWebApplicationFactory<TestWebApplicationProgram>>, IDisposable
{
    private readonly MockHttpMessageHandler _mockClientHandler = new();
    private readonly Uri _layoutServiceUri = new("http://layout.service");

    [Fact]
    public async Task LocalizationRouteProvider_SetsCorrectRequestsLanguage()
    {
        // Arrange
        _mockClientHandler.Responses.Push(new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(Serializer.Serialize(CannedResponses.WithNestedPlaceholder))
        });

        HttpClient client = BuildDefaultLocalizationWebApplicationFactory().CreateClient();

        // Act
        await client.GetStringAsync(new Uri("/da/UsingGlobalMiddleware", UriKind.Relative));

        _mockClientHandler.Requests.Single().RequestUri!.AbsoluteUri.Should().Contain("sc_lang=da");
    }

    public void Dispose()
    {
        _mockClientHandler.Dispose();
        GC.SuppressFinalize(this);
    }

    private WebApplicationFactory<TestWebApplicationProgram> BuildDefaultLocalizationWebApplicationFactory()
    {
        return factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                services.AddLocalization(options => options.ResourcesPath = "Resources");
                services
                    .AddSitecoreLayoutService().WithDefaultRequestOptions(request => { request.Language("da"); })
                    .AddHttpHandler("mock", _ => new HttpClient(_mockClientHandler) { BaseAddress = _layoutServiceUri })
                    .AsDefaultHandler();

                services.AddSitecoreRenderingEngine(options =>
                {
                    options
                        .AddModelBoundView<ComponentModels.Component4>("Component-4", "Component4")
                        .AddDefaultComponentRenderer();
                });
            });

            builder.Configure(app =>
            {
                app.UseRouting();

                app.UseSitecoreRenderingEngine();

                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapSitecoreLocalizedRoute("Localized", "Index", "UsingGlobalMiddleware");
                    endpoints.MapDefaultControllerRoute();
                });
            });
        });
    }
}