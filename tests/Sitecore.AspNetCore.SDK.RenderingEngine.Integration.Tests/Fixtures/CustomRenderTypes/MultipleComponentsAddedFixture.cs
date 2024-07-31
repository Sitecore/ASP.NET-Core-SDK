using System.Net;
using System.Text.Encodings.Web;
using FluentAssertions;
using HtmlAgilityPack;
using Microsoft.AspNetCore.TestHost;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Extensions;
using Sitecore.AspNetCore.SDK.RenderingEngine.Extensions;
using Sitecore.AspNetCore.SDK.TestData;
using Xunit;

namespace Sitecore.AspNetCore.SDK.RenderingEngine.Integration.Tests.Fixtures.CustomRenderTypes;

public class MultipleComponentsAddedFixture : IDisposable
{
    private readonly TestServer _server;
    private readonly HttpLayoutClientMessageHandler _mockClientHandler;
    private readonly Uri _layoutServiceUri = new("http://layout.service");

    public MultipleComponentsAddedFixture()
    {
        TestServerBuilder testHostBuilder = new();
        _mockClientHandler = new HttpLayoutClientMessageHandler();
        testHostBuilder
            .ConfigureServices(builder =>
            {
                builder
                    .AddSitecoreLayoutService()
                    .AddHttpHandler("mock", _ => new HttpClient(_mockClientHandler) { BaseAddress = _layoutServiceUri })
                    .AsDefaultHandler();
                builder.AddSitecoreRenderingEngine(options =>
                {
                    options
                        .AddModelBoundView<ComponentModels.Component3>(name => name.Equals("Component-3", StringComparison.OrdinalIgnoreCase), "Component3")
                        .AddModelBoundView<ComponentModels.Component3>(name => name.Equals("Component-6", StringComparison.OrdinalIgnoreCase), "Component6")
                        .AddDefaultComponentRenderer();
                });
            })
            .Configure(app =>
            {
                app.UseRouting();
                app.UseSitecoreRenderingEngine();
                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapDefaultControllerRoute();
                });
            });

        _server = testHostBuilder.BuildServer(new Uri("http://localhost"));
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

        HttpClient client = _server.CreateClient();

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
        _server.Dispose();
        GC.SuppressFinalize(this);
    }
}