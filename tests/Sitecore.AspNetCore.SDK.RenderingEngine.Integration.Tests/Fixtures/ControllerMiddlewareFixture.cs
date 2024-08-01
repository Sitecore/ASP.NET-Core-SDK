using System.Net;
using FluentAssertions;
using Microsoft.AspNetCore.TestHost;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Extensions;
using Xunit;

namespace Sitecore.AspNetCore.SDK.RenderingEngine.Integration.Tests.Fixtures;

public class ControllerMiddlewareFixture : IDisposable
{
    private const string MiddlewareController = "ControllerMiddleware";

    private const string GlobalMiddlewareController = "GlobalMiddleware";

    private readonly TestServer _server;

    private readonly HttpLayoutClientMessageHandler _mockClientHandler;

    private readonly Uri _layoutServiceUri = new("http://layout.service");

    public ControllerMiddlewareFixture()
    {
        TestServerBuilder testHostBuilder = new();
        _mockClientHandler = new HttpLayoutClientMessageHandler();
        testHostBuilder
            .ConfigureServices(builder =>
            {
                builder
                    .AddSitecoreLayoutService()
                    .AddHttpHandler(
                        "mock",
                        _ => new HttpClient(_mockClientHandler)
                        {
                            BaseAddress = _layoutServiceUri
                        })
                    .AsDefaultHandler();
            })
            .Configure(_ => { });

        _server = testHostBuilder.BuildServer(new Uri("http://localhost"));
    }

    [Fact]
    public async Task HttpClient_IsInvoked()
    {
        _mockClientHandler.Responses.Push(new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK
        });

        HttpClient client = _server.CreateClient();
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

        HttpClient client = _server.CreateClient();
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

        HttpClient client = _server.CreateClient();
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

        HttpClient client = _server.CreateClient();
        await client.GetAsync(MiddlewareController);

        _mockClientHandler.Requests.Single().RequestUri!.AbsoluteUri.Should()
            .BeEquivalentTo($"{_layoutServiceUri.AbsoluteUri}?item=%2f{MiddlewareController}");
    }

    public void Dispose()
    {
        _server.Dispose();
        _mockClientHandler.Dispose();
        GC.SuppressFinalize(this);
    }
}