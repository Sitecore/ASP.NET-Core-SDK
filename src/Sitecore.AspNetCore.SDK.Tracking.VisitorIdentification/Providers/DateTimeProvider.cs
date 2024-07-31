namespace Sitecore.AspNetCore.SDK.Tracking.VisitorIdentification.Providers;

/// <inheritdoc />
public class DateTimeProvider : IDateTimeProvider
{
    /// <inheritdoc />
    public DateTime GetUtcNow()
    {
        return DateTime.UtcNow;
    }
}