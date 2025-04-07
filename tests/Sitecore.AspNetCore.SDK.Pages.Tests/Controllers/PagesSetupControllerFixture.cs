using System.Diagnostics.CodeAnalysis;
using AutoFixture;
using AutoFixture.Idioms;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using NSubstitute;
using Sitecore.AspNetCore.SDK.AutoFixture.Attributes;
using Sitecore.AspNetCore.SDK.AutoFixture.Extensions;
using Sitecore.AspNetCore.SDK.Pages.Configuration;
using Sitecore.AspNetCore.SDK.Pages.Controllers;
using Sitecore.AspNetCore.SDK.Pages.Models;
using Sitecore.AspNetCore.SDK.RenderingEngine.Configuration;
using Sitecore.AspNetCore.SDK.RenderingEngine.Rendering;
using Xunit;

namespace Sitecore.AspNetCore.SDK.Pages.Tests.Controllers
{
    public class PagesSetupControllerFixture
    {
        private const string ValidConfigEndpoint = "/api/editing/config";
        private const string ValidRenderEndpoint = "/api/editing/render";
        private const string ValidEditingOrigin = "http://some.editing.domain";
        private const string ValidOrigins = "http://some.origin.domain";
        private const string ValidEditingSecret = "editing_secret_1234";

        [ExcludeFromCodeCoverage]
        public static Action<IFixture> AutoSetup => f =>
        {
            RequestDelegate requestDelegate = Substitute.For<RequestDelegate>();
            f.Inject(requestDelegate);

            IOptions<PagesOptions> pagesOptions = Substitute.For<IOptions<PagesOptions>>();
            PagesOptions pagesOptionsValues = new PagesOptions
            {
                ConfigEndpoint = ValidConfigEndpoint,
                RenderEndpoint = ValidRenderEndpoint,
                ValidEditingOrigin = ValidEditingOrigin,
                ValidOrigins = ValidOrigins,
                EditingSecret = ValidEditingSecret
            };
            pagesOptions.Value.Returns(pagesOptionsValues);
            f.Inject(pagesOptions);

            ILogger<PagesSetupController> logger = Substitute.For<ILogger<PagesSetupController>>();
            f.Inject(logger);

            IOptions<RenderingEngineOptions> renderingEngineOptions = Substitute.For<IOptions<RenderingEngineOptions>>();
            string componentName = "TestComponent";
            ComponentRendererDescriptor componentRendererDescriptor = new(name => name == componentName, _ => null!, componentName);
            RenderingEngineOptions renderingEngineOptionsValues = new RenderingEngineOptions
            {
                RendererRegistry = new SortedList<int, ComponentRendererDescriptor>
                {
                    { 1, componentRendererDescriptor }
                }
            };
            renderingEngineOptions.Value.Returns(renderingEngineOptionsValues);
            f.Inject(renderingEngineOptions);
        };

        [Theory]
        [AutoNSubstituteData]
        public void Ctor_InvalidArgs_Throws(GuardClauseAssertion guard)
        {
            guard.VerifyConstructors<PagesSetupController>();
        }

        [Theory]
        [AutoNSubstituteData]
        public void ConfigRoute_InvalidEditingSecret_ReturnsBadRequestResponse(IOptions<PagesOptions> pageOptions, ILogger<PagesSetupController> logger, IOptions<RenderingEngineOptions> renderingEngineOptions)
        {
            // Arrange
            HttpContext httpContext = Substitute.For<HttpContext>();
            httpContext.Request.Method.Returns("GET");
            httpContext.Request.Path.Returns(new PathString(ValidConfigEndpoint));
            httpContext.Request.Query.Returns(new QueryCollection(new Dictionary<string, StringValues> { { "secret", new StringValues("incorrect_secret_value") } }));
            PagesSetupController sut = new(pageOptions, logger, renderingEngineOptions);
            sut.ControllerContext = new ControllerContext()
            {
                HttpContext = httpContext
            };

            // Act
            ActionResult<PagesConfigResponse> response = sut.Config();

            // Assert
            logger.Received().Log(LogLevel.Error, Arg.Any<EventId>(), Arg.Is<object>(o => o.ToString() == "Invalid Pages Editing Secret Value"), null, Arg.Any<Func<object, Exception?, string>>());
            response.Result.Should().BeOfType<BadRequestResult>();
        }

