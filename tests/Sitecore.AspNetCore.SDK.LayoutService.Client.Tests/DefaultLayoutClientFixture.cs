using AutoFixture;
using AutoFixture.Idioms;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Sitecore.AspNetCore.SDK.AutoFixture.Attributes;
using Sitecore.AspNetCore.SDK.AutoFixture.Extensions;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Configuration;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Exceptions;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Interfaces;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Request;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Response;
using Xunit;

namespace Sitecore.AspNetCore.SDK.LayoutService.Client.Tests;

public class DefaultLayoutClientFixture
{
    // ReSharper disable once UnusedMember.Global - Used by testing framework
    public static Action<IFixture> AutoSetup => f =>
    {
        f.Freeze<IOptions<SitecoreLayoutClientOptions>>();
    };

    public static Action<IFixture> HandlerInvocationThrowsException => f =>
    {
        IServiceProvider? services = Substitute.For<IServiceProvider>();

        Func<IServiceProvider, ILayoutRequestHandler>? handler = Substitute.For<Func<IServiceProvider, ILayoutRequestHandler>>();
        handler.Invoke(Arg.Any<IServiceProvider>()).Throws(new SitecoreLayoutServiceClientException());

        SitecoreLayoutClientOptions options = new()
        {
            DefaultHandler = "HandlerDoesNotExist",
            HandlerRegistry = new Dictionary<string, Func<IServiceProvider, ILayoutRequestHandler>> { { "HandlerDoesNotExist", handler } }
        };

        IOptions<SitecoreLayoutClientOptions>? configureOptions = f.Freeze<IOptions<SitecoreLayoutClientOptions>>();
        configureOptions.Value.Returns(options);

        IOptionsSnapshot<SitecoreLayoutRequestOptions>? layoutRequestOptions = Substitute.For<IOptionsSnapshot<SitecoreLayoutRequestOptions>>();
        ILogger<DefaultLayoutClient>? logger = Substitute.For<ILogger<DefaultLayoutClient>>();
        f.Inject(new DefaultLayoutClient(services, configureOptions, layoutRequestOptions, logger));
    };

    [Theory]
    [AutoNSubstituteData]
    public void Ctor_InvalidArgs_Throws(GuardClauseAssertion guard)
    {
        guard.VerifyConstructors<DefaultLayoutClient>();
    }

    [Theory]
    [AutoNSubstituteData]
    public async Task Request_WithNullRequest_Throws(DefaultLayoutClient sut)
    {
        // Arrange
        Func<Task<SitecoreLayoutResponse>> act =
            () => sut.Request(null!, string.Empty);

        // Act & Assert
        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Theory]
    [AutoNSubstituteData]
    public async Task Request_WithEmptyHandlerName_Throws(
        DefaultLayoutClient sut,
        IOptions<SitecoreLayoutClientOptions> options,
        SitecoreLayoutRequest request)
    {
        // Arrange
        options.Value.DefaultHandler = string.Empty;
        Func<Task<SitecoreLayoutResponse>> act =
            () => sut.Request(request, string.Empty);

        // Act & Assert
        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Theory]
    [AutoNSubstituteData]
    public async Task Request_WithNullRegistryFunc_Throws(
        DefaultLayoutClient sut,
        IOptions<SitecoreLayoutClientOptions> options,
        SitecoreLayoutRequest request,
        string handlerName)
    {
        // Arrange
        options.Value.HandlerRegistry.Add(handlerName, null!);
        Func<Task<SitecoreLayoutResponse>> act =
            () => sut.Request(request, handlerName);

        // Act / Assert
        await act.Should().ThrowAsync<NullReferenceException>();
    }

    [Theory]
    [AutoNSubstituteData]
    public async Task Request_WithNullHandler_Throws(
        DefaultLayoutClient sut,
        IOptions<SitecoreLayoutClientOptions> options,
        SitecoreLayoutRequest request,
        string handlerName)
    {
        // Arrange
        options.Value.HandlerRegistry.Add(handlerName, Func);
        Func<Task<SitecoreLayoutResponse>> act =
            () => sut.Request(request, handlerName);

        // Act / Assert
        await act.Should().ThrowAsync<NullReferenceException>();
        return;

        static ILayoutRequestHandler Func(IServiceProvider sp) => null!;
    }

