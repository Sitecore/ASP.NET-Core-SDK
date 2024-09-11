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
using Sitecore.AspNetCore.SDK.LayoutService.Client.Request.Handlers.GraphQL;
using Xunit;

namespace Sitecore.AspNetCore.SDK.LayoutService.Client.Tests.Extensions;

public class SitecoreLayoutClientBuilderExtensionsFixture
{
    // ReSharper disable once UnusedMember.Global - Used by testing framework
    public static Action<IFixture> AutoSetup => f =>
    {
        SitecoreLayoutClientBuilder builder = new(new ServiceCollection());
        f.Inject(builder);
    };

    [Fact]
    public void AddHandler_NullBuilder_Throws()
    {
        // Arrange
        Action action = () => SitecoreLayoutClientBuilderExtensions.AddHandler<ILayoutRequestHandler>(null!, "string");

        // Act / Assert
        action.Should().Throw<ArgumentNullException>()
            .And.ParamName.Should().Be("builder");
    }

    [Theory]
    [MemberAutoNSubstituteData(nameof(EmptyStrings))]
    public void AddHandler_InvalidName_Throws(string value, SitecoreLayoutClientBuilder builder)
    {
        // Arrange
        Action action = () => builder.AddHandler<ILayoutRequestHandler>(value);

        // Act / Assert
        action.Should().Throw<ArgumentException>()
            .And.ParamName.Should().Be("name");
    }

    [Theory]
    [AutoNSubstituteData]
    public void AddHandler_Throws_IfTServiceIsAnInterface(SitecoreLayoutClientBuilder builder, string handlerName)
    {
        // Arrange
        Action action = () => builder.AddHandler<ILayoutRequestHandler>(handlerName);

        // Act / Assert
        action.Should().Throw<ArgumentException>()
            .WithMessage($"Can only register implementations of {typeof(ILayoutRequestHandler)} as layout services.");
    }

    [Theory]
    [AutoNSubstituteData]
    public void AddHandler_Throws_IfTServiceIsAnAbstractClass(SitecoreLayoutClientBuilder builder, string handlerName)
    {
        // Arrange
        Action action = () => builder.AddHandler<TestAbstractSitecoreLayoutRequestHandler>(handlerName);

        // Act / Assert
        action.Should().Throw<ArgumentException>()
            .WithMessage("Abstract registrations must provide a factory to resolve a layout service.");
    }

    [Theory]
    [AutoNSubstituteData]
    public void AddHandler_CreatesFactory_IfFactoryIsNull(SitecoreLayoutClientBuilder builder, string handlerName)
    {
        // Arrange / Act
        ILayoutRequestHandlerBuilder<HttpLayoutRequestHandler> result = builder.AddHandler<HttpLayoutRequestHandler>(handlerName);

        // Assert
        ServiceProvider provider = result.Services.BuildServiceProvider();
        SitecoreLayoutClientOptions options = provider.GetRequiredService<IOptions<SitecoreLayoutClientOptions>>().Value;
        options.HandlerRegistry[handlerName].Should().NotBeNull();
    }

    [Theory]
    [AutoNSubstituteData]
    public void AddHandler_UsesTheProvidedFactory_IfFactoryIsNotNull(
        SitecoreLayoutClientBuilder builder,
        string handlerName,
        HttpLayoutRequestHandler service)
    {
        // Arrange / Act
        ILayoutRequestHandlerBuilder<HttpLayoutRequestHandler> result = builder.AddHandler(handlerName, _ => service);

        // Assert
        ServiceProvider provider = result.Services.BuildServiceProvider();
        SitecoreLayoutClientOptions options = provider.GetRequiredService<IOptions<SitecoreLayoutClientOptions>>().Value;
        ILayoutRequestHandler instance = options.HandlerRegistry[handlerName].Invoke(provider);
        instance.Should().BeSameAs(service);
    }

