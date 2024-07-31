using Sitecore.AspNetCore.SDK.LayoutService.Client.Interfaces;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Request;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Response;

namespace Sitecore.AspNetCore.SDK.LayoutService.Client.Tests.Extensions;

public abstract class TestAbstractSitecoreLayoutRequestHandler : ILayoutRequestHandler
{
    /// <inheritdoc />
    public abstract Task<SitecoreLayoutResponse> Request(SitecoreLayoutRequest request, string handlerName);
}