    [Theory]
    [AutoNSubstituteData]
    public async Task Request_WithEmptyRegistry_Throws(
        DefaultLayoutClient sut,
        IOptions<SitecoreLayoutClientOptions> options,
        SitecoreLayoutRequest request,
        string handlerName)
    {
        // Arrange
        options.Value.HandlerRegistry.Clear();
        Func<Task<SitecoreLayoutResponse>> act =
            () => sut.Request(request, handlerName);

        // Act / Assert
        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Theory]
    [AutoNSubstituteData]
    public async Task Request_WithValidRequest_InvokesHandler(
        DefaultLayoutClient sut,
        SitecoreLayoutRequest request,
        IOptions<SitecoreLayoutClientOptions> sitecoreLayoutServiceOptions,
        string handlerName)
    {
        // Arrange
        ILayoutRequestHandler? sitecoreLayoutService = Substitute.For<ILayoutRequestHandler>();

        Func<IServiceProvider, ILayoutRequestHandler>? handler = Substitute.For<Func<IServiceProvider, ILayoutRequestHandler>>();
        sitecoreLayoutServiceOptions.Value.HandlerRegistry.Clear();
        sitecoreLayoutServiceOptions.Value.HandlerRegistry.Add(handlerName, handler);

        handler.Invoke(Arg.Any<IServiceProvider>()).Returns(sitecoreLayoutService);

        // Act
        await sut.Request(request, handlerName);

        // Assert
        await sitecoreLayoutService.Received(1).Request(Arg.Any<SitecoreLayoutRequest>(), Arg.Any<string>());
    }

    [Fact]
    public async Task Request_OptionsHasDefaultHandlerNameAndMethodHandlerNameArgIsNull_RequestUsesOptionsDefaultHandlerName()
    {
        // Arrange
        IServiceProvider? serviceProvider = Substitute.For<IServiceProvider>();
        const string defaultHandlerName = "DefaultHandlerName";
        SitecoreLayoutRequest request = [];

        SitecoreLayoutClientOptions layoutClientOptions = new() { DefaultHandler = defaultHandlerName };
        IOptions<SitecoreLayoutClientOptions>? layoutClientOptionsStub = Substitute.For<IOptions<SitecoreLayoutClientOptions>>();
        layoutClientOptionsStub.Value.Returns(layoutClientOptions);

        SitecoreLayoutRequestOptions layoutRequestOptions = new();
        IOptionsSnapshot<SitecoreLayoutRequestOptions>? layoutRequestOptionsStub = Substitute.For<IOptionsSnapshot<SitecoreLayoutRequestOptions>>();
        layoutRequestOptionsStub.Value.Returns(layoutRequestOptions);
        layoutRequestOptionsStub.Get(Arg.Is(defaultHandlerName)).Returns(layoutRequestOptions);
        ILogger<DefaultLayoutClient>? logger = Substitute.For<ILogger<DefaultLayoutClient>>();

        DefaultLayoutClient sut = new(serviceProvider, layoutClientOptionsStub, layoutRequestOptionsStub, logger);

        ILayoutRequestHandler? layoutRequestHandler = Substitute.For<ILayoutRequestHandler>();

        Func<IServiceProvider, ILayoutRequestHandler>? handler = Substitute.For<Func<IServiceProvider, ILayoutRequestHandler>>();
        layoutClientOptions.HandlerRegistry.Clear();
        layoutClientOptions.HandlerRegistry.Add(defaultHandlerName, handler);

        handler.Invoke(Arg.Any<IServiceProvider>()).Returns(layoutRequestHandler);

        // Act
        await sut.Request(request, null!);

        // Assert
        await layoutRequestHandler.Received(1).Request(Arg.Is(request), Arg.Is(defaultHandlerName));
    }

