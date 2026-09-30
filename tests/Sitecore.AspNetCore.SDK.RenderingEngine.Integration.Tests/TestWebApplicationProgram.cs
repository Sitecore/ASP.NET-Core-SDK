using Sitecore.AspNetCore.SDK.GraphQL.Extensions;
using Sitecore.AspNetCore.SDK.TestData;

namespace Sitecore.AspNetCore.SDK.RenderingEngine.Integration.Tests;

/// <summary>
/// Entry point created by <see cref="Microsoft.AspNetCore.Mvc.Testing.WebApplicationFactory{TEntryPoint}" /> for integration testing.
/// </summary>
public partial class TestWebApplicationProgram
{
    public static void Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

        builder.Services.AddRouting()
                        .AddMvc();

        builder.Services.AddGraphQLClient(configuration =>
        {
            configuration.ContextId = TestConstants.ContextId;
        });

        WebApplication app = builder.Build();
        app.Start();
    }
}