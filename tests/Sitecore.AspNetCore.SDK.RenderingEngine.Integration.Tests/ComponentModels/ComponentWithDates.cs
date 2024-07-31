using Sitecore.AspNetCore.SDK.LayoutService.Client.Response.Model.Fields;

namespace Sitecore.AspNetCore.SDK.RenderingEngine.Integration.Tests.ComponentModels;

public class ComponentWithDates
{
    public DateField? Date { get; set; }

    public TextField? Text { get; set; }
}