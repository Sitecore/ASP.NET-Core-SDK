using Sitecore.AspNetCore.SDK.LayoutService.Client.Properties;

namespace Sitecore.AspNetCore.SDK.LayoutService.Client.Exceptions;

/// <summary>
/// Details an exception that may occur when the Sitecore layout service cannot be contacted.
/// </summary>
public class CouldNotContactSitecoreLayoutServiceClientException : SitecoreLayoutServiceClientException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CouldNotContactSitecoreLayoutServiceClientException"/> class.
    /// </summary>
    /// <param name="message">The exception message.</param>
    public CouldNotContactSitecoreLayoutServiceClientException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CouldNotContactSitecoreLayoutServiceClientException"/> class.
    /// </summary>
    /// <param name="message">The exception message.</param>
    /// <param name="innerException">The inner exception to be wrapped.</param>
    public CouldNotContactSitecoreLayoutServiceClientException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CouldNotContactSitecoreLayoutServiceClientException"/> class.
    /// </summary>
    public CouldNotContactSitecoreLayoutServiceClientException()
        : base(Resources.Exception_CouldNotContactService)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CouldNotContactSitecoreLayoutServiceClientException"/> class.
    /// </summary>
    /// <param name="innerException">The inner exception to be wrapped.</param>
    public CouldNotContactSitecoreLayoutServiceClientException(Exception innerException)
        : this(Resources.Exception_CouldNotContactService, innerException)
    {
    }
}