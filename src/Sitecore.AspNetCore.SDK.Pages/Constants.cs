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
    }
}
