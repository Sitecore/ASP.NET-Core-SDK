using System.Net;
using AwesomeAssertions;
using Microsoft.AspNetCore.TestHost;
using Sitecore.AspNetCore.SDK.AutoFixture.Mocks;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Extensions;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Request;
using Sitecore.AspNetCore.SDK.RenderingEngine.Extensions;
using Sitecore.AspNetCore.SDK.TestData;
using Xunit;

namespace Sitecore.AspNetCore.SDK.RenderingEngine.Integration.Tests.Fixtures.Localization;

public class DefaultLocalizationFixture : IDisposable
{
    private readonly TestServer _server;
    private readonly MockHttpMessageHandler _mockClientHandler;
    private readonly Uri _layoutServiceUri = new("http://layout.service");

    public DefaultLocalizationFixture()
    {
        TestServerBuilder testHostBuilder = new();
        _mockClientHandler = new MockHttpMessageHandler();
        testHostBuilder
            .ConfigureServices(builder =>
            {
                builder.AddLocalization(options => options.ResourcesPath = "Resources");
                builder
                    .AddSitecoreLayoutService().WithDefaultRequestOptions(request =>
                    {
                        request
                            .Language("da");
                    })
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

                app.UseSitecoreRenderingEngine();

                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapSitecoreLocalizedRoute("Localized", "Index", "UsingGlobalMiddleware");
                    endpoints.MapDefaultControllerRoute();
                });
            });

        _server = testHostBuilder.BuildServer(new Uri("http://localhost"));
    }

    [Fact]
    public async Task LocalizationRouteProvider_SetsCorrectRequestsLanguage()
    {
        // Arrange
        _mockClientHandler.Responses.Push(new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(Serializer.Serialize(CannedResponses.WithNestedPlaceholder))
        });

        HttpClient client = _server.CreateClient();

        // Act
        await client.GetStringAsync(new Uri("/da/UsingGlobalMiddleware", UriKind.Relative));

        _mockClientHandler.Requests.Single().RequestUri!.AbsoluteUri.Should().Contain("sc_lang=da");
    }

    public void Dispose()
    {
        _mockClientHandler.Dispose();
        _server.Dispose();
        GC.SuppressFinalize(this);
    }
}