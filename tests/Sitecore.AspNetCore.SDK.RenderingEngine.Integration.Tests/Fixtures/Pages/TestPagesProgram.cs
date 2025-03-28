using Sitecore.AspNetCore.SDK.GraphQL.Extensions;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Extensions;
using Sitecore.AspNetCore.SDK.Pages.Configuration;
using Sitecore.AspNetCore.SDK.Pages.Extensions;
using Sitecore.AspNetCore.SDK.RenderingEngine.Extensions;
using Sitecore.AspNetCore.SDK.TestData;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddRouting()
                .AddMvc();

builder.Services.AddGraphQlClient(configuration =>
{
    configuration.ContextId = TestConstants.ContextId;
});

builder.Services.AddSitecoreLayoutService()
                .AddSitecorePagesHandler()
                .AddGraphQlWithContextHandler("default", TestConstants.ContextId!, siteName: TestConstants.SiteName!)
                .AsDefaultHandler();

builder.Services.AddSitecoreRenderingEngine(options =>
                {
                    options.AddDefaultPartialView("_ComponentNotFound");
                })
                .WithSitecorePages(TestConstants.ContextId, options => { options.EditingSecret = TestConstants.JssEditingSecret; });

WebApplication app = builder.Build();
app.UseSitecorePages(new PagesOptions { ConfigEndpoint = TestConstants.ConfigRoute });
app.UseRouting();
app.Run();

/// <summary>
/// Partial class allowing this TestProgram to be created by a WebApplicationFactory for integration testing.
/// </summary>
public partial class TestPagesProgram
{
}