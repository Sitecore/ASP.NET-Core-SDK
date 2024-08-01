using Sitecore.AspNetCore.SDK.LayoutService.Client.Response.Model;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Response.Model.Fields;

namespace Sitecore.AspNetCore.SDK.LayoutService.Client.Tests.Response.Model;

public class SimpleTestModel
{
    public TextField? Text { get; set; }

    public Field<decimal?>? Number { get; set; }
}