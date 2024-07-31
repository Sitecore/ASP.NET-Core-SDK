using Sitecore.AspNetCore.SDK.LayoutService.Client.Properties;

namespace Sitecore.AspNetCore.SDK.LayoutService.Client.Exceptions;

/// <summary>
/// Details an exception that may occur when the Sitecore layout service returns a server related error.
/// </summary>
public class SitecoreLayoutServiceServerException : InvalidResponseSitecoreLayoutServiceClientException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SitecoreLayoutServiceServerException"/> class.
    /// </summary>
    /// <param name="message">The exception message.</param>
    public SitecoreLayoutServiceServerException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SitecoreLayoutServiceServerException"/> class.
    /// </summary>
    /// <param name="message">The exception message.</param>
    /// <param name="innerException">The inner exception to be wrapped.</param>
    public SitecoreLayoutServiceServerException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SitecoreLayoutServiceServerException"/> class.
    /// </summary>
    public SitecoreLayoutServiceServerException()
        : base(Resources.Exception_LayoutServiceServerError)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SitecoreLayoutServiceServerException"/> class.
    /// </summary>
    /// <param name="innerException">The inner exception to be wrapped.</param>
    public SitecoreLayoutServiceServerException(Exception innerException)
        : this(Resources.Exception_LayoutServiceServerError, innerException)
    {
    }
}