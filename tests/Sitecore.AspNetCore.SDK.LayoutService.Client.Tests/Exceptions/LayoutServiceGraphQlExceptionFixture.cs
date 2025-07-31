using AwesomeAssertions;
using GraphQL;
using Sitecore.AspNetCore.SDK.AutoFixture.Attributes;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Request.Handlers.GraphQL;
using Xunit;

namespace Sitecore.AspNetCore.SDK.LayoutService.Client.Tests.Exceptions;

public class LayoutServiceGraphQLExceptionFixture
{
    [Theory]
    [AutoNSubstituteData]
    public void LayoutServiceGraphQLException_GraphQLError_Get(GraphQLError error)
    {
        // Arrange
        LayoutServiceGraphQLException sut = new(error);

        // Act
        GraphQLError result = sut.GraphQLError;

        // Assert
        result.Should().Be(error);
    }
}