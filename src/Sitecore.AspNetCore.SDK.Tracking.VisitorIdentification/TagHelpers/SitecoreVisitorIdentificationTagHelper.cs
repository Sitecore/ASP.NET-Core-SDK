using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Sitecore.AspNetCore.SDK.Tracking.VisitorIdentification.Providers;

// ReSharper disable StringLiteralTypo
namespace Sitecore.AspNetCore.SDK.Tracking.VisitorIdentification.TagHelpers;

/// <summary>
/// This tag helper renders necessary artifacts for Visitor identification.
/// </summary>
[HtmlTargetElement(VisitorIdentificationConstants.TagHelpers.VisitorIdentificationHtmlTag)]
public class SitecoreVisitorIdentificationTagHelper : TagHelper
{
    private const string CookieName = "SC_ANALYTICS_GLOBAL_COOKIE";

    /// <summary>
    /// Gets or sets view context.
    /// </summary>
    [ViewContext]
    public ViewContext? ViewContext { get; set; }

    /// <inheritdoc />
    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(output);

        IOptions<SitecoreVisitorIdentificationOptions>? options = ViewContext?.HttpContext.RequestServices.GetService<IOptions<SitecoreVisitorIdentificationOptions>>();
        IDateTimeProvider? dateTimeProvider = ViewContext?.HttpContext.RequestServices.GetService<IDateTimeProvider>();

        if (options == null || options.Value.SitecoreInstanceUri == null)
        {
            output.SuppressOutput();
            return;
        }

        bool isVisitorClassified = IsVisitorIdentified();

        if (!isVisitorClassified)
        {
            output.TagName = string.Empty;
            output.Content.AppendHtml($"<meta name=\"VIcurrentDateTime\" content=\"{dateTimeProvider?.GetUtcNow().Ticks}>\"/>");
            output.Content.AppendHtml("<meta name=\"VirtualFolder\" content=\"\\\"/>");
            output.Content.AppendHtml("<script type='text/javascript' src='/layouts/system/VisitorIdentification.js'></script>");
            return;
        }

        output.SuppressOutput();
    }

    private bool IsVisitorIdentifiedInRequestCookie()
    {
        return ViewContext!.HttpContext.Request.Cookies.Any(c => c.Key == CookieName && c.Value.Contains("true", StringComparison.OrdinalIgnoreCase));
    }

    private bool IsVisitorIdentified()
    {
        string? cookieStr = ViewContext!.HttpContext.Response.Headers.SetCookie.FirstOrDefault(c => c != null && c.Contains(CookieName, StringComparison.OrdinalIgnoreCase));

        if (cookieStr == null)
        {
            return IsVisitorIdentifiedInRequestCookie();
        }

        if (cookieStr.Contains("True", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        return false;
    }
}