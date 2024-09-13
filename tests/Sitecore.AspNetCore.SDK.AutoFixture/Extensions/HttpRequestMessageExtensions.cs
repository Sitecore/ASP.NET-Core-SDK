using System.Diagnostics.CodeAnalysis;

namespace Sitecore.AspNetCore.SDK.AutoFixture.Extensions;

[ExcludeFromCodeCoverage]
public static class HttpRequestMessageExtensions
{
    public static async Task<HttpRequestMessage> Clone(this HttpRequestMessage request)
    {
        HttpRequestMessage clone = new(request.Method, request.RequestUri)
        {
            Version = request.Version
        };

        if (request.Content != null)
        {
            MemoryStream ms = new();
            await request.Content.CopyToAsync(ms);
            ms.Position = 0;
            clone.Content = new StreamContent(ms);

            request.Content.Headers.ToList().ForEach(header => clone.Content.Headers.TryAddWithoutValidation(header.Key, header.Value));
        }

        request.Options.ToList().ForEach(option => clone.Options.TryAdd(option.Key, option.Value));
        request.Headers.ToList().ForEach(header => clone.Headers.TryAddWithoutValidation(header.Key, header.Value));

        return clone;
    }
}