using Sitecore.AspNetCore.SDK.LayoutService.Client.Response.Model.Fields;

namespace Sitecore.AspNetCore.SDK.RenderingEngine.Integration.Tests.ComponentModels;

public class ComponentWithImages
{
    public ImageField? FirstImage { get; set; }

    public ImageField? SecondImage { get; set; }

    public TextField? Heading { get; set; }

    public ImageField? ThirdImage { get; set; }

    public ImageField? FourthImage { get; set; }

}