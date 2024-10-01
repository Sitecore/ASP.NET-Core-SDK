using System.Net;
using AutoFixture;
using FluentAssertions;
using HtmlAgilityPack;
using Microsoft.AspNetCore.TestHost;
using Sitecore.AspNetCore.SDK.AutoFixture.Attributes;
using Sitecore.AspNetCore.SDK.AutoFixture.Mocks;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Exceptions;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Extensions;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Interfaces;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Request;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Response;
using Sitecore.AspNetCore.SDK.RenderingEngine.Extensions;
using Xunit;

// ReSharper disable StringLiteralTypo
namespace Sitecore.AspNetCore.SDK.RenderingEngine.Integration.Tests.Fixtures;

public class ErrorHandlingFixture
{
    private const string HttpStatusCodeKeyName = "HTTP Status Code";

    public static Action<IFixture> ValidHttpClient => f =>
    {
        TestServerBuilder testHostBuilder = new();
        MockHttpMessageHandler mockClientHandler = new();
        testHostBuilder
            .ConfigureServices(builder =>
            {
                builder
                    .AddSitecoreLayoutService()
                    .AddHttpHandler("valid", _ => new HttpClient(mockClientHandler) { BaseAddress = new Uri("http://layout.service") })
                    .AsDefaultHandler();
            })
            .Configure(app =>
            {
                app.UseSitecoreRenderingEngine();
            });

        TestServer server = testHostBuilder.BuildServer(new Uri("http://localhost"));

        f.Inject(mockClientHandler);
        f.Inject(server);
    };

    public static Action<IFixture> InvalidHttpClient => f =>
    {
        TestServerBuilder testHostBuilder = new();
        MockHttpMessageHandler mockClientHandler = new();
        testHostBuilder
            .ConfigureServices(builder =>
            {
                builder
                    .AddSitecoreLayoutService()
                    .AddHttpHandler("invalid", _ => new HttpClient { BaseAddress = new Uri("http://invalid.url") })
                    .AsDefaultHandler();
            })
            .Configure(app =>
            {
                app.UseSitecoreRenderingEngine();
            });

        TestServer server = testHostBuilder.BuildServer(new Uri("http://localhost"));

        f.Inject(mockClientHandler);
        f.Inject(server);
    };

    public static Action<IFixture> InvalidHttpMessageConfiguration => f =>
    {
        TestServerBuilder testHostBuilder = new();
        MockHttpMessageHandler mockClientHandler = new();
        testHostBuilder
            .ConfigureServices(builder =>
            {
                builder
                    .AddSitecoreLayoutService()
                    .AddHttpHandler("valid", _ => new HttpClient(mockClientHandler) { BaseAddress = new Uri("http://layout.service") })
                    .MapFromRequest((layoutRequest, httpMessage) => httpMessage.Headers.Add("test", layoutRequest["invalidkey"]!.ToString()))
                    .AsDefaultHandler();
            })
            .Configure(app =>
            {
                app.UseSitecoreRenderingEngine();
            });

        TestServer server = testHostBuilder.BuildServer(new Uri("http://localhost"));

        f.Inject(mockClientHandler);
        f.Inject(server);
    };

    [Theory]
    [AutoNSubstituteData(nameof(InvalidHttpMessageConfiguration))]
    public async Task HttpMessageConfigurationError_Returns_SitecoreLayoutServiceMessageConfigurationException(TestServer server, MockHttpMessageHandler clientHandler)
    {
        // Arrange
        clientHandler.Responses.Push(new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK
        });

        ISitecoreLayoutClient layoutClient = server.Services.GetRequiredService<ISitecoreLayoutClient>();

        SitecoreLayoutRequest request = new SitecoreLayoutRequest()
            .Path("test");

        // Act
        SitecoreLayoutResponse response = await layoutClient.Request(request);

