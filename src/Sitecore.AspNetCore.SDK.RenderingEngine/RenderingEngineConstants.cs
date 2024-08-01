using Microsoft.AspNetCore.Mvc.TagHelpers;
using Sitecore.AspNetCore.SDK.RenderingEngine.TagHelpers;

namespace Sitecore.AspNetCore.SDK.RenderingEngine;

/// <summary>
/// Various constants relevant to the Sitecore Rendering Engine.
/// </summary>
public static class RenderingEngineConstants
{
    /// <summary>
    /// The name of the default partial view used when a requested component cannot be found.
    /// </summary>
    public const string DefaultMissingComponentPartialViewName = "_ComponentNotFound";

    /// <summary>
    /// Constants relevant to the Sitecore tag helpers.
    /// </summary>
    public static class SitecoreTagHelpers
    {
        /// <summary>
        /// The HTML tag used by the <see cref="TagHelpers.Fields.RichTextTagHelper"/> tag helper.
        /// </summary>
        public const string RichTextHtmlTag = "sc-text";

        /// <summary>
        /// The HTML tag used by the <see cref="LinkTagHelper"/> tag helper.
        /// </summary>
        public const string LinkHtmlTag = "sc-link";

        /// <summary>
        /// The HTML tag used by the <see cref="TagHelpers.Fields.DateTagHelper"/> tag helper.
        /// </summary>
        public const string DateHtmlTag = "sc-date";

        /// <summary>
        /// The HTML tag used by the <see cref="TagHelpers.Fields.NumberTagHelper"/> tag helper.
        /// </summary>
        public const string NumberHtmlTag = "sc-number";

        /// <summary>
        /// The HTML tag used by the <see cref="ImageTagHelper"/> tag helper.
        /// </summary>
        public const string ImageHtmlTag = "sc-img";

        /// <summary>
        /// The HTML tag used by the <see cref="TagHelpers.Fields.FileTagHelper"/> tag helper.
        /// </summary>
        public const string FileHtmlTag = "sc-file";

        /// <summary>
        /// The name of the asp-for attribute.
        /// </summary>
        public const string AspForTagHelperAttribute = "asp-for";

        /// <summary>
        /// The HTML tag used by the <see cref="PlaceholderTagHelper"/> tag helper.
        /// </summary>
        public const string PlaceholderHtmlTag = "sc-placeholder";

        /// <summary>
        /// The name of the rich-text attribute.
        /// </summary>
        public const string TextTagHelperAttribute = "asp-text";

        /// <summary>
        /// The name of the image attribute.
        /// </summary>
        public const string ImageTagHelperAttribute = "asp-image";

        /// <summary>
        /// The name of the date attribute.
        /// </summary>
        public const string DateTagHelperAttribute = "asp-date";

        /// <summary>
        /// The name of the number attribute.
        /// </summary>
        public const string NumberTagHelperAttribute = "asp-number";

        /// <summary>
        /// The name of the link attribute.
        /// </summary>
        public const string LinkTagHelperAttribute = "asp-link";

        /// <summary>
        /// The name of the file attribute.
        /// </summary>
        public const string FileTagHelperAttribute = "asp-file";
    }

    /// <summary>
    /// Constants relevant to Sitecore view components.
    /// </summary>
    public static class SitecoreViewComponents
    {
        /// <summary>
        /// The name of the default Sitecore View Component.
        /// </summary>
        public const string DefaultSitecoreViewComponentName = "SitecoreComponent";
    }

    /// <summary>
    /// Constants relevant to Sitecore localization functionality.
    /// </summary>
    public static class SitecoreLocalization
    {
        /// <summary>
        /// The name of request culture prefix in the route.
        /// </summary>
        public const string RequestCulturePrefix = "culture";
    }

    /// <summary>
    /// Constants relevant to Request Route Values.
    /// </summary>
    public static class RouteValues
    {
        /// <summary>
        /// The name of sitecoreRoute  the request.
        /// </summary>
        public const string SitecoreRoute = "sitecoreRoute";
    }
}