    [Fact]
    public async Task Request_OptionsHasDefaultHandlerNameNameAndMethodHandlerNameArgIsSet_RequestUsesMethodArgHandlerName()
    {
        // Arrange
        IServiceProvider? serviceProvider = Substitute.For<IServiceProvider>();
        const string defaultHandlerName = "DefaultHandlerName";
        const string handlerName = "HandlerName";
        SitecoreLayoutRequest request = [];

        SitecoreLayoutClientOptions layoutClientOptions = new() { DefaultHandler = defaultHandlerName };
        IOptions<SitecoreLayoutClientOptions>? layoutClientOptionsStub = Substitute.For<IOptions<SitecoreLayoutClientOptions>>();
        layoutClientOptionsStub.Value.Returns(layoutClientOptions);

        SitecoreLayoutRequestOptions layoutRequestOptions = new();
        IOptionsSnapshot<SitecoreLayoutRequestOptions>? layoutRequestOptionsStub = Substitute.For<IOptionsSnapshot<SitecoreLayoutRequestOptions>>();
        layoutRequestOptionsStub.Value.Returns(layoutRequestOptions);
        layoutRequestOptionsStub.Get(Arg.Is(handlerName)).Returns(layoutRequestOptions);
        ILogger<DefaultLayoutClient>? logger = Substitute.For<ILogger<DefaultLayoutClient>>();

        DefaultLayoutClient sut = new(serviceProvider, layoutClientOptionsStub, layoutRequestOptionsStub, logger);

        ILayoutRequestHandler? layoutRequestHandler = Substitute.For<ILayoutRequestHandler>();

        Func<IServiceProvider, ILayoutRequestHandler>? handler = Substitute.For<Func<IServiceProvider, ILayoutRequestHandler>>();
        layoutClientOptions.HandlerRegistry.Clear();
        layoutClientOptions.HandlerRegistry.Add(defaultHandlerName, handler);
        layoutClientOptions.HandlerRegistry.Add(handlerName, handler);

        handler.Invoke(Arg.Any<IServiceProvider>()).Returns(layoutRequestHandler);

        // Act
        await sut.Request(request, handlerName);

        // Assert
        await layoutRequestHandler.Received(1).Request(Arg.Is(request), Arg.Is(handlerName));
    }

    [Fact]
    public async Task Request_OptionsHasNullDefaultHandlerNameNameAndMethodHandlerNameArgIsNull_Throws()
    {
        // Arrange
        IServiceProvider? serviceProvider = Substitute.For<IServiceProvider>();
        SitecoreLayoutRequest request = [];

        SitecoreLayoutClientOptions layoutClientOptions = new() { DefaultHandler = null };
        IOptions<SitecoreLayoutClientOptions>? layoutClientOptionsStub = Substitute.For<IOptions<SitecoreLayoutClientOptions>>();
        layoutClientOptionsStub.Value.Returns(layoutClientOptions);

        IOptionsSnapshot<SitecoreLayoutRequestOptions>? layoutRequestOptions = Substitute.For<IOptionsSnapshot<SitecoreLayoutRequestOptions>>();
        ILogger<DefaultLayoutClient>? logger = Substitute.For<ILogger<DefaultLayoutClient>>();

        DefaultLayoutClient sut = new(serviceProvider, layoutClientOptionsStub, layoutRequestOptions, logger);

        ILayoutRequestHandler? layoutRequestHandler = Substitute.For<ILayoutRequestHandler>();

        Func<IServiceProvider, ILayoutRequestHandler>? handler = Substitute.For<Func<IServiceProvider, ILayoutRequestHandler>>();
        layoutClientOptions.HandlerRegistry.Clear();
        layoutClientOptions.HandlerRegistry.Add("DefaultClientName", handler);

        handler.Invoke(Arg.Any<IServiceProvider>()).Returns(layoutRequestHandler);
        Func<Task<SitecoreLayoutResponse>> act =
            async () => await sut.Request(request, null!);

        // Act & Assert
        await act.Should().ThrowAsync<ArgumentNullException>().WithMessage("Handler name cannot be null.");
    }

