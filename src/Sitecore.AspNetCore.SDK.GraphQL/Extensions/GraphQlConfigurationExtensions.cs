﻿using GraphQL.Client.Abstractions;
using GraphQL.Client.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Sitecore.AspNetCore.SDK.GraphQL.Client.Models;
using Sitecore.AspNetCore.SDK.GraphQL.Exceptions;
using Sitecore.AspNetCore.SDK.GraphQL.Properties;

namespace Sitecore.AspNetCore.SDK.GraphQL.Extensions;

/// <summary>
/// Sitemap configuration.
/// </summary>
public static class GraphQlConfigurationExtensions
{
    /// <summary>
    /// Configuration for GraphQLClient.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection" /> to add services to.</param>
    /// <param name="configuration">The <see cref="SitecoreGraphQlClientOptions" /> configuration for GraphQL client.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddGraphQlClient(this IServiceCollection services, Action<SitecoreGraphQlClientOptions> configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        services.Configure<SitecoreGraphQlClientOptions>(x => TryGetConfiguration(x, configuration));

        services.AddSingleton<IGraphQLClient, GraphQLHttpClient>(sp =>
        {
            var options = sp.GetRequiredService<IOptions<SitecoreGraphQlClientOptions>>().Value;
            ValidateOptions(options);

            if (!string.IsNullOrWhiteSpace(options.ContextId))
            {
                options.EndPoint = options.EndPoint.AddQueryString(
                    SitecoreGraphQlClientOptions.ContextIdQueryStringKey,
                    options.ContextId);
            }

            GraphQLHttpClient graphQlHttpClient = new(options, options.GraphQlJsonSerializer);

            if (!string.IsNullOrWhiteSpace(options.ApiKey))
            {
                graphQlHttpClient.HttpClient.DefaultRequestHeaders.Add(SitecoreGraphQlClientOptions.ApiKeyHeaderName, options.ApiKey);
            }

            return graphQlHttpClient;
        });

        return services;
    }

    private static SitecoreGraphQlClientOptions TryGetConfiguration(SitecoreGraphQlClientOptions options, Action<SitecoreGraphQlClientOptions> configuration)
    {
        configuration.Invoke(options);

        if (options.EndPoint == null && !string.IsNullOrWhiteSpace(options.ContextId))
        {
            options.EndPoint = SitecoreGraphQlClientOptions.DefaultEdgeEndpoint;
        }

        return options;
    }

    private static void ValidateOptions(SitecoreGraphQlClientOptions options)
    {
        if (string.IsNullOrWhiteSpace(options.ApiKey) && string.IsNullOrWhiteSpace(options.ContextId))
        {
            throw new InvalidGraphQlConfigurationException(Resources.Exception_MissingApiKeyAndContextId);
        }

        if (options.EndPoint == null && !string.IsNullOrWhiteSpace(options.ContextId))
        {
            options.EndPoint = SitecoreGraphQlClientOptions.DefaultEdgeEndpoint;
        }
        else if (options.EndPoint == null)
        {
            throw new InvalidGraphQlConfigurationException(Resources.Exception_MissingEndpoint);
        }
    }
}