        [Theory]
        [AutoNSubstituteData]
        public void ConfigRoute_InvalidEditingOrigin_ReturnsBadRequestResponse(IOptions<PagesOptions> pageOptions, ILogger<PagesSetupController> logger, IOptions<RenderingEngineOptions> renderingEngineOptions)
        {
            // Arrange
            HttpContext httpContext = Substitute.For<HttpContext>();
            httpContext.Request.Method.Returns("GET");
            httpContext.Request.Path.Returns(new PathString(ValidConfigEndpoint));
            httpContext.Request.Query.Returns(new QueryCollection(new Dictionary<string, StringValues> { { "secret", new StringValues(ValidEditingSecret) } }));
            httpContext.Request.Headers.Returns(new HeaderDictionary(new Dictionary<string, StringValues> { { "Origin", new StringValues("http://an.invalid.origin.domain") } }));
            PagesSetupController sut = new(pageOptions, logger, renderingEngineOptions);
            sut.ControllerContext = new ControllerContext()
            {
                HttpContext = httpContext
            };

            // Act
            ActionResult<PagesConfigResponse> response = sut.Config();

            // Assert
            logger.Received().Log(LogLevel.Error, Arg.Any<EventId>(), Arg.Is<object>(o => o.ToString() == "Invalid Pages Editing Origin"), null, Arg.Any<Func<object, Exception?, string>>());
            response.Result.Should().BeOfType<BadRequestResult>();
        }

        [Theory]
        [AutoNSubstituteData]
        public void ConfigRoute_ValidRequest_OkResponseReturned(IOptions<PagesOptions> pageOptions, ILogger<PagesSetupController> logger, IOptions<RenderingEngineOptions> renderingEngineOptions)
        {
            // Arrange
            HttpContext httpContext = Substitute.For<HttpContext>();
            httpContext.Request.Method.Returns("GET");
            httpContext.Request.Path.Returns(new PathString(ValidConfigEndpoint));
            httpContext.Request.Query.Returns(new QueryCollection(new Dictionary<string, StringValues> { { "secret", new StringValues(ValidEditingSecret) } }));
            httpContext.Request.Headers.Returns(new HeaderDictionary(new Dictionary<string, StringValues> { { "Origin", new StringValues(ValidEditingOrigin) } }));
            PagesSetupController sut = new(pageOptions, logger, renderingEngineOptions);
            sut.ControllerContext = new ControllerContext()
            {
                HttpContext = httpContext
            };

            // Act
            ActionResult<PagesConfigResponse> response = sut.Config();

            // Assert
            logger.Received().Log(LogLevel.Debug, Arg.Any<EventId>(), Arg.Is<object>(o => o.ToString() == "Processing valid Pages Config request"), null, Arg.Any<Func<object, Exception?, string>>());
            httpContext.Response.Headers.ContentSecurityPolicy.Should().Equal($"frame-ancestors 'self' {ValidOrigins} {ValidEditingOrigin}");
            httpContext.Response.Headers.AccessControlAllowOrigin.Should().Equal(ValidEditingOrigin);
            httpContext.Response.Headers.AccessControlAllowMethods.Should().Equal("GET, POST, OPTIONS, PUT, PATCH, DELETE");
            httpContext.Response.Headers.AccessControlAllowHeaders.Should().Equal("Authorization");
            httpContext.Response.StatusCode.Should().Be(StatusCodes.Status200OK);
            httpContext.Response.ContentType.Should().Be("application/json");
            response.Result.Should().BeOfType<OkObjectResult>();
            response.Result.As<OkObjectResult>().Value.Should().NotBeNull();
            response.Result.As<OkObjectResult>().Value.As<PagesConfigResponse>().Should().NotBeNull();
            response.Result.As<OkObjectResult>().Value.As<PagesConfigResponse>().EditMode.Should().Be("metadata");
            response.Result.As<OkObjectResult>().Value.As<PagesConfigResponse>().Components.Count.Should().Be(1);
            response.Result.As<OkObjectResult>().Value.As<PagesConfigResponse>().Components[0].Should().Be("TestComponent");
        }