        // Assert
        response.Should().NotBeNull();
        response.HasErrors.Should().BeTrue();
        response.Errors.Should().ContainSingle(e => e.GetType() == typeof(SitecoreLayoutServiceMessageConfigurationException));
    }

    [Theory]
    [AutoNSubstituteData(nameof(InvalidHttpClient))]
    public async Task HttpRequestTimeoutError_Returns_CouldNotContactSitecoreLayoutServiceClientException(TestServer server)
    {
        // Arrange
        ISitecoreLayoutClient layoutClient = server.Services.GetRequiredService<ISitecoreLayoutClient>();

        SitecoreLayoutRequest request = new SitecoreLayoutRequest()
            .Path("test");

        // Act
        SitecoreLayoutResponse response = await layoutClient.Request(request);

        // Assert
        response.Should().NotBeNull();
        response.HasErrors.Should().BeTrue();
        response.Errors.Should().ContainSingle(e => e.GetType() == typeof(CouldNotContactSitecoreLayoutServiceClientException));
    }

    [Theory]
    [AutoNSubstituteData(nameof(ValidHttpClient))]
    public async Task HttpResponse50xErrors_Return_InvalidResponseSitecoreLayoutServiceClientException(TestServer server, MockHttpMessageHandler clientHandler)
    {
        // Arrange
        HttpStatusCode[] responseStatuses =
        [
            HttpStatusCode.InternalServerError,
            HttpStatusCode.NotImplemented,
            HttpStatusCode.BadGateway,
            HttpStatusCode.ServiceUnavailable,
            HttpStatusCode.GatewayTimeout,
            HttpStatusCode.HttpVersionNotSupported,
            HttpStatusCode.VariantAlsoNegotiates,
            HttpStatusCode.InsufficientStorage,
            HttpStatusCode.LoopDetected,
            HttpStatusCode.NotExtended,
            HttpStatusCode.NetworkAuthenticationRequired
        ];

        foreach (HttpStatusCode responseStatus in responseStatuses)
        {
            clientHandler.Responses.Push(new HttpResponseMessage
            {
                StatusCode = responseStatus
            });

            ISitecoreLayoutClient layoutClient = server.Services.GetRequiredService<ISitecoreLayoutClient>();

            SitecoreLayoutRequest request = new SitecoreLayoutRequest()
                .Path("test");

            // Act
            SitecoreLayoutResponse response = await layoutClient.Request(request);

            // Assert
            response.Should().NotBeNull();
            response.HasErrors.Should().BeTrue();
            response.Errors.Should().ContainSingle(e => e.GetType() == typeof(InvalidResponseSitecoreLayoutServiceClientException));

            SitecoreLayoutServiceClientException exception = response.Errors.First();
            exception.Should().NotBeNull();
            exception.Data.Values.Count.Should().BePositive();
            exception.Data[HttpStatusCodeKeyName].Should().Be((int)responseStatus);

            exception.InnerException.Should().NotBeNull();
            exception.InnerException!.GetType().Should().Be(typeof(SitecoreLayoutServiceServerException));
        }
    }

    [Theory]
    [AutoNSubstituteData(nameof(ValidHttpClient))]
    public async Task HttpResponse40xErrors_Return_InvalidRequestSitecoreLayoutServiceClientException(TestServer server, MockHttpMessageHandler clientHandler)
    {
        // Arrange
        HttpStatusCode[] responseStatuses =
        [
            HttpStatusCode.BadRequest,
            HttpStatusCode.Unauthorized,
            HttpStatusCode.PaymentRequired,
            HttpStatusCode.Forbidden,
            HttpStatusCode.MethodNotAllowed,
            HttpStatusCode.NotAcceptable,
            HttpStatusCode.ProxyAuthenticationRequired,
            HttpStatusCode.RequestTimeout,
            HttpStatusCode.Conflict,
            HttpStatusCode.Gone,
            HttpStatusCode.LengthRequired,
            HttpStatusCode.PreconditionFailed,
            HttpStatusCode.RequestEntityTooLarge,
            HttpStatusCode.RequestUriTooLong,
            HttpStatusCode.UnsupportedMediaType,
            HttpStatusCode.RequestedRangeNotSatisfiable,
            HttpStatusCode.ExpectationFailed,
            HttpStatusCode.MisdirectedRequest,
            HttpStatusCode.UnprocessableEntity,
            HttpStatusCode.Locked,
            HttpStatusCode.FailedDependency,
            HttpStatusCode.UpgradeRequired,
            HttpStatusCode.PreconditionFailed,
            HttpStatusCode.TooManyRequests,
            HttpStatusCode.RequestHeaderFieldsTooLarge,
            HttpStatusCode.UnavailableForLegalReasons
        ];

        foreach (HttpStatusCode responseStatus in responseStatuses)
        {
            clientHandler.Responses.Push(new HttpResponseMessage
            {
                StatusCode = responseStatus
            });

            ISitecoreLayoutClient layoutClient = server.Services.GetRequiredService<ISitecoreLayoutClient>();

            SitecoreLayoutRequest request = new SitecoreLayoutRequest()
                .Path("test");

            // Act
            SitecoreLayoutResponse response = await layoutClient.Request(request);

            // Assert
            response.Should().NotBeNull();
            response.HasErrors.Should().BeTrue();
            response.Errors.Should().ContainSingle(e => e.GetType() == typeof(InvalidRequestSitecoreLayoutServiceClientException));

            SitecoreLayoutServiceClientException exception = response.Errors.First();
            exception.Should().NotBeNull();
            exception.Data.Values.Count.Should().BePositive();
            exception.Data[HttpStatusCodeKeyName].Should().Be((int)responseStatus);
        }
    }

    [Theory]
    [AutoNSubstituteData(nameof(ValidHttpClient))]
    public async Task HttpResponse404Error_Returns_ContentAndItemNotFoundSitecoreLayoutServiceClientException(TestServer server, MockHttpMessageHandler clientHandler)
    {
        // Arrange
        const HttpStatusCode responseStatus = HttpStatusCode.NotFound;

        clientHandler.Responses.Push(new HttpResponseMessage
        {
            StatusCode = responseStatus,
            Content = new StringContent("""{ "sitecore": { "sitecoredata": { "context": { "site": { "name": "404test" }}}}}""")
        });

        ISitecoreLayoutClient layoutClient = server.Services.GetRequiredService<ISitecoreLayoutClient>();

        SitecoreLayoutRequest request = new SitecoreLayoutRequest()
            .Path("test");

        // Act
        SitecoreLayoutResponse response = await layoutClient.Request(request);

        // Assert
        response.Should().NotBeNull();
        response.HasErrors.Should().BeTrue();
        response.Errors.Should().ContainSingle(e => e.GetType() == typeof(ItemNotFoundSitecoreLayoutServiceClientException));

        response.Content.Should().NotBeNull();

        SitecoreLayoutServiceClientException exception = response.Errors.First();
        exception.Should().NotBeNull();
        exception.Data.Values.Count.Should().BePositive();
        exception.Data[HttpStatusCodeKeyName].Should().Be((int)responseStatus);
    }

    [Theory]
    [AutoNSubstituteData(nameof(ValidHttpClient))]
    public async Task HttpResponseDeserializationError_Returns_InvalidResponseSitecoreLayoutServiceClientException(TestServer server, MockHttpMessageHandler clientHandler)
    {
        // Arrange
        HttpStatusCode responseStatus = HttpStatusCode.NotFound;

        clientHandler.Responses.Push(new HttpResponseMessage
        {
            StatusCode = responseStatus,
            Content = new StringContent("invalid json")
        });

        ISitecoreLayoutClient layoutClient = server.Services.GetRequiredService<ISitecoreLayoutClient>();

        SitecoreLayoutRequest request = new SitecoreLayoutRequest()
            .Path("test");

        // Act
        SitecoreLayoutResponse response = await layoutClient.Request(request);

        // Assert
        response.Should().NotBeNull();
        response.HasErrors.Should().BeTrue();
        response.Errors.Should().ContainSingle(e => e.GetType() == typeof(InvalidResponseSitecoreLayoutServiceClientException));
    }

    [Theory]
    [AutoNSubstituteData(nameof(ValidHttpClient))]
    public async Task HttpResponse10xErrors_Return_SitecoreLayoutServiceClientException(TestServer server, MockHttpMessageHandler clientHandler)
    {
        // Arrange
        HttpStatusCode[] responseStatuses =
        [
            HttpStatusCode.Continue,
            HttpStatusCode.SwitchingProtocols,
            HttpStatusCode.Processing,
            HttpStatusCode.EarlyHints
        ];

        foreach (HttpStatusCode responseStatus in responseStatuses)
        {
            clientHandler.Responses.Push(new HttpResponseMessage
            {
                StatusCode = responseStatus
            });

            ISitecoreLayoutClient layoutClient = server.Services.GetRequiredService<ISitecoreLayoutClient>();

            SitecoreLayoutRequest request = new SitecoreLayoutRequest()
                .Path("test");

            // Act
            SitecoreLayoutResponse response = await layoutClient.Request(request);

            // Assert
            response.Should().NotBeNull();
            response.HasErrors.Should().BeTrue();
            response.Errors.Should().ContainSingle(e => e.GetType() == typeof(SitecoreLayoutServiceClientException));

            SitecoreLayoutServiceClientException exception = response.Errors.First();
            exception.Should().NotBeNull();
            exception.Data.Values.Count.Should().BePositive();
            exception.Data[HttpStatusCodeKeyName].Should().Be((int)responseStatus);
        }
    }

    [Theory]
    [AutoNSubstituteData(nameof(ValidHttpClient))]
    public async Task HttpResponse30xErrors_Return_SitecoreLayoutServiceClientException(TestServer server, MockHttpMessageHandler clientHandler)
    {
        // Arrange
        HttpStatusCode[] responseStatuses =
        [
            HttpStatusCode.Ambiguous,
            HttpStatusCode.MultipleChoices,
            HttpStatusCode.Moved,
            HttpStatusCode.MovedPermanently,
            HttpStatusCode.Found,
            HttpStatusCode.Redirect,
            HttpStatusCode.RedirectMethod,
            HttpStatusCode.SeeOther,
            HttpStatusCode.NotModified,
            HttpStatusCode.UseProxy,
            HttpStatusCode.Unused,
            HttpStatusCode.RedirectKeepVerb,
            HttpStatusCode.TemporaryRedirect,
            HttpStatusCode.PermanentRedirect
        ];

        foreach (HttpStatusCode responseStatus in responseStatuses)
        {
            clientHandler.Responses.Push(new HttpResponseMessage
            {
                StatusCode = responseStatus
            });

            ISitecoreLayoutClient layoutClient = server.Services.GetRequiredService<ISitecoreLayoutClient>();

            SitecoreLayoutRequest request = new SitecoreLayoutRequest()
                .Path("test");

            // Act
            SitecoreLayoutResponse response = await layoutClient.Request(request);

            // Assert
            response.Should().NotBeNull();
            response.HasErrors.Should().BeTrue();
            response.Errors.Should().ContainSingle(e => e.GetType() == typeof(SitecoreLayoutServiceClientException));

            SitecoreLayoutServiceClientException exception = response.Errors.First();
            exception.Should().NotBeNull();
            exception.Data.Values.Count.Should().BePositive();
            exception.Data[HttpStatusCodeKeyName].Should().Be((int)responseStatus);
        }
    }

    [Theory]
    [AutoNSubstituteData(nameof(ValidHttpClient))]
    public async Task ErrorView_Returns_InvalidResponseSitecoreLayoutServiceClientException(TestServer server, MockHttpMessageHandler clientHandler)
    {
        clientHandler.Responses.Push(new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.BadRequest
        });

        HttpClient client = server.CreateClient();

        // Act
        string response = await client.GetStringAsync("Error");

        // Assert
        HtmlDocument doc = new();
        doc.LoadHtml(response);

        doc.GetElementbyId("errorMessage").InnerHtml.Should()
            .Contain(nameof(InvalidRequestSitecoreLayoutServiceClientException));
    }
}