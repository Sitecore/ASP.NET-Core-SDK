using System.Globalization;
using System.Net;
using FluentAssertions;
using HtmlAgilityPack;
using Microsoft.AspNetCore.TestHost;
using Sitecore.AspNetCore.SDK.AutoFixture.Mocks;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Extensions;
using Sitecore.AspNetCore.SDK.RenderingEngine.Extensions;
using Sitecore.AspNetCore.SDK.TestData;
using Xunit;

// ReSharper disable StringLiteralTypo
namespace Sitecore.AspNetCore.SDK.RenderingEngine.Integration.Tests.Fixtures.TagHelpers;

public class AllFieldTagHelpersFixture : IDisposable
{
    private readonly TestServer _server;
    private readonly MockHttpMessageHandler _mockClientHandler;
    private readonly Uri _layoutServiceUri = new("http://layout.service");

    public AllFieldTagHelpersFixture()
    {
        TestServerBuilder testHostBuilder = new();
        _mockClientHandler = new MockHttpMessageHandler();
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
                        .AddModelBoundView<ComponentModels.ComponentWithAllFieldTypes>("Component-With-All-Field-Types", "ComponentWithAllFieldTypes")
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
    public async Task ComponentWithAllFieldTypes_RendersFieldsCorrectly()
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
        HtmlNode? sectionNode = doc.DocumentNode.ChildNodes.First(n => n.HasClass("component-allfields"));

        // Assert
        sectionNode.ChildNodes.First(n => n.Name.Equals("h1", StringComparison.OrdinalIgnoreCase)).InnerText.Should().Be(TestConstants.TestFieldValue);

        sectionNode.ChildNodes.First(n => n.Name.Equals("div", StringComparison.OrdinalIgnoreCase) && n.Id.Equals("div1", StringComparison.OrdinalIgnoreCase)).InnerHtml
            .Should().Be(TestConstants.TestMultilineFieldValue.Replace(Environment.NewLine, "<br>", StringComparison.OrdinalIgnoreCase));
        sectionNode.ChildNodes.First(n => n.Name.Equals("div", StringComparison.OrdinalIgnoreCase) && n.Id.Equals("div2", StringComparison.OrdinalIgnoreCase)).InnerHtml
            .Should().Be(TestConstants.RichTextFieldValue1);
        sectionNode.ChildNodes.First(n => n.Name.Equals("div", StringComparison.OrdinalIgnoreCase) && n.Id.Equals("div3", StringComparison.OrdinalIgnoreCase)).InnerHtml
            .Should().Be(TestConstants.RichTextFieldValue2);
        sectionNode.ChildNodes.First(n => n.Name.Equals("div", StringComparison.OrdinalIgnoreCase) && n.Id.Equals("div4", StringComparison.OrdinalIgnoreCase)).InnerHtml
            .Should().Be(TestConstants.LinkFieldValue);
        sectionNode.ChildNodes.First(n => n.Name.Equals("div", StringComparison.OrdinalIgnoreCase) && n.Id.Equals("div5", StringComparison.OrdinalIgnoreCase)).InnerHtml
            .Should().Be(TestConstants.AllFieldsImageValue);
        sectionNode.ChildNodes.First(n => n.Name.Equals("div", StringComparison.OrdinalIgnoreCase) && n.Id.Equals("div6", StringComparison.OrdinalIgnoreCase)).InnerHtml
            .Should().Be(TestConstants.DateTimeValue.ToString("MM/dd/yyyy", CultureInfo.CurrentCulture));
        sectionNode.ChildNodes.First(n => n.Name.Equals("div", StringComparison.OrdinalIgnoreCase) && n.Id.Equals("div7", StringComparison.OrdinalIgnoreCase)).InnerHtml
            .Should().Be(TestConstants.MediaLibraryItemImageFieldValue);
        sectionNode.ChildNodes.First(n => n.Name.Equals("div", StringComparison.OrdinalIgnoreCase) && n.Id.Equals("div8", StringComparison.OrdinalIgnoreCase)).InnerHtml
            .Should().Be(9.99m.ToString(CultureInfo.CurrentCulture));
    }

    public void Dispose()
    {
        _mockClientHandler.Dispose();
        _server.Dispose();
        GC.SuppressFinalize(this);
    }
}