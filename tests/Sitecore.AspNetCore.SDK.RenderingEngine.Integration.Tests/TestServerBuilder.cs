using Microsoft.AspNetCore;
using Microsoft.AspNetCore.TestHost;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Extensions;
using Sitecore.AspNetCore.SDK.RenderingEngine.Extensions;

namespace Sitecore.AspNetCore.SDK.RenderingEngine.Integration.Tests;

public class TestServerBuilder
{
    private readonly IWebHostBuilder _webHostBuilder = PrepareDefault();

    private readonly List<Action<IApplicationBuilder>> _appDelegates = [];

    public TestServer BuildServer(Uri uri)
    {
        _webHostBuilder.Configure(app =>
        {
            app.UseRouting();
            _appDelegates.ForEach(configure => configure(app));
            app.UseEndpoints(configure =>
            {
                configure.MapDefaultControllerRoute();
            });
        });

        return new TestServer(_webHostBuilder)
        {
            BaseAddress = uri
        };
    }

    public IWebHost Build() => _webHostBuilder.Build();

    public TestServerBuilder ConfigureAppConfiguration(Action<WebHostBuilderContext, IConfigurationBuilder> configureDelegate)
    {
        _webHostBuilder.ConfigureAppConfiguration(configureDelegate);
        return this;
    }

    public TestServerBuilder ConfigureServices(Action<WebHostBuilderContext, IServiceCollection> configureServices)
    {
        _webHostBuilder.ConfigureServices(configureServices);
        return this;
    }

    public TestServerBuilder ConfigureServices(Action<IServiceCollection> configureServices)
    {
        _webHostBuilder.ConfigureServices(configureServices);
        return this;
    }

    public string? GetSetting(string key) => _webHostBuilder.GetSetting(key);

    public TestServerBuilder UseSetting(string key, string value)
    {
        _webHostBuilder.UseSetting(key, value);
        return this;
    }

    public TestServerBuilder Configure(Action<IApplicationBuilder> configureApp)
    {
        _appDelegates.Add(configureApp);
        return this;
    }

    private static IWebHostBuilder PrepareDefault()
    {
        return WebHost.CreateDefaultBuilder()
            .ConfigureServices(services =>
            {
                services
                    .AddRouting()
                    .AddSitecoreLayoutService();

                services.AddSitecoreRenderingEngine();
            });
    }
}