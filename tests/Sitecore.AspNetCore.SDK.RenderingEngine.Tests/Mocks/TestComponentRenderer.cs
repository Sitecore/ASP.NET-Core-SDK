using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Sitecore.AspNetCore.SDK.RenderingEngine.Interfaces;
using Sitecore.AspNetCore.SDK.RenderingEngine.Rendering;

namespace Sitecore.AspNetCore.SDK.RenderingEngine.Tests.Mocks;

internal class TestComponentRenderer : IComponentRenderer
{
    public const string HtmlContent = "<blink>Test Component</blink>";

    public const string ChromeContent = """{"commands":[{"click":"chrome:placeholder:addControl","header":"Add to here","icon":"/temp/iconcache/office/16x16/add.png","disabledIcon":"/temp/add_disabled16x16.png","isDivider":false,"tooltip":"Add a new rendering to the '{0}' placeholder.","type":""},{"click":"chrome:placeholder:editSettings","header":"","icon":"/temp/iconcache/office/16x16/window_gear.png","disabledIcon":"/temp/window_gear_disabled16x16.png","isDivider":false,"tooltip":"Edit the placeholder settings.","type":""}],"contextItemUri":"sitecore://master/{616E2DAA-BB71-5117-82B1-B360EF600213}?lang=en&ver=1","custom":{"allowedRenderings":["1DE91AADC1465D8983FA31A8FD63EBB3","4E3C94B3A9D25478B7548D87283D8AA6","26D9B310A5365D6B975442DB6BE1D381","27EA18D87B6456108919947077956819"],"editable":"true"},"displayName":"Main","expandedDisplayName":null}""";

    public Task<IHtmlContent> Render(ISitecoreRenderingContext renderingContext, ViewContext viewContext)
    {
        IHtmlContent htmlContent = new HtmlString(HtmlContent);
        return Task.FromResult(htmlContent);
    }
}