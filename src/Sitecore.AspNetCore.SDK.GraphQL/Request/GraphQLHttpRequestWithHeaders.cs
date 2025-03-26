using GraphQL.Client.Abstractions;
using GraphQL.Client.Http;

namespace Sitecore.AspNetCore.SDK.GraphQL.Request
{
    /// <summary>
    /// GraphQLHttpRequestWithHeaders is a class that extends GraphQLHttpRequest. It is designed to handle GraphQL HTTP
    /// requests with additional header support.
    /// </summary>
    public class GraphQLHttpRequestWithHeaders : GraphQLHttpRequest
    {
        /// <summary>
        /// Gets or sets a dictionary that stores key-value pairs of headers, where both keys and values are strings. It allows for
        /// easy access and manipulation of header information.
        /// </summary>
        public Dictionary<string, string> Headers { get; set; } = [];

        /// <summary>
        /// Converts the current instance into an HTTP request message for GraphQL operations.
        /// </summary>
        /// <param name="options">Specifies configuration options for the GraphQL HTTP client.</param>
        /// <param name="serializer">Defines the method for serializing GraphQL requests and responses.</param>
        /// <returns>Returns an HTTP request message with the necessary headers for the GraphQL operation.</returns>
        public override HttpRequestMessage ToHttpRequestMessage(GraphQLHttpClientOptions options, IGraphQLJsonSerializer serializer)
        {
            HttpRequestMessage request = base.ToHttpRequestMessage(options, serializer);

            foreach (string headerKey in Headers.Keys)
            {
                request.Headers.Add(headerKey, Headers[headerKey]);
            }

            return request;
        }
    }
}
