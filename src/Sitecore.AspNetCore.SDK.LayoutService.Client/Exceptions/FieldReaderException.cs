using Sitecore.AspNetCore.SDK.LayoutService.Client.Properties;

namespace Sitecore.AspNetCore.SDK.LayoutService.Client.Exceptions;

/// <summary>
/// Details an exception that may occur when reading a Field.
/// </summary>
public class FieldReaderException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FieldReaderException"/> class.
    /// </summary>
    /// <param name="message">The exception message.</param>
    public FieldReaderException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FieldReaderException"/> class.
    /// </summary>
    /// <param name="message">The exception message.</param>
    /// <param name="innerException">The inner exception to be wrapped.</param>
    public FieldReaderException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FieldReaderException"/> class.
    /// </summary>
    /// <param name="type">The type attempting to be read.</param>
    public FieldReaderException(Type type)
        : base(string.Format(Resources.Exception_ReadingField, type))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FieldReaderException"/> class.
    /// </summary>
    /// <param name="type">The type attempting to be read.</param>
    /// <param name="innerException">The inner exception to be wrapped.</param>
    public FieldReaderException(Type type, Exception innerException)
        : base(string.Format(Resources.Exception_ReadingField, type), innerException)
    {
    }
}