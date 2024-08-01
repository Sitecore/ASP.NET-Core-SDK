using System.Collections.ObjectModel;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Exceptions;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Request;

namespace Sitecore.AspNetCore.SDK.LayoutService.Client.Response;

/// <summary>
/// Models a result from calling the Sitecore layout service.
/// </summary>
public class SitecoreLayoutResponse
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SitecoreLayoutResponse"/> class.
    /// </summary>
    /// /// <param name="request">The <see cref="SitecoreLayoutRequest"/> object.</param>
    /// /// <param name="errors">The list of <see cref="SitecoreLayoutServiceClientException"/> objects.</param>
    public SitecoreLayoutResponse(SitecoreLayoutRequest request, List<SitecoreLayoutServiceClientException> errors)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(errors);

        Request = request;
        Errors = new ReadOnlyCollection<SitecoreLayoutServiceClientException>(errors);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SitecoreLayoutResponse"/> class.
    /// </summary>
    /// <param name="request">The <see cref="SitecoreLayoutRequest"/> object.</param>
    public SitecoreLayoutResponse(SitecoreLayoutRequest request)
        : this(request, [])
    {
    }

    /// <summary>
    /// Gets or sets the metadata of the response.
    /// </summary>
    public ILookup<string, string>? Metadata { get; set; }

    /// <summary>
    /// Gets or sets the content of the response.
    /// </summary>
    public SitecoreLayoutResponseContent? Content { get; set; }

    /// <summary>
    /// Gets a value indicating whether the response has errors.
    /// </summary>
    public bool HasErrors => Errors.Count != 0;

    /// <summary>
    /// Gets the list of errors returned by the Sitecore layout service response.
    /// </summary>
    public IReadOnlyCollection<SitecoreLayoutServiceClientException> Errors { get; }

    /// <summary>
    /// Gets the original request.
    /// </summary>
    public SitecoreLayoutRequest Request { get; }
}