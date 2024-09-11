using System.Net;
using BenchmarkDotNet.Attributes;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.TestHost;
using Sitecore.AspNetCore.SDK.AutoFixture.Mocks;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Extensions;
using Sitecore.AspNetCore.SDK.RenderingEngine.Extensions;
using Sitecore.AspNetCore.SDK.RenderingEngine.Integration.Tests;
using Sitecore.AspNetCore.SDK.TestData;

namespace Sitecore.AspNetCore.SDK.RenderingEngine.Benchmarks;

[SimpleJob]
[MemoryDiagnoser]
public class RenderingEngineBenchmarks : IDisposable
{
    private TestServer? _server;
    private HttpClient? _client;
    private MockHttpMessageHandler? _mockClientHandler;

    [GlobalSetup]
    public void Setup()
    {
        TestServerBuilder testHostBuilder = new();
        _mockClientHandler = new MockHttpMessageHandler();
        testHostBuilder
            .ConfigureServices(builder =>
            {
                builder
                    .AddSitecoreLayoutService()
                    .AddHttpHandler("mock", _ => new HttpClient(_mockClientHandler) { BaseAddress = new Uri("http://layout.service") })
                    .AsDefaultHandler();

                builder.AddSitecoreRenderingEngine(options =>
                {
                    options.AddDefaultComponentRenderer();
                });
            })
            .Configure(app =>
            {
                app.UseRouting();
                app.UseSitecoreRenderingEngine();
            });

        _server = testHostBuilder.BuildServer(new Uri("http://localhost"));

        _client = _server.CreateClient();
    }

    [Benchmark(Baseline = true)]
    public async Task RegularHomePageRequest()
    {
        _mockClientHandler!.Responses.Push(new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(Serializer.Serialize(CannedResponses.WithVisitorIdentificationLayoutPlaceholder)),
        });

        HttpRequestMessage request = new(HttpMethod.Get, new Uri("/", UriKind.Relative));
        request.Headers.Add("X-Forwarded-For", "192.168.1.0, 172.217.16.14");

        HttpResponseMessage response = await _client!.SendAsync(request).ConfigureAwait(false);
        string result = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        if (result.Length <= 0)
        {
            throw new Exception("Response content is expected.");
        }
    }

    public void Dispose()
    {
        _server?.Dispose();
        _client?.Dispose();
        _mockClientHandler?.Dispose();
        GC.SuppressFinalize(this);
    }
}