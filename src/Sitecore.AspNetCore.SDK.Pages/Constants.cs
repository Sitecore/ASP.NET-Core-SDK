namespace Sitecore.AspNetCore.SDK.Pages
{
    /// <summary>
    /// Class used to stored constants referenced throughout the Pages project.
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// Class used to hold the names of different Layout Clients used by the Pages project.
        /// </summary>
        public static class LayoutClients
        {
            /// <summary>
            /// Name of the Pages Editing Layout Client.
            /// </summary>
            public const string Pages = "pages";
        }

        /// <summary>
        /// Class used to hold the names of the different tag helpers defined in the Pages project.
        /// </summary>
        public static class SitecoreTagHelpers
        {
            /// <summary>
            /// The HTML tag used to render the Editing Scripts tag helper.
            /// </summary>
            public const string EditScriptsHtmlTag = "sc-editingscripts";
        }

        /// <summary>
        /// Const values for query string keys.
        /// </summary>
        public static class QueryStringKeys
        {
            /// <summary>
            /// Mode query string key.
            /// </summary>
            public const string Mode = "mode";

            /// <summary>
            /// Item id query string key.
            /// </summary>
            public const string ItemId = "sc_itemid";

            /// <summary>
            /// Secret query string key.
            /// </summary>
            public const string Secret = "secret";

            /// <summary>
            /// Language query string key.
            /// </summary>
            public const string Language = "sc_lang";

            /// <summary>
            /// Layout kind query string key.
            /// </summary>
            public const string LayoutKind = "sc_layoutKind";

            /// <summary>
            /// Route query string key.
            /// </summary>
            public const string Route = "route";

            /// <summary>
            /// Site query string key.
            /// </summary>
            public const string Site = "sc_site";

            /// <summary>
            /// Version query string key.
            /// </summary>
            public const string Version = "sc_version";

            /// <summary>
            /// Tenant id query string key.
            /// </summary>
            public const string TenantId = "tenant_id";

            /// <summary>
            /// Sitecore Edit mode query string key.
            /// </summary>
            public const string EditMode = "sc_editmode";
        }
    }
}
