using Microsoft.AspNetCore.Http;

namespace Sitecore.AspNetCore.SDK.RenderingEngine.Configuration;

/// <summary>
/// Options to configure headers forwarding.
/// </summary>
public class ForwardHeadersOptions
{
    /// <summary>
    /// Gets whitelist of headers allowed to be transferred to Layout Service request.
    /// </summary>
    public HashSet<string> HeadersWhitelist { get; } = [];

    /// <summary>
    /// Gets filters, which will be applied sequentially during copying of headers from request to Layout service request.
    /// </summary>
    public IList<Action<HttpRequest, IDictionary<string, string[]>>> RequestHeadersFilters { get; } = [];

    /// <summary>
    /// Gets filters, which will be applied sequentially during copying of headers from Layout service response to response sent by rendering host.
    /// </summary>
    public IList<Action<ILookup<string, string>, IDictionary<string, string[]>>> ResponseHeadersFilters { get; } = [];

    /// <summary>
    /// Gets or sets the XForwardedProto header key to push the request schema under for the request when forwarding.
    /// </summary>
    public string XForwardedProtoHeader { get; set; } = "X-Forwarded-Proto";
}