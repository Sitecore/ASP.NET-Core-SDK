namespace Sitecore.AspNetCore.SDK.LayoutService.Client.Response.Model;

/// <summary>
/// Supports delayed reading of an <see cref="IField"/>.
/// </summary>
public interface IFieldReader
{
    /// <summary>
    /// Reads the current Field as the specified type.
    /// </summary>
    /// <typeparam name="TField">The type of Field to be read.</typeparam>
    /// <returns>A new instance of <typeparamref name="TField"/>.</returns>
    TField? Read<TField>()
        where TField : IField;

    /// <summary>
    /// Attempts to read the current Field as the specified type.
    /// </summary>
    /// <typeparam name="TField">The type of Field to be read.</typeparam>
    /// <param name="field">The resulting instance if successful.</param>
    /// <returns>True if the field could be read as the specified type, otherwise false.</returns>
    bool TryRead<TField>(out TField? field)
        where TField : IField;

    /// <summary>
    /// Reads the current Field as the specified type.
    /// The type must implement <see cref="IField"/>.
    /// </summary>
    /// <param name="type">The type of field to be read.</param>
    /// <returns>A new instance if successful.</returns>
    object? Read(Type type);

    /// <summary>
    /// Attempts to read the current Field as the specified type.
    /// The type must implement <see cref="IField"/>.
    /// </summary>
    /// <param name="type">The type of field to be read.</param>
    /// <param name="field">The resulting field.</param>
    /// <returns>True if the field could be read as the specified type, otherwise false.</returns>
    bool TryRead(Type type, out IField? field);
}