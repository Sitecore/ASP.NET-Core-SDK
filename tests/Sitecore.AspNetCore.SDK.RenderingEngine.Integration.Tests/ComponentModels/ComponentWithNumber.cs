using Sitecore.AspNetCore.SDK.LayoutService.Client.Response.Model.Fields;

namespace Sitecore.AspNetCore.SDK.RenderingEngine.Integration.Tests.ComponentModels;

public class ComponentWithNumber
{
    public NumberField? Number { get; set; }

    public TextField? Text { get; set; }
}