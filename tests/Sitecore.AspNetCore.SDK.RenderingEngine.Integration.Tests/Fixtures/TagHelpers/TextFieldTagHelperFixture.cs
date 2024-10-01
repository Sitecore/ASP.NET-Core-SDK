using System.Net;
using System.Text.Encodings.Web;
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

public class TextFieldTagHelperFixture : IDisposable
{
    private readonly TestServer _server;
    private readonly MockHttpMessageHandler _mockClientHandler;
    private readonly Uri _layoutServiceUri = new("http://layout.service");

    public TextFieldTagHelperFixture()
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
                        .AddModelBoundView<ComponentModels.Component3>("Component-3", "Component3")
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
    public async Task TextFieldTagHelper_RendersFieldsCorrectly()
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
        HtmlNode? sectionNode = doc.DocumentNode.ChildNodes.First(n => n.HasClass("component-3"));

        // Assert
        sectionNode.ChildNodes.First(n => n.Name.Equals("h1", StringComparison.OrdinalIgnoreCase)).InnerText.Should().Be(TestConstants.TestFieldValue);

        sectionNode.ChildNodes.First(n => n.Name.Equals("div", StringComparison.OrdinalIgnoreCase)).InnerText
            .Should().BeEmpty();

        sectionNode.ChildNodes.First(n => n.Name.Equals("p", StringComparison.OrdinalIgnoreCase)).InnerText
            .Should().BeEmpty();

        sectionNode.ChildNodes.First(n => n.Name.Equals("textarea", StringComparison.OrdinalIgnoreCase)).InnerText
            .Should().Be(HtmlEncoder.Default.Encode(TestConstants.RichTextFieldValue1));

        sectionNode.ChildNodes.First(n => n.Name.Equals("span", StringComparison.OrdinalIgnoreCase)).InnerHtml
            .Should().Be(TestConstants.TestMultilineFieldValue.Replace(Environment.NewLine, "<br>", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public async Task TextFieldTagHelper_RendersEditableFieldsCorrectly()
    {
        // Arrange
        _mockClientHandler.Responses.Push(new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(Serializer.Serialize(CannedResponses.HorizonEditablePage))
        });

        HttpClient client = _server.CreateClient();

        // Act
        string response = await client.GetStringAsync(new Uri("/", UriKind.Relative));

        HtmlDocument doc = new();

        doc.LoadHtml(response);
        HtmlNode? sectionNode = doc.DocumentNode.ChildNodes.First(n => n.HasClass("component-3"));

        HtmlDocument expected = new();
        expected.LoadHtml("<input id='fld_8F7BEF7528A554F0B7C4998B51B67C75_152F40EDFE765861B425522375549742_en_1_60748843912c4eb5a66c94e9e275e52b_391' class='scFieldValue' name='fld_8F7BEF7528A554F0B7C4998B51B67C75_152F40EDFE765861B425522375549742_en_1_60748843912c4eb5a66c94e9e275e52b_391' type='hidden' value=\"" +
                          HtmlEncoder.Default.Encode(TestConstants.TestFieldValue) +
                          "\" /><span class=\"scChromeData\">{\"contextItem\":{\"id\":\"8f7bef75-28a5-54f0-b7c4-998b51b67c75\",\"version\":1,\"language\":\"en\",\"revision\":\"60748843912c4eb5a66c94e9e275e52b\"},\"fieldId\":\"152f40ed-fe76-5861-b425-522375549742\",\"fieldType\":\"Single-Line Text\",\"fieldWebEditParameters\":{\"prevent-line-break\":\"true\"},\"commands\":[{\"click\":\"chrome:common:edititem({command:\\\"webedit:open\\\"})\",\"header\":\"Edit the related item\",\"icon\":\"/temp/iconcache/office/16x16/cubes.png\",\"disabledIcon\":\"/temp/cubes_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"Edit the related item in the Content Editor.\",\"type\":\"common\"},{\"click\":\"chrome:rendering:personalize({command:\\\"webedit:personalize\\\"})\",\"header\":\"Personalize\",\"icon\":\"/temp/iconcache/office/16x16/users_family.png\",\"disabledIcon\":\"/temp/users_family_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"Create or edit personalization for this component.\",\"type\":\"sticky\"}],\"contextItemUri\":\"sitecore://master/{8F7BEF75-28A5-54F0-B7C4-998B51B67C75}?lang=en&ver=1\",\"custom\":{},\"displayName\":\"Page Title\",\"expandedDisplayName\":null}</span><span id=\"fld_8F7BEF7528A554F0B7C4998B51B67C75_152F40EDFE765861B425522375549742_en_1_60748843912c4eb5a66c94e9e275e52b_391_edit\" sc_parameters=\"prevent-line-break=true\" contenteditable=\"true\" class=\"scWebEditInput\" scFieldType=\"single-line text\" scDefaultText=\"[No text in field]\">" +
                          TestConstants.TestFieldValue + "</span>");

        // Assert
        // TestField is editable
        sectionNode.ChildNodes.First(n => n.Name.Equals("h1", StringComparison.OrdinalIgnoreCase)).InnerHtml
            .Should().Be(expected.DocumentNode.InnerHtml);

        // EmptyField is NOT editable
        sectionNode.ChildNodes.First(n => n.Name.Equals("div", StringComparison.OrdinalIgnoreCase)).InnerHtml
            .Should().Be(string.Empty);
    }

    public void Dispose()
    {
        _mockClientHandler.Dispose();
        _server.Dispose();
        GC.SuppressFinalize(this);
    }
}