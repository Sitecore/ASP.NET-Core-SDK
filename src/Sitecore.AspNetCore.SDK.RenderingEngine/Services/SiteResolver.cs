using System.Globalization;
using System.Text.RegularExpressions;
using Sitecore.AspNetCore.SDK.RenderingEngine.Middleware.Models;

namespace Sitecore.AspNetCore.SDK.RenderingEngine.Services;

/// <inheritdoc/>
internal class SiteResolver(ISiteCollectionService siteCollectionService)
    : ISiteResolver
{
    private readonly ISiteCollectionService _siteCollectionService = siteCollectionService ?? throw new ArgumentNullException(nameof(siteCollectionService));

    /// <inheritdoc/>
    public async Task<string?> GetByHost(string hostName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(hostName);

        Dictionary<string, SiteInfo> hostMap = await GetHostMapAsync().ConfigureAwait(false);
        foreach (KeyValuePair<string, SiteInfo> pair in hostMap)
        {
            if (MatchesPattern(hostName, pair.Key))
            {
                return pair.Value.Name;
            }
        }

        return null;
    }

    private static bool MatchesPattern(string hostname, string pattern)
    {
        // Dots should be treated as chars
        // Stars should be treated as wildcards
        string regExpPattern = pattern.Replace(".", "\\.", StringComparison.Ordinal).Replace("*", ".*", StringComparison.Ordinal);

        Regex regExp = new($"^{regExpPattern}$", RegexOptions.IgnoreCase);

        return regExp.IsMatch(hostname);
    }

    private async Task<Dictionary<string, SiteInfo>> GetHostMapAsync()
    {
        Dictionary<string, SiteInfo> map = [];

        SiteInfo?[]? siteCollection = await _siteCollectionService.GetSitesCollection().ConfigureAwait(false);

        if (siteCollection != null)
        {
            foreach (SiteInfo site in siteCollection.OfType<SiteInfo>())
            {
                string[]? hostNames = site.HostName?
                    .Replace(" ", string.Empty, StringComparison.Ordinal)
                    .ToLower(CultureInfo.InvariantCulture).Split('|');

                if (hostNames != null)
                {
                    foreach (string hostName in hostNames)
                    {
                        map.TryAdd(hostName, site);
                    }
                }
            }
        }

        // Now order by specificity.
        // This is equivalent to sorting from longest to shortest, assuming
        // that a match is less specific as wildcards are introduced.
        // E.g., order.eu.site.com → *.eu.site.com → *.site.com → *
        // In case of a tie (e.g., *.site.com vs i.site.com), prefer the one with fewer wildcards.
        return new Dictionary<string, SiteInfo>(
            map.OrderBy(pair =>
            {
                if (pair.Key.Length == 0)
                {
                    return 0;
                }

                return pair.Key.Count(c => c == '*') - pair.Key.Length;
            }));
    }
}