        [Theory]
        [AutoNSubstituteData]
        public void RenderRoute_InvalidEditingSecret_ReturnsBadRequestResponse(IOptions<PagesOptions> pageOptions, ILogger<PagesSetupController> logger, IOptions<RenderingEngineOptions> renderingEngineOptions)
        {
            // Arrange
            HttpContext httpContext = Substitute.For<HttpContext>();
            httpContext.Request.Method.Returns("GET");
            httpContext.Request.Path.Returns(new PathString(ValidRenderEndpoint));
            httpContext.Request.Query.Returns(new QueryCollection(new Dictionary<string, StringValues> { { "secret", new StringValues("incorrect_secret_value") } }));
            PagesSetupController sut = new(pageOptions, logger, renderingEngineOptions);
            sut.ControllerContext = new ControllerContext()
            {
                HttpContext = httpContext
            };

            // Act
            IActionResult response = sut.Render();

            // Assert
            logger.Received().Log(LogLevel.Error, Arg.Any<EventId>(), Arg.Is<object>(o => o.ToString() == "Invalid Pages Editing Secret Value"), null, Arg.Any<Func<object, Exception?, string>>());
            response.Should().BeOfType<BadRequestResult>();
        }

        [Theory]
        [AutoNSubstituteData]
        public void RenderRoute_ValidRequest_OkResponseReturned(IOptions<PagesOptions> pageOptions, ILogger<PagesSetupController> logger, IOptions<RenderingEngineOptions> renderingEngineOptions)
        {
            // Arrange
            string expectedRoute = "test_route";
            string expectedMode = "test_mode";
            Guid expectedItemId = Guid.NewGuid();
            int expectedVersion = 0;
            string expectedLanguage = "test_lang";
            string expectedSite = "test_site";
            string expectedLayoutKind = "test_layoutKind";
            string expectedTenantId = "test_tenant_id";
            HttpContext httpContext = Substitute.For<HttpContext>();
            httpContext.Request.Method.Returns("GET");
            httpContext.Request.Path.Returns(new PathString(ValidRenderEndpoint));
            httpContext.Request.Headers.Returns(new HeaderDictionary(new Dictionary<string, StringValues> { { "Origin", new StringValues(ValidEditingOrigin) } }));
            httpContext.Request.Query.Returns(new QueryCollection(
                new Dictionary<string, StringValues>
                {
                    { "secret", new StringValues(ValidEditingSecret) },
                    { "sc_itemid", new StringValues(expectedItemId.ToString()) },
                    { "sc_lang", new StringValues(expectedLanguage) },
                    { "sc_layoutKind", new StringValues(expectedLayoutKind) },
                    { "mode", new StringValues(expectedMode) },
                    { "route", new StringValues(expectedRoute) },
                    { "sc_site", new StringValues(expectedSite) },
                    { "sc_version", new StringValues(expectedVersion.ToString()) },
                    { "tenant_id", new StringValues(expectedTenantId) }
                }));
            HttpResponse httpResponse = Substitute.For<HttpResponse>();
            httpContext.Response.Returns(httpResponse);
            MemoryStream memoryStream = new();
            httpResponse.Body = memoryStream;
            PagesSetupController sut = new(pageOptions, logger, renderingEngineOptions);
            sut.ControllerContext = new ControllerContext()
            {
                HttpContext = httpContext
            };

            // Act
            IActionResult response = sut.Render();

            // Assert
            logger.Received().Log(LogLevel.Debug, Arg.Any<EventId>(), Arg.Is<object>(o => o.ToString() == "Processing valid Pages Render request."), null, Arg.Any<Func<object, Exception?, string>>());
            response.Should().BeOfType<RedirectResult>();
            response.As<RedirectResult>().Permanent.Should().BeFalse();
            string validRedirectString = $"{expectedRoute}?mode={expectedMode}&sc_itemid={expectedItemId}&sc_version={expectedVersion}&sc_lang={expectedLanguage}&sc_site={expectedSite}&sc_layoutKind={expectedLayoutKind}&secret={ValidEditingSecret}&tenant_id={expectedTenantId}&route={expectedRoute}";
            response.As<RedirectResult>().Url.Should().Be(validRedirectString);
        }
    }
}
