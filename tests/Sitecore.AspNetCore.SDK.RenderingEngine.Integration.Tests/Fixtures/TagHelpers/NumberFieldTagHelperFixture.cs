using System.Globalization;
using System.Net;
using FluentAssertions;
using HtmlAgilityPack;
using Microsoft.AspNetCore.TestHost;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Extensions;
using Sitecore.AspNetCore.SDK.RenderingEngine.Extensions;
using Sitecore.AspNetCore.SDK.TestData;
using Xunit;

namespace Sitecore.AspNetCore.SDK.RenderingEngine.Integration.Tests.Fixtures.TagHelpers;

public class NumberFieldTagHelperFixture : IDisposable
{
    private const decimal TestValue = 1.21M;
    private readonly TestServer _server;
    private readonly HttpLayoutClientMessageHandler _mockClientHandler;
    private readonly Uri _layoutServiceUri = new("http://layout.service");

    public NumberFieldTagHelperFixture()
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
                        .AddModelBoundView<ComponentModels.ComponentWithNumber>("Component-With-Number", "ComponentWithNumber")
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
    public async Task NumberTagHelper_DoesNotResetOtherTagHelperOutput()
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
        HtmlNode? sectionNode = doc.DocumentNode.ChildNodes.First(n => n.HasClass("component-with-number"));
        HtmlNode? text = sectionNode.ChildNodes[3];

        // Assert
        // check scenario that NumberTagHelper does not reset values of nested helpers.
        text.InnerHtml.Should().Contain(TestConstants.TestFieldValue);
    }

    [Fact]
    public async Task NumberHelper_GeneratesProperNumber()
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
        HtmlNode? sectionNode = doc.DocumentNode.ChildNodes.First(n => n.HasClass("component-with-number"));

        // Assert
        sectionNode.ChildNodes[1].ChildNodes[1].InnerHtml.Should().Contain(TestValue.ToString("C", CultureInfo.CurrentCulture));
        sectionNode.ChildNodes[1].ChildNodes[2].InnerHtml.Should().Contain(TestValue.ToString("C", CultureInfo.CreateSpecificCulture("ua-ua")));
        sectionNode.ChildNodes[1].ChildNodes[3].InnerHtml.Should().Contain(TestValue.ToString(CultureInfo.CurrentCulture));
        sectionNode.ChildNodes[1].ChildNodes[4].InnerHtml.Should().Contain(TestValue.ToString("P", CultureInfo.CurrentCulture));
    }

    public void Dispose()
    {
        _mockClientHandler.Dispose();
        _server.Dispose();
        GC.SuppressFinalize(this);
    }
}