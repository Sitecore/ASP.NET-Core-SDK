using Microsoft.Extensions.Options;
using Sitecore.AspNetCore.SDK.SearchOptimization.Models;

namespace Sitecore.AspNetCore.SDK.SearchOptimization.Services;

/// <summary>
/// Implements Sitemap service to apply configuration options to Urls.
/// </summary>
/// <param name="options">Sitemap Options.</param>
internal class SitemapService(IOptions<SitemapOptions> options)
    : ISitemapService
{
    private readonly SitemapOptions _options = options.Value;

    /// <inheritdoc />
    public Task<string> GetSitemapUrl(string requestedUrl, string? siteName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(requestedUrl);

        if (_options.Url != null)
        {
            string finalUrl = new Uri(_options.Url, requestedUrl).ToString();
            return Task.FromResult(finalUrl);
        }

        return Task.FromResult(string.Empty);
    }
}