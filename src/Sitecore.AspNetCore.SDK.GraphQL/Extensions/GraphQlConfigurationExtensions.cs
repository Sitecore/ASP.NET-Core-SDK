using GraphQL.Client.Abstractions;
using GraphQL.Client.Http;
using Microsoft.Extensions.DependencyInjection;
using Sitecore.AspNetCore.SDK.GraphQL.Client.Models;
using Sitecore.AspNetCore.SDK.GraphQL.Exceptions;
using Sitecore.AspNetCore.SDK.GraphQL.Properties;

namespace Sitecore.AspNetCore.SDK.GraphQL.Extensions;

/// <summary>
/// Sitemap configuration.
/// </summary>
public static class GraphQLConfigurationExtensions
{
    /// <summary>
    /// Configuration for GraphQLClient.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection" /> to add services to.</param>
    /// <param name="configuration">The <see cref="SitecoreGraphQLClientOptions" /> configuration for GraphQL client.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddGraphQLClient(this IServiceCollection services, Action<SitecoreGraphQLClientOptions> configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        services.Configure(configuration);

        SitecoreGraphQLClientOptions options = TryGetConfiguration(configuration);

        services.AddSingleton<IGraphQLClient, GraphQLHttpClient>(_ =>
        {
            if (!string.IsNullOrWhiteSpace(options.ContextId))
            {
                options.EndPoint = options.EndPoint.AddQueryString(
                    SitecoreGraphQLClientOptions.ContextIdQueryStringKey,
                    options.ContextId);
            }

            GraphQLHttpClient graphQlHttpClient = new(options.EndPoint!, options.GraphQLJsonSerializer);

            if (!string.IsNullOrWhiteSpace(options.ApiKey))
            {
                graphQlHttpClient.HttpClient.DefaultRequestHeaders.Add(SitecoreGraphQLClientOptions.ApiKeyHeaderName, options.ApiKey);
            }

            return graphQlHttpClient;
        });

        return services;
    }

    private static SitecoreGraphQLClientOptions TryGetConfiguration(Action<SitecoreGraphQLClientOptions> configuration)
    {
        SitecoreGraphQLClientOptions options = new();
        configuration.Invoke(options);

        if (string.IsNullOrWhiteSpace(options.ApiKey) && string.IsNullOrWhiteSpace(options.ContextId))
        {
            throw new InvalidGraphQLConfigurationException(Resources.Exception_MissingApiKeyAndContextId);
        }

        if (options.EndPoint == null && !string.IsNullOrWhiteSpace(options.ContextId))
        {
            options.EndPoint = SitecoreGraphQLClientOptions.DefaultEdgeEndpoint;
        }
        else if (options.EndPoint == null)
        {
            throw new InvalidGraphQLConfigurationException(Resources.Exception_MissingEndpoint);
        }

        return options;
    }
}