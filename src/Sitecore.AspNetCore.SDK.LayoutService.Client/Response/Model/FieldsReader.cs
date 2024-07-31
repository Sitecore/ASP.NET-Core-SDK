using System.Reflection;
using System.Xml.Linq;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Properties;

namespace Sitecore.AspNetCore.SDK.LayoutService.Client.Response.Model;

/// <summary>
/// Implements reading of a collection of fields as a specified type.
/// </summary>
public abstract class FieldsReader : IFieldsReader
{
    /// <inheritdoc />
    public Dictionary<string, IFieldReader> Fields { get; set; } = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Reads a field from the collection as the specified type.
    /// </summary>
    /// <typeparam name="TField">The type of object to return.</typeparam>
    /// <param name="name">The name of the field to be read.</param>
    /// <returns>A new instance of <typeparamref name="TField"/>.</returns>
    public virtual TField? ReadField<TField>(string name)
        where TField : IField
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        return (TField?)ReadField(typeof(TField), name);
    }

    /// <summary>
    /// Attempts to read a field from the collection as the specified type.
    /// </summary>
    /// <typeparam name="TField">The type of object to return.</typeparam>
    /// <param name="name">The name of the field to be read.</param>
    /// <param name="instance">The resulting instance if successful.</param>
    /// <returns>True if successful, otherwise false.</returns>
    public virtual bool TryReadField<TField>(string name, out TField? instance)
        where TField : IField
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        if (TryReadField(typeof(TField), name, out object? resultInstance))
        {
            instance = (TField?)resultInstance;
            return true;
        }

        instance = default;
        return false;
    }

    /// <summary>
    /// Reads the collection of fields as the specified type.
    /// </summary>
    /// <typeparam name="T">The type of object to return.</typeparam>
    /// <returns>A new instance of <typeparamref name="T"/>.</returns>
    public virtual T? ReadFields<T>()
        where T : new()
    {
        return (T?)ReadFields(typeof(T));
    }

    /// <summary>
    /// Attempts to read the collection of fields as the specified type.
    /// </summary>
    /// <typeparam name="T">The type of object to return.</typeparam>
    /// <param name="instance">The resulting instance if successful.</param>
    /// <returns>True if successful, otherwise false.</returns>
    public virtual bool TryReadFields<T>(out T? instance)
        where T : new()
    {
        if (TryReadFields(typeof(T), out object? resultInstance))
        {
            instance = (T?)resultInstance;
            return true;
        }

        instance = default;
        return false;
    }

    /// <inheritdoc />
    public virtual object? ReadField(Type type, string name)
    {
        ArgumentNullException.ThrowIfNull(type);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        IFieldReader? field = Fields
            .FirstOrDefault(x => x.Key.Equals(name, StringComparison.OrdinalIgnoreCase))
            .Value;

        return field?.Read(type);
    }

    /// <inheritdoc />
    public virtual bool TryReadField(Type type, string name, out object? instance)
    {
        ArgumentNullException.ThrowIfNull(type);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        instance = default;
        bool result;

        try
        {
            instance = ReadField(type, name);
            result = instance != null;
        }
        catch
        {
            result = false;
        }

        return result;
    }

    /// <inheritdoc />
    public bool TryReadFields(Type type, out object? instance)
    {
        ArgumentNullException.ThrowIfNull(type);

        instance = default;
        bool result;

        try
        {
            instance = HandleReadFields(type);
            result = true;
        }
        catch
        {
            result = false;
        }

        return result;
    }

    /// <inheritdoc />
    public object? ReadFields(Type type)
    {
        ArgumentNullException.ThrowIfNull(type);

        try
        {
            return HandleReadFields(type);
        }
        catch (Exception ex)
        {
            throw new InvalidCastException(string.Format(Resources.Exception_CouldNotConvertFieldToType, GetType(), type), ex);
        }
    }

    /// <summary>
    /// Handles reading the field collection, binding to the specified type.
    /// </summary>
    /// <param name="type">The type to be bound.</param>
    /// <returns>A new instance of the specified type, if successful.</returns>
    protected virtual object? HandleReadFields(Type type)
    {
        object? instance = Activator.CreateInstance(type);

        foreach (PropertyInfo prop in type.GetProperties().Where(x => x.CanWrite))
        {
            prop.SetValue(instance, ReadField(prop.PropertyType, prop.Name));
        }

        return instance;
    }
}