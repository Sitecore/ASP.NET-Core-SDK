namespace Sitecore.AspNetCore.SDK.LayoutService.Client.Response.Model;

/// <summary>
/// Extension methods for <see cref="Placeholder"/>.
/// </summary>
public static class PlaceholderExtensions
{
    /// <summary>
    /// Returns the <see cref="Component"/> from the specified index in the placeholder feature collection.
    /// </summary>
    /// <param name="placeholder">The placeholder feature collection.</param>
    /// <param name="index">The index of the component to be returned.</param>
    /// <returns>A <see cref="Component"/>.</returns>
    public static Component? ComponentAt(this Placeholder placeholder, int index)
    {
        ArgumentNullException.ThrowIfNull(placeholder);
        return placeholder.FeatureAt<Component>(index);
    }

    /// <summary>
    /// Returns the <see cref="EditableChrome"/> from the specified index in the placeholder feature collection.
    /// </summary>
    /// <param name="placeholder">The placeholder feature collection.</param>
    /// <param name="index">The index of the chrome to be returned.</param>
    /// <returns>An <see cref="EditableChrome"/>.</returns>
    public static EditableChrome? ChromeAt(this Placeholder placeholder, int index)
    {
        ArgumentNullException.ThrowIfNull(placeholder);
        return placeholder.FeatureAt<EditableChrome>(index);
    }

    /// <summary>
    /// Returns the <see cref="IPlaceholderFeature"/> from the specified index in the placeholder feature collection.
    /// </summary>
    /// <param name="placeholder">The placeholder feature collection.</param>
    /// <param name="index">The index of the feature to be returned.</param>
    /// <typeparam name="T">The type of the placeholder feature.</typeparam>
    /// <returns>An <see cref="IPlaceholderFeature"/>.</returns>
    private static T? FeatureAt<T>(this Placeholder placeholder, int index)
        where T : class, IPlaceholderFeature
    {
        ArgumentNullException.ThrowIfNull(placeholder);

        IPlaceholderFeature feature = placeholder.ElementAt(index);
        if (feature is T placeholderFeature)
        {
            return placeholderFeature;
        }

        return default;
    }
}