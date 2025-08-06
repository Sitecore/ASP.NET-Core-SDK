using System.Globalization;
using System.Net;
using FluentAssertions;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.TestHost;
using Sitecore.AspNetCore.SDK.AutoFixture.Mocks;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Extensions;
using Sitecore.AspNetCore.SDK.RenderingEngine.Extensions;
using Sitecore.AspNetCore.SDK.TestData;
using Xunit;

namespace Sitecore.AspNetCore.SDK.RenderingEngine.Integration.Tests.Fixtures.TagHelpers;

public class DateFieldTagHelperFixture : IDisposable
{
    private readonly TestServer _server;
    private readonly MockHttpMessageHandler _mockClientHandler;
    private readonly Uri _layoutServiceUri = new("http://layout.service");

    public DateFieldTagHelperFixture()
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
                        .AddModelBoundView<ComponentModels.ComponentWithDates>("Component-With-Dates", "ComponentWithDates")
                        .AddDefaultComponentRenderer();
                });
            })
            .Configure(app =>
            {
                app.UseRouting();
                app.UseRequestLocalization(options =>
                {
                    var culture = new CultureInfo("da-DK");
                    options.DefaultRequestCulture = new RequestCulture(culture);
                    options.SupportedCultures = [culture];
                    options.SupportedUICultures = [culture];
                });
                app.UseSitecoreRenderingEngine();
                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapDefaultControllerRoute();
                });
            });

        _server = testHostBuilder.BuildServer(new Uri("http://localhost"));
    }

    [Fact]
    public async Task DateTagHelper_DoesNotResetOtherTagHelperOutput()
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
        HtmlNode? sectionNode = doc.DocumentNode.ChildNodes.First(n => n.HasClass("component-with-dates"));
        HtmlNode? text = sectionNode.ChildNodes[7];

        // Assert
        // check scenario that DateTagHelper does not reset values of nested helpers.
        text.InnerHtml.Should().Contain(TestConstants.TestFieldValue);
    }

    [Fact]
    public async Task DateTagHelper_GeneratesProperDate()
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
        HtmlNode? sectionNode = doc.DocumentNode.ChildNodes.First(n => n.HasClass("component-with-dates"));

        // Assert
        sectionNode.ChildNodes[1].InnerHtml.Should().Be("05/04/2012");
        sectionNode.ChildNodes[3].InnerHtml.Should().Be("05/04/2012 00:00:00");
        sectionNode.ChildNodes[5].InnerHtml.Should().Be(TestConstants.DateTimeValue.ToString(new CultureInfo("da-DK")));
        sectionNode.ChildNodes[9].InnerHtml.Should().Contain("04.05.2012");
    }

    public void Dispose()
    {
        _mockClientHandler.Dispose();
        _server.Dispose();
        GC.SuppressFinalize(this);
    }
}