using Sitecore.AspNetCore.SDK.LayoutService.Client.Exceptions;

namespace Sitecore.AspNetCore.SDK.LayoutService.Client.Response.Model;

/// <summary>
/// Implements reading of a Field as a specified type.
/// </summary>
public abstract class FieldReader : IFieldReader
{
    /// <inheritdoc />
    public virtual TField? Read<TField>()
        where TField : IField
    {
        return (TField?)Read(typeof(TField));
    }

    /// <inheritdoc />
    public virtual bool TryRead<TField>(out TField? field)
        where TField : IField
    {
        if (TryRead(typeof(TField), out IField? resultField))
        {
            field = (TField?)resultField;
            return true;
        }

        field = default;
        return false;
    }

    /// <inheritdoc />
    public virtual bool TryRead(Type type, out IField? field)
    {
        ArgumentNullException.ThrowIfNull(type);

        bool result = false;
        field = default;

        if (typeof(IField).IsAssignableFrom(type))
        {
            try
            {
                field = HandleRead(type) as IField;
                result = true;
            }
            catch
            {
                result = false;
            }
        }

        return result;
    }

    /// <inheritdoc />
    public virtual object? Read(Type type)
    {
        ArgumentNullException.ThrowIfNull(type);

        try
        {
            return HandleRead(type);
        }
        catch (Exception ex)
        {
            throw new FieldReaderException(type, ex);
        }
    }

    /// <summary>
    /// Returns an instance of the Field data as a specified type.
    /// </summary>
    /// <param name="type">The type to read the field as.</param>
    /// <returns>A new instance of the specified type.</returns>
    protected abstract object? HandleRead(Type type);
}