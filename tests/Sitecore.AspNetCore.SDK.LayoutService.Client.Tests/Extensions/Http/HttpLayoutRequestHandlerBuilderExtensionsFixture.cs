using AutoFixture;
using AwesomeAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sitecore.AspNetCore.SDK.AutoFixture.Attributes;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Configuration;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Extensions;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Interfaces;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Request;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Request.Handlers;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Serialization;
using Xunit;

namespace Sitecore.AspNetCore.SDK.LayoutService.Client.Tests.Extensions.Http;

public class HttpLayoutRequestHandlerBuilderExtensionsFixture
{
    // ReSharper disable once UnusedMember.Global - Used by testing framework
    public static Action<IFixture> AutoSetup => f =>
    {
        ServiceCollection services = [];
        f.Inject<IServiceCollection>(services);

        ISitecoreLayoutSerializer? serializer = f.Create<ISitecoreLayoutSerializer>();
        services.AddSingleton(serializer);

        IHttpClientFactory? httpClientFactory = f.Freeze<IHttpClientFactory>();
        services.AddSingleton(httpClientFactory);

        ILogger<HttpLayoutRequestHandler>? logger = f.Freeze<ILogger<HttpLayoutRequestHandler>>();
        services.AddSingleton(logger);
    };

    [Theory]
    [AutoNSubstituteData]
    public void AddHttpHandler1a_IsGuarded(ISitecoreLayoutClientBuilder builder, string handlerName, Func<IServiceProvider, HttpClient> resolveClient)
    {
        // Arrange
        Func<ILayoutRequestHandlerBuilder<HttpLayoutRequestHandler>> builderNull =
            () => SitecoreLayoutClientBuilderExtensions.AddHttpHandler(null!, handlerName, resolveClient);
        Func<ILayoutRequestHandlerBuilder<HttpLayoutRequestHandler>> handlerNull =
            () => builder.AddHttpHandler(null!, resolveClient);
        Func<ILayoutRequestHandlerBuilder<HttpLayoutRequestHandler>> resolveClientNull = () =>
            builder.AddHttpHandler(handlerName, (Func<IServiceProvider, HttpClient>)null!);

        // Assert
        builderNull.Should().Throw<ArgumentNullException>();
        handlerNull.Should().Throw<ArgumentNullException>();
        resolveClientNull.Should().Throw<ArgumentNullException>();
    }

    [Theory]
    [AutoNSubstituteData]
    public void AddHttpHandler1a_Returns_HttpLayoutRequestHandler(ISitecoreLayoutClientBuilder builder, string handlerName, Func<IServiceProvider, HttpClient> resolveClient)
    {
        // Arrange & Act
        builder.AddHttpHandler(handlerName, resolveClient);

        ServiceProvider provider = builder.Services.BuildServiceProvider();
        IOptions<SitecoreLayoutClientOptions>? options = provider.GetService<IOptions<SitecoreLayoutClientOptions>>();
        ILayoutRequestHandler result = options!.Value.HandlerRegistry[handlerName].Invoke(provider);

        // Assert
        result.Should().BeOfType<HttpLayoutRequestHandler>();
    }

    [Theory]
    [AutoNSubstituteData]
    public void AddHttpHandler1a_Adds_DefaultSitecoreRequestMapping(ISitecoreLayoutClientBuilder builder, string handlerName, Func<IServiceProvider, HttpClient> resolveClient)
    {
        // Arrange & Act
        ILayoutRequestHandlerBuilder<HttpLayoutRequestHandler> clientBuilder = builder.AddHttpHandler(handlerName, resolveClient);

        ServiceProvider sp = clientBuilder.Services.BuildServiceProvider();
        IOptionsSnapshot<HttpLayoutRequestHandlerOptions>? handlerOptions = sp.GetService<IOptionsSnapshot<HttpLayoutRequestHandlerOptions>>();
        HttpLayoutRequestHandlerOptions namedHandlerOptions = handlerOptions!.Get(handlerName);

        // Assert
        namedHandlerOptions.Should().NotBeNull();
        namedHandlerOptions.RequestMap.Should().NotBeNull();
        namedHandlerOptions.RequestMap.Should().ContainSingle();
    }

