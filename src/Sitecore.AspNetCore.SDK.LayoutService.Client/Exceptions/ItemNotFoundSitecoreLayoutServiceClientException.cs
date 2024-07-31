using Sitecore.AspNetCore.SDK.LayoutService.Client.Properties;

namespace Sitecore.AspNetCore.SDK.LayoutService.Client.Exceptions;

/// <summary>
/// Details an exception that may occur when the Sitecore layout service returns a 'not found' (404) response.
/// </summary>
public class ItemNotFoundSitecoreLayoutServiceClientException : SitecoreLayoutServiceClientException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ItemNotFoundSitecoreLayoutServiceClientException"/> class.
    /// </summary>
    /// <param name="message">The exception message.</param>
    public ItemNotFoundSitecoreLayoutServiceClientException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ItemNotFoundSitecoreLayoutServiceClientException"/> class.
    /// </summary>
    /// <param name="message">The exception message.</param>
    /// <param name="innerException">The inner exception to be wrapped.</param>
    public ItemNotFoundSitecoreLayoutServiceClientException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ItemNotFoundSitecoreLayoutServiceClientException"/> class.
    /// </summary>
    public ItemNotFoundSitecoreLayoutServiceClientException()
        : base(Resources.Exception_ItemNotFoundError)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ItemNotFoundSitecoreLayoutServiceClientException"/> class.
    /// </summary>
    /// <param name="innerException">The inner exception to be wrapped.</param>
    public ItemNotFoundSitecoreLayoutServiceClientException(Exception innerException)
        : this(Resources.Exception_ItemNotFoundError, innerException)
    {
    }
}