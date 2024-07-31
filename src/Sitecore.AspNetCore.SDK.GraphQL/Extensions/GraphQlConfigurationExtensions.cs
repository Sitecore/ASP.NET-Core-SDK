using GraphQL.Client.Abstractions;
using GraphQL.Client.Http;
using Microsoft.Extensions.DependencyInjection;
using Sitecore.AspNetCore.SDK.GraphQL.Client.Models;
using Sitecore.AspNetCore.SDK.GraphQL.Exceptions;

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

        services.Configure(configuration);

        SitecoreGraphQlClientOptions graphQlClientOptions = TryGetConfiguration(configuration);

        services.AddSingleton<IGraphQLClient, GraphQLHttpClient>(_ =>
        {
            GraphQLHttpClient graphQlHttpClient = new(graphQlClientOptions.EndPoint!, graphQlClientOptions.GraphQlJsonSerializer);

            graphQlHttpClient.HttpClient.DefaultRequestHeaders.Add("sc_apikey", graphQlClientOptions.ApiKey);
            return graphQlHttpClient;
        });

        return services;
    }

    private static SitecoreGraphQlClientOptions TryGetConfiguration(Action<SitecoreGraphQlClientOptions> configuration)
    {
        SitecoreGraphQlClientOptions graphQlClientOptions = new();
        configuration.Invoke(graphQlClientOptions);

        if (string.IsNullOrWhiteSpace(graphQlClientOptions.ApiKey))
        {
            throw new InvalidGraphQlConfigurationException("Empty ApiKey, provided in GraphQLClientOptions.");
        }

        if (graphQlClientOptions.EndPoint == null)
        {
            throw new InvalidGraphQlConfigurationException("Empty EndPoint, provided in GraphQLClientOptions.");
        }

        return graphQlClientOptions;
    }
}