using GraphQL;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Exceptions;

namespace Sitecore.AspNetCore.SDK.LayoutService.Client.Request.Handlers.GraphQL;

/// <inheritdoc />
/// <summary>
/// Initializes a new instance of the <see cref="LayoutServiceGraphQlException"/> class.
/// </summary>
/// <param name="error">GraphQL Error of a GraphQL Query.</param>
public class LayoutServiceGraphQlException(GraphQLError error)
    : SitecoreLayoutServiceClientException(error.Message)
{
    /// <summary>
    /// Gets GraphQL Error of a GraphQL Query.
    /// </summary>
    public GraphQLError GraphQlError { get; } = error ?? throw new ArgumentNullException(nameof(error));
}