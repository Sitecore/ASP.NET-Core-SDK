using System.Net;
using AwesomeAssertions;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Sitecore.AspNetCore.SDK.AutoFixture.Mocks;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Extensions;
using Sitecore.AspNetCore.SDK.RenderingEngine.Extensions;
using Sitecore.AspNetCore.SDK.TestData;
using Xunit;

namespace Sitecore.AspNetCore.SDK.RenderingEngine.Integration.Tests.Fixtures.Binding;

public class ModelBindingErrorHandlingFixture : IClassFixture<TestWebApplicationFactory<TestWebApplicationProgram>>, IDisposable
{
    private readonly MockHttpMessageHandler _mockClientHandler = new();
    private readonly Uri _layoutServiceUri = new("http://layout.service");

    private readonly WebApplicationFactory<TestWebApplicationProgram> _factory;

    public ModelBindingErrorHandlingFixture(TestWebApplicationFactory<TestWebApplicationProgram> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
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
                        .AddModelBoundView<ComponentModels.ComponentWithMissingData>(name => name.Equals("Component-With-Missing-Data", StringComparison.OrdinalIgnoreCase), "ComponentWithMissingData")
                        .AddModelBoundView<ComponentModels.ComponentWithoutId>(name => name.Equals("Component-Without-Id", StringComparison.OrdinalIgnoreCase), "ComponentWithoutId");
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

        // Accessing _factory.Server forces the TestServer to start. The variable is unused; this is intentional.
        _ = _factory.Server;
    }

    [Fact]
    public async Task SitecoreLayoutModelBinders_HandleMissingDataCorrectly()
    {
        // Arrange
        _mockClientHandler.Responses.Push(new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(Serializer.Serialize(CannedResponses.WithMissingData))
        });

        HttpClient client = _factory.CreateClient();

        // Act
        string response = await client.GetStringAsync(new Uri("/", UriKind.Relative));

        HtmlDocument doc = new();
        doc.LoadHtml(response);
        HtmlNode? sectionNode = doc.DocumentNode.ChildNodes.Single(n => n.HasClass("component-with-missing-data"));
        HtmlNode? nestedSectionNode = sectionNode.ChildNodes.Single(n => n.HasClass("component-without-id"));

        // Assert
        sectionNode.ChildNodes.Single(n => n.Id.Equals("textField", StringComparison.OrdinalIgnoreCase)).InnerText
            .Should().BeEmpty();

        sectionNode.ChildNodes.Single(n => n.Id.Equals("routeProperty", StringComparison.OrdinalIgnoreCase)).InnerText
            .Should().BeEmpty();

        sectionNode.ChildNodes.Single(n => n.Id.Equals("routeField", StringComparison.OrdinalIgnoreCase)).InnerText
            .Should().BeEmpty();

        sectionNode.ChildNodes.Single(n => n.Id.Equals("contextProperty", StringComparison.OrdinalIgnoreCase)).InnerText.
            Should().BeEmpty();

        nestedSectionNode.ChildNodes.Single(n => n.Id.Equals("nestedRichTextField", StringComparison.OrdinalIgnoreCase)).InnerHtml.
            Should().BeEmpty();

        nestedSectionNode.ChildNodes.Single(n => n.Id.Equals("nestedTextField", StringComparison.OrdinalIgnoreCase)).InnerText
            .Should().BeEmpty();

        nestedSectionNode.ChildNodes.Single(n => n.Id.Equals("nestedComponentId", StringComparison.OrdinalIgnoreCase)).InnerText
            .Should().BeEmpty();
    }

    public void Dispose()
    {
        _mockClientHandler.Dispose();
        _factory.Dispose();
        GC.SuppressFinalize(this);
    }
}