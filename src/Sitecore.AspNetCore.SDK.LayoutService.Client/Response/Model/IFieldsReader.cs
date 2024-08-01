namespace Sitecore.AspNetCore.SDK.LayoutService.Client.Response.Model;

/// <summary>
/// Exposes a collection of <see cref="IFieldReader"/>.
/// </summary>
public interface IFieldsReader
{
    /// <summary>
    /// Gets or sets the Fields associated with this instance.
    /// </summary>
    Dictionary<string, IFieldReader> Fields { get; set; }

    /// <summary>
    /// Reads a field from the collection as the specified type.
    /// </summary>
    /// <typeparam name="TField">The type of object to return.</typeparam>
    /// <param name="name">The name of the field to be read.</param>
    /// <returns>A new instance of <typeparamref name="TField"/>.</returns>
    TField? ReadField<TField>(string name)
        where TField : IField;

    /// <summary>
    /// Attempts to read a field from the collection as the specified type.
    /// </summary>
    /// <typeparam name="TField">The type of object to return.</typeparam>
    /// <param name="name">The name of the field to be read.</param>
    /// <param name="instance">The resulting instance if successful.</param>
    /// <returns>True if successful, otherwise false.</returns>
    bool TryReadField<TField>(string name, out TField? instance)
        where TField : IField;

    /// <summary>
    /// Reads the collection of fields as the specified type.
    /// </summary>
    /// <typeparam name="T">The type of object to return.</typeparam>
    /// <returns>A new instance of <typeparamref name="T"/>.</returns>
    T? ReadFields<T>()
        where T : new();

    /// <summary>
    /// Attempts to read the collection of fields as the specified type.
    /// </summary>
    /// <typeparam name="T">The type of object to return.</typeparam>
    /// <param name="instance">The resulting instance if successful.</param>
    /// <returns>True if successful, otherwise false.</returns>
    bool TryReadFields<T>(out T? instance)
        where T : new();

    /// <summary>
    /// Reads a field from the collection as the specified type.
    /// </summary>
    /// <param name="type">The type of object to return.</param>
    /// <param name="name">The name of the field to be read.</param>
    /// <returns>A new instance of the specified type.</returns>
    object? ReadField(Type type, string name);

    /// <summary>
    /// Attempts to read a field from the collection as the specified type.
    /// </summary>
    /// <param name="type">The type of object to return.</param>
    /// <param name="name">The name of the field to be read.</param>
    /// <param name="instance">A new instance of <paramref name="type"/> if successful.</param>
    /// <returns>True if successful, otherwise false.</returns>
    bool TryReadField(Type type, string name, out object? instance);

    /// <summary>
    /// Reads the collection of fields as the specified type.
    /// </summary>
    /// <param name="type">The type of object to return.</param>
    /// <returns>A new instance of the specified type.</returns>
    object? ReadFields(Type type);

    /// <summary>
    /// Attempts to read the collection of fields as the specified type.
    /// </summary>
    /// <param name="type">The type of object to return.</param>
    /// <param name="instance">A new instance of <paramref name="type"/> if successful.</param>
    /// <returns>True if successful, otherwise false.</returns>
    bool TryReadFields(Type type, out object? instance);
}