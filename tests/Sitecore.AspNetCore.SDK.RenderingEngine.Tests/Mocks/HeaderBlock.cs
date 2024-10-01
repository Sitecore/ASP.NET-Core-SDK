using System.Diagnostics.CodeAnalysis;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Response.Model.Fields;

namespace Sitecore.AspNetCore.SDK.RenderingEngine.Tests.Mocks;

[ExcludeFromCodeCoverage]
public class HeaderBlock
{
    public TextField? Heading1 { get; set; }

    public TextField? Heading2 { get; set; }
}