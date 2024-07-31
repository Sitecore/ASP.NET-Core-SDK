namespace Sitecore.AspNetCore.SDK.Tracking.VisitorIdentification.Providers;

/// <summary>
/// DateTimeProvider - abstract realization for DateTime.
/// </summary>
public interface IDateTimeProvider
{
    /// <summary>
    /// Gets UtcNow DateTime.
    /// </summary>
    /// <returns>UtcNow DateTime <see cref="DateTime"/>.</returns>
    DateTime GetUtcNow();
}