using System.Net;
using FluentAssertions;
using Microsoft.AspNetCore.TestHost;
using NSubstitute;
using Sitecore.AspNetCore.SDK.ExperienceEditor.Extensions;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Interfaces;
using Sitecore.AspNetCore.SDK.RenderingEngine.Extensions;
using Sitecore.AspNetCore.SDK.TestData;
using Xunit;

namespace Sitecore.AspNetCore.SDK.RenderingEngine.Integration.Tests.Fixtures.ExperienceEditor;

public class ExperienceEditorCustomRoutingFixture : IDisposable
{
    private readonly TestServer _server;

    public ExperienceEditorCustomRoutingFixture()
    {
        TestServerBuilder testHostBuilder = new();
        _ = testHostBuilder
            .ConfigureServices(builder =>
            {
                builder.AddSingleton(Substitute.For<ISitecoreLayoutClient>());
                builder.AddSitecoreRenderingEngine(options =>
                {
                    options.AddDefaultComponentRenderer();
                }).WithExperienceEditor(options =>
                {
                    options.Endpoint = TestConstants.EEMiddlewarePostEndpoint;
                    options.JssEditingSecret = TestConstants.JssEditingSecret;

                    options.MapToRequest((sitecoreResponse, scPath, httpRequest) =>
                        httpRequest.Path = scPath + "/" + sitecoreResponse.Sitecore?.Route?.DatabaseName);
                });
            })
            .Configure(app =>
            {
                app.UseSitecoreExperienceEditor();
                app.UseRouting();
                app.UseSitecoreRenderingEngine();
                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapFallbackToController("Default", "Home");
                });
            });

        _server = testHostBuilder.BuildServer(new Uri("http://localhost"));
    }

    public void Dispose()
    {
        _server.Dispose();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task EECustomRoute_MapsToCorrectRoute_WhenCustomRouteSetInOptions()
    {
        // Arrange
        HttpClient client = _server.CreateClient();
        StringContent content = new(TestConstants.EESampleRequest);

        // Act
        HttpResponseMessage response = await client
            .PostAsync(TestConstants.EEMiddlewarePostEndpoint, content);
        response.EnsureSuccessStatusCode();
        string responseString = await response.Content.ReadAsStringAsync();

        // Asserts
        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.ReasonPhrase.Should().Be("OK");
        response.Content.Headers.ContentType!.MediaType.Should().Be("application/json");
        response.Content.Headers.ContentType.CharSet.Should().Be("utf-8");
        responseString.Should().Contain("{\"html\":\"");
        responseString.Should().EndWith("}");
        responseString.Should().Contain("master");
    }
}