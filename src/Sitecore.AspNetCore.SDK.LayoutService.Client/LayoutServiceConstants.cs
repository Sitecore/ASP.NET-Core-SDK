namespace Sitecore.AspNetCore.SDK.LayoutService.Client;

/// <summary>
/// Constants of the Layout Service Client.
/// </summary>
[System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1649:File name should match first type name", Justification = "Multi layered constants for easy use.")]
public static class LayoutServiceClientConstants
{
    /// <summary>
    /// Constants relevant to Sitecore layout service response chromes.
    /// </summary>
    public static class SitecoreChromes
    {
        /// <summary>
        /// The name of the chrome type attribute.
        /// </summary>
        public const string ChromeTypeName = "type";

        /// <summary>
        /// The value of the chrome type attribute.
        /// </summary>
        public const string ChromeTypeValue = "text/sitecore";

        /// <summary>
        /// The default chrome HTML tag.
        /// </summary>
        public const string ChromeTag = "code";
    }

    /// <summary>
    /// Constants relevant to Serialization.
    /// </summary>
    public static class Serialization
    {
        /// <summary>
        /// The name of the SitecoreData property.
        /// </summary>
        public const string SitecoreDataPropertyName = "sitecore";

        /// <summary>
        /// The name of the Context property.
        /// </summary>
        public const string ContextPropertyName = "context";
    }
}