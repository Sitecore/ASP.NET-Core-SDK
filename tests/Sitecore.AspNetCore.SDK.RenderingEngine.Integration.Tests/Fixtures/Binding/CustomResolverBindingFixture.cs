using System.Net;
using AwesomeAssertions;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Sitecore.AspNetCore.SDK.AutoFixture.Mocks;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Extensions;
using Sitecore.AspNetCore.SDK.RenderingEngine.Extensions;
using Sitecore.AspNetCore.SDK.RenderingEngine.Integration.Tests.ComponentModels;
using Xunit;

// ReSharper disable StringLiteralTypo
namespace Sitecore.AspNetCore.SDK.RenderingEngine.Integration.Tests.Fixtures.Binding;

public class CustomResolverBindingFixture(TestWebApplicationFactory<TestWebApplicationProgram> factory) : IClassFixture<TestWebApplicationFactory<TestWebApplicationProgram>>, IDisposable
{
    private readonly MockHttpMessageHandler _mockClientHandler = new();
    private readonly Uri _layoutServiceUri = new("http://layout.service");

    [Fact]
    public async Task SitecoreLayoutModelBinders_BindDataCorrectly()
    {
        // Arrange
        string json = await File.ReadAllTextAsync("./Json/headlessSxa.json");
        _mockClientHandler.Responses.Push(new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(json)
        });

        HttpClient client = BuildCustomResolverBindingWebApplicationFactory().CreateClient();

        // Act
        string response = await client.GetStringAsync(new Uri("/", UriKind.Relative));

        HtmlDocument doc = new();
        doc.LoadHtml(response);
        HtmlNode? sectionNode = doc.DocumentNode.ChildNodes["head"].ChildNodes.First(n => n.HasClass("custom-resolver"));

        // Assert
        sectionNode.ChildNodes["ul"].ChildNodes.Count(c => c.Name.Equals("li")).Should().Be(1);
        sectionNode.ChildNodes["ul"].ChildNodes["li"].ChildNodes.Count(c => c.Name.Equals("span")).Should().Be(1);
        sectionNode.ChildNodes["ul"].ChildNodes["li"].ChildNodes["span"].InnerText.Should().Be("Home");

        sectionNode.ChildNodes["ul"].ChildNodes["li"].ChildNodes["ul"].ChildNodes.Count(c => c.Name.Equals("li")).Should().Be(1);
        sectionNode.ChildNodes["ul"].ChildNodes["li"].ChildNodes["ul"].ChildNodes["li"].ChildNodes["span"].InnerText.Should().Be("About");
    }

    public void Dispose()
    {
        _mockClientHandler.Dispose();
        GC.SuppressFinalize(this);
    }

    private WebApplicationFactory<TestWebApplicationProgram> BuildCustomResolverBindingWebApplicationFactory()
    {
        return factory
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services
                        .AddSitecoreLayoutService()
                        .AddHttpHandler("mock", _ => new HttpClient(_mockClientHandler) { BaseAddress = _layoutServiceUri })
                        .AsDefaultHandler();
                    services.AddSitecoreRenderingEngine(options =>
                    {
                        options
                            .AddModelBoundView<PartialDesignDynamicPlaceholder>("PartialDesignDynamicPlaceholder")
                            .AddModelBoundView<CustomResolver>(name => name.Equals("Navigation", StringComparison.OrdinalIgnoreCase), "CustomResolver")
                            .AddDefaultComponentRenderer();
                    });
                });

                builder.Configure(app =>
                {
                    app.UseRouting();
                    app.UseSitecoreRenderingEngine();
                    app.UseEndpoints(endpoints =>
                    {
                        endpoints.MapDefaultControllerRoute();
                    });
                });
            });
    }
}