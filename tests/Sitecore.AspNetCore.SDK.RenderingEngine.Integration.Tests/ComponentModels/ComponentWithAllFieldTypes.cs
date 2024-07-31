using Sitecore.AspNetCore.SDK.LayoutService.Client.Response.Model.Fields;

namespace Sitecore.AspNetCore.SDK.RenderingEngine.Integration.Tests.ComponentModels;

public class ComponentWithAllFieldTypes
{
    public TextField? TextField { get; set; }

    public TextField? MultiLineField { get; set; }

    public RichTextField? RichTextField1 { get; set; }

    public RichTextField? RichTextField2 { get; set; }

    public HyperLinkField? LinkField { get; set; }

    public ImageField? ImageField { get; set; }

    public ImageField? MediaLibraryItemImageField { get; set; }

    public DateField? DateField { get; set; }

    public NumberField? NumberField { get; set; }
}