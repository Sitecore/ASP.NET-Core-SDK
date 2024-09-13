using FluentAssertions;
using GraphQL;
using Sitecore.AspNetCore.SDK.AutoFixture.Attributes;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Request.Handlers.GraphQL;
using Xunit;

namespace Sitecore.AspNetCore.SDK.LayoutService.Client.Tests.Exceptions;

public class LayoutServiceGraphQlExceptionFixture
{
    [Theory]
    [AutoNSubstituteData]
    public void LayoutServiceGraphQlException_GraphQlError_Get(GraphQLError error)
    {
        // Arrange
        LayoutServiceGraphQlException sut = new(error);

        // Act
        GraphQLError result = sut.GraphQlError;

        // Assert
        result.Should().Be(error);
    }
}