    [Fact]
    public async Task Request_OptionsHasSiteNameAndRequestHasSiteName_RequestSiteNameIsUsed()
    {
        // Arrange
        const string defaultHandlerName = "DefaultHandlerName";
        const string requestSiteName = "requestsitename";
        IServiceProvider? serviceProvider = Substitute.For<IServiceProvider>();
        SitecoreLayoutRequest request = new SitecoreLayoutRequest().SiteName(requestSiteName);

        SitecoreLayoutClientOptions layoutClientOptions = new()
        {
            DefaultHandler = defaultHandlerName
        };

        SitecoreLayoutRequestOptions layoutRequestOptions = new()
        {
            RequestDefaults = new SitecoreLayoutRequest
            {
                { RequestKeys.SiteName, "optionssitename" }
            }
        };

        IOptions<SitecoreLayoutClientOptions>? layoutClientOptionsStub = Substitute.For<IOptions<SitecoreLayoutClientOptions>>();
        layoutClientOptionsStub.Value.Returns(layoutClientOptions);

        IOptionsSnapshot<SitecoreLayoutRequestOptions>? layoutRequestOptionsStub = Substitute.For<IOptionsSnapshot<SitecoreLayoutRequestOptions>>();
        layoutRequestOptionsStub.Value.Returns(new SitecoreLayoutRequestOptions());
        layoutRequestOptionsStub.Get(Arg.Is(defaultHandlerName)).Returns(layoutRequestOptions);
        ILogger<DefaultLayoutClient>? logger = Substitute.For<ILogger<DefaultLayoutClient>>();

        DefaultLayoutClient sut = new(serviceProvider, layoutClientOptionsStub, layoutRequestOptionsStub, logger);

        ILayoutRequestHandler? layoutRequestHandler = Substitute.For<ILayoutRequestHandler>();

        Func<IServiceProvider, ILayoutRequestHandler>? handler = Substitute.For<Func<IServiceProvider, ILayoutRequestHandler>>();
        layoutClientOptions.HandlerRegistry.Clear();
        layoutClientOptions.HandlerRegistry.Add(defaultHandlerName, handler);

        handler.Invoke(Arg.Any<IServiceProvider>()).Returns(layoutRequestHandler);

        // Act
        await sut.Request(request, defaultHandlerName);

        // Assert
        await layoutRequestHandler.Received(1).Request(Arg.Is<SitecoreLayoutRequest>(x => x.SiteName() == requestSiteName), Arg.Any<string>());
    }

    [Fact]
    public async Task Request_OptionsHasSiteNameAndRequestHasNullSiteName_SiteNameIsRemovedFromRequest()
    {
        // Arrange
        string defaultHandlerName = "DefaultHandlerName";
        IServiceProvider? serviceProvider = Substitute.For<IServiceProvider>();
        SitecoreLayoutRequest request = new SitecoreLayoutRequest().SiteName(null);

        SitecoreLayoutClientOptions layoutClientOptions = new()
        {
            DefaultHandler = defaultHandlerName
        };

        SitecoreLayoutRequestOptions layoutRequestOptions = new()
        {
            RequestDefaults = new SitecoreLayoutRequest
            {
                { RequestKeys.SiteName, "optionssitename" }
            }
        };

        IOptions<SitecoreLayoutClientOptions>? layoutClientOptionsStub = Substitute.For<IOptions<SitecoreLayoutClientOptions>>();
        layoutClientOptionsStub.Value.Returns(layoutClientOptions);

        IOptionsSnapshot<SitecoreLayoutRequestOptions>? layoutRequestOptionsStub = Substitute.For<IOptionsSnapshot<SitecoreLayoutRequestOptions>>();
        layoutRequestOptionsStub.Value.Returns(new SitecoreLayoutRequestOptions());
        layoutRequestOptionsStub.Get(Arg.Is(defaultHandlerName)).Returns(layoutRequestOptions);
        ILogger<DefaultLayoutClient>? logger = Substitute.For<ILogger<DefaultLayoutClient>>();

        DefaultLayoutClient sut = new(serviceProvider, layoutClientOptionsStub, layoutRequestOptionsStub, logger);

        ILayoutRequestHandler? layoutRequestHandler = Substitute.For<ILayoutRequestHandler>();

        Func<IServiceProvider, ILayoutRequestHandler>? handler = Substitute.For<Func<IServiceProvider, ILayoutRequestHandler>>();
        layoutClientOptions.HandlerRegistry.Clear();
        layoutClientOptions.HandlerRegistry.Add(defaultHandlerName, handler);

        handler.Invoke(Arg.Any<IServiceProvider>()).Returns(layoutRequestHandler);

        // Act
        await sut.Request(request, defaultHandlerName);

        // Assert
        await layoutRequestHandler.Received(1).Request(Arg.Is<SitecoreLayoutRequest>(x => !x.ContainsKey(RequestKeys.SiteName)), Arg.Any<string>());
    }

