namespace Sitecore.AspNetCore.SDK.GraphQL.Exceptions;

/// <summary>
///  Details an exception that may occur during GraphQl configuration.
/// </summary>
public class InvalidGraphQlConfigurationException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidGraphQlConfigurationException"/> class.
    /// </summary>
    public InvalidGraphQlConfigurationException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidGraphQlConfigurationException"/> class.
    /// </summary>
    /// <param name="message">The exception message.</param>
    public InvalidGraphQlConfigurationException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidGraphQlConfigurationException"/> class.
    /// </summary>
    /// <param name="message">The exception message.</param>
    /// <param name="innerException">The inner exception to be wrapped.</param>
    public InvalidGraphQlConfigurationException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}