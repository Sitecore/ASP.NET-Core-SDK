using System.Diagnostics.CodeAnalysis;
using AutoFixture;
using FluentAssertions;
using GraphQL.Client.Abstractions;
using GraphQL.Client.Http;
using NSubstitute;
using Sitecore.AspNetCore.SDK.AutoFixture.Attributes;
using Sitecore.AspNetCore.SDK.GraphQL.Request;
using Xunit;

namespace Sitecore.AspNetCore.SDK.GraphQL.Tests.Request;

public class GraphQLHttpRequestWithHeadersFixture
{
    [ExcludeFromCodeCoverage]
    public static Action<IFixture> AutoSetup => f =>
    {
        GraphQLHttpClientOptions options = Substitute.For<GraphQLHttpClientOptions>();
        f.Inject(options);

        IGraphQLJsonSerializer serializer = Substitute.For<IGraphQLJsonSerializer>();
        f.Inject(serializer);
    };

    [Theory]
    [AutoNSubstituteData]
    public void ToHttpRequestMessage_HeadersAreAddedToReturnedMessage(GraphQLHttpRequestWithHeaders sut, GraphQLHttpClientOptions options, IGraphQLJsonSerializer serializer)
    {
        // Arrange
        sut.Headers = new Dictionary<string, string>
        {
            { "key1", "value1" },
            { "key2", "value2" }
        };

        // Act
        HttpRequestMessage result = sut.ToHttpRequestMessage(options, serializer);

        // Assert
        result.Should().NotBeNull();
        result.Headers.GetValues("key1").Should().Contain("value1");
        result.Headers.GetValues("key2").Should().Contain("value2");
    }
}
