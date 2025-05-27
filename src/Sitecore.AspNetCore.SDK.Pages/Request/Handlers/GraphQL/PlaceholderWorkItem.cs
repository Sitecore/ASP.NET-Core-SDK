using Sitecore.AspNetCore.SDK.LayoutService.Client.Response.Model;

namespace Sitecore.AspNetCore.SDK.Pages.Request.Handlers.GraphQL;

/// <summary>
/// Represents a work item with associated features and output, optionally linked to a parent component.
/// </summary>
/// <param name="placeholderKey">Specifies the name of the work item.</param>
/// <param name="id">Defines a unique identifier for the work item.</param>
/// <param name="features">Holds the features associated with the work item.</param>
/// <param name="output">Contains the output related to the work item.</param>
/// <param name="parentComponent">Links to a parent component if applicable.</param>
public class PlaceholderWorkItem(
    string placeholderKey,
    string id,
    Placeholder features,
    Placeholder output,
    Component? parentComponent = null)
{
    /// <summary>
    /// Gets the PlaceholderKey of an entity. It is a read-only property initialized with the value of the 'PlaceholderKey' variable.
    /// </summary>
    public string PlaceholderKey { get; } = placeholderKey;

    /// <summary>
    /// Gets the unique identifier as a read-only string property. It is initialized with the value of 'id'.
    /// </summary>
    public string Id { get; } = id;

    /// <summary>
    /// Gets the collection of features. It is a read-only property that initializes with the value of 'features'.
    /// </summary>
    public Placeholder Features { get; } = features;

    /// <summary>
    /// Gets the output placeholder. It is a read-only property initialized with the value of 'output'.
    /// </summary>
    public Placeholder Output { get; } = output;

    /// <summary>
    /// Gets the parent component of the current component. It is a read-only property initialized with the provided parentComponent.
    /// </summary>
    public Component? ParentComponent { get; } = parentComponent;
}
