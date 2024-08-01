using System.Globalization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Sitecore.AspNetCore.SDK.RenderingEngine.Routing;

/// <inheritdoc />
public class LanguageRouteConstraint : IRouteConstraint
{
    /// <inheritdoc />
    public bool Match(HttpContext? httpContext, IRouter? route, string routeKey, RouteValueDictionary values, RouteDirection routeDirection)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(routeKey);
        ArgumentNullException.ThrowIfNull(values);
        ArgumentNullException.ThrowIfNull(routeDirection);

        if (!values.TryGetValue("culture", out object? value))
        {
            return false;
        }

        string? lang = value?.ToString();
        CultureInfo? culture = null;
        if (!string.IsNullOrEmpty(lang))
        {
            culture = CultureInfo.GetCultures(CultureTypes.AllCultures)
                .SingleOrDefault(c => c.IetfLanguageTag.Equals(lang, StringComparison.InvariantCultureIgnoreCase));
        }

        return culture != null;
    }
}