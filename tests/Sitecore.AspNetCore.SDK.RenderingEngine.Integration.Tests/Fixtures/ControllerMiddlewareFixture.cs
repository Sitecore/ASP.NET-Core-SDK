using System.Net;
using AwesomeAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Sitecore.AspNetCore.SDK.AutoFixture.Mocks;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Extensions;
using Sitecore.AspNetCore.SDK.RenderingEngine.Extensions;
using Xunit;

namespace Sitecore.AspNetCore.SDK.RenderingEngine.Integration.Tests.Fixtures;

public class ControllerMiddlewareFixture(TestWebApplicationFactory<TestWebApplicationProgram> factory) : IClassFixture<TestWebApplicationFactory<TestWebApplicationProgram>>, IDisposable
{
    private const string MiddlewareController = "ControllerMiddleware";

    private const string GlobalMiddlewareController = "GlobalMiddleware";

    private readonly MockHttpMessageHandler _mockClientHandler = new MockHttpMessageHandler();

    private readonly Uri _layoutServiceUri = new("http://layout.service");

    [Fact]
    public async Task HttpClient_IsInvoked()
    {
        _mockClientHandler.Responses.Push(new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK
        });

        HttpClient client = BuildControllerMiddlewareWebApplicationFactory().CreateClient();
        await client.GetAsync(MiddlewareController);

        _mockClientHandler.WasInvoked.Should().BeTrue();
    }

    [Fact]
    public async Task HttpClient_IsNotInvoked()
    {
        _mockClientHandler.Responses.Push(new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK
        });

        HttpClient client = BuildControllerMiddlewareWebApplicationFactory().CreateClient();
        await client.GetAsync(GlobalMiddlewareController);

        _mockClientHandler.WasInvoked.Should().BeFalse();
    }

    [Fact]
    public async Task Controller_ReturnsCorrectContent()
    {
        _mockClientHandler.Responses.Push(new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK
        });

        HttpClient client = BuildControllerMiddlewareWebApplicationFactory().CreateClient();
        string response = await client.GetStringAsync(GlobalMiddlewareController);

        response.Should().Be("\"success\"");
    }

    [Fact]
    public async Task HttpClient_LayoutServiceUriMapped()
    {
        _mockClientHandler.Responses.Push(new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK
        });

        HttpClient client = BuildControllerMiddlewareWebApplicationFactory().CreateClient();
        await client.GetAsync(MiddlewareController);

        _mockClientHandler.Requests.Single().RequestUri!.AbsoluteUri.Should()
            .BeEquivalentTo($"{_layoutServiceUri.AbsoluteUri}?item=%2f{MiddlewareController}");
    }

    public void Dispose()
    {
        _mockClientHandler.Dispose();
        GC.SuppressFinalize(this);
    }

    private WebApplicationFactory<TestWebApplicationProgram> BuildControllerMiddlewareWebApplicationFactory()
    {
        return factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                services
                    .AddRouting()
                    .AddSitecoreLayoutService()
                    .AddHttpHandler(
                        "mock",
                        _ => new HttpClient(_mockClientHandler)
                        {
                            BaseAddress = _layoutServiceUri
                        })
                    .AsDefaultHandler();

                services.AddSitecoreRenderingEngine();
            });

            builder.Configure(app =>
            {
                app.UseRouting();
                app.UseEndpoints(endpoints => { endpoints.MapDefaultControllerRoute(); });
            });
        });
    }
}