using Sitecore.AspNetCore.SDK.GraphQL.Extensions;
using Sitecore.AspNetCore.SDK.TestData;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddRouting()
                .AddMvc();

builder.Services.AddGraphQLClient(configuration =>
{
    configuration.ContextId = TestConstants.ContextId;
});

WebApplication app = builder.Build();
app.Start();

/// <summary>
/// Partial class allowing this TestProgram to be created by a WebApplicationFactory for integration testing.
/// </summary>
public partial class TestWebApplicationProgram
{
}