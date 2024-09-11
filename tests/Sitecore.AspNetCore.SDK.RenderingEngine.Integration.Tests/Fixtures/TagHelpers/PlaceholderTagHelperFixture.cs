using System.Net;
using FluentAssertions;
using Microsoft.AspNetCore.TestHost;
using Sitecore.AspNetCore.SDK.AutoFixture.Mocks;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Extensions;
using Sitecore.AspNetCore.SDK.RenderingEngine.Extensions;
using Sitecore.AspNetCore.SDK.TestData;
using Xunit;

// ReSharper disable StringLiteralTypo
namespace Sitecore.AspNetCore.SDK.RenderingEngine.Integration.Tests.Fixtures.TagHelpers;

public class PlaceholderTagHelperFixture : IDisposable
{
    private readonly TestServer _server;
    private readonly MockHttpMessageHandler _mockClientHandler;
    private readonly Uri _layoutServiceUri = new("http://layout.service");

    public PlaceholderTagHelperFixture()
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
                        .AddViewComponent("Component-1", "Component1")
                        .AddModelBoundView<ComponentModels.Component2>("Component-2", "Component2")
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
    public async Task PageInNormalMode_WithNestedPlaceholderComponent_ComponentIsRenderedCorrectly()
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

        // Assert
        response.Should().Contain(TestConstants.RichTextFieldValue1);
    }

    [Fact]
    public async Task PageInEditableMode_WithComponentsAndChromes_ComponentAndChromesAreRenderedCorrectly()
    {
        // Arrange
        _mockClientHandler.Responses.Push(new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(Serializer.Serialize(CannedResponses.EditablePage))
        });

        HttpClient client = _server.CreateClient();

        // Act
        string response = await client.GetStringAsync(new Uri("/", UriKind.Relative));

        // Assert
        response.Should().Contain(TestConstants.RichTextFieldValue1);
        response.Should().Contain("<code type='text/sitecore' chrometype='placeholder' kind='open' id='placeholder_1' key='placeholder-1' class='scpm' data-selectable='true'>{\"commands\":[{\"click\":\"chrome:placeholder:addControl\",\"header\":\"Add to here\",\"icon\":\"/temp/iconcache/office/16x16/add.png\",\"disabledIcon\":\"/temp/add_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"Add a new rendering to the '{0}' placeholder.\",\"type\":\"\"},{\"click\":\"chrome:placeholder:editSettings\",\"header\":\"\",\"icon\":\"/temp/iconcache/office/16x16/window_gear.png\",\"disabledIcon\":\"/temp/window_gear_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"Edit the placeholder settings.\",\"type\":\"\"}],\"contextItemUri\":\"sitecore://master/{616E2DAA-BB71-5117-82B1-B360EF600213}?lang=en&ver=1\",\"custom\":{\"allowedRenderings\":[\"1DE91AADC1465D8983FA31A8FD63EBB3\",\"4E3C94B3A9D25478B7548D87283D8AA6\",\"26D9B310A5365D6B975442DB6BE1D381\",\"27EA18D87B6456108919947077956819\"],\"editable\":\"true\"},\"displayName\":\"Main\",\"expandedDisplayName\":null}</code>");
        response.Should().Contain("<code type='text/sitecore' chrometype='rendering' kind='open' id='r_E02DDB9BA0625E50924A1940D7E053CE' hintname='Component 1' class='scpm' data-selectable='true'>{\"commands\":[{\"click\":\"javascript:Sitecore.PageModes.PageEditor.postRequest('webedit:fieldeditor(command={2F7D34E2-EB93-47ED-8177-B1F429A9FCD7},fields=heading|content,id={585596CA-7903-500B-8DF2-0357DD6E3FAC})',null,false)\",\"header\":\"Edit Fields\",\"icon\":\"/temp/iconcache/office/16x16/pencil.png\",\"disabledIcon\":\"/temp/pencil_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"\",\"type\":null},{\"click\":\"chrome:dummy\",\"header\":\"Separator\",\"icon\":\"\",\"disabledIcon\":\"\",\"isDivider\":false,\"tooltip\":null,\"type\":\"separator\"},{\"click\":\"chrome:rendering:sort\",\"header\":\"Change position\",\"icon\":\"/temp/iconcache/office/16x16/document_size.png\",\"disabledIcon\":\"/temp/document_size_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"Move component.\",\"type\":\"\"},{\"click\":\"javascript:Sitecore.PageModes.PageEditor.postRequest('webedit:componentoptions(referenceId={E02DDB9B-A062-5E50-924A-1940D7E053CE},renderingId={1DE91AAD-C146-5D89-83FA-31A8FD63EBB3},id={585596CA-7903-500B-8DF2-0357DD6E3FAC})',null,false)\",\"header\":\"Edit Experience Editor Options\",\"icon\":\"/temp/iconcache/office/16x16/clipboard_check_edit.png\",\"disabledIcon\":\"/temp/clipboard_check_edit_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"Edit the Experience Editor options for the component.\",\"type\":\"common\"},{\"click\":\"chrome:rendering:properties\",\"header\":\"Edit component properties\",\"icon\":\"/temp/iconcache/office/16x16/elements_branch.png\",\"disabledIcon\":\"/temp/elements_branch_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"Edit the properties for the component.\",\"type\":\"common\"},{\"click\":\"javascript:Sitecore.PageModes.PageEditor.postRequest('webedit:setdatasource(referenceId={E02DDB9B-A062-5E50-924A-1940D7E053CE},renderingId={1DE91AAD-C146-5D89-83FA-31A8FD63EBB3},id={585596CA-7903-500B-8DF2-0357DD6E3FAC})',null,false)\",\"header\":\"dsHeaderParameter\",\"icon\":\"/temp/iconcache/office/16x16/data.png\",\"disabledIcon\":\"/temp/data_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"dsTooltipParameter\",\"type\":\"datasourcesmenu\"},{\"click\":\"chrome:rendering:personalize({command:\\\"webedit:personalize\\\"})\",\"header\":\"Personalize\",\"icon\":\"/temp/iconcache/office/16x16/users_family.png\",\"disabledIcon\":\"/temp/users_family_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"Create or edit personalization for this component.\",\"type\":\"sticky\"},{\"click\":\"chrome:common:edititem({command:\\\"webedit:open\\\"})\",\"header\":\"Edit the related item\",\"icon\":\"/temp/iconcache/office/16x16/cubes.png\",\"disabledIcon\":\"/temp/cubes_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"Edit the related item in the Content Editor.\",\"type\":\"datasourcesmenu\"},{\"click\":\"chrome:rendering:delete\",\"header\":\"Delete\",\"icon\":\"/temp/iconcache/office/16x16/delete.png\",\"disabledIcon\":\"/temp/delete_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"Remove component.\",\"type\":\"sticky\"}],\"contextItemUri\":\"sitecore://master/{585596CA-7903-500B-8DF2-0357DD6E3FAC}?lang=en&ver=1\",\"custom\":{\"renderingID\":\"1DE91AADC1465D8983FA31A8FD63EBB3\",\"editable\":\"true\"},\"displayName\":\"Content Block\",\"expandedDisplayName\":null}</code>");
        response.Should().Contain("<code type='text/sitecore' chrometype='placeholder' kind='open' id='_placeholder_1_placeholder_2__34A6553C_81DE_5CD3_989E_853F6CB6DF8C__0' key='/placeholder-1/placeholder-2-{34A6553C-81DE-5CD3-989E-853F6CB6DF8C}-0' class='scpm' data-selectable='true'>{\"commands\":[{\"click\":\"chrome:placeholder:addControl\",\"header\":\"Add to here\",\"icon\":\"/temp/iconcache/office/16x16/add.png\",\"disabledIcon\":\"/temp/add_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"Add a new rendering to the '{0}' placeholder.\",\"type\":\"\"},{\"click\":\"chrome:placeholder:editSettings\",\"header\":\"\",\"icon\":\"/temp/iconcache/office/16x16/window_gear.png\",\"disabledIcon\":\"/temp/window_gear_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"Edit the placeholder settings.\",\"type\":\"\"}],\"contextItemUri\":\"sitecore://master/{616E2DAA-BB71-5117-82B1-B360EF600213}?lang=en&ver=1\",\"custom\":{\"allowedRenderings\":[\"1DE91AADC1465D8983FA31A8FD63EBB3\",\"4E3C94B3A9D25478B7548D87283D8AA6\",\"26D9B310A5365D6B975442DB6BE1D381\",\"27EA18D87B6456108919947077956819\"],\"editable\":\"true\"},\"displayName\":\"Main\",\"expandedDisplayName\":null}</code>");
        response.Should().Contain("<code type='text/sitecore' chrometype='rendering' kind='open' id='r_B7C779DA2B75586CB40D081FCB864256' hintname='Component 2' class='scpm' data-selectable='true'>{\"commands\":[{\"click\":\"javascript:Sitecore.PageModes.PageEditor.postRequest('webedit:fieldeditor(command={2F7D34E2-EB93-47ED-8177-B1F429A9FCD7},fields=heading|content,id={585596CA-7903-500B-8DF2-0357DD6E3FAC})',null,false)\",\"header\":\"Edit Fields\",\"icon\":\"/temp/iconcache/office/16x16/pencil.png\",\"disabledIcon\":\"/temp/pencil_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"\",\"type\":null},{\"click\":\"chrome:dummy\",\"header\":\"Separator\",\"icon\":\"\",\"disabledIcon\":\"\",\"isDivider\":false,\"tooltip\":null,\"type\":\"separator\"},{\"click\":\"chrome:rendering:sort\",\"header\":\"Change position\",\"icon\":\"/temp/iconcache/office/16x16/document_size.png\",\"disabledIcon\":\"/temp/document_size_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"Move component.\",\"type\":\"\"},{\"click\":\"javascript:Sitecore.PageModes.PageEditor.postRequest('webedit:componentoptions(referenceId={E02DDB9B-A062-5E50-924A-1940D7E053CE},renderingId={1DE91AAD-C146-5D89-83FA-31A8FD63EBB3},id={585596CA-7903-500B-8DF2-0357DD6E3FAC})',null,false)\",\"header\":\"Edit Experience Editor Options\",\"icon\":\"/temp/iconcache/office/16x16/clipboard_check_edit.png\",\"disabledIcon\":\"/temp/clipboard_check_edit_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"Edit the Experience Editor options for the component.\",\"type\":\"common\"},{\"click\":\"chrome:rendering:properties\",\"header\":\"Edit component properties\",\"icon\":\"/temp/iconcache/office/16x16/elements_branch.png\",\"disabledIcon\":\"/temp/elements_branch_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"Edit the properties for the component.\",\"type\":\"common\"},{\"click\":\"javascript:Sitecore.PageModes.PageEditor.postRequest('webedit:setdatasource(referenceId={E02DDB9B-A062-5E50-924A-1940D7E053CE},renderingId={1DE91AAD-C146-5D89-83FA-31A8FD63EBB3},id={585596CA-7903-500B-8DF2-0357DD6E3FAC})',null,false)\",\"header\":\"dsHeaderParameter\",\"icon\":\"/temp/iconcache/office/16x16/data.png\",\"disabledIcon\":\"/temp/data_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"dsTooltipParameter\",\"type\":\"datasourcesmenu\"},{\"click\":\"chrome:rendering:personalize({command:\\\"webedit:personalize\\\"})\",\"header\":\"Personalize\",\"icon\":\"/temp/iconcache/office/16x16/users_family.png\",\"disabledIcon\":\"/temp/users_family_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"Create or edit personalization for this component.\",\"type\":\"sticky\"},{\"click\":\"chrome:common:edititem({command:\\\"webedit:open\\\"})\",\"header\":\"Edit the related item\",\"icon\":\"/temp/iconcache/office/16x16/cubes.png\",\"disabledIcon\":\"/temp/cubes_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"Edit the related item in the Content Editor.\",\"type\":\"datasourcesmenu\"},{\"click\":\"chrome:rendering:delete\",\"header\":\"Delete\",\"icon\":\"/temp/iconcache/office/16x16/delete.png\",\"disabledIcon\":\"/temp/delete_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"Remove component.\",\"type\":\"sticky\"}],\"contextItemUri\":\"sitecore://master/{585596CA-7903-500B-8DF2-0357DD6E3FAC}?lang=en&ver=1\",\"custom\":{\"renderingID\":\"1DE91AADC1465D8983FA31A8FD63EBB3\",\"editable\":\"true\"},\"displayName\":\"Content Block\",\"expandedDisplayName\":null}</code>");
        response.Should().Contain("<code type='text/sitecore' chrometype='rendering' kind='close' id='scEnclosingTag_r_' hintkey='Component 2' class='scpm'></code>");
        response.Should().Contain("<code type='text/sitecore' chrometype='rendering' kind='close' id='scEnclosingTag_r_' hintkey='Component 1' class='scpm'></code>");
        response.Should().Contain("<code type='text/sitecore' chrometype='placeholder' kind='close' id='scEnclosingTag_' hintname='Placeholder-2' class='scpm'></code>");
        response.Should().Contain("<code type='text/sitecore' chrometype='placeholder' kind='close' id='scEnclosingTag_' hintname='Placeholder-1' class='scpm'></code>");
    }

    [Fact]
    public async Task PageInHorizonEditableMode_WithComponentsAndChromes_ComponentAndChromesAreRenderedCorrectly()
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

        // Assert
        response.Should().Contain(TestConstants.RichTextFieldValue1);
        response.Should().Contain("<code type='text/sitecore' chrometype='placeholder' kind='open' id='placeholder_1' key='placeholder-1' class='scpm' data-selectable='true'>{\"contextItem\":{\"id\":\"8f7bef75-28a5-54f0-b7c4-998b51b67c75\",\"version\":1,\"language\":\"en\",\"revision\":\"60748843912c4eb5a66c94e9e275e52b\"},\"placeholderKey\":\"jss-main\",\"placeholderMetadataKeys\":[\"jss-main\"],\"editable\":true,\"commands\":[{\"click\":\"chrome:placeholder:addControl\",\"header\":\"Add to here\",\"icon\":\"/temp/iconcache/office/16x16/add.png\",\"disabledIcon\":\"/temp/add_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"Add a new rendering to the '{0}' placeholder.\",\"type\":\"\"},{\"click\":\"chrome:placeholder:editSettings\",\"header\":\"\",\"icon\":\"/temp/iconcache/office/16x16/window_gear.png\",\"disabledIcon\":\"/temp/window_gear_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"Edit the placeholder settings.\",\"type\":\"\"}],\"contextItemUri\":\"sitecore://master/{8F7BEF75-28A5-54F0-B7C4-998B51B67C75}?lang=en&ver=1\",\"custom\":{\"allowedRenderings\":[\"D8120D14950758B8BEF2E7A5158BD50F\",\"71AAF5C592425806BE7CECDF9154840B\",\"F351174D07C0547FBBDAEE51349C7DF5\",\"9B43C994B9835EC1A578401EA9478115\",\"FB33EEE82D6F5DD49CC71A5CBEBA728F\"],\"editable\":\"true\"},\"displayName\":\"Main\",\"expandedDisplayName\":null}</code>");
        response.Should().Contain("<code type='text/sitecore' chrometype='rendering' kind='open' id='r_E02DDB9BA0625E50924A1940D7E053CE' hintname='Component 1' class='scpm' data-selectable='true'>{\"contextItem\":{\"id\":\"a2484483-af6f-5723-a29f-785e12ced97b\",\"version\":1,\"language\":\"en\",\"revision\":\"c950fc1bd5484df88dc99bce389d51a0\"},\"renderingId\":\"71aaf5c5-9242-5806-be7c-ecdf9154840b\",\"renderingInstanceId\":\"{E02DDB9B-A062-5E50-924A-1940D7E053CE}\",\"editable\":true,\"commands\":[{\"click\":\"javascript:Sitecore.PageModes.PageEditor.postRequest('webedit:fieldeditor(command={2F7D34E2-EB93-47ED-8177-B1F429A9FCD7},fields=heading|content,id={A2484483-AF6F-5723-A29F-785E12CED97B})',null,false)\",\"header\":\"Edit Fields\",\"icon\":\"/temp/iconcache/office/16x16/pencil.png\",\"disabledIcon\":\"/temp/pencil_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"\",\"type\":null},{\"click\":\"chrome:dummy\",\"header\":\"Separator\",\"icon\":\"\",\"disabledIcon\":\"\",\"isDivider\":false,\"tooltip\":null,\"type\":\"separator\"},{\"click\":\"chrome:rendering:sort\",\"header\":\"Change position\",\"icon\":\"/temp/iconcache/office/16x16/document_size.png\",\"disabledIcon\":\"/temp/document_size_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"Move component.\",\"type\":\"\"},{\"click\":\"javascript:Sitecore.PageModes.PageEditor.postRequest('webedit:componentoptions(referenceId={E02DDB9B-A062-5E50-924A-1940D7E053CE},renderingId={71AAF5C5-9242-5806-BE7C-ECDF9154840B},id={A2484483-AF6F-5723-A29F-785E12CED97B})',null,false)\",\"header\":\"Edit Experience Editor Options\",\"icon\":\"/temp/iconcache/office/16x16/clipboard_check_edit.png\",\"disabledIcon\":\"/temp/clipboard_check_edit_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"Edit the Experience Editor options for the component.\",\"type\":\"common\"},{\"click\":\"chrome:rendering:properties\",\"header\":\"Edit component properties\",\"icon\":\"/temp/iconcache/office/16x16/elements_branch.png\",\"disabledIcon\":\"/temp/elements_branch_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"Edit the properties for the component.\",\"type\":\"common\"},{\"click\":\"javascript:Sitecore.PageModes.PageEditor.postRequest('webedit:setdatasource(referenceId={E02DDB9B-A062-5E50-924A-1940D7E053CE},renderingId={71AAF5C5-9242-5806-BE7C-ECDF9154840B},id={A2484483-AF6F-5723-A29F-785E12CED97B})',null,false)\",\"header\":\"dsHeaderParameter\",\"icon\":\"/temp/iconcache/office/16x16/data.png\",\"disabledIcon\":\"/temp/data_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"dsTooltipParameter\",\"type\":\"datasourcesmenu\"},{\"click\":\"chrome:rendering:personalize({command:\\\"webedit:personalize\\\"})\",\"header\":\"Personalize\",\"icon\":\"/temp/iconcache/office/16x16/users_family.png\",\"disabledIcon\":\"/temp/users_family_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"Create or edit personalization for this component.\",\"type\":\"sticky\"},{\"click\":\"chrome:common:edititem({command:\\\"webedit:open\\\"})\",\"header\":\"Edit the related item\",\"icon\":\"/temp/iconcache/office/16x16/cubes.png\",\"disabledIcon\":\"/temp/cubes_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"Edit the related item in the Content Editor.\",\"type\":\"datasourcesmenu\"},{\"click\":\"chrome:rendering:delete\",\"header\":\"Delete\",\"icon\":\"/temp/iconcache/office/16x16/delete.png\",\"disabledIcon\":\"/temp/delete_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"Remove component.\",\"type\":\"sticky\"}],\"contextItemUri\":\"sitecore://master/{A2484483-AF6F-5723-A29F-785E12CED97B}?lang=en&ver=1\",\"custom\":{\"renderingID\":\"71AAF5C592425806BE7CECDF9154840B\",\"editable\":\"true\"},\"displayName\":\"Content Block\",\"expandedDisplayName\":null}</code>");
        response.Should().Contain("<code type='text/sitecore' chrometype='placeholder' kind='open' id='_placeholder_1_placeholder_2__34A6553C_81DE_5CD3_989E_853F6CB6DF8C__0' key='/placeholder-1/placeholder-2-{34A6553C-81DE-5CD3-989E-853F6CB6DF8C}-0' class='scpm' data-selectable='true'>{\"contextItem\":{\"id\":\"8f7bef75-28a5-54f0-b7c4-998b51b67c75\",\"version\":1,\"language\":\"en\",\"revision\":\"60748843912c4eb5a66c94e9e275e52b\"},\"placeholderKey\":\"/jss-main/jss-styleguide-layout-{34A6553C-81DE-5CD3-989E-853F6CB6DF8C}-0\",\"placeholderMetadataKeys\":[\"/jss-main/jss-styleguide-layout\",\"jss-styleguide-layout\"],\"editable\":true,\"commands\":[{\"click\":\"chrome:placeholder:addControl\",\"header\":\"Add to here\",\"icon\":\"/temp/iconcache/office/16x16/add.png\",\"disabledIcon\":\"/temp/add_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"Add a new rendering to the '{0}' placeholder.\",\"type\":\"\"},{\"click\":\"chrome:placeholder:editSettings\",\"header\":\"\",\"icon\":\"/temp/iconcache/office/16x16/window_gear.png\",\"disabledIcon\":\"/temp/window_gear_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"Edit the placeholder settings.\",\"type\":\"\"}],\"contextItemUri\":\"sitecore://master/{8F7BEF75-28A5-54F0-B7C4-998B51B67C75}?lang=en&ver=1\",\"custom\":{\"allowedRenderings\":[\"A7BFC343F487521593488FBB0D209FA6\"],\"editable\":\"true\"},\"displayName\":\"jss-styleguide-layout\",\"expandedDisplayName\":null}</code>");
        response.Should().Contain("<code type='text/sitecore' chrometype='rendering' kind='open' id='r_B7C779DA2B75586CB40D081FCB864256' hintname='Component 4' class='scpm' data-selectable='true'>{\"contextItem\":{\"id\":\"9f76f747-bc96-572a-bbcb-9d7655f98ac2\",\"version\":1,\"language\":\"en\",\"revision\":\"39157d6881cc4ef5aba1dae028fd4fb9\"},\"renderingId\":\"a7bfc343-f487-5215-9348-8fbb0d209fa6\",\"renderingInstanceId\":\"{B7C779DA-2B75-586C-B40D-081FCB864256}\",\"editable\":true,\"commands\":[{\"click\":\"javascript:Sitecore.PageModes.PageEditor.postRequest('webedit:fieldeditor(command={2F7D34E2-EB93-47ED-8177-B1F429A9FCD7},fields=heading,id={9F76F747-BC96-572A-BBCB-9D7655F98AC2})',null,false)\",\"header\":\"Edit Fields\",\"icon\":\"/temp/iconcache/office/16x16/pencil.png\",\"disabledIcon\":\"/temp/pencil_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"\",\"type\":null},{\"click\":\"chrome:dummy\",\"header\":\"Separator\",\"icon\":\"\",\"disabledIcon\":\"\",\"isDivider\":false,\"tooltip\":null,\"type\":\"separator\"},{\"click\":\"chrome:rendering:sort\",\"header\":\"Change position\",\"icon\":\"/temp/iconcache/office/16x16/document_size.png\",\"disabledIcon\":\"/temp/document_size_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"Move component.\",\"type\":\"\"},{\"click\":\"javascript:Sitecore.PageModes.PageEditor.postRequest('webedit:componentoptions(referenceId={B7C779DA-2B75-586C-B40D-081FCB864256},renderingId={A7BFC343-F487-5215-9348-8FBB0D209FA6},id={9F76F747-BC96-572A-BBCB-9D7655F98AC2})',null,false)\",\"header\":\"Edit Experience Editor Options\",\"icon\":\"/temp/iconcache/office/16x16/clipboard_check_edit.png\",\"disabledIcon\":\"/temp/clipboard_check_edit_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"Edit the Experience Editor options for the component.\",\"type\":\"common\"},{\"click\":\"chrome:rendering:properties\",\"header\":\"Edit component properties\",\"icon\":\"/temp/iconcache/office/16x16/elements_branch.png\",\"disabledIcon\":\"/temp/elements_branch_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"Edit the properties for the component.\",\"type\":\"common\"},{\"click\":\"javascript:Sitecore.PageModes.PageEditor.postRequest('webedit:setdatasource(referenceId={B7C779DA-2B75-586C-B40D-081FCB864256},renderingId={A7BFC343-F487-5215-9348-8FBB0D209FA6},id={9F76F747-BC96-572A-BBCB-9D7655F98AC2})',null,false)\",\"header\":\"dsHeaderParameter\",\"icon\":\"/temp/iconcache/office/16x16/data.png\",\"disabledIcon\":\"/temp/data_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"dsTooltipParameter\",\"type\":\"datasourcesmenu\"},{\"click\":\"chrome:rendering:personalize({command:\\\"webedit:personalize\\\"})\",\"header\":\"Personalize\",\"icon\":\"/temp/iconcache/office/16x16/users_family.png\",\"disabledIcon\":\"/temp/users_family_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"Create or edit personalization for this component.\",\"type\":\"sticky\"},{\"click\":\"chrome:common:edititem({command:\\\"webedit:open\\\"})\",\"header\":\"Edit the related item\",\"icon\":\"/temp/iconcache/office/16x16/cubes.png\",\"disabledIcon\":\"/temp/cubes_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"Edit the related item in the Content Editor.\",\"type\":\"datasourcesmenu\"},{\"click\":\"chrome:rendering:delete\",\"header\":\"Delete\",\"icon\":\"/temp/iconcache/office/16x16/delete.png\",\"disabledIcon\":\"/temp/delete_disabled16x16.png\",\"isDivider\":false,\"tooltip\":\"Remove component.\",\"type\":\"sticky\"}],\"contextItemUri\":\"sitecore://master/{9F76F747-BC96-572A-BBCB-9D7655F98AC2}?lang=en&ver=1\",\"custom\":{\"renderingID\":\"A7BFC343F487521593488FBB0D209FA6\",\"editable\":\"true\"},\"displayName\":\"Styleguide-Section\",\"expandedDisplayName\":null}</code>");
        response.Should().Contain("<code type='text/sitecore' chrometype='rendering' kind='close' id='scEnclosingTag_r_' hintkey='Component 2' class='scpm'></code>");
        response.Should().Contain("<code type='text/sitecore' chrometype='rendering' kind='close' id='scEnclosingTag_r_' hintkey='Component 1' class='scpm'></code>");
        response.Should().Contain("<code type='text/sitecore' chrometype='placeholder' kind='close' id='scEnclosingTag_' hintname='Placeholder-2' class='scpm'></code>");
        response.Should().Contain("<code type='text/sitecore' chrometype='placeholder' kind='close' id='scEnclosingTag_' hintname='Placeholder-1' class='scpm'></code>");
    }

    public void Dispose()
    {
        _mockClientHandler.Dispose();
        _server.Dispose();
        GC.SuppressFinalize(this);
    }
}