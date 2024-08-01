using Sitecore.AspNetCore.SDK.LayoutService.Client.Properties;

namespace Sitecore.AspNetCore.SDK.LayoutService.Client.Exceptions;

/// <summary>
/// Details an exception that may occur when communicating with the Sitecore layout service.
/// </summary>
public class SitecoreLayoutServiceClientException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SitecoreLayoutServiceClientException"/> class.
    /// </summary>
    /// <param name="message">The exception message.</param>
    public SitecoreLayoutServiceClientException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SitecoreLayoutServiceClientException"/> class.
    /// </summary>
    /// <param name="message">The exception message.</param>
    /// <param name="innerException">The inner exception to be wrapped.</param>
    public SitecoreLayoutServiceClientException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SitecoreLayoutServiceClientException"/> class.
    /// </summary>
    public SitecoreLayoutServiceClientException()
        : this(Resources.Exception_GeneralServiceError)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SitecoreLayoutServiceClientException"/> class.
    /// </summary>
    /// <param name="innerException">The inner exception to be wrapped.</param>
    public SitecoreLayoutServiceClientException(Exception innerException)
        : this(Resources.Exception_GeneralServiceError, innerException)
    {
    }
}