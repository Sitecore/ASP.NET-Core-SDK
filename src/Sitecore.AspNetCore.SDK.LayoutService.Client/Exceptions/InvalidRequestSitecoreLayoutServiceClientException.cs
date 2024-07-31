using Sitecore.AspNetCore.SDK.LayoutService.Client.Properties;

namespace Sitecore.AspNetCore.SDK.LayoutService.Client.Exceptions;

/// <summary>
/// Details an exception that may occur when the Sitecore layout service is invoked with an invalid request.
/// </summary>
public class InvalidRequestSitecoreLayoutServiceClientException : SitecoreLayoutServiceClientException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidRequestSitecoreLayoutServiceClientException"/> class.
    /// </summary>
    /// <param name="message">The exception message.</param>
    public InvalidRequestSitecoreLayoutServiceClientException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidRequestSitecoreLayoutServiceClientException"/> class.
    /// </summary>
    /// <param name="message">The exception message.</param>
    /// <param name="innerException">The inner exception to be wrapped.</param>
    public InvalidRequestSitecoreLayoutServiceClientException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidRequestSitecoreLayoutServiceClientException"/> class.
    /// </summary>
    public InvalidRequestSitecoreLayoutServiceClientException()
        : base(Resources.Exception_InvalidRequestError)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidRequestSitecoreLayoutServiceClientException"/> class.
    /// </summary>
    /// <param name="innerException">The inner exception to be wrapped.</param>
    public InvalidRequestSitecoreLayoutServiceClientException(Exception innerException)
        : this(Resources.Exception_InvalidRequestError, innerException)
    {
    }
}