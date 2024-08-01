using Sitecore.AspNetCore.SDK.LayoutService.Client.Exceptions;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Properties;

namespace Sitecore.AspNetCore.SDK.LayoutService.Client.Response.Model.Fields;

/// <summary>
/// Allows list field types to be mapped to a specific model.
/// </summary>
/// <typeparam name="TTargetModel">Strongly typed model to map the target of each list item to.</typeparam>
[System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1649:File name should match first type name", Justification = "Generic Type.")]
public class ContentListField<TTargetModel> : List<ItemLinkField<TTargetModel>>, IField, IFieldReader
    where TTargetModel : class
{
    /// <inheritdoc />
    public virtual TField Read<TField>()
        where TField : IField
    {
        return (TField)Read(typeof(TField));
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
    public virtual object Read(Type type)
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
    protected virtual object HandleRead(Type type)
    {
        if (type.IsAssignableFrom(GetType()))
        {
            return this;
        }

        throw new InvalidCastException(string.Format(Resources.Exception_CouldNotConvertFieldToType, GetType(), type));
    }
}