using GraphQL.Client.Abstractions;

namespace Sitecore.AspNetCore.SDK.Pages.GraphQL;

/// <summary>
/// Interface used to define the contract that IGraphQlClientFactories need to adhere to.
/// </summary>
public interface IGraphQLClientFactory
{
    /// <summary>
    /// Method used to generate an instance of <see cref="IGraphQLClient" />.
    /// </summary>
    /// <param name="uri">GraphQl endpoint uri.</param>
    /// <param name="layoutKind">The layout type for this request, shared or final.</param>
    /// <param name="editMode">The edit mode version for this client.</param>
    /// <returns>Concrete implementation of <see cref="IGraphQLClient" /> interface.</returns>
    public IGraphQLClient GenerateClient(Uri? uri, string layoutKind, string editMode);

    /// <summary>
    /// Method used to generate an instance of <see cref="IGraphQLClient" />.
    /// </summary>
    /// <param name="layoutKind">The layout type for this request, shared or final.</param>
    /// <param name="editMode">The edit mode version for this client.</param>
    /// <returns>Concrete implementation of <see cref="IGraphQLClient" /> interface.</returns>
    public IGraphQLClient GenerateClient(string layoutKind, string editMode);
}
