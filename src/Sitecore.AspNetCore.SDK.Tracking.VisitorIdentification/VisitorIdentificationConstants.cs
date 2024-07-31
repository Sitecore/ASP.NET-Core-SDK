using Sitecore.AspNetCore.SDK.Tracking.VisitorIdentification.TagHelpers;

namespace Sitecore.AspNetCore.SDK.Tracking.VisitorIdentification;

/// <summary>
/// Constants for tracking logic.
/// </summary>
internal static class VisitorIdentificationConstants
{
    /// <summary>
    /// Tag helpers section.
    /// </summary>
    internal static class TagHelpers
    {
        /// <summary>
        /// The HTML tag used by the <see cref="SitecoreVisitorIdentificationTagHelper"/> tag helper.
        /// </summary>
        internal const string VisitorIdentificationHtmlTag = "sc-visitor-identification";
    }
}