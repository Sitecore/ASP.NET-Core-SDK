using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Localization.Routing;
using Sitecore.AspNetCore.SDK.RenderingEngine.Localization;

namespace Sitecore.AspNetCore.SDK.RenderingEngine.Extensions;

/// <summary>
/// Extensions to help configure <see cref="RequestLocalizationOptions"/>.
/// </summary>
public static class RequestLocalizationOptionsExtensions
{
    /// <summary>
    /// Adds list of <see cref="RequestCultureProvider"/> to <see cref="RequestLocalizationOptions"/>.
    /// </summary>
    /// <param name="options">The <see cref="RequestLocalizationOptions"/> to configure.</param>
    public static void UseSitecoreRequestLocalization(this RequestLocalizationOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        options.RequestCultureProviders.Insert(0, new SitecoreQueryStringCultureProvider());
        options.RequestCultureProviders.Insert(1, new RouteDataRequestCultureProvider
        {
            RouteDataStringKey = RenderingEngineConstants.SitecoreLocalization.RequestCulturePrefix,
            UIRouteDataStringKey = RenderingEngineConstants.SitecoreLocalization.RequestCulturePrefix
        });
    }
}