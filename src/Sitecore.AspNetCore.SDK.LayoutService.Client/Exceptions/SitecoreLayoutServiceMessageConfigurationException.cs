using Sitecore.AspNetCore.SDK.LayoutService.Client.Properties;

namespace Sitecore.AspNetCore.SDK.LayoutService.Client.Exceptions;

/// <summary>
/// Details an exception that may occur when invalid configuration is applied to the message sent to the Sitecore layout service.
/// </summary>
public class SitecoreLayoutServiceMessageConfigurationException : SitecoreLayoutServiceClientException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SitecoreLayoutServiceMessageConfigurationException"/> class.
    /// </summary>
    /// <param name="message">The exception message.</param>
    public SitecoreLayoutServiceMessageConfigurationException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SitecoreLayoutServiceMessageConfigurationException"/> class.
    /// </summary>
    /// <param name="message">The exception message.</param>
    /// <param name="innerException">The inner exception to be wrapped.</param>
    public SitecoreLayoutServiceMessageConfigurationException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SitecoreLayoutServiceMessageConfigurationException"/> class.
    /// </summary>
    public SitecoreLayoutServiceMessageConfigurationException()
        : base(Resources.Exception_MessageConfigurationError)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SitecoreLayoutServiceMessageConfigurationException"/> class.
    /// </summary>
    /// <param name="innerException">The inner exception to be wrapped.</param>
    public SitecoreLayoutServiceMessageConfigurationException(Exception innerException)
        : this(Resources.Exception_MessageConfigurationError, innerException)
    {
    }
}