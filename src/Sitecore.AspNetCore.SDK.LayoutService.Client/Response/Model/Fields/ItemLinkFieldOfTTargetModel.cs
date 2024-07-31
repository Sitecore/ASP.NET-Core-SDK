namespace Sitecore.AspNetCore.SDK.LayoutService.Client.Response.Model.Fields;

/// <summary>
/// Facilitates mapping a link field to a specific target model type.
/// </summary>
/// <typeparam name="TTargetModel">Target model type.</typeparam>
[System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1649:File name should match first type name", Justification = "Generic Type.")]
public class ItemLinkField<TTargetModel> : ItemLinkField
    where TTargetModel : class
{
    /// <summary>
    /// Gets or sets the strongly types target if the item link.
    /// </summary>
    public TTargetModel? Target { get; protected set; }

    /// <summary>
    /// Gets or sets the strongly typed target.
    /// </summary>
    // This property overrides the fields property in the base class and allows the JSON serializer to convert it to a strongly typed
    // TTargetModel. The deserialized class is stored in the Target property, so it makes the developer code make more sense.
    public new TTargetModel? Fields { get => Target; set => Target = value; }
}