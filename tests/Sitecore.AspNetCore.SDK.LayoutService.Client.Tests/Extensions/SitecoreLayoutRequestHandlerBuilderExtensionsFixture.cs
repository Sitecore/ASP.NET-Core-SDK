using AutoFixture;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Sitecore.AspNetCore.SDK.AutoFixture.Attributes;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Configuration;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Extensions;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Interfaces;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Request;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Request.Handlers;
using Xunit;

namespace Sitecore.AspNetCore.SDK.LayoutService.Client.Tests.Extensions;

public class SitecoreLayoutRequestHandlerBuilderExtensionsFixture
{
    // ReSharper disable once UnusedMember.Global - Used by testing framework
    public static Action<IFixture> AutoSetup => f =>
    {
        ServiceCollection services = [];
        f.Inject<IServiceCollection>(services);

        SitecoreLayoutRequestHandlerBuilder<HttpLayoutRequestHandler> clientBuilder = new("testing", new ServiceCollection());
        f.Inject(clientBuilder);
    };

    [Fact]
    public void AsDefaultHandler_NullBuilder_Throws()
    {
        // Arrange
        Action action = () => LayoutRequestHandlerBuilderExtensions.AsDefaultHandler<HttpLayoutRequestHandler>(null!);

        // Act / Assert
        action.Should().Throw<ArgumentNullException>()
            .And.ParamName.Should().Be("builder");
    }

    [Theory]
    [AutoNSubstituteData]
    public void AsDefaultHandler_WithBuilder_ConfiguresDefault(SitecoreLayoutRequestHandlerBuilder<HttpLayoutRequestHandler> builder)
    {
        // Arrange / Act
        ILayoutRequestHandlerBuilder<HttpLayoutRequestHandler> result = builder.AsDefaultHandler();

        // Assert
        ServiceProvider provider = result.Services.BuildServiceProvider();
        SitecoreLayoutClientOptions options = provider.GetRequiredService<IOptions<SitecoreLayoutClientOptions>>().Value;
        options.DefaultHandler.Should().Be(builder.HandlerName);
    }

    [Fact]
    public void WithRequestOptions_BuilderIsNull_ThrowsArgumentNullException()
    {
        // Arrange
        ILayoutRequestHandlerBuilder<ILayoutRequestHandler> builder = null!;
        Func<ILayoutRequestHandlerBuilder<ILayoutRequestHandler>> act =
            () => builder.WithRequestOptions(null!);

        // Act & Assert
        act.Should().Throw<ArgumentNullException>().WithMessage("Value cannot be null. (Parameter 'builder')");
    }

    [Fact]
    public void WithRequestOptions_ActionIsNull_ThrowsArgumentNullException()
    {
        // Arrange
        ServiceCollection services = [];
        SitecoreLayoutRequestHandlerBuilder<HttpLayoutRequestHandler> builder = new("test", services);
        Func<ILayoutRequestHandlerBuilder<HttpLayoutRequestHandler>> act =
            () => builder.WithRequestOptions(null!);

        // Arrange & Act
        act.Should().Throw<ArgumentNullException>().WithMessage("Value cannot be null. (Parameter 'configureRequest')");
    }

    [Theory]
    [AutoNSubstituteData]
    public void WithRequestOptions_RequestDefaultsHasApiKeySpecified_ServiceProviderReturnsOptionsWithRequestDefaultsContainingApiKey(IServiceCollection services, string handlerName)
    {
        // Arrange
        SitecoreLayoutRequestHandlerBuilder<HttpLayoutRequestHandler> builder = new(handlerName, services);

        // Act
        ILayoutRequestHandlerBuilder<HttpLayoutRequestHandler> result = builder.WithRequestOptions(o => { o.ApiKey("test_api_key"); });

        // Assert
        ServiceProvider provider = result.Services.BuildServiceProvider();
        IOptionsSnapshot<SitecoreLayoutRequestOptions>? options = provider.GetService<IOptionsSnapshot<SitecoreLayoutRequestOptions>>();

        options.Should().NotBeNull();
        options!.Value.Should().NotBeNull();
        options.Value.Should().BeOfType<SitecoreLayoutRequestOptions>();
        options.Get(handlerName).RequestDefaults.Should().ContainKey(RequestKeys.ApiKey);
        options.Get(handlerName).RequestDefaults.ApiKey().Should().Be("test_api_key");
    }
}