    [Theory]
    [AutoNSubstituteData]
    public void AddHttpHandler2a_IsGuarded(ISitecoreLayoutClientBuilder builder, string handlerName, Action<IServiceProvider, HttpClient> resolveClient)
    {
        // Arrange
        Func<ILayoutRequestHandlerBuilder<HttpLayoutRequestHandler>> builderNull =
            () => SitecoreLayoutClientBuilderExtensions.AddHttpHandler(null!, handlerName, resolveClient);
        Func<ILayoutRequestHandlerBuilder<HttpLayoutRequestHandler>> handlerNameNull =
            () => builder.AddHttpHandler(null!, resolveClient);
        Func<ILayoutRequestHandlerBuilder<HttpLayoutRequestHandler>> configure =
            () => builder.AddHttpHandler(handlerName, (Action<IServiceProvider, HttpClient>)null!);

        // Assert
        builderNull.Should().Throw<ArgumentNullException>();
        handlerNameNull.Should().Throw<ArgumentNullException>();
        configure.Should().Throw<ArgumentNullException>();
    }

    [Theory]
    [AutoNSubstituteData]
    public void AddHttpHandler2a_Returns_HttpLayoutRequestHandler(ISitecoreLayoutClientBuilder builder, string handlerName, Action<IServiceProvider, HttpClient> resolveClient)
    {
        // Arrange & Act
        builder.AddHttpHandler(handlerName, resolveClient);

        ServiceProvider provider = builder.Services.BuildServiceProvider();
        IOptions<SitecoreLayoutClientOptions>? options = provider.GetService<IOptions<SitecoreLayoutClientOptions>>();
        ILayoutRequestHandler result = options!.Value.HandlerRegistry[handlerName].Invoke(provider);

        // Assert
        result.Should().BeOfType<HttpLayoutRequestHandler>();
    }

    [Theory]
    [AutoNSubstituteData]
    public void AddHttpHandler3a_IsGuarded(ISitecoreLayoutClientBuilder builder, string handlerName, Action<HttpClient> configureClient)
    {
        // Arrange
        Func<ILayoutRequestHandlerBuilder<HttpLayoutRequestHandler>> builderNull =
            () => SitecoreLayoutClientBuilderExtensions.AddHttpHandler(null!, handlerName, configureClient);
        Func<ILayoutRequestHandlerBuilder<HttpLayoutRequestHandler>> handlerNameNull =
            () => builder.AddHttpHandler(null!, configureClient);
        Func<ILayoutRequestHandlerBuilder<HttpLayoutRequestHandler>> configureNull =
            () => builder.AddHttpHandler(handlerName, (Action<HttpClient>)null!);

        // Assert
        builderNull.Should().Throw<ArgumentNullException>();
        handlerNameNull.Should().Throw<ArgumentNullException>();
        configureNull.Should().Throw<ArgumentNullException>();
    }

    [Theory]
    [AutoNSubstituteData]
    public void AddHttpHandler3a_Returns_HttpLayoutRequestHandler(ISitecoreLayoutClientBuilder builder, string handlerName, Action<HttpClient> resolveClient)
    {
        // Arrange & Act
        builder.AddHttpHandler(handlerName, resolveClient);

        ServiceProvider provider = builder.Services.BuildServiceProvider();
        IOptions<SitecoreLayoutClientOptions>? options = provider.GetService<IOptions<SitecoreLayoutClientOptions>>();
        ILayoutRequestHandler result = options!.Value.HandlerRegistry[handlerName].Invoke(provider);

        // Assert
        result.Should().BeOfType<HttpLayoutRequestHandler>();
    }

    [Theory]
    [AutoNSubstituteData]
    public void AddHttpHandler4a_IsGuarded(ISitecoreLayoutClientBuilder builder, string handlerName, Uri uri)
    {
        // Arrange
        Func<ILayoutRequestHandlerBuilder<HttpLayoutRequestHandler>> builderNull =
            () => SitecoreLayoutClientBuilderExtensions.AddHttpHandler(null!, handlerName, uri);
        Func<ILayoutRequestHandlerBuilder<HttpLayoutRequestHandler>> handlerNameNull =
            () => builder.AddHttpHandler(null!, uri);
        Func<ILayoutRequestHandlerBuilder<HttpLayoutRequestHandler>> uriNull =
            () => builder.AddHttpHandler(handlerName, (Uri)null!);

        // Assert
        builderNull.Should().Throw<ArgumentNullException>();
        handlerNameNull.Should().Throw<ArgumentNullException>();
        uriNull.Should().Throw<ArgumentNullException>();
    }

