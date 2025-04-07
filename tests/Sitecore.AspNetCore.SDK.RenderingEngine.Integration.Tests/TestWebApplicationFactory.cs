using GraphQL.Client.Abstractions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NSubstitute;

namespace Sitecore.AspNetCore.SDK.RenderingEngine.Integration.Tests
{
    public class TestWebApplicationFactory<T>
        : WebApplicationFactory<T>
        where T : class
    {
        public IGraphQLClient MockGraphQLClient { get; set; } = Substitute.For<IGraphQLClient>();

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseContentRoot(Path.GetFullPath(Directory.GetCurrentDirectory()))
                   .ConfigureTestServices(services =>
                   {
                       ServiceProvider serviceProvider = services.BuildServiceProvider();
                       ServiceDescriptor descriptor = new(typeof(IGraphQLClient), MockGraphQLClient);
                       services.Replace(descriptor);
                   });
        }
    }
}