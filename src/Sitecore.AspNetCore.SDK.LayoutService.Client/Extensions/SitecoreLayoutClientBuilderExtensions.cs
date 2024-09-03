using System.Text.Json.Serialization;
using GraphQL.Client.Abstractions;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.SystemTextJson;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Configuration;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Interfaces;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Properties;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Request;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Request.Handlers;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Request.Handlers.GraphQL;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Serialization;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Serialization.Converter;

namespace Sitecore.AspNetCore.SDK.LayoutService.Client.Extensions;

/// <summary>
/// Extension methods to support configuration of layout service services.
/// </summary>
public static class SitecoreLayoutClientBuilderExtensions
{
    /// <summary>
    /// Registers a handler of type <typeparamref name="THandler"/> to handle requests.
    /// </summary>
    /// <typeparam name="THandler">The type of service to be registered for this <paramref name="name"/>.</typeparam>
    /// <param name="builder">The <see cref="ISitecoreLayoutClientBuilder"/> being configured.</param>
    /// <param name="name">The name used to identify the handler.</param>
    /// <param name="factory">Optional factory to control the instantiation of the client.</param>
    /// <returns>The <see cref="ILayoutRequestHandlerBuilder{THandler}"/> so that additional calls can be chained.</returns>
    public static ILayoutRequestHandlerBuilder<THandler> AddHandler<THandler>(
        this ISitecoreLayoutClientBuilder builder,
        string name,
        Func<IServiceProvider, THandler>? factory = null)
        where THandler : ILayoutRequestHandler
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        Type handlerType = typeof(THandler);
        if (handlerType.IsInterface)
        {
            throw new ArgumentException(string.Format(Resources.Exception_RegisterTypesOfService, typeof(THandler)));
        }

        if (handlerType.IsAbstract && factory == null)
        {
            throw new ArgumentException(Resources.Exception_AbstractRegistrationsMustProvideFactory);
        }

        factory ??= sp => ActivatorUtilities.CreateInstance<THandler>(sp);

        builder.Services.Configure<SitecoreLayoutClientOptions>(options =>
        {
            options.HandlerRegistry[name] = sp =>
            {
                using IServiceScope scope = sp.CreateScope();
                return factory(scope.ServiceProvider);
            };
        });

