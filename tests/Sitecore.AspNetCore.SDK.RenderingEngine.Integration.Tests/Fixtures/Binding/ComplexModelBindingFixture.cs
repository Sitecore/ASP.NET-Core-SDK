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

public class ComplexModelBindingFixture : IClassFixture<TestWebApplicationFactory<TestWebApplicationProgram>>, IDisposable
{
    private readonly MockHttpMessageHandler _mockClientHandler = new();
    private readonly Uri _layoutServiceUri = new("http://layout.service");

    private readonly WebApplicationFactory<TestWebApplicationProgram> _factory;

    public ComplexModelBindingFixture(TestWebApplicationFactory<TestWebApplicationProgram> factory)
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
                        .AddModelBoundView<ComponentModels.ComplexComponent>(name => name.Equals("Complex-Component", StringComparison.OrdinalIgnoreCase), "ComplexComponent")
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
    public async Task SitecoreLayoutModelBinders_BindDataCorrectly()
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
        HtmlNode? sectionNode = doc.DocumentNode.ChildNodes.First(n => n.HasClass("complex-component"));

        // Assert
        sectionNode.ChildNodes.First(n => n.Id.Equals("fieldHeader", StringComparison.OrdinalIgnoreCase)).InnerText
            .Should().Be(TestConstants.PageTitle);

        sectionNode.ChildNodes.First(n => n.Id.Equals("routeField", StringComparison.OrdinalIgnoreCase)).InnerText
            .Should().Be(TestConstants.PageTitle);

        sectionNode.ChildNodes.First(n => n.Id.Equals("routeNestedField", StringComparison.OrdinalIgnoreCase)).InnerText
            .Should().Be(TestConstants.SearchKeywords);

        sectionNode.ChildNodes.First(n => n.Id.Equals("componentProperty", StringComparison.OrdinalIgnoreCase)).InnerText
            .Should().Be("Complex-Component");

        sectionNode.ChildNodes.First(n => n.Id.Equals("routeProperty", StringComparison.OrdinalIgnoreCase)).InnerText.
            Should().Be("styleguide");

        sectionNode.ChildNodes.First(n => n.Id.Equals("fieldContent", StringComparison.OrdinalIgnoreCase)).InnerHtml.
            Should().Be(TestConstants.RichTextFieldValue1);

        sectionNode.ChildNodes.First(n => n.Id.Equals("contextProperty", StringComparison.OrdinalIgnoreCase)).InnerText
            .Should().Contain("False");

        sectionNode.ChildNodes.First(n => n.Id.Equals("nestedComponentHeaderField", StringComparison.OrdinalIgnoreCase)).InnerText
            .Should().Be(TestConstants.PageTitle);

        sectionNode.ChildNodes.First(n => n.Id.Equals("nestedComponentHeader2Field", StringComparison.OrdinalIgnoreCase)).InnerText
            .Should().Contain(TestConstants.TestFieldValue);

        sectionNode.ChildNodes.First(n => n.Id.Contains("customField", StringComparison.OrdinalIgnoreCase)).InnerText
            .Should().Contain(TestConstants.TestFieldValue + "custom");

        sectionNode.ChildNodes.First(n => n.Id.Equals("paramContent", StringComparison.OrdinalIgnoreCase)).InnerText
            .Should().Be(TestConstants.TestParamNameValue);
    }

    public void Dispose()
    {
        _mockClientHandler.Dispose();
        _factory.Dispose();
        GC.SuppressFinalize(this);
    }
}