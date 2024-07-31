// ReSharper disable InconsistentNaming - Must match with string representation
namespace Sitecore.AspNetCore.SDK.SearchOptimization.Models;

/// <summary>
/// Redirect type enumeration.
/// </summary>
internal enum RedirectType
{
    /// <summary>
    /// Permanent Redirect.
    /// </summary>
    REDIRECT_301,

    /// <summary>
    /// Temporary Redirect.
    /// </summary>
    REDIRECT_302,

    /// <summary>
    /// Server Transfer.
    /// </summary>
    SERVER_TRANSFER
}