using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.Json;
using AutoFixture;
using AutoFixture.Idioms;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using NSubstitute;
using Sitecore.AspNetCore.SDK.AutoFixture.Attributes;
using Sitecore.AspNetCore.SDK.AutoFixture.Extensions;
using Sitecore.AspNetCore.SDK.Pages.Configuration;
using Sitecore.AspNetCore.SDK.Pages.Middleware;
using Sitecore.AspNetCore.SDK.RenderingEngine.Configuration;
using Sitecore.AspNetCore.SDK.RenderingEngine.Rendering;
using Xunit;

namespace Sitecore.AspNetCore.SDK.Pages.Tests.Middleware
{
    public class PageSetupMiddlewareFixture
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
            PagesOptions PagesOptionsValues = new PagesOptions
            {
                ConfigEndpoint = ValidConfigEndpoint,
                RenderEndpoint = ValidRenderEndpoint,
                ValidEditingOrigin = ValidEditingOrigin,
                ValidOrigins = ValidOrigins,
                EditingSecret = ValidEditingSecret
            };
            pagesOptions.Value.Returns(PagesOptionsValues);
            f.Inject(pagesOptions);

            ILogger<PageSetupMiddleware> logger = Substitute.For<ILogger<PageSetupMiddleware>>();
            f.Inject(logger);

