namespace Sitecore.AspNetCore.SDK.GraphQL.Exceptions;

/// <summary>
///  Details an exception that may occur during GraphQL configuration.
/// </summary>
public class InvalidGraphQLConfigurationException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidGraphQLConfigurationException"/> class.
    /// </summary>
    public InvalidGraphQLConfigurationException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidGraphQLConfigurationException"/> class.
    /// </summary>
    /// <param name="message">The exception message.</param>
    public InvalidGraphQLConfigurationException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidGraphQLConfigurationException"/> class.
    /// </summary>
    /// <param name="message">The exception message.</param>
    /// <param name="innerException">The inner exception to be wrapped.</param>
    public InvalidGraphQLConfigurationException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}