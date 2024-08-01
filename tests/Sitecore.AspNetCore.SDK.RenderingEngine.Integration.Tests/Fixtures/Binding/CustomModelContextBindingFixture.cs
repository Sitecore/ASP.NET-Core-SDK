using System.Net;
using System.Text.Json.Nodes;
using FluentAssertions;
using HtmlAgilityPack;
using Microsoft.AspNetCore.TestHost;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Extensions;
using Sitecore.AspNetCore.SDK.RenderingEngine.Extensions;
using Sitecore.AspNetCore.SDK.TestData;
using Xunit;

// ReSharper disable StringLiteralTypo
namespace Sitecore.AspNetCore.SDK.RenderingEngine.Integration.Tests.Fixtures.Binding;

public class CustomModelContextBindingFixture : IDisposable
{
    private readonly TestServer _server;
    private readonly HttpLayoutClientMessageHandler _mockClientHandler;
    private readonly Uri _layoutServiceUri = new("http://layout.service");

    public CustomModelContextBindingFixture()
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
                        .AddModelBoundView<ComponentModels.CustomModelContextComponent>(name => name.Equals("Custom-Model-Context-Component", StringComparison.OrdinalIgnoreCase), "CustomModelContextComponent")
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
    public async Task SitecoreLayoutModelBinders_BindDataCorrectly()
    {
        // Arrange
        string serObj = Serializer.Serialize(CannedResponses.WithNestedPlaceholder);
        JsonNode? jObject = JsonNode.Parse(serObj);
        JsonNode? contextJObject = jObject?["sitecore"]?["context"];
        contextJObject!["testClass1"] = new JsonObject([
            new KeyValuePair<string, JsonNode?>("testString", "stringExample"),
            new KeyValuePair<string, JsonNode?>("testInt", "123"),
            new KeyValuePair<string, JsonNode?>("testtime", "2020-12-08T13:09:44.1255842+02:00")
        ]);
        contextJObject["testClass2"] = new JsonObject([
            new KeyValuePair<string, JsonNode?>("testString", "stringExample2"),
            new KeyValuePair<string, JsonNode?>("testInt", "1234")
        ]);
        contextJObject["singleProperty"] = "SinglePropertyData";

        _mockClientHandler.Responses.Push(new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(jObject!.ToJsonString(Serializer.GetOptions()))
        });

        HttpClient client = _server.CreateClient();

        // Act
        string response = await client.GetStringAsync(new Uri("/", UriKind.Relative));

        HtmlDocument doc = new();
        doc.LoadHtml(response);
        HtmlNode? sectionNode = doc.DocumentNode.ChildNodes.First(n => n.HasClass("custom-model-context-component"));

        // Assert
        sectionNode.ChildNodes.First(n => n.Id.Equals("singleProp", StringComparison.OrdinalIgnoreCase)).InnerText
            .Should().Be("SinglePropertyData");
        sectionNode.ChildNodes.First(n => n.Id.Equals("class1string", StringComparison.OrdinalIgnoreCase)).InnerText
            .Should().Be("stringExample");
        sectionNode.ChildNodes.First(n => n.Id.Equals("class1date", StringComparison.OrdinalIgnoreCase)).InnerText
            .Should().NotBeNullOrWhiteSpace();
        sectionNode.ChildNodes.First(n => n.Id.Equals("class1int", StringComparison.OrdinalIgnoreCase)).InnerText
            .Should().Be("123");
        sectionNode.ChildNodes.First(n => n.Id.Equals("class2string", StringComparison.OrdinalIgnoreCase)).InnerText
            .Should().Be("stringExample2");
        sectionNode.ChildNodes.First(n => n.Id.Equals("class2int", StringComparison.OrdinalIgnoreCase)).InnerText
            .Should().Be("1234");
        sectionNode.ChildNodes.First(n => n.Id.Equals("class1Indint", StringComparison.OrdinalIgnoreCase)).InnerText
            .Should().Be("123");
        sectionNode.ChildNodes.First(n => n.Id.Equals("class1Indstr", StringComparison.OrdinalIgnoreCase)).InnerText
            .Should().Be("stringExample");
        sectionNode.ChildNodes.First(n => n.Id.Equals("customCtxIndInt", StringComparison.OrdinalIgnoreCase)).InnerText
            .Should().Be("123");
        sectionNode.ChildNodes.First(n => n.Id.Equals("customCtxIndStr", StringComparison.OrdinalIgnoreCase)).InnerText
            .Should().Be("stringExample");
        sectionNode.ChildNodes.First(n => n.Id.Equals("individualProperty", StringComparison.OrdinalIgnoreCase)).InnerText
            .Should().Be("SinglePropertyData");
    }

    public void Dispose()
    {
        _server.Dispose();
        _mockClientHandler.Dispose();
        GC.SuppressFinalize(this);
    }
}