    [Theory]
    [AutoNSubstituteData]
    public void AddHandler_WithValidValues_ReturnsNewBuilderWithCorrectValues(
        SitecoreLayoutClientBuilder builder,
        string handlerName)
    {
        // Arrange / Act
        ILayoutRequestHandlerBuilder<HttpLayoutRequestHandler> result = builder.AddHandler<HttpLayoutRequestHandler>(handlerName);

        // Assert
        result.Should().BeOfType<SitecoreLayoutRequestHandlerBuilder<HttpLayoutRequestHandler>>();
        result.Services.Should().BeSameAs(builder.Services);
        result.HandlerName.Should().Be(handlerName);
    }

    [Fact]
    public void WithDefaultRequestOptions_BuilderIsNUll_ThrowsArgumentNullException()
    {
        // Arrange
        Func<ISitecoreLayoutClientBuilder> act =
            () => SitecoreLayoutClientBuilderExtensions.WithDefaultRequestOptions(null!, null!);

        // Act & Assert
        act.Should().Throw<ArgumentNullException>().WithMessage("Value cannot be null. (Parameter 'builder')");
    }

    [Theory]
    [AutoNSubstituteData]
    public void WithDefaultRequestOptions_ConfigureActionIsNUll_ThrowsArgumentNullException(SitecoreLayoutClientBuilder builder)
    {
        // Arrange
        Func<ISitecoreLayoutClientBuilder> act =
            () => builder.WithDefaultRequestOptions(null!);

        // Act & Assert
        act.Should().Throw<ArgumentNullException>().WithMessage("Value cannot be null. (Parameter 'configureRequest')");
    }

    [Theory]
    [AutoNSubstituteData]
    public void WithDefaultRequestOptions_RequestDefaultsIsNotNull_ServiceProviderReturnsOptionsWithRequestDefaults(SitecoreLayoutClientBuilder builder)
    {
        // Act
        ISitecoreLayoutClientBuilder result = builder.WithDefaultRequestOptions(_ => { });

        // Assert
        ServiceProvider provider = result.Services.BuildServiceProvider();
        SitecoreLayoutRequestOptions options = provider.GetRequiredService<IOptions<SitecoreLayoutRequestOptions>>().Value;
        options.RequestDefaults.Should().NotBeNull();
    }

    [Theory]
    [AutoNSubstituteData]
    public void WithDefaultRequestOptions_RequestDefaultsHasApiKeySpecified_ServiceProviderReturnsOptionsWithRequestDefaultsContainingApiKey(SitecoreLayoutClientBuilder builder)
    {
        // Act
        ISitecoreLayoutClientBuilder result = builder.WithDefaultRequestOptions(o => { o.ApiKey("test_api_key"); });

        // Assert
        ServiceProvider provider = result.Services.BuildServiceProvider();
        SitecoreLayoutRequestOptions options = provider.GetRequiredService<IOptions<SitecoreLayoutRequestOptions>>().Value;
        options.RequestDefaults.Should().NotBeNull();
        options.RequestDefaults.Should().BeOfType<SitecoreLayoutRequest>();
        options.RequestDefaults.Should().ContainKey(RequestKeys.ApiKey);
        options.RequestDefaults.ApiKey().Should().Be("test_api_key");
    }

    [Theory]
    [AutoNSubstituteData]
    public void AddGraphQlWithContextHandler_Minimal_IsValid(SitecoreLayoutClientBuilder builder, string contextId)
    {
        // Act
        ILayoutRequestHandlerBuilder<GraphQlLayoutServiceHandler> result = builder.AddGraphQlWithContextHandler("Test", contextId);

        // Assert
        ServiceProvider provider = result.Services.BuildServiceProvider();
        SitecoreLayoutRequestOptions options = provider.GetRequiredService<IOptions<SitecoreLayoutRequestOptions>>().Value;
        options.RequestDefaults.ContextId().Should().Be(contextId);
        options.RequestDefaults.SiteName().Should().BeNull();
        options.RequestDefaults.Language().Should().Be("en");
    }

    private static IEnumerable<object[]> EmptyStrings()
    {
        yield return [null!];
        yield return [string.Empty];
        yield return ["\t\t   "];
    }
}