using GraphQL;

namespace Sitecore.AspNetCore.SDK.Pages.Models
{
    /// <summary>
    /// The model used to store the args passed in to the Render route when using Pages MetaData Editing mode.
    /// </summary>
    public class PagesRenderArgs
    {
        /// <summary>
        /// Gets or sets the Id of the item being edited.
        /// </summary>
        public Guid ItemId { get; set; }

        /// <summary>
        /// Gets or sets the lanugage of the item being edited.
        /// </summary>
        public string Language { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the name of the site being edited.
        /// </summary>
        public string Site { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the Version of the item being edited.
        /// </summary>
        public int Version { get; set; } = 1;

        /// <summary>
        /// Gets or sets the Layout kind of item being edited.
        /// </summary>
        public string LayoutKind { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the mode that the redering is running.
        /// </summary>
        public string Mode { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the Editing Secret.
        /// </summary>
        public string EditingSecret { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the route to the item within the site.
        /// </summary>
        public string Route { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the ID of the tenant that editing is being used on.
        /// </summary>
        public string TenantId { get; set; } = string.Empty;
    }
}
