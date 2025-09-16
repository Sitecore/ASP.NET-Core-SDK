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

public class ViewFieldsBindingFixture(TestWebApplicationFactory<TestWebApplicationProgram> factory) : IClassFixture<TestWebApplicationFactory<TestWebApplicationProgram>>, IDisposable
{
    private readonly MockHttpMessageHandler _mockClientHandler = new();
    private readonly Uri _layoutServiceUri = new("http://layout.service");

    [Fact]
    public async Task SitecoreLayoutModelBinders_BindDataCorrectly()
    {
        // Arrange
        _mockClientHandler.Responses.Push(new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(Serializer.Serialize(CannedResponses.WithNestedPlaceholder))
        });

        HttpClient client = BuildViewFieldsBindingWebApplicationFactory().CreateClient();

        // Act
        string response = await client.GetStringAsync(new Uri("/", UriKind.Relative));

        HtmlDocument doc = new();
        doc.LoadHtml(response);
        HtmlNode? sectionNode = doc.DocumentNode.ChildNodes.First(n => n.HasClass("component-5"));

        // Assert
        sectionNode.ChildNodes.First(n => n.Name.Equals("h1", StringComparison.OrdinalIgnoreCase)).InnerText.Should().Be(TestConstants.TestFieldValue);

        sectionNode.ChildNodes.First(n => n.Name.Equals("div", StringComparison.OrdinalIgnoreCase)).InnerText
            .Should().BeEmpty();

        sectionNode.ChildNodes.First(n => n.Name.Equals("p", StringComparison.OrdinalIgnoreCase)).InnerText
            .Should().BeEmpty();

        sectionNode.ChildNodes.First(n => n.Name.Equals("textarea", StringComparison.OrdinalIgnoreCase)).InnerText.Should().Contain("12/12/2019");

        sectionNode.ChildNodes.First(n => n.Name.Equals("span", StringComparison.OrdinalIgnoreCase)).InnerHtml
            .Should().Be(TestConstants.TestMultilineFieldValue.Replace(Environment.NewLine, "<br>", StringComparison.OrdinalIgnoreCase));
    }

    public void Dispose()
    {
        _mockClientHandler.Dispose();
        GC.SuppressFinalize(this);
    }

    private WebApplicationFactory<TestWebApplicationProgram> BuildViewFieldsBindingWebApplicationFactory()
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
                            .AddModelBoundView<ComponentModels.Component5>(name => name.Equals("Component-5", StringComparison.OrdinalIgnoreCase), "Component5")
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