    [Theory]
    [AutoNSubstituteData]
    public void AddHttpHandler4a_Returns_HttpLayoutRequestHandler(ISitecoreLayoutClientBuilder builder, string handlerName, Uri uri)
    {
        // Arrange & Act
        builder.AddHttpHandler(handlerName, uri);

        ServiceProvider provider = builder.Services.BuildServiceProvider();
        IOptions<SitecoreLayoutClientOptions>? options = provider.GetService<IOptions<SitecoreLayoutClientOptions>>();

        ILayoutRequestHandler result = options!.Value.HandlerRegistry[handlerName].Invoke(provider);

        // Assert
        result.Should().BeOfType<HttpLayoutRequestHandler>();
    }

    [Theory]
    [AutoNSubstituteData]
    public void AddHttpHandler5a_IsGuarded(ISitecoreLayoutClientBuilder builder, string handlerName, string uri)
    {
        // Arrange
        Func<ILayoutRequestHandlerBuilder<HttpLayoutRequestHandler>> builderNull =
            () => SitecoreLayoutClientBuilderExtensions.AddHttpHandler(null!, handlerName, uri);
        Func<ILayoutRequestHandlerBuilder<HttpLayoutRequestHandler>> handlerNameNull =
            () => builder.AddHttpHandler(null!, uri);
        Func<ILayoutRequestHandlerBuilder<HttpLayoutRequestHandler>> uriStringNull =
            () => builder.AddHttpHandler(handlerName, (string)null!);

        // Assert
        builderNull.Should().Throw<ArgumentNullException>();
        handlerNameNull.Should().Throw<ArgumentNullException>();
        uriStringNull.Should().Throw<ArgumentNullException>();
    }

    [Theory]
    [AutoNSubstituteData]
    public void AddHttpHandler5a_Returns_HttpLayoutRequestHandler(ISitecoreLayoutClientBuilder builder, string handlerName)
    {
        // Arrange
        const string uri = "http://www.test.com";

        // Act
        builder.AddHttpHandler(handlerName, uri);

        ServiceProvider provider = builder.Services.BuildServiceProvider();
        IOptions<SitecoreLayoutClientOptions>? options = provider.GetService<IOptions<SitecoreLayoutClientOptions>>();
        ILayoutRequestHandler result = options!.Value.HandlerRegistry[handlerName].Invoke(provider);

        // Assert
        result.Should().BeOfType<HttpLayoutRequestHandler>();
    }

    [Theory]
    [AutoNSubstituteData]
    public void AddHttpHandler6a_IsGuarded(ISitecoreLayoutClientBuilder builder, string handlerName, Func<IServiceProvider, HttpClient> resolveClient)
    {
        // Arrange
        Func<ILayoutRequestHandlerBuilder<HttpLayoutRequestHandler>> builderNull =
            () => SitecoreLayoutClientBuilderExtensions.AddHttpHandler(null!, handlerName, resolveClient, []);
        Func<ILayoutRequestHandlerBuilder<HttpLayoutRequestHandler>> handlerNameNull =
            () => builder.AddHttpHandler(null!, resolveClient, []);
        Func<ILayoutRequestHandlerBuilder<HttpLayoutRequestHandler>> resolveClientNull =
            () => builder.AddHttpHandler(handlerName, null!, []);
        Func<ILayoutRequestHandlerBuilder<HttpLayoutRequestHandler>> nonValidatedHeadersNull =
            () => builder.AddHttpHandler(handlerName, resolveClient, null!);

        // Assert
        builderNull.Should().Throw<ArgumentNullException>();
        handlerNameNull.Should().Throw<ArgumentNullException>();
        resolveClientNull.Should().Throw<ArgumentNullException>();
        nonValidatedHeadersNull.Should().Throw<ArgumentNullException>();
    }

    [Theory]
    [AutoNSubstituteData]
    public void AddHttpHandler6a_Returns_HttpLayoutRequestHandler(ISitecoreLayoutClientBuilder builder, string handlerName, Func<IServiceProvider, HttpClient> resolveClient, string[] nonValidatedHeaders)
    {
        // Arrange & Act
        builder.AddHttpHandler(handlerName, resolveClient, nonValidatedHeaders);

        ServiceProvider provider = builder.Services.BuildServiceProvider();
        IOptions<SitecoreLayoutClientOptions>? options = provider.GetService<IOptions<SitecoreLayoutClientOptions>>();
        ILayoutRequestHandler result = options!.Value.HandlerRegistry[handlerName].Invoke(provider);

        // Assert
        result.Should().BeOfType<HttpLayoutRequestHandler>();
    }

