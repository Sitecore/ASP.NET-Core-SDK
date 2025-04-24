using AutoFixture.Xunit2;
using FluentAssertions;
using GraphQL.Client.Abstractions;
using GraphQL.Client.Http;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Sitecore.AspNetCore.SDK.AutoFixture.Attributes;
using Sitecore.AspNetCore.SDK.GraphQL.Client.Models;
using Sitecore.AspNetCore.SDK.GraphQL.Exceptions;
using Sitecore.AspNetCore.SDK.GraphQL.Extensions;
using Sitecore.AspNetCore.SDK.GraphQL.Properties;
using Xunit;

namespace Sitecore.AspNetCore.SDK.GraphQL.Tests.Extensions;

public class GraphQlConfigurationExtensionsFixture
{
    [Theory]
    [AutoNSubstituteData]
    public void AddGraphQlClient_NullProperties_ThrowsExceptions(IServiceCollection serviceCollection)
    {
        Func<IServiceCollection> servicesNull =
            () => GraphQlConfigurationExtensions.AddGraphQlClient(null!, null!);
        Func<IServiceCollection> configNull =
            () => serviceCollection.AddGraphQlClient(null!);

        servicesNull.Should().Throw<ArgumentNullException>().WithParameterName("services");
        configNull.Should().Throw<ArgumentNullException>().WithParameterName("configuration");
    }

    [Fact]
    public void AddGraphQlClient_EmptyApiKey_InConfiguration_ThrowsExceptions()
    {
        var services = new ServiceCollection();
        services.AddGraphQlClient(_ => { });
        var sp = services.BuildServiceProvider();
        Func<IGraphQLClient> act = () => sp.GetRequiredService<IGraphQLClient>();
        act.Should().Throw<InvalidGraphQlConfigurationException>()
            .WithMessage(Resources.Exception_MissingApiKeyAndContextId);
    }

    [Theory]
    [AutoData]
    public void AddGraphQlClient_EmptyEndpoint_WithApiKey_ThrowsExceptions(string apiKey)
    {
        var services = new ServiceCollection();
        services.AddGraphQlClient(options =>
        {
            options.ApiKey = apiKey;
        });
        var sp = services.BuildServiceProvider();
        Func<IGraphQLClient> act = () => sp.GetRequiredService<IGraphQLClient>();
        act.Should().Throw<InvalidGraphQlConfigurationException>()
            .WithMessage(Resources.Exception_MissingEndpoint);
    }

    [Theory]
    [AutoData]
    public void AddGraphQlClient_EmptyEndpointUri_WithContextId_UsesDefault(string contextId)
    {
        // Arrange
        ServiceCollection serviceCollection = [];

        // Act
        serviceCollection.AddGraphQlClient(configuration =>
        {
            configuration.ContextId = contextId;
        });
        GraphQLHttpClient? graphQlClient = serviceCollection.BuildServiceProvider().GetService<IGraphQLClient>() as GraphQLHttpClient;

        // Assert
        graphQlClient!.Options.EndPoint!.OriginalString.Should().Contain(SitecoreGraphQlClientOptions.DefaultEdgeEndpoint.OriginalString);
    }

    [Theory]
    [AutoData]
    public void AddGraphQlClient_AddConfiguredGraphQlClient_To_ServiceCollection(string apiKey, Uri endpointUri, string defaultSiteName)
    {
        // Arrange
        ServiceCollection serviceCollection = [];

        // Act
        serviceCollection.AddGraphQlClient(
            configuration =>
            {
                configuration.ApiKey = apiKey;
                configuration.EndPoint = endpointUri;
                configuration.DefaultSiteName = defaultSiteName;
            });
        GraphQLHttpClient? graphQlClient = serviceCollection.BuildServiceProvider().GetService<IGraphQLClient>() as GraphQLHttpClient;

        // Assert
        graphQlClient.Should().NotBeNull();
        graphQlClient!.Options.EndPoint.Should().Be(endpointUri);
        graphQlClient.HttpClient.DefaultRequestHeaders.Contains("sc_apikey").Should().BeTrue();
        apiKey.Should().Be(graphQlClient.HttpClient.DefaultRequestHeaders.GetValues("sc_apikey").FirstOrDefault());
    }

    [Theory]
    [AutoData]
    public void AddGraphQlClient_WithContext_To_ServiceCollection(string contextId, Uri endpointUri, string defaultSiteName)
    {
        // Arrange
        ServiceCollection serviceCollection = [];

        // Act
        serviceCollection.AddGraphQlClient(
            configuration =>
            {
                configuration.ContextId = contextId;
                configuration.EndPoint = endpointUri;
                configuration.DefaultSiteName = defaultSiteName;
            });
        GraphQLHttpClient? graphQlClient = serviceCollection.BuildServiceProvider().GetService<IGraphQLClient>() as GraphQLHttpClient;

        // Assert
        graphQlClient.Should().NotBeNull();
        graphQlClient!.Options.EndPoint!.Host.Should().Be(endpointUri.Host);
        graphQlClient.Options.EndPoint.Query.Should().Contain($"sitecoreContextId={contextId}");
    }
}