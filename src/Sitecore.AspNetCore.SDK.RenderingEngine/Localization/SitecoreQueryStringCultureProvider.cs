using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Primitives;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Request;

namespace Sitecore.AspNetCore.SDK.RenderingEngine.Localization;

/// <inheritdoc />
public class SitecoreQueryStringCultureProvider : QueryStringRequestCultureProvider
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SitecoreQueryStringCultureProvider"/> class.
    /// </summary>
    public SitecoreQueryStringCultureProvider()
    {
        QueryStringKey = RequestKeys.Language;
        UIQueryStringKey = RequestKeys.Language;
    }

    /// <inheritdoc />
    public override async Task<ProviderCultureResult?> DetermineProviderCultureResult(HttpContext httpContext)
    {
        ArgumentNullException.ThrowIfNull(httpContext);

        ProviderCultureResult? cultureResult = await base.DetermineProviderCultureResult(httpContext).ConfigureAwait(false);
        if (cultureResult != null)
        {
            return cultureResult;
        }

        string? cookie = httpContext.Request.Cookies[RequestKeys.Language];

        if (string.IsNullOrEmpty(cookie))
        {
            return await NullProviderCultureResult.ConfigureAwait(false);
        }

        return Task.FromResult(new ProviderCultureResult(new StringSegment(cookie))).Result;
    }
}