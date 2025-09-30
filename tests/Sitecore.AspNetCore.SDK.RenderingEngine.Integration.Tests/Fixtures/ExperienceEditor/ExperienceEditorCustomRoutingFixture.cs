using System.Net;
using AwesomeAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using NSubstitute;
using Sitecore.AspNetCore.SDK.ExperienceEditor.Extensions;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Extensions;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Interfaces;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Request;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Response;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Response.Model;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Serialization;
using Sitecore.AspNetCore.SDK.RenderingEngine.Extensions;
using Sitecore.AspNetCore.SDK.TestData;
using Xunit;
using Route = Sitecore.AspNetCore.SDK.LayoutService.Client.Response.Model.Route;

namespace Sitecore.AspNetCore.SDK.RenderingEngine.Integration.Tests.Fixtures.ExperienceEditor;

public class ExperienceEditorCustomRoutingFixture : IDisposable
{
    private readonly WebApplicationFactory<TestWebApplicationProgram> _factory;

    public ExperienceEditorCustomRoutingFixture()
    {
        _factory = BuildExperienceEditorCustomRoutingWebApplicationFactory();
    }

    public void Dispose()
    {
        _factory.Dispose();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task EECustomRoute_MapsToCorrectRoute_WhenCustomRouteSetInOptions()
    {
        // Arrange
        HttpClient client = _factory.CreateClient();
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

    private WebApplicationFactory<TestWebApplicationProgram> BuildExperienceEditorCustomRoutingWebApplicationFactory()
    {
        WebApplicationFactory<TestWebApplicationProgram> factory = new TestWebApplicationFactory<TestWebApplicationProgram>();

        return factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                services.AddSingleton(Substitute.For<ISitecoreLayoutClient>());
                services.AddRouting();
                services.AddSitecoreLayoutService();
                services.AddControllersWithViews();

                services.AddSitecoreRenderingEngine(options =>
                {
                    options.AddDefaultComponentRenderer();
                }).WithExperienceEditor(options =>
                {
                    options.Endpoint = TestConstants.EEMiddlewarePostEndpoint;
                    options.JssEditingSecret = TestConstants.JssEditingSecret;

                    options.MapToRequest((sitecoreResponse, scPath, httpRequest) =>
                        httpRequest.Path = scPath + "/" + sitecoreResponse.Sitecore?.Route?.DatabaseName);
                });
            });

            builder.Configure(app =>
            {
                app.UseSitecoreExperienceEditor();
                app.UseRouting();
                app.UseSitecoreRenderingEngine();
                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapFallbackToController("Default", "Home");
                });
            });
        });
    }
}