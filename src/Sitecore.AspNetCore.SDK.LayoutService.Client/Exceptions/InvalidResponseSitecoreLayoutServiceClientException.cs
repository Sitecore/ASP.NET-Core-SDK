using Sitecore.AspNetCore.SDK.LayoutService.Client.Properties;

namespace Sitecore.AspNetCore.SDK.LayoutService.Client.Exceptions;

/// <summary>
/// Details an exception that may occur when the Sitecore layout service returns an invalid response.
/// </summary>
public class InvalidResponseSitecoreLayoutServiceClientException : SitecoreLayoutServiceClientException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidResponseSitecoreLayoutServiceClientException"/> class.
    /// </summary>
    /// <param name="message">The exception message.</param>
    public InvalidResponseSitecoreLayoutServiceClientException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidResponseSitecoreLayoutServiceClientException"/> class.
    /// </summary>
    /// <param name="message">The exception message.</param>
    /// <param name="innerException">The inner exception to be wrapped.</param>
    public InvalidResponseSitecoreLayoutServiceClientException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidResponseSitecoreLayoutServiceClientException"/> class.
    /// </summary>
    public InvalidResponseSitecoreLayoutServiceClientException()
        : base(Resources.Exception_InvalidResponseFormat)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidResponseSitecoreLayoutServiceClientException"/> class.
    /// </summary>
    /// <param name="innerException">The inner exception to be wrapped.</param>
    public InvalidResponseSitecoreLayoutServiceClientException(Exception innerException)
        : this(Resources.Exception_InvalidResponseFormat, innerException)
    {
    }
}