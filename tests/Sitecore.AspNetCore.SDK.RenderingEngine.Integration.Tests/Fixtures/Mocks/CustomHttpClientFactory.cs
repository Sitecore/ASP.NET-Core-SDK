namespace Sitecore.AspNetCore.SDK.RenderingEngine.Integration.Tests.Fixtures.Mocks;

public class CustomHttpClientFactory(Func<HttpClient> createClient)
    : IHttpClientFactory
{
    public HttpClient CreateClient(string name)
    {
        return createClient();
    }
}