    [Theory]
    [AutoNSubstituteData]
    public void AddHttpHandler6a_Adds_DefaultSitecoreRequestMapping(ISitecoreLayoutClientBuilder builder, string handlerName, Func<IServiceProvider, HttpClient> resolveClient, string[] nonValidatedHeaders)
    {
        // Arrange & Act
        ILayoutRequestHandlerBuilder<HttpLayoutRequestHandler> clientBuilder = builder.AddHttpHandler(handlerName, resolveClient, nonValidatedHeaders);

        ServiceProvider sp = clientBuilder.Services.BuildServiceProvider();
        IOptionsSnapshot<HttpLayoutRequestHandlerOptions>? handlerOptions = sp.GetService<IOptionsSnapshot<HttpLayoutRequestHandlerOptions>>();
        HttpLayoutRequestHandlerOptions namedHandlerOptions = handlerOptions!.Get(handlerName);

        // Assert
        namedHandlerOptions.Should().NotBeNull();
        namedHandlerOptions.RequestMap.Should().NotBeNull();
        namedHandlerOptions.RequestMap.Should().ContainSingle();
    }

    [Theory]
    [AutoNSubstituteData]
    public void MapFromRequest_IsGuarded(ILayoutRequestHandlerBuilder<HttpLayoutRequestHandler> builder, Action<SitecoreLayoutRequest, HttpRequestMessage> configureHttpRequestMessage)
    {
        // Arrange
        Func<ILayoutRequestHandlerBuilder<HttpLayoutRequestHandler>> builderNull =
            () => LayoutRequestHandlerBuilderExtensions.MapFromRequest(null!, configureHttpRequestMessage);
        Func<ILayoutRequestHandlerBuilder<HttpLayoutRequestHandler>> configureHttpRequestMessageNull =
            () => builder.MapFromRequest(null!);
        Func<ILayoutRequestHandlerBuilder<HttpLayoutRequestHandler>> allNull =
            () => LayoutRequestHandlerBuilderExtensions.MapFromRequest(null!, null!);

        // Assert
        builderNull.Should().Throw<ArgumentNullException>();
        configureHttpRequestMessageNull.Should().Throw<ArgumentNullException>();
        allNull.Should().Throw<ArgumentNullException>();
    }

    [Theory]
    [AutoNSubstituteData]
    public void MapFromRequest_WithValidAction_AddsHttpLayoutRequestHandlerOption(ISitecoreLayoutClientBuilder clientBuilder, string handlerName)
    {
        // Arrange
        ILayoutRequestHandlerBuilder<HttpLayoutRequestHandler> handlerBuilder = clientBuilder.AddHttpHandler(handlerName, "http://www.test.com");

        // Act
        handlerBuilder.MapFromRequest(ConfigureHttpRequestMessage);
        ServiceProvider sp = clientBuilder.Services.BuildServiceProvider();
        IOptionsSnapshot<HttpLayoutRequestHandlerOptions>? handlerOptions = sp.GetService<IOptionsSnapshot<HttpLayoutRequestHandlerOptions>>();
        HttpLayoutRequestHandlerOptions namedHandlerOptions = handlerOptions!.Get(handlerName);

        // Assert
        namedHandlerOptions.Should().NotBeNull();
        namedHandlerOptions.RequestMap.Should().NotBeNull();
        namedHandlerOptions.RequestMap.Should().HaveCount(2);
        return;

        static void ConfigureHttpRequestMessage(SitecoreLayoutRequest request, HttpRequestMessage message) => message.Method = HttpMethod.Post;
    }

    [Theory]
    [AutoNSubstituteData]
    public void ConfigureRequest_IsGuarded(ILayoutRequestHandlerBuilder<HttpLayoutRequestHandler> handler, string[] nonValidatedHeaders)
    {
        // Arrange
        Func<ILayoutRequestHandlerBuilder<HttpLayoutRequestHandler>> httpHandlerBuilderNull =
            () => LayoutRequestHandlerBuilderExtensions.ConfigureRequest(null!, nonValidatedHeaders);
        Func<ILayoutRequestHandlerBuilder<HttpLayoutRequestHandler>> nonValidatedHeadersNull =
            () => handler.ConfigureRequest(null!);

        // Assert
        httpHandlerBuilderNull.Should().Throw<ArgumentNullException>();
        nonValidatedHeadersNull.Should().Throw<ArgumentNullException>();
    }
}