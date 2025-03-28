using Microsoft.AspNetCore.Mvc.Testing;

namespace Sitecore.AspNetCore.SDK.RenderingEngine.Integration.Tests
{
    public class TestWebApplicationFactory<T>
        : WebApplicationFactory<T>
        where T : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseContentRoot(Path.GetFullPath(Directory.GetCurrentDirectory()));
        }
    }
}