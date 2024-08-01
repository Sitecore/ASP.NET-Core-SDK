using System.Net;
using System.Net.Http.Headers;

namespace Sitecore.AspNetCore.SDK.LayoutService.Client.Tests.MockModels;

public class HttpResponseMessageWrapper(HttpStatusCode statusCode)
    : HttpResponseMessage(statusCode)
{
    public new HttpResponseHeaders? Headers { get; set; }
}