namespace Sitecore.AspNetCore.SDK.RenderingEngine.Middleware.Models;

/// <summary>
/// POCO class to deserialize the personalize query response.
/// </summary>
public class PersonalizeQueryResult
{
    /// <summary>
    /// Gets or sets the layout deserialized information.
    /// </summary>
    public LayoutModel? Layout { get; set; }

    /// <summary>
    /// Small intermediary model to match query response format.
    /// </summary>
    public class LayoutModel
    {
        /// <summary>
        /// Gets or sets the item deserialized information.
        /// </summary>
        public PersonalizeInfo? Item { get; set; }
    }
}