using System.Net;
using System.Net.Http.Headers;
using FluentAssertions;
using GraphQL.Client.Abstractions;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.SystemTextJson;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Sitecore.AspNetCore.SDK.AutoFixture.Attributes;
using Sitecore.AspNetCore.SDK.AutoFixture.Mocks;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Extensions;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Interfaces;
using Xunit;

namespace Sitecore.AspNetCore.SDK.LayoutService.Client.Integration.Tests;

public class RequestsFixture
{
    [Theory]
    [AutoNSubstituteData]
    public async Task ContextRequest_Ok(string contextId)
    {
        // Arrange
        IServiceCollection services = new ServiceCollection();

        // Logging is required for the client
        services.AddLogging();

        // Set up the client
        ISitecoreLayoutClientBuilder builder = services.AddSitecoreLayoutService();
        builder.AddGraphQlWithContextHandler("contextHandler", contextId).AsDefaultHandler();

        // Create an intercept for the actual HTTP call
        MockHttpMessageHandler result = new();
        string json = await File.ReadAllTextAsync("./Json/headlessSxa.json");
        HttpResponseMessage message = new(HttpStatusCode.OK)
        {
            Content = new StringContent($"{{ \"data\": {json} }}")
        };
        message.Content.Headers.ContentType = new MediaTypeHeaderValue("application/graphql+json");
        result.Responses.Push(message);
        string serviceKey = $"https://edge-platform.sitecorecloud.io/v1/content/api/graphql/v1?sitecoreContextId={contextId}";
        services.RemoveAllKeyed<IGraphQLClient>(serviceKey);
        services.AddKeyedSingleton<IGraphQLClient>(serviceKey, new GraphQLHttpClient(
            o =>
            {
                o.EndPoint = new Uri(serviceKey);
                o.HttpMessageHandler = result;
            },
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
}