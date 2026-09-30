using System.Diagnostics.CodeAnalysis;
using BenchmarkDotNet.Attributes;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Sitecore.AspNetCore.SDK.ExperienceEditor.Extensions;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Interfaces;
using Sitecore.AspNetCore.SDK.RenderingEngine.Extensions;
using Sitecore.AspNetCore.SDK.RenderingEngine.Integration.Tests;
using Sitecore.AspNetCore.SDK.TestData;

namespace Sitecore.AspNetCore.SDK.RenderingEngine.Benchmarks;

[SimpleJob]
[MemoryDiagnoser]
[ExcludeFromCodeCoverage]
public class ExperienceEditorMiddlewareBenchmarks : IDisposable
{
    private readonly WebApplicationFactory<TestWebApplicationProgram> _factory;
    private readonly HttpClient _client;
    private readonly StringContent _content;
    private RenderingEngineBenchmarks? _baseLineTestInstance;

    public ExperienceEditorMiddlewareBenchmarks()
    {
        WebApplicationFactory<TestWebApplicationProgram> factory = new TestWebApplicationFactory<TestWebApplicationProgram>();

        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                services.AddRouting();

                services.AddSingleton(Substitute.For<ISitecoreLayoutClient>());

                services.AddSitecoreRenderingEngine(options =>
                {
                    options.AddDefaultComponentRenderer();
                }).WithExperienceEditor(options =>
                {
                    options.Endpoint = TestConstants.EEMiddlewarePostEndpoint;
                    options.JssEditingSecret = TestConstants.JssEditingSecret;
                });
            });

            builder.Configure(app =>
            {
                app.UseSitecoreExperienceEditor();
                app.UseRouting();
                app.UseSitecoreRenderingEngine();
            });
        });

        _client = _factory.CreateClient();
        _content = new StringContent(TestConstants.EESampleRequest);
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
    public async Task RegularExperienceEditorRequestHandling()
    {
        HttpResponseMessage response = await _client
            .PostAsync(TestConstants.EEMiddlewarePostEndpoint, _content)
            .ConfigureAwait(false);
        response.EnsureSuccessStatusCode();
        await response.Content.ReadAsStringAsync().ConfigureAwait(false);
    }

    public void Dispose()
    {
        _factory.Dispose();
        _client.Dispose();
        _content.Dispose();
        _baseLineTestInstance?.Dispose();
        GC.SuppressFinalize(this);
    }
}