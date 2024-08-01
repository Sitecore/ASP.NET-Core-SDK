using System.Net;
using BenchmarkDotNet.Attributes;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Extensions;
using Sitecore.AspNetCore.SDK.RenderingEngine.Extensions;
using Sitecore.AspNetCore.SDK.RenderingEngine.Integration.Tests;
using Sitecore.AspNetCore.SDK.TestData;
using Sitecore.AspNetCore.SDK.Tracking;
using Sitecore.AspNetCore.SDK.Tracking.VisitorIdentification;

// ReSharper disable StringLiteralTypo
namespace Sitecore.AspNetCore.SDK.RenderingEngine.Benchmarks;

[SimpleJob]
[MemoryDiagnoser]
public class TrackingBenchmarks : IDisposable
{
    private TestServer? _server;
    private HttpClient? _client;
    private HttpLayoutClientMessageHandler? _mockClientHandler;
    private RenderingEngineBenchmarks? _baseLineTestInstance;

    [GlobalSetup(Target = nameof(RegularHomePageRequestWithTracking))]
    public void TrackingBenchmarksSetup()
    {
        TestServerBuilder testHostBuilder = new();
        _mockClientHandler = new HttpLayoutClientMessageHandler();
        testHostBuilder
            .ConfigureServices(builder =>
            {
                builder.Configure<ForwardedHeadersOptions>(options =>
                {
                    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor |
                                               ForwardedHeaders.XForwardedProto;
                });

                builder
                    .AddSitecoreLayoutService()
                    .AddHttpHandler("mock", _ => new HttpClient(_mockClientHandler) { BaseAddress = new Uri("http://layout.service") })
                    .AsDefaultHandler();

                builder.AddSitecoreRenderingEngine(options =>
                    {
                        options.AddDefaultComponentRenderer();
                    }).ForwardHeaders(options =>
                    {
                        options.HeadersWhitelist.Add("HEADERTOCOPY");
                        options.RequestHeadersFilters.Add(
                            (_, result) =>
                            {
                                result.AppendValue("testNonWhitelistedHeader", "testNonWhitelistedHeaderValue");
                                result.AppendValue("headerToModify", "newModifiedHeaderValue");
                            });

                        options.ResponseHeadersFilters.Add(
                            (_, result) =>
                            {
                                result.AppendValue("testheaderRhResponse", "testHeaderResponseValueFromRenderingHost");
                            });
                    })
                    .WithTracking();

                builder.AddSitecoreVisitorIdentification(options =>
                {
                    options.SitecoreInstanceUri = new Uri("http://layout.service");
                });
            })
            .Configure(app =>
            {
                app.UseForwardedHeaders();
                app.UseRouting();
                app.UseSitecoreVisitorIdentification();
                app.UseSitecoreRenderingEngine();
            });

        _server = testHostBuilder.BuildServer(new Uri("http://localhost"));

        _client = _server.CreateClient();
    }

    [GlobalSetup(Target = nameof(RegularHomePageRequest))]
    public void RenderingEngineBenchmarks()
    {
        _baseLineTestInstance = new RenderingEngineBenchmarks();
        _baseLineTestInstance.Setup();
    }

    [Benchmark(Baseline = true)]
    public Task RegularHomePageRequest()
    {
        return _baseLineTestInstance!.RegularHomePageRequest();
    }

    [Benchmark]
    public async Task RegularHomePageRequestWithTracking()
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
        _baseLineTestInstance?.Dispose();
        GC.SuppressFinalize(this);
    }
}