    [Theory]
    [AutoNSubstituteData]
    public async Task RequestMethod_Returns_Propagated_CouldNotContactSitecoreLayoutServiceClientException(
        DefaultLayoutClient sut,
        IOptions<SitecoreLayoutClientOptions> options,
        SitecoreLayoutRequest request,
        string handlerName)
    {
        // Arrange
        ILayoutRequestHandler? sitecoreLayoutService = Substitute.For<ILayoutRequestHandler>();

        List<SitecoreLayoutServiceClientException> errors =
            [new CouldNotContactSitecoreLayoutServiceClientException()];
        SitecoreLayoutResponse responseWithErrors = new(request, errors);
        sitecoreLayoutService.Request(Arg.Any<SitecoreLayoutRequest>(), Arg.Any<string>()).Returns(responseWithErrors);

        Func<IServiceProvider, ILayoutRequestHandler>? handler = Substitute.For<Func<IServiceProvider, ILayoutRequestHandler>>();
        handler.Invoke(Arg.Any<IServiceProvider>()).Returns(sitecoreLayoutService);

        options.Value.HandlerRegistry.Add(handlerName, handler);

        // Act
        SitecoreLayoutResponse response = await sut.Request(request, handlerName);

        // Assert
        response.Should().NotBeNull();
        response.HasErrors.Should().BeTrue();
        response.Errors.Should().ContainSingle(x => x.GetType() == typeof(CouldNotContactSitecoreLayoutServiceClientException));
    }

    [Theory]
    [AutoNSubstituteData]
    public async Task RequestMethod_Returns_Propagated_InvalidResponseSitecoreLayoutServiceClientException(
        DefaultLayoutClient sut,
        IOptions<SitecoreLayoutClientOptions> options,
        SitecoreLayoutRequest request,
        string handlerName)
    {
        // Arrange
        ILayoutRequestHandler? sitecoreLayoutService = Substitute.For<ILayoutRequestHandler>();

        List<SitecoreLayoutServiceClientException> errors =
            [new InvalidResponseSitecoreLayoutServiceClientException()];
        SitecoreLayoutResponse responseWithErrors = new(request, errors);
        sitecoreLayoutService.Request(Arg.Any<SitecoreLayoutRequest>(), Arg.Any<string>()).Returns(responseWithErrors);

        Func<IServiceProvider, ILayoutRequestHandler>? handler = Substitute.For<Func<IServiceProvider, ILayoutRequestHandler>>();
        handler.Invoke(Arg.Any<IServiceProvider>()).Returns(sitecoreLayoutService);

        options.Value.HandlerRegistry.Add(handlerName, handler);

        // Act
        SitecoreLayoutResponse response = await sut.Request(request, handlerName);

        // Assert
        response.HasErrors.Should().BeTrue();
        response.Errors.Should().ContainSingle(x => x.GetType() == typeof(InvalidResponseSitecoreLayoutServiceClientException));
    }

    [Theory]
    [AutoNSubstituteData(nameof(HandlerInvocationThrowsException))]
    public void RequestMethod_WithInvalidHandlerName_Throws(
        DefaultLayoutClient sut,
        SitecoreLayoutRequest request,
        IOptions<SitecoreLayoutClientOptions> layoutClientOptions)
    {
        // Arrange
        layoutClientOptions.Value.HandlerRegistry.Clear();

        // Act
        Func<Task> action = async () => await sut.Request(request, "HandlerDoesNotExist");

        // Assert
        action.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Theory]
    [AutoNSubstituteData]
    public void RequestMethod_Throws_Propagated_UnhandledException(
        DefaultLayoutClient sut,
        SitecoreLayoutRequest request,
        string handlerName)
    {
        // Arrange
        Func<IServiceProvider, ILayoutRequestHandler>? handler = Substitute.For<Func<IServiceProvider, ILayoutRequestHandler>>();

        handler.Invoke(Arg.Any<IServiceProvider>()).Throws(new Exception());

        // Act
        Func<Task> action = async () => await sut.Request(request, handlerName);

        // Assert
        action.Should().ThrowAsync<Exception>();
    }
}