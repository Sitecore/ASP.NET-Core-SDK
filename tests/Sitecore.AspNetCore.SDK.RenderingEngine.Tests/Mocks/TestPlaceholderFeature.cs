using System.Diagnostics.CodeAnalysis;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Response.Model;

namespace Sitecore.AspNetCore.SDK.RenderingEngine.Tests.Mocks;

[ExcludeFromCodeCoverage]
public class TestPlaceholderFeature : IPlaceholderFeature
{
    public string? Content { get; set; }
}