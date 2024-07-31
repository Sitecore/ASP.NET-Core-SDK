using System.Text;

namespace Sitecore.AspNetCore.SDK.RenderingEngine.Extensions;

/// <summary>
/// Extension methods to support custom encoding / decoding.
/// </summary>
internal static class EncodingExtensions
{
    private static readonly string[] HeaderEncodingTable =
    [
        "%00", "%01", "%02", "%03", "%04", "%05", "%06", "%07",
        "%08", "%09", "%0a", "%0b", "%0c", "%0d", "%0e", "%0f",
        "%10", "%11", "%12", "%13", "%14", "%15", "%16", "%17",
        "%18", "%19", "%1a", "%1b", "%1c", "%1d", "%1e", "%1f"
    ];

    /// <summary>
    /// Encode the following characters in the provided value:
    /// - All CTL characters except HT (horizontal tab)
    /// - DEL character (\x7f)
    /// This is useful in preventing CRLF Injection attacks.
    /// Utility method based on: https://referencesource.microsoft.com/#System.Web/Util/HttpEncoder.cs,b5c8b7b5bb004908,references.
    /// </summary>
    /// <param name="value">The string value to encode.</param>
    /// <returns>The encoded value.</returns>
    public static string EncodeControlCharacters(this string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return string.Empty;
        }

        StringBuilder sanitizedHeader = new();
        foreach (char c in value)
        {
            if (c < 32 && c != 9)
            {
                sanitizedHeader.Append(HeaderEncodingTable[c]);
            }
            else if (c == 127)
            {
                sanitizedHeader.Append("%7f");
            }
            else
            {
                sanitizedHeader.Append(c);
            }
        }

        return sanitizedHeader.ToString();
    }
}