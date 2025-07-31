using System.Net;
using System.Net.Http.Headers;
using AwesomeAssertions;
using GraphQL.Client.Abstractions;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.SystemTextJson;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Sitecore.AspNetCore.SDK.AutoFixture.Attributes;
using Sitecore.AspNetCore.SDK.AutoFixture.Mocks;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Extensions;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Interfaces;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Request;
using Xunit;

namespace Sitecore.AspNetCore.SDK.LayoutService.Client.Integration.Tests;

public class RequestsFixture
{
    [Theory]
    [AutoNSubstituteData]
    public async Task ContextRequest_Ok(string handlerName, string contextId)
    {
        // Arrange
        IServiceCollection services = new ServiceCollection();

        // Logging is required for the client
        services.AddLogging();

        // Set up the client
        ISitecoreLayoutClientBuilder builder = services.AddSitecoreLayoutService();
        builder.AddGraphQLWithContextHandler(handlerName, contextId).AsDefaultHandler();

        // Create an intercept for the actual HTTP call
        MockHttpMessageHandler result = new();
        string json = await File.ReadAllTextAsync("./Json/headlessSxa.json");
        HttpResponseMessage message = new(HttpStatusCode.OK)
        {
            Content = new StringContent($"{{ \"data\": {json} }}")
        };
        message.Content.Headers.ContentType = new MediaTypeHeaderValue("application/graphql+json");
        result.Responses.Push(message);
        ServiceDescriptor gqlClient = services.Single(s => s.ServiceKey?.ToString() == handlerName);
        GraphQLHttpClientOptions options = ((GraphQLHttpClient)gqlClient.KeyedImplementationInstance!).Options;
        options.HttpMessageHandler = result;
        services.RemoveAllKeyed<IGraphQLClient>(handlerName);
        services.AddKeyedSingleton<IGraphQLClient>(handlerName, new GraphQLHttpClient(
            options,
            new SystemTextJsonSerializer()));

        // Build and grab the sut
        IServiceProvider provider = services.BuildServiceProvider();
        ISitecoreLayoutClient sut = provider.GetRequiredService<ISitecoreLayoutClient>();

        // Act
        _ = await sut.Request([]);

        // Assert
        result.Requests.Should().ContainSingle();
        result.Requests.Single().RequestUri!.Query.Should().Contain(contextId);
    }

    [Theory]
    [AutoNSubstituteData]
    public async Task ApiRequest_Ok(string handler1Name, string handler2Name, string site1Name, string site2Name, string apiKey, Uri endpoint)
    {
        // Arrange
        IServiceCollection services = new ServiceCollection();

        // Logging is required for the client
        services.AddLogging();

        // Set up the client
        ISitecoreLayoutClientBuilder builder = services.AddSitecoreLayoutService();
        builder.AddGraphQLHandler(handler1Name, site1Name, apiKey, endpoint).AsDefaultHandler();
        builder.AddGraphQLHandler(handler2Name, site2Name);

        // Create an intercept for the actual HTTP call
        MockHttpMessageHandler result = new();
        string json = await File.ReadAllTextAsync("./Json/headlessSxa.json");
        result.Responses.Push(GenerateGqlResponse(json));
        result.Responses.Push(GenerateGqlResponse(json));
        result.Responses.Push(GenerateGqlResponse(json));
        ServiceDescriptor gqlClient = services.Single(s => s.ServiceKey?.ToString() == handler1Name);
        GraphQLHttpClient originalClient = (GraphQLHttpClient)gqlClient.KeyedImplementationInstance!;
        GraphQLHttpClientOptions options = originalClient.Options;
        options.HttpMessageHandler = result;
        services.RemoveAllKeyed<IGraphQLClient>(handler1Name);
        services.RemoveAll<IGraphQLClient>();
        GraphQLHttpClient interceptClient = new(
            options,
            new SystemTextJsonSerializer());
        services.AddKeyedSingleton<IGraphQLClient>(handler1Name, interceptClient);
        services.AddSingleton<IGraphQLClient>(interceptClient);

        // Build and grab the sut
        IServiceProvider provider = services.BuildServiceProvider();
        ISitecoreLayoutClient sut = provider.GetRequiredService<ISitecoreLayoutClient>();

        // Act
        _ = await sut.Request([]);
        _ = await sut.Request([], handler2Name);
        _ = await sut.Request(new SitecoreLayoutRequest { { RequestKeys.SiteName, site2Name } }, handler2Name);

        // Assert
        result.Requests.Should().HaveCount(3);
        string req1 = await result.Requests[0].Content!.ReadAsStringAsync();
        req1.Should().Contain(site1Name);
        string req2 = await result.Requests[1].Content!.ReadAsStringAsync();
        req2.Should().Contain(site1Name);
        string req3 = await result.Requests[2].Content!.ReadAsStringAsync();
        req3.Should().Contain(site2Name);

        // Since we replace the client we validate the original
        originalClient.HttpClient.DefaultRequestHeaders.Single(h => h.Key == "sc_apikey").Value.Single().Should().Be(apiKey);
    }

    private static HttpResponseMessage GenerateGqlResponse(string json)
    {
        HttpResponseMessage result = new(HttpStatusCode.OK)
        {
            Content = new StringContent($"{{ \"data\": {json} }}")
        };
        result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/graphql+json");

        return result;
    }
}