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

public class RichTextFieldTagHelperFixture : IDisposable
{
    private readonly TestServer _server;
    private readonly MockHttpMessageHandler _mockClientHandler;
    private readonly Uri _layoutServiceUri = new("http://layout.service");

    public RichTextFieldTagHelperFixture()
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
                        .AddModelBoundView<ComponentModels.Component4>("Component-4", "Component4")
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
    public async Task RichTextFieldTagHelper_DoesNotResetOtherTagHelperOutput()
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
        HtmlNode? sectionNode = doc.DocumentNode.ChildNodes.First(n => n.HasClass("component-4"));

        // Assert
        // check scenario that RichTextTagHelper does not reset values of another helpers.
        sectionNode.ChildNodes.First(n => n.Name.Equals("textarea", StringComparison.OrdinalIgnoreCase)).InnerText.Should().Contain("12/12/2019");
    }

    [Fact]
    public async Task RichTextFieldTagHelper_RendersFieldsCorrectly()
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
        HtmlNode? sectionNode = doc.DocumentNode.ChildNodes.First(n => n.HasClass("component-4"));

        // Assert
        sectionNode.ChildNodes.First(n => n.Name.Equals("div", StringComparison.OrdinalIgnoreCase) && n.Id.Equals("div1", StringComparison.OrdinalIgnoreCase)).InnerHtml
            .Should().Be(TestConstants.RichTextFieldValue1);
        sectionNode.ChildNodes.First(n => n.Name.Equals("div", StringComparison.OrdinalIgnoreCase) && n.Id.Equals("div2", StringComparison.OrdinalIgnoreCase)).InnerHtml
            .Should().Be(TestConstants.RichTextFieldValue2);
        sectionNode.ChildNodes.First(n => n.Name.Equals("div", StringComparison.OrdinalIgnoreCase) && n.Id.Equals("div3", StringComparison.OrdinalIgnoreCase)).InnerHtml
            .Should().BeEmpty();
        sectionNode.ChildNodes.First(n => n.Name.Equals("div", StringComparison.OrdinalIgnoreCase) && n.Id.Equals("div4", StringComparison.OrdinalIgnoreCase)).InnerHtml
            .Should().BeEmpty();
        sectionNode.ChildNodes.First(n => n.Name.Equals("div", StringComparison.OrdinalIgnoreCase) && n.Id.Equals("div5", StringComparison.OrdinalIgnoreCase)).InnerHtml
            .Should().Be(TestConstants.TestFieldValue);
    }

    [Fact]
    public async Task RichTextFieldTagHelper_RendersEditableFieldsCorrectly()
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
        HtmlNode? sectionNode = doc.DocumentNode.ChildNodes.First(n => n.HasClass("component-4"));

        HtmlDocument expected = new();
        expected.LoadHtml("<input id='fld_A2484483AF6F5723A29F785E12CED97B_6856AF27B4135FCEB3FDC560612F1199_en_1_c950fc1bd5484df88dc99bce389d51a0_393' class='scFieldValue' name='fld_A2484483AF6F5723A29F785E12CED97B_6856AF27B4135FCEB3FDC560612F1199_en_1_c950fc1bd5484df88dc99bce389d51a0_393' type='hidden' value=\"" +
                          HtmlEncoder.Default.Encode(TestConstants.RichTextFieldValue1) +
                          "\"/><span class=\"scChromeData\">{\"contextItem\":{\"id\":\"a2484483-af6f-5723-a29f-785e12ced97b\",\"version\":1,\"language\":\"en\",\"revision\":\"c950fc1bd5484df88dc99bce389d51a0\"},\"fieldId\":\"6856af27-b413-5fce-b3fd-c560612f1199\",\"fieldType\":\"Rich Text\",\"fieldWebEditParameters\":{},\"commands\":[{\"click\":\"chrome:field:editcontrol({command:\\\"webedit:edithtml\\\"})\",\"header\":\"Edit Text\",\"icon\":\"/temp/iconcache/office/16x16/pencil.png\",\"disabledIcon\":\"/temp/pencil_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"Edit the text\",\"type\":null},{\"click\":\"chrome:field:execute({command:\\\"bold\\\", userInterface:true, value:true})\",\"header\":\"\",\"icon\":\"/temp/iconcache/office/16x16/font_style_bold.png\",\"disabledIcon\":\"/temp/font_style_bold_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"Bold\",\"type\":null},{\"click\":\"chrome:field:execute({command:\\\"Italic\\\", userInterface:true, value:true})\",\"header\":\"\",\"icon\":\"/temp/iconcache/office/16x16/font_style_italics.png\",\"disabledIcon\":\"/temp/font_style_italics_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"Italic\",\"type\":null},{\"click\":\"chrome:field:execute({command:\\\"Underline\\\", userInterface:true, value:true})\",\"header\":\"\",\"icon\":\"/temp/iconcache/office/16x16/font_style_underline.png\",\"disabledIcon\":\"/temp/font_style_underline_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"Underline\",\"type\":null},{\"click\":\"chrome:field:insertexternallink\",\"header\":\"\",\"icon\":\"/temp/iconcache/office/16x16/earth_link.png\",\"disabledIcon\":\"/temp/earth_link_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"Insert an external link into the text field.\",\"type\":null},{\"click\":\"chrome:field:insertlink\",\"header\":\"\",\"icon\":\"/temp/iconcache/office/16x16/link.png\",\"disabledIcon\":\"/temp/link_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"Insert a link into the text field.\",\"type\":null},{\"click\":\"chrome:field:removelink\",\"header\":\"\",\"icon\":\"/temp/iconcache/office/16x16/link_broken.png\",\"disabledIcon\":\"/temp/link_broken_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"Remove link.\",\"type\":null},{\"click\":\"chrome:field:insertimage\",\"header\":\"Insert image\",\"icon\":\"/temp/iconcache/office/16x16/photo_landscape.png\",\"disabledIcon\":\"/temp/photo_landscape_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"Insert an image into the text field.\",\"type\":null},{\"click\":\"chrome:common:edititem({command:\\\"webedit:open\\\"})\",\"header\":\"Edit the related item\",\"icon\":\"/temp/iconcache/office/16x16/cubes.png\",\"disabledIcon\":\"/temp/cubes_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"Edit the related item in the Content Editor.\",\"type\":\"common\"},{\"click\":\"chrome:rendering:personalize({command:\\\"webedit:personalize\\\"})\",\"header\":\"Personalize\",\"icon\":\"/temp/iconcache/office/16x16/users_family.png\",\"disabledIcon\":\"/temp/users_family_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"Create or edit personalization for this component.\",\"type\":\"sticky\"}],\"contextItemUri\":\"sitecore://master/{A2484483-AF6F-5723-A29F-785E12CED97B}?lang=en&ver=1\",\"custom\":{},\"displayName\":\"content\",\"expandedDisplayName\":null}</span><span scFieldType=\"rich text\" scDefaultText=\"[No text in field]\" contenteditable=\"true\" class=\"scWebEditInput\" id=\"fld_A2484483AF6F5723A29F785E12CED97B_6856AF27B4135FCEB3FDC560612F1199_en_1_c950fc1bd5484df88dc99bce389d51a0_393_edit\">" +
                          TestConstants.RichTextFieldValue1 + "</span>");

        // Assert
        // RichTextField1 is editable
        sectionNode.ChildNodes.First(n => n.Name.Equals("div", StringComparison.OrdinalIgnoreCase) && n.Id.Equals("div1", StringComparison.OrdinalIgnoreCase)).InnerHtml
            .Should().Be(expected.DocumentNode.InnerHtml);

        // RichTextField2 is NOT editable
        sectionNode.ChildNodes.First(n => n.Name.Equals("div", StringComparison.OrdinalIgnoreCase) && n.Id.Equals("div2", StringComparison.OrdinalIgnoreCase)).InnerHtml
            .Should().Be(TestConstants.RichTextFieldValue2);
    }

    public void Dispose()
    {
        _mockClientHandler.Dispose();
        _server.Dispose();
        GC.SuppressFinalize(this);
    }
}