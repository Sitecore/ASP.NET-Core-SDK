using System.Text.Json;
using AutoFixture.Xunit2;
using FluentAssertions;
using GraphQL;
using GraphQL.Client.Abstractions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Exceptions;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Request;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Request.Handlers.GraphQL;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Response;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Serialization;
using Xunit;

namespace Sitecore.AspNetCore.SDK.LayoutService.Client.Tests.Request.Handlers;

public class GraphQLLayoutServiceHandlerFixture
{
    private readonly IGraphQLClient _client;
    private readonly ISitecoreLayoutSerializer _serializer;
    private readonly GraphQLLayoutServiceHandler _graphQlLayoutServiceHandler;

    public GraphQLLayoutServiceHandlerFixture()
    {
        _client = Substitute.For<IGraphQLClient>();
        _serializer = Substitute.For<ISitecoreLayoutSerializer>();
        ILogger<GraphQLLayoutServiceHandler>? logger = Substitute.For<ILogger<GraphQLLayoutServiceHandler>>();
        _graphQlLayoutServiceHandler = new GraphQLLayoutServiceHandler(_client, _serializer, logger);
    }

    [Theory]
    [AutoData]
    public async Task Request_Should_CallSendQueryAsync(string name, string apiKey, string language, string handlerName)
    {
        // Arrange
        const string json = "{\"sitecore\": {\"itemId\": \"44ce10f1-325a-4c75-b026-b68c59d86971\"}}";
        SitecoreLayoutResponseContent resultContent = new();
        LayoutQueryResponse layoutQueryResponse = new()
        {
            Layout = new LayoutModel
            {
                Item = new ItemModel
                {
                    Rendered = JsonSerializer.SerializeToElement(json)
                }
            }
        };
        GraphQLResponse<LayoutQueryResponse> response = new()
        {
            Data = layoutQueryResponse
        };

        SitecoreLayoutRequest request = [];
        request
            .SiteName(name)
            .ApiKey(apiKey);

        request.Add("sc_lang", language);

        string query = @"
                        query LayoutQuery($path: String!, $language: String!, $site: String!) {
                            layout(routePath: $path, language: $language, site: $site) {
                                item {
                                    rendered
                                }
                            }
                        }";

        _client.SendQueryAsync<LayoutQueryResponse>(Arg.Any<GraphQLRequest>()).Returns(response);

        string? data = response.Data.Layout?.Item?.Rendered.ToString();
        _serializer.Deserialize(data ?? string.Empty).Returns(resultContent);

        // Act
        await _graphQlLayoutServiceHandler.Request(request, handlerName);

        // Assert
        await _client.Received().SendQueryAsync<LayoutQueryResponse>(Arg.Is<GraphQLRequest>(r
            => r.Query!.Equals(query, StringComparison.Ordinal)));
    }

    [Theory]
    [AutoData]
    public async Task Request_Should_CallDeserialize(string name, string apiKey, string language, string handlerName)
    {
        // Arrange
        string json = " {\"sitecore\": {\"itemId\": \"44ce10f1-325a-4c75-b026-b68c59d86971\"}}";
        SitecoreLayoutResponseContent resultContent = new();
        LayoutQueryResponse layoutQueryResponse = new()
        {
            Layout = new LayoutModel
            {
                Item = new ItemModel
                {
                    Rendered = JsonSerializer.SerializeToElement(json)
                }
            }
        };
        GraphQLResponse<LayoutQueryResponse> response = new()
        {
            Data = layoutQueryResponse
        };

        SitecoreLayoutRequest request = [];
        request
            .SiteName(name)
            .ApiKey(apiKey);

        request.Add("sc_lang", language);

        _client.SendQueryAsync<LayoutQueryResponse>(Arg.Any<GraphQLRequest>()).Returns(response);
        string? data = response.Data.Layout?.Item?.Rendered.ToString();

        _serializer.Deserialize(data ?? string.Empty).Returns(resultContent);

        // Act
        await _graphQlLayoutServiceHandler.Request(request, handlerName);

        // Assert
        _serializer.Received().Deserialize(data ?? string.Empty);
    }

    [Theory]
    [AutoData]
    public async Task Request_Should_ReturnResponse(string name, string apiKey, string language, string handlerName)
    {
        // Arrange
        string json = " {\"sitecore\": {\"itemId\": \"44ce10f1-325a-4c75-b026-b68c59d86971\"}}";
        SitecoreLayoutResponseContent resultContent = new();
        LayoutQueryResponse layoutQueryResponse = new()
        {
            Layout = new LayoutModel
            {
                Item = new ItemModel
                {
                    Rendered = JsonSerializer.SerializeToElement(json)
                }
            }
        };
        GraphQLResponse<LayoutQueryResponse> response = new()
        {
            Data = layoutQueryResponse
        };

        SitecoreLayoutRequest request = [];
        request
            .SiteName(name)
            .ApiKey(apiKey);

        request.Add("sc_lang", language);

        _client.SendQueryAsync<LayoutQueryResponse>(Arg.Any<GraphQLRequest>()).Returns(response);
        string? data = response.Data.Layout?.Item?.Rendered.ToString();
        _serializer.Deserialize(data ?? string.Empty).Returns(resultContent);

        // Act
        SitecoreLayoutResponse result = await _graphQlLayoutServiceHandler.Request(request, handlerName);

        // Assert
        result.Content.Should().Be(resultContent);
    }

    [Theory]
    [InlineAutoData("en")]
    [InlineAutoData(null)]
    public async Task Request_Should_ReturnItemNotFoundSitecoreLayoutServiceClientException(string language, string name, string apiKey, string handlerName)
    {
        // Arrange
        GraphQLResponse<LayoutQueryResponse> response = new();

        SitecoreLayoutRequest request = [];
        request
            .SiteName(name)
            .ApiKey(apiKey);
        _client.SendQueryAsync<LayoutQueryResponse>(Arg.Any<GraphQLRequest>()).Returns(response);

        request.Add("sc_lang", language);

        // Act
        SitecoreLayoutResponse result = await _graphQlLayoutServiceHandler.Request(request, handlerName);

        // Assert
        result.Errors.Should().Contain(e => e is ItemNotFoundSitecoreLayoutServiceClientException);
        result.Content.Should().BeNull();
    }

    [Theory]
    [AutoData]
    public async Task Request_Should_SitecoreLayoutServiceClientException(string name, string apiKey, string language, string handlerName)
    {
        // Arrange
        const string json = " {\"sitecore\": {\"itemId\": \"44ce10f1-325a-4c75-b026-b68c59d86971\"}}";
        SitecoreLayoutResponseContent resultContent = new();
        LayoutQueryResponse layoutQueryResponse = new()
        {
            Layout = new LayoutModel
            {
                Item = new ItemModel
                {
                    Rendered = JsonSerializer.SerializeToElement(json)
                }
            }
        };
        GraphQLResponse<LayoutQueryResponse> response = new()
        {
            Errors =
            [
                new GraphQLError()
            ],
            Data = layoutQueryResponse
        };
        SitecoreLayoutRequest request = [];
        request
            .SiteName(name)
            .ApiKey(apiKey);

        request.Add("sc_lang", language);

        _client.SendQueryAsync<LayoutQueryResponse>(Arg.Any<GraphQLRequest>()).Returns(response);
        _serializer.Deserialize(Arg.Any<string>()).Returns(resultContent);

        // Act
        SitecoreLayoutResponse result = await _graphQlLayoutServiceHandler.Request(request, handlerName);

        // Assert
        result.Errors.Should().Contain(e => e != null);
        result.Content.Should().NotBeNull();
    }
}