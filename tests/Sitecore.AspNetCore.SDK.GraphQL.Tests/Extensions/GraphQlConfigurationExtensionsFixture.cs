using AutoFixture.Xunit2;
using FluentAssertions;
using GraphQL.Client.Abstractions;
using GraphQL.Client.Http;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Sitecore.AspNetCore.SDK.AutoFixture.Attributes;
using Sitecore.AspNetCore.SDK.GraphQL.Exceptions;
using Sitecore.AspNetCore.SDK.GraphQL.Extensions;
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
        Func<IServiceCollection> act =
            () => Substitute.For<IServiceCollection>().AddGraphQlClient(delegate { });
        act.Should().Throw<InvalidGraphQlConfigurationException>()
            .WithMessage("Empty ApiKey, provided in GraphQLClientOptions.");
    }

    [Theory]
    [AutoData]
    public void AddGraphQlClient_EmptyEndpointUri_InConfiguration_ThrowsExceptions(string apiKey)
    {
        Func<IServiceCollection> act =
            () => Substitute.For<IServiceCollection>().AddGraphQlClient(configuration =>
            {
                configuration.ApiKey = apiKey;
            });
        act.Should().Throw<InvalidGraphQlConfigurationException>()
            .WithMessage("Empty EndPoint, provided in GraphQLClientOptions.");
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
}