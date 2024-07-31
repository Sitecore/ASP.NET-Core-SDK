using Sitecore.AspNetCore.SDK.LayoutService.Client.Response.Model;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Response.Model.Fields;

namespace Sitecore.AspNetCore.SDK.RenderingEngine.Integration.Tests.ComponentModels;

public class CustomClass
{
    public TextField? Header { get; set; }

    public Field<string>? Header2 { get; set; }
}