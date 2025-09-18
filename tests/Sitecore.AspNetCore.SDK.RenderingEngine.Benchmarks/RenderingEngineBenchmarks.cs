using System.Diagnostics.CodeAnalysis;
using System.Net;
using BenchmarkDotNet.Attributes;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Sitecore.AspNetCore.SDK.AutoFixture.Mocks;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Extensions;
using Sitecore.AspNetCore.SDK.RenderingEngine.Extensions;
using Sitecore.AspNetCore.SDK.TestData;

namespace Sitecore.AspNetCore.SDK.RenderingEngine.Benchmarks;

[SimpleJob]
[MemoryDiagnoser]
[ExcludeFromCodeCoverage]
public class RenderingEngineBenchmarks : IDisposable
{
    private WebApplicationFactory<TestWebApplicationProgram>? _factory;
    private HttpClient? _client;
    private MockHttpMessageHandler? _mockClientHandler;

    [GlobalSetup]
    public void Setup()
        {
            _mockClientHandler = new MockHttpMessageHandler();

            _factory = new WebApplicationFactory<TestWebApplicationProgram>()
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureServices(services =>
                    {
                        services.AddRouting();
                        services.AddSitecoreLayoutService();
                        services.AddHttpClient("mock").ConfigurePrimaryHttpMessageHandler(() => _mockClientHandler!);
                        services.AddSitecoreRenderingEngine(options =>
                        {
                            options.AddDefaultComponentRenderer();
                        });
                    });

                    builder.Configure(app =>
                    {
                        app.UseRouting();
                        app.UseSitecoreRenderingEngine();
                    });
                });

            _client = _factory.CreateClient();
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
        _client?.Dispose();
        _factory?.Dispose();
        _mockClientHandler?.Dispose();
        GC.SuppressFinalize(this);
    }
}