namespace Sitecore.AspNetCore.SDK.LayoutService.Client.Extensions;

/// <summary>
/// Extension methods to convert dictionary collection to string format.
/// </summary>
internal static class DictionaryExtensions
{
    /// <summary>
    /// Converts dictionary collection to string format.
    /// </summary>
    /// <typeparam name="TKey">The key of the dictionary.</typeparam>
    /// <typeparam name="TValue">The value of the dictionary.</typeparam>
    /// <param name="dictionary">The dictionary being configured.</param>
    /// <returns>The configured <see cref="string"/>.</returns>
    public static string ToDebugString<TKey, TValue>(this IDictionary<TKey, TValue> dictionary)
    {
        ArgumentNullException.ThrowIfNull(dictionary);
        return "{" + string.Join(",", dictionary.Select(kv => kv.Key + "=" + kv.Value).ToArray()) + "}";
    }
}