using Sitecore.AspNetCore.SDK.LayoutService.Client.Response.Model.Fields;

namespace Sitecore.AspNetCore.SDK.RenderingEngine.Integration.Tests.ComponentModels;

public class Component4
{
    public DateField? Date { get; set; }

    public RichTextField? RichTextField1 { get; set; }

    public RichTextField? RichTextField2 { get; set; }

    public RichTextField? EmptyField { get; set; }

    public RichTextField? NullValueField { get; set; }

    public TextField? TestField { get; set; }
}