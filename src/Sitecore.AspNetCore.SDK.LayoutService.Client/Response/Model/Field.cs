using Sitecore.AspNetCore.SDK.LayoutService.Client.Properties;

namespace Sitecore.AspNetCore.SDK.LayoutService.Client.Response.Model;

/// <summary>
/// Base implementation of an <see cref="IField"/>.
/// </summary>
public abstract class Field : FieldReader, IField
{
    /// <inheritdoc />
    protected override object HandleRead(Type type)
    {
        if (type.IsInstanceOfType(this))
        {
            return this;
        }

        throw new InvalidCastException(string.Format(Resources.Exception_CouldNotConvertFieldToType, GetType(), type));
    }
}