            IOptions<RenderingEngineOptions > renderingEngineOptions = Substitute.For<IOptions<RenderingEngineOptions>>();
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
            guard.VerifyConstructors<PageSetupMiddleware>();
        }

        [Theory]
        [AutoNSubstituteData]
        public async Task Invoke_RequestIsntConfigOrRender_NextDelegateCalled(RequestDelegate requestDelegate, IOptions<PagesOptions> pageOptions, ILogger<PageSetupMiddleware> logger, IOptions<RenderingEngineOptions> renderingEngineOptions)
        {
            // Arrange
            PageSetupMiddleware sut = new(requestDelegate, pageOptions, logger, renderingEngineOptions);
            HttpContext httpContext = Substitute.For<HttpContext>();
            HttpRequest httpRequest = Substitute.For<HttpRequest>();
            httpRequest.Method.Returns("Post");
            httpContext.Request.Returns(httpRequest);

            // Act
            await sut.Invoke(httpContext);

            // Assert
            await requestDelegate.Received()(httpContext);
        }

        [Theory]
        [AutoNSubstituteData]
        public async Task Invoke_ConfigRequest_InvalidEditingSecret_NextDelegateCalled(RequestDelegate requestDelegate, IOptions<PagesOptions> pageOptions, ILogger<PageSetupMiddleware> logger, IOptions<RenderingEngineOptions> renderingEngineOptions)
        {
            // Arrange
            PageSetupMiddleware sut = new(requestDelegate, pageOptions, logger, renderingEngineOptions);
            HttpContext httpContext = Substitute.For<HttpContext>();
            httpContext.Request.Method.Returns("GET");
            httpContext.Request.Path.Returns(new PathString(ValidConfigEndpoint));
            httpContext.Request.Query.Returns(new QueryCollection(new Dictionary<string, StringValues> { { "secret", new StringValues("incorrect_secret_value") } }));

            // Act
            await sut.Invoke(httpContext);

            // Assert
            logger.Received().Log(LogLevel.Error, Arg.Any<EventId>(), Arg.Is<object>(o => o.ToString() == "Invalid Pages Editing Secret Value"), null, Arg.Any<Func<object, Exception?, string>>());
            await requestDelegate.Received()(httpContext);
        }

        [Theory]
        [AutoNSubstituteData]
        public async Task Invoke_ConfigRequest_InvalidEditingOrigin_NextDelegateCalled(RequestDelegate requestDelegate, IOptions<PagesOptions> pageOptions, ILogger<PageSetupMiddleware> logger, IOptions<RenderingEngineOptions> renderingEngineOptions)
        {
            // Arrange
            PageSetupMiddleware sut = new(requestDelegate, pageOptions, logger, renderingEngineOptions);
            HttpContext httpContext = Substitute.For<HttpContext>();
            httpContext.Request.Method.Returns("GET");
            httpContext.Request.Path.Returns(new PathString(ValidConfigEndpoint));
            httpContext.Request.Query.Returns(new QueryCollection(new Dictionary<string, StringValues> { { "secret", new StringValues(ValidEditingSecret) } }));
            httpContext.Request.Headers.Returns(new HeaderDictionary(new Dictionary<string, StringValues> { { "Origin", new StringValues("http://an.invalid.origin.domain") } }));

            // Act
            await sut.Invoke(httpContext);

            // Assert
            logger.Received().Log(LogLevel.Error, Arg.Any<EventId>(), Arg.Is<object>(o => o.ToString() == "Invalid Pages Editing Origin"), null, Arg.Any<Func<object, Exception?, string>>());
            await requestDelegate.Received()(httpContext);
        }

        [Theory]
        [AutoNSubstituteData]
        public async Task Invoke_ConfigRequest_ValidResponseReturned(RequestDelegate requestDelegate, IOptions<PagesOptions> pageOptions, ILogger<PageSetupMiddleware> logger, IOptions<RenderingEngineOptions> renderingEngineOptions)
        {
            // Arrange
            PageSetupMiddleware sut = new(requestDelegate, pageOptions, logger, renderingEngineOptions);
            HttpContext httpContext = Substitute.For<HttpContext>();
            httpContext.Request.Method.Returns("GET");
            httpContext.Request.Path.Returns(new PathString(ValidConfigEndpoint));
            httpContext.Request.Query.Returns(new QueryCollection(new Dictionary<string, StringValues> { { "secret", new StringValues(ValidEditingSecret) } }));
            httpContext.Request.Headers.Returns(new HeaderDictionary(new Dictionary<string, StringValues> { { "Origin", new StringValues(ValidEditingOrigin) } }));
            MemoryStream memoryStream = new();
            httpContext.Response.Body = memoryStream;

            // Act
            await sut.Invoke(httpContext);

            // Assert
            logger.Received().Log(LogLevel.Debug, Arg.Any<EventId>(), Arg.Is<object>(o => o.ToString() == "Processing valid Pages Config request"), null, Arg.Any<Func<object, Exception?, string>>());

            byte[] contents = memoryStream.ToArray();
            string returnedBody = Encoding.UTF8.GetString(contents);
            var jsonDoc = JsonDocument.Parse(returnedBody);
            jsonDoc.RootElement.TryGetProperty("editMode", out JsonElement editNode).Should().BeTrue();
            editNode.GetString().Should().Be("metadata");
            jsonDoc.RootElement.TryGetProperty("components", out JsonElement componentsNode).Should().BeTrue();
            componentsNode[0].GetString().Should().Be("TestComponent");
            httpContext.Response.Headers.ContentSecurityPolicy.Should().Equal($"frame-ancestors 'self' {ValidOrigins} {ValidEditingOrigin}");
            httpContext.Response.Headers.AccessControlAllowOrigin.Should().Equal(ValidEditingOrigin);
            httpContext.Response.Headers.AccessControlAllowMethods.Should().Equal("GET, POST, OPTIONS, PUT, PATCH, DELETE");
            httpContext.Response.StatusCode.Should().Be(StatusCodes.Status200OK);
            httpContext.Response.ContentType.Should().Be("application/json");

            await requestDelegate.DidNotReceive()(httpContext);
        }

        [Theory]
        [AutoNSubstituteData]
        public async Task Invoke_RenderRequest_InvalidEditingSecret_NextDelegateCalled(RequestDelegate requestDelegate, IOptions<PagesOptions> pageOptions, ILogger<PageSetupMiddleware> logger, IOptions<RenderingEngineOptions> renderingEngineOptions)
        {
            // Arrange
            PageSetupMiddleware sut = new(requestDelegate, pageOptions, logger, renderingEngineOptions);
            HttpContext httpContext = Substitute.For<HttpContext>();
            httpContext.Request.Method.Returns("GET");
            httpContext.Request.Path.Returns(new PathString(ValidRenderEndpoint));
            httpContext.Request.Query.Returns(new QueryCollection(new Dictionary<string, StringValues> { { "secret", new StringValues("incorrect_secret_value") } }));

            // Act
            await sut.Invoke(httpContext);

            // Assert
            logger.Received().Log(LogLevel.Error, Arg.Any<EventId>(), Arg.Is<object>(o => o.ToString() == "Invalid Pages Editing Secret Value"), null, Arg.Any<Func<object, Exception?, string>>());
            await requestDelegate.Received()(httpContext);
        }

        [Theory]
        [AutoNSubstituteData]
        public async Task Invoke_RenderRequest_ValidResponseReturned(RequestDelegate requestDelegate, IOptions<PagesOptions> pageOptions, ILogger<PageSetupMiddleware> logger, IOptions<RenderingEngineOptions> renderingEngineOptions)
        {
            // Arrange
            string expectedRoute = "test_route";
            string expectedMode = "test_mode";
            string expectedItemId = "test_item_id";
            string expectedVersion = "test_version";
            string expectedLanguage = "test_lang";
            string expectedSite = "test_site";
            string expectedLayoutKind = "test_layoutKind";
            string expectedTenantId = "test_tenant_id";
            PageSetupMiddleware sut = new(requestDelegate, pageOptions, logger, renderingEngineOptions);
            HttpContext httpContext = Substitute.For<HttpContext>();
            httpContext.Request.Method.Returns("GET");
            httpContext.Request.Path.Returns(new PathString(ValidRenderEndpoint));
            httpContext.Request.Headers.Returns(new HeaderDictionary(new Dictionary<string, StringValues> { { "Origin", new StringValues(ValidEditingOrigin) } }));
            httpContext.Request.Query.Returns(new QueryCollection(
                new Dictionary<string, StringValues>
                {
                    { "secret", new StringValues(ValidEditingSecret) },
                    { "sc_itemid", new StringValues(expectedItemId) },
                    { "sc_lang", new StringValues(expectedLanguage) },
                    { "sc_layoutKind", new StringValues(expectedLayoutKind) },
                    { "mode", new StringValues(expectedMode) },
                    { "route", new StringValues(expectedRoute) },
                    { "sc_site", new StringValues(expectedSite) },
                    { "sc_version", new StringValues(expectedVersion) },
                    { "tenant_id", new StringValues(expectedTenantId) }
                }));

            HttpResponse httpResponse = Substitute.For<HttpResponse>();
            httpContext.Response.Returns(httpResponse);
            MemoryStream memoryStream = new();
            httpResponse.Body = memoryStream;

            // Act
            await sut.Invoke(httpContext);

            // Assert
            logger.Received().Log(LogLevel.Debug, Arg.Any<EventId>(), Arg.Is<object>(o => o.ToString() == "Processing valid Pages Render request"), null, Arg.Any<Func<object, Exception?, string>>());

            httpContext.Response.Headers.ContentSecurityPolicy.Should().Equal($"frame-ancestors 'self' {ValidOrigins} {ValidEditingOrigin}");
            string validRedirectString = $"{expectedRoute}?mode={expectedMode}&sc_itemid={expectedItemId}&sc_version={expectedVersion}&sc_lang={expectedLanguage}&sc_site={expectedSite}&sc_layoutKind={expectedLayoutKind}&secret={ValidEditingSecret}&tenant_id={expectedTenantId}&route={expectedRoute}";
            httpResponse.ReceivedWithAnyArgs().Redirect(validRedirectString, permanent: false);

            await requestDelegate.DidNotReceive()(httpContext);
        }
    }
}
