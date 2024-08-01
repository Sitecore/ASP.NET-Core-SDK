using Sitecore.AspNetCore.SDK.LayoutService.Client.Response.Model.Fields;

namespace Sitecore.AspNetCore.SDK.RenderingEngine.Integration.Tests.ComponentModels;

public class Component3
{
    public TextField? TestField { get; set; }

    public TextField? EmptyField { get; set; }

    public TextField? NullValueField { get; set; }

    public TextField? MultiLineField { get; set; }

    public TextField? EncodedField { get; set; }
}