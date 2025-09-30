using System.Net;
using System.Text.Encodings.Web;
using AwesomeAssertions;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Sitecore.AspNetCore.SDK.AutoFixture.Mocks;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Extensions;
using Sitecore.AspNetCore.SDK.RenderingEngine.Extensions;
using Sitecore.AspNetCore.SDK.TestData;
using Xunit;

namespace Sitecore.AspNetCore.SDK.RenderingEngine.Integration.Tests.Fixtures.CustomRenderTypes;

public class MultipleComponentsAddedFixture : IClassFixture<TestWebApplicationFactory<TestWebApplicationProgram>>, IDisposable
{
    private readonly MockHttpMessageHandler _mockClientHandler = new();
    private readonly Uri _layoutServiceUri = new("http://layout.service");
    private readonly WebApplicationFactory<TestWebApplicationProgram> _factory;

    public MultipleComponentsAddedFixture(TestWebApplicationFactory<TestWebApplicationProgram> factory)
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
                        .AddModelBoundView<ComponentModels.Component3>(name => name.Equals("Component-3", StringComparison.OrdinalIgnoreCase), "Component3")
                        .AddModelBoundView<ComponentModels.Component3>(name => name.Equals("Component-6", StringComparison.OrdinalIgnoreCase), "Component6")
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

        // Accessing _factory.Server forces the TestServer to start. The variable is unused; this is intentional.
        TestServer startedServer = _factory.Server;
    }

    [Fact]
    public async Task CustomRenderTypes_MultipleComponentsBoundsInCorrectOrder()
    {
        // Arrange
        _mockClientHandler.Responses.Push(new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(Serializer.Serialize(CannedResponses.WithNestedPlaceholder))
        });

        HttpClient client = _factory.CreateClient();

        // Act
        string response = await client.GetStringAsync(new Uri("/", UriKind.Relative));

        HtmlDocument doc = new();
        doc.LoadHtml(response);
        HtmlNodeCollection? docNodes = doc.DocumentNode.ChildNodes;

        // Assert
        docNodes.GetNodeIndex(docNodes.First(n => n.HasClass("component-3")))
            .Should()
            .BeLessThan(docNodes.GetNodeIndex(docNodes.First(n => n.HasClass("component-6"))));

        HtmlNode? sectionNode = docNodes.First(n => n.HasClass("component-3"));
        sectionNode.ChildNodes.First(n => n.Name.Equals("h1", StringComparison.OrdinalIgnoreCase)).InnerText
            .Should().Be(HtmlEncoder.Default.Encode(TestConstants.TestFieldValue));

        sectionNode = doc.DocumentNode.ChildNodes.First(n => n.HasClass("component-6"));
        sectionNode.ChildNodes.First(n => n.Name.Equals("textarea", StringComparison.OrdinalIgnoreCase)).InnerText
            .Should().Be(HtmlEncoder.Default.Encode(TestConstants.TestFieldValue + " from Component-6"));
    }

    public void Dispose()
    {
        _mockClientHandler.Dispose();
        _factory.Dispose();
        GC.SuppressFinalize(this);
    }
}