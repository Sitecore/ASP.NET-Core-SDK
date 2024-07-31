using System.ComponentModel;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Properties;

namespace Sitecore.AspNetCore.SDK.LayoutService.Client.Request;

/// <summary>
/// Represents Sitecore layout service request data.
/// </summary>
public class SitecoreLayoutRequest : Dictionary<string, object?>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SitecoreLayoutRequest"/> class.
    /// </summary>
    public SitecoreLayoutRequest()
        : base(StringComparer.OrdinalIgnoreCase)
    {
    }

    /// <summary>
    /// Safely gets a typed value from the underlying dictionary.
    /// </summary>
    /// <typeparam name="T">The type to be resolved.</typeparam>
    /// <param name="key">The key to be located.</param>
    /// <param name="value">The discovered value.</param>
    /// <returns>True if successful, otherwise false.</returns>
    public virtual bool TryReadValue<T>(string key, out T? value)
    {
        ArgumentNullException.ThrowIfNull(key);

        value = default;
        if (TryGetValue(key, out object? result) && result != null)
        {
            if (result is T typed)
            {
                value = typed;
            }
            else
            {
                try
                {
                    value = ConvertValue<T>(result);
                }
                catch
                {
                    return false;
                }
            }

            return true;
        }

        return false;
    }

    /// <summary>
    /// Converts the given value to type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The destination type.</typeparam>
    /// <param name="value">The input value.</param>
    /// <returns>An instance of <typeparamref name="T"/> if successful.</returns>
    protected virtual T? ConvertValue<T>(object value)
    {
        TypeConverter? converter = TypeDescriptor.GetConverter(typeof(T));
        return converter == null
            ? throw new InvalidOperationException(string.Format(Resources.Exception_CouldNotFindConverter, typeof(T)))
            : (T?)converter.ConvertFrom(value);
    }
}