        return new SitecoreLayoutRequestHandlerBuilder<THandler>(name, builder.Services);
    }

    /// <summary>
    /// Registers a graphQl handler to handle requests.
    /// </summary>
    /// <param name="builder">The <see cref="ISitecoreLayoutClientBuilder"/> being configured.</param>
    /// <param name="name">The name used to identify the handler.</param>
    /// <param name="siteName">The siteName used to identify the handler.</param>
    /// <param name="apiKey">The apiKey to access graphQl endpoint.</param>
    /// <param name="uri">GraphQl endpoint uri.</param>
    /// <param name="defaultLanguage">Default language for GraphQl requests.</param>
    /// <returns>The <see cref="ILayoutRequestHandlerBuilder{THandler}"/> so that additional calls can be chained.</returns>
    public static ILayoutRequestHandlerBuilder<GraphQlLayoutServiceHandler> AddGraphQlHandler(
        this ISitecoreLayoutClientBuilder builder,
        string name,
        string siteName,
        string apiKey,
        Uri uri,
        string defaultLanguage = "en")
    {
        ArgumentNullException.ThrowIfNull(name);
        ArgumentNullException.ThrowIfNull(siteName);
        ArgumentNullException.ThrowIfNull(apiKey);
        ArgumentNullException.ThrowIfNull(uri);

        GraphQLHttpClient client = new(uri, new SystemTextJsonSerializer());
        client.HttpClient.DefaultRequestHeaders.Add("sc_apikey", apiKey);

        builder.WithDefaultRequestOptions(request =>
        {
            request
                .SiteName(siteName)
                .ApiKey(apiKey);
            if (!request.ContainsKey(RequestKeys.Language))
            {
                request.Language(defaultLanguage);
            }
        });
        return builder.AddHandler(name, sp
            => ActivatorUtilities.CreateInstance<GraphQlLayoutServiceHandler>(
                sp, client, sp.GetRequiredService<ISitecoreLayoutSerializer>(), sp.GetRequiredService<ILogger<GraphQlLayoutServiceHandler>>()));
    }

    /// <summary>
    /// Registers a graphQl handler to handle requests, it uses already configured GraphQL client.
    /// </summary>
    /// <param name="builder">The <see cref="ISitecoreLayoutClientBuilder"/> being configured.</param>
    /// <param name="name">The name used to identify the handler.</param>
    /// <param name="siteName">The siteName used to identify the handler.</param>
    /// <param name="defaultLanguage">Default language for GraphQl requests.</param>
    /// <returns>The <see cref="ILayoutRequestHandlerBuilder{THandler}"/> so that additional calls can be chained.</returns>
    public static ILayoutRequestHandlerBuilder<GraphQlLayoutServiceHandler> AddGraphQlHandler(
        this ISitecoreLayoutClientBuilder builder,
        string name,
        string siteName,
        string defaultLanguage = "en")
    {
        ArgumentNullException.ThrowIfNull(name);
        ArgumentNullException.ThrowIfNull(siteName);

        builder.WithDefaultRequestOptions(request =>
        {
            request
                .SiteName(siteName);
            if (!request.ContainsKey(RequestKeys.Language))
            {
                request.Language(defaultLanguage);
            }
        });
        return builder.AddHandler(name, sp
            => ActivatorUtilities.CreateInstance<GraphQlLayoutServiceHandler>(
                sp, sp.GetRequiredService<IGraphQLClient>(), sp.GetRequiredService<ISitecoreLayoutSerializer>(), sp.GetRequiredService<ILogger<GraphQlLayoutServiceHandler>>()));
    }

    /// <summary>
    /// Registers the default layout service request options for all handlers.
    /// </summary>
    /// <param name="builder">The <see cref="ISitecoreLayoutClientBuilder"/> being configured.</param>
    /// <param name="configureRequest">The <see cref="SitecoreLayoutRequest"/> request options configuration.</param>
    /// <returns>The configured <see cref="ISitecoreLayoutClientBuilder"/>.</returns>
    public static ISitecoreLayoutClientBuilder WithDefaultRequestOptions(this ISitecoreLayoutClientBuilder builder, Action<SitecoreLayoutRequest> configureRequest)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(configureRequest);

        builder.Services.ConfigureAll<SitecoreLayoutRequestOptions>(options => configureRequest(options.RequestDefaults));

        return builder;
    }

    /// <summary>
    /// Configures System.Text.Json specific features such as input and output formatters.
    /// </summary>
    /// <param name="builder">The <see cref="ISitecoreLayoutClientBuilder"/> being configured.</param>
    /// <returns>The <see cref="ILayoutRequestHandlerBuilder{THandler}"/> so that additional calls can be chained.</returns>
    public static ISitecoreLayoutClientBuilder AddSystemTextJson(this ISitecoreLayoutClientBuilder builder)
    {
        ServiceDescriptor descriptor = new(typeof(ISitecoreLayoutSerializer), typeof(JsonLayoutServiceSerializer), ServiceLifetime.Singleton);
        builder.Services.Replace(descriptor);

        builder.Services.AddSingleton<IFieldParser, FieldParser>();
        builder.Services.AddSingleton<JsonConverter, FieldConverter>();
        builder.Services.AddSingleton<JsonConverter, PlaceholderFeatureConverter>();

        return builder;
    }

    /// <summary>
    /// Registers a HTTP request handler for the Sitecore layout service client.
    /// </summary>
    /// <param name="builder">The <see cref="ISitecoreLayoutClientBuilder"/> to configure.</param>
    /// <param name="handlerName">The name of the request handler being registered.</param>
    /// <param name="resolveClient">A function to resolve the <see cref="HttpClient"/> to be used. Be aware, that the underlying <see cref="HttpMessageHandler"/> associated to the HttpClient will be reused across multiple sessions.
    /// To prevent data, leaking among sessions, make sure Cookies are not cached. See for reference https://docs.microsoft.com/en-us/aspnet/core/fundamentals/http-requests?view=aspnetcore-3.1#cookies .</param>
    /// <returns>The <see cref="ILayoutRequestHandlerBuilder{HttpLayoutRequestHandler}"/> so that additional calls can be chained.</returns>
    public static ILayoutRequestHandlerBuilder<HttpLayoutRequestHandler> AddHttpHandler(
        this ISitecoreLayoutClientBuilder builder,
        string handlerName,
        Func<IServiceProvider, HttpClient> resolveClient)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(handlerName);
        ArgumentNullException.ThrowIfNull(resolveClient);

        ILayoutRequestHandlerBuilder<HttpLayoutRequestHandler> httpHandlerBuilder = builder.AddHandler(handlerName, sp =>
        {
            HttpClient client = resolveClient(sp);
            return ActivatorUtilities.CreateInstance<HttpLayoutRequestHandler>(sp, client);
        });

        httpHandlerBuilder.ConfigureRequest([]);

        return httpHandlerBuilder;
    }

    /// <summary>
    /// Registers an HTTP request handler for the Sitecore layout service client.
    /// </summary>
    /// <param name="builder">The <see cref="ISitecoreLayoutClientBuilder"/> to configure.</param>
    /// <param name="handlerName">The name of the request handler being registered.</param>
    /// <param name="resolveClient">A function to resolve the <see cref="HttpClient"/> to be used. Be aware, that the underlying <see cref="HttpMessageHandler"/> associated to the HttpClient will be reused across multiple sessions.
    /// To prevent data, leaking among sessions, make sure Cookies are not cached. See for reference https://docs.microsoft.com/en-us/aspnet/core/fundamentals/http-requests?view=aspnetcore-3.1#cookies .</param>
    /// <param name="nonValidatedHeaders">The list of headers which should not be validated.</param>
    /// <returns>The <see cref="ILayoutRequestHandlerBuilder{HttpLayoutRequestHandler}"/> so that additional calls can be chained.</returns>
    public static ILayoutRequestHandlerBuilder<HttpLayoutRequestHandler> AddHttpHandler(
        this ISitecoreLayoutClientBuilder builder,
        string handlerName,
        Func<IServiceProvider, HttpClient> resolveClient,
        string[] nonValidatedHeaders)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(handlerName);
        ArgumentNullException.ThrowIfNull(resolveClient);
        ArgumentNullException.ThrowIfNull(nonValidatedHeaders);

        ILayoutRequestHandlerBuilder<HttpLayoutRequestHandler> httpHandlerBuilder = builder.AddHandler(handlerName, sp =>
        {
            HttpClient client = resolveClient(sp);
            return ActivatorUtilities.CreateInstance<HttpLayoutRequestHandler>(sp, client);
        });

        httpHandlerBuilder.ConfigureRequest(nonValidatedHeaders);

        return httpHandlerBuilder;
    }

    /// <summary>
    /// Registers an HTTP request handler for the Sitecore layout service client.
    /// </summary>
    /// <param name="builder">The <see cref="ISitecoreLayoutClientBuilder"/> to configure.</param>
    /// <param name="handlerName">The name of the request handler being registered.</param>
    /// <param name="configure">An action to configure the <see cref="HttpClient"/>.</param>
    /// <returns>The <see cref="ILayoutRequestHandlerBuilder{HttpLayoutRequestHandler}"/> so that additional calls can be chained.</returns>
    public static ILayoutRequestHandlerBuilder<HttpLayoutRequestHandler> AddHttpHandler(
        this ISitecoreLayoutClientBuilder builder,
        string handlerName,
        Action<IServiceProvider, HttpClient> configure)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(handlerName);
        ArgumentNullException.ThrowIfNull(configure);

        builder.Services.AddHttpClient(handlerName, configure);

        return AddHttpHandler(builder, handlerName, sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient(handlerName));
    }

    /// <summary>
    /// Registers an HTTP request handler for the Sitecore layout service client.
    /// </summary>
    /// <param name="builder">The <see cref="ISitecoreLayoutClientBuilder"/> to configure.</param>
    /// <param name="handlerName">The name of the request handler being registered.</param>
    /// <param name="configure">An action to configure the <see cref="HttpClient"/>.</param>
    /// <returns>The <see cref="ILayoutRequestHandlerBuilder{HttpLayoutRequestHandler}"/> so that additional calls can be chained.</returns>
    public static ILayoutRequestHandlerBuilder<HttpLayoutRequestHandler> AddHttpHandler(
        this ISitecoreLayoutClientBuilder builder,
        string handlerName,
        Action<HttpClient> configure)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(handlerName);
        ArgumentNullException.ThrowIfNull(configure);

        builder.Services.AddHttpClient(handlerName, configure).ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
        {
            // DO NOT REMOVE - this prevents from cookies being shared among private sessions.
            UseCookies = false
        });

        return AddHttpHandler(builder, handlerName, sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient(handlerName));
    }

    /// <summary>
    /// Registers an HTTP request handler for the Sitecore layout service client.
    /// </summary>
    /// <param name="builder">The <see cref="ISitecoreLayoutClientBuilder"/> to configure.</param>
    /// <param name="handlerName">The name of the request handler being registered.</param>
    /// <param name="uri">The <see cref="Uri"/> used for the <see cref="HttpClient.BaseAddress"/>.</param>
    /// <returns>The <see cref="ILayoutRequestHandlerBuilder{HttpLayoutRequestHandler}"/> so that additional calls can be chained.</returns>
    public static ILayoutRequestHandlerBuilder<HttpLayoutRequestHandler> AddHttpHandler(
        this ISitecoreLayoutClientBuilder builder,
        string handlerName,
        Uri uri)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(handlerName);
        ArgumentNullException.ThrowIfNull(uri);

        return AddHttpHandler(builder, handlerName, client => client.BaseAddress = uri);
    }

    /// <summary>
    /// Registers an HTTP request handler for the Sitecore layout service client.
    /// </summary>
    /// <param name="builder">The <see cref="ISitecoreLayoutClientBuilder"/> to configure.</param>
    /// <param name="handlerName">The name of the request handler being registered.</param>
    /// <param name="uri">The <see cref="Uri"/> used for the <see cref="HttpClient.BaseAddress"/>.</param>
    /// <returns>The <see cref="ILayoutRequestHandlerBuilder{HttpLayoutRequestHandler}"/> so that additional calls can be chained.</returns>
    public static ILayoutRequestHandlerBuilder<HttpLayoutRequestHandler> AddHttpHandler(
        this ISitecoreLayoutClientBuilder builder,
        string handlerName,
        string uri)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(handlerName);
        ArgumentNullException.ThrowIfNull(uri);

        return AddHttpHandler(builder, handlerName, new Uri(uri));
    }
}