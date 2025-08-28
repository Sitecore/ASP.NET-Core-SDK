using System.Diagnostics.CodeAnalysis;
using AutoFixture;
using AutoFixture.Idioms;
using AwesomeAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Sitecore.AspNetCore.SDK.AutoFixture.Attributes;
using Sitecore.AspNetCore.SDK.AutoFixture.Extensions;
using Sitecore.AspNetCore.SDK.AutoFixture.Mocks;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Configuration;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Exceptions;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Extensions;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Request;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Request.Handlers;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Response;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Serialization;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Tests.MockModels;
using Sitecore.AspNetCore.SDK.TestData;
using Xunit;

// ReSharper disable StringLiteralTypo
namespace Sitecore.AspNetCore.SDK.LayoutService.Client.Tests.Request.Handlers;

public class HttpLayoutRequestHandlerFixture
{
    private const string StatusCodeKey = "HTTP Status Code";

    // ReSharper disable once UnusedMember.Global - Used by testing framework
    public static Action<IFixture> AutoSetup => f =>
    {
        f.Behaviors.Add(new OmitOnRecursionBehavior());

        HttpClient client = new()
        {
            DefaultRequestVersion = new Version(1, 0),
            BaseAddress = new Uri("http://localhost.test")
        };

        f.Inject(client);

        ISitecoreLayoutSerializer? serializer = f.Freeze<ISitecoreLayoutSerializer>();
        serializer.Deserialize("JSON").Returns(CannedResponses.Simple);

        f.Freeze<HttpLayoutRequestHandlerWrapper>();

        f.Inject(new SitecoreLayoutRequest());
    };

    public static Action<IFixture> HttpClientWithMockedHttpMessageHandler => f =>
    {
        MockHttpMessageHandler mockHttpHandler = new();

        HttpClient client = new(mockHttpHandler)
        {
            DefaultRequestVersion = new Version(1, 0),
            BaseAddress = new Uri("http://localhost.test")
        };

        f.Inject(client);
        f.Inject(mockHttpHandler);
    };

    [Theory]
    [AutoNSubstituteData]
    public void Ctor_InvalidArgs_Throws(GuardClauseAssertion guard)
    {
        guard.VerifyConstructors<HttpLayoutRequestHandler>();
    }

    [Theory]
    [AutoNSubstituteData]
    public void Ctor_HttpClientWithNullBaseAddress_Throws(HttpClient client, ISitecoreLayoutSerializer serializer, IOptionsSnapshot<HttpLayoutRequestHandlerOptions> options, ILogger<HttpLayoutRequestHandler> logger)
    {
        // Arrange
        client.BaseAddress = null;
        Func<HttpLayoutRequestHandler> act =
            () => new HttpLayoutRequestHandler(client, serializer, options, logger);

        // Act & assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Theory]
    [AutoNSubstituteData]
    public async Task Request_WithNullRequest_Throws(HttpLayoutRequestHandler sut)
    {
        // Arrange
        Func<Task<SitecoreLayoutResponse>> act =
            () => sut.Request(null!, string.Empty);

        // Act & Assert
        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Theory]
    [AutoNSubstituteData]
    public async Task Request_WithEntriesMappedToHttpRequest_GenerateCorrectHttpRequestMessage(
        HttpClient client,
        ISitecoreLayoutSerializer serializer,
        IOptionsSnapshot<HttpLayoutRequestHandlerOptions> options,
        SitecoreLayoutRequest request,
        string handlerName,
        ILogger<HttpLayoutRequestHandler> logger)
    {
        // Arrange
        request.Add("alpha", "this");
        request.Add("beta", "that");

        const string hostHeader = "testhost";
        StringContent messageContent = new("test");
        HttpMethod messageMethod = HttpMethod.Post;
        const string authCookie = "auth=test";

        HttpLayoutRequestHandlerOptions handlerOptions = new()
        {
            RequestMap =
            [
                (req, message) =>
                {
                    message.RequestUri = req.BuildUri(client.BaseAddress!, ["alpha", "beta"]);
                    message.Headers.Add("Host", hostHeader);
                    message.Headers.Add("Cookie", authCookie);
                    message.Method = messageMethod;
                    message.Content = messageContent;
                }

            ]
        };
        options.Get(handlerName).Returns(handlerOptions);

        HttpLayoutRequestHandlerWrapper stub = new(client, serializer, options, logger);

        // Act
        await stub.Request(request, handlerName);
        stub.RequestMessage!.Headers.TryGetValues("Cookie", out IEnumerable<string>? cookies);

        // Assert
        stub.RequestMessage!.RequestUri!.Query.Should().Contain("alpha=this&beta=that");
        stub.RequestMessage.Headers.Should().NotBeEmpty();
        stub.RequestMessage.Headers.Host.Should().Be(hostHeader);
        cookies.Should().Contain(authCookie);
        stub.RequestMessage.Method.Should().Be(messageMethod);
        stub.RequestMessage.Content.Should().Be(messageContent);
    }

    [Theory]
    [InlineAutoNSubstituteData("<script type=\"text/javascript\">", "?%3Cscript+type%3D%22text%2Fjavascript%22%3E=%3Cscript+type%3D%22text%2Fjavascript%22%3E")]
    [InlineAutoNSubstituteData("Than#ks/Mi&hae=la", "?Than%23ks%2FMi%26hae%3Dla=Than%23ks%2FMi%26hae%3Dla")]
    public async Task Request_WithEntries_EncodeQueryStringArgs(
        string entry,
        string expectedQueryString,
        HttpClient client,
        ISitecoreLayoutSerializer serializer,
        IOptionsSnapshot<HttpLayoutRequestHandlerOptions> options,
        SitecoreLayoutRequest request,
        string handlerName,
        ILogger<HttpLayoutRequestHandler> logger)
    {
        // Arrange
        request.Add(entry, entry);

        HttpLayoutRequestHandlerOptions handlerOptions = new()
        {
            RequestMap =
            [
                (req, message) =>
                {
                    message.RequestUri = req.BuildUri(client.BaseAddress!, [entry]);
                }

            ]
        };
        options.Get(handlerName).Returns(handlerOptions);

        HttpLayoutRequestHandlerWrapper stub = new(client, serializer, options, logger);

        // Act
        await stub.Request(request, handlerName);

        // Assert
        stub.RequestMessage!.RequestUri!.Query.Should().Be(expectedQueryString);
    }

    [Theory]
    [AutoNSubstituteData]
    [SuppressMessage("StyleCop.CSharp.SpacingRules", "SA1008:OpeningParenthesisMustBeSpacedCorrectly", Justification = "Lambda inside new collection initializer.")]
    public async Task Request_WithHandlerOptionError_ReturnsError(
        HttpClient client,
        ISitecoreLayoutSerializer serializer,
        IOptionsSnapshot<HttpLayoutRequestHandlerOptions> options,
        SitecoreLayoutRequest request,
        string handlerName,
        ILogger<HttpLayoutRequestHandler> logger)
    {
        // Arrange
        HttpLayoutRequestHandlerOptions handlerOptions = new()
        {
            RequestMap = [(req, message) => { message.Headers.Add("test", req["inexistent_key"]!.ToString()); }]
        };
        options.Get(handlerName).Returns(handlerOptions);

        HttpLayoutRequestHandlerWrapper stub = new(client, serializer, options, logger);

        // Act
        SitecoreLayoutResponse response = await stub.Request(request, handlerName);

        // Assert
        response.HasErrors.Should().BeTrue();
        response.Errors.Should().ContainSingle(x => x.GetType() == typeof(SitecoreLayoutServiceMessageConfigurationException));
        response.Errors.FirstOrDefault(error => error.InnerException != null && error.InnerException.Message.Equals("The given key 'inexistent_key' was not present in the dictionary.", StringComparison.OrdinalIgnoreCase)).Should().NotBeNull();
    }

    [Theory]
    [AutoNSubstituteData]
    public async Task Request_WithValidRequest_DeserializesResult(
        HttpLayoutRequestHandlerWrapper stub,
        ISitecoreLayoutSerializer serializer,
        SitecoreLayoutRequest request)
    {
        // Arrange / Act
        SitecoreLayoutResponse result = await stub.Request(request, string.Empty);

        // Assert
        serializer.Received(1).Deserialize("JSON");
        result.Content.Should().BeEquivalentTo(CannedResponses.Simple);
    }

    [Theory]
    [AutoNSubstituteData]
    public async Task Request_WithValidRequest_SetsHeaders(
        HttpLayoutRequestHandlerWrapper stub,
        SitecoreLayoutRequest request)
    {
        // Arrange
        HttpResponseMessage response = new()
        {
            Content = new StringContent("JSON")
        };

        response.Headers.Add("Header1", "Value1");
        stub.Responses.Push(response);

        // Act
        SitecoreLayoutResponse result = await stub.Request(request, string.Empty);

        // Assert
        result.Metadata.Should().NotBeEmpty();
        result.Metadata!["Header1"].Should().ContainSingle(x => x == "Value1");
    }

    [Theory]
    [AutoNSubstituteData]
    public async Task Request_WithInvalidStatusCode_ReceivesHeaders(
        HttpLayoutRequestHandlerWrapper stub,
        SitecoreLayoutRequest request)
    {
        // Arrange
        HttpResponseMessage response = new()
        {
            StatusCode = System.Net.HttpStatusCode.Unauthorized
        };
        response.Headers.Add("Header1", "Value1");
        stub.Responses.Push(response);

        // Act
        SitecoreLayoutResponse result = await stub.Request(request, string.Empty);

        // Assert
        result.Metadata.Should().NotBeEmpty();
        result.Metadata!["Header1"].Should().ContainSingle(x => x == "Value1");
    }

    [Theory]
    [AutoNSubstituteData]
    public async Task Request_WithInvalidRequestData_PopulatesErrors(
        HttpLayoutRequestHandler sut,
        SitecoreLayoutRequest request)
    {
        // Arrange
        SitecoreLayoutRequest testRequest = request.Path("invalid");

        // Act
        SitecoreLayoutResponse result = await sut.Request(testRequest, string.Empty);

        // Assert
        result.HasErrors.Should().BeTrue();
        result.Errors.Should().ContainSingle(x => x.GetType() == typeof(CouldNotContactSitecoreLayoutServiceClientException));
    }

    [Theory]
    [AutoNSubstituteData]
    public async Task Request_With10xStatusCode_PopulatesErrors(
        HttpLayoutRequestHandlerWrapper stub,
        SitecoreLayoutRequest request)
    {
        // Arrange
        stub.Responses.Push(new HttpResponseMessage
        {
            StatusCode = System.Net.HttpStatusCode.Continue
        });

        // Act
        SitecoreLayoutResponse result = await stub.Request(request, string.Empty);

        // Assert
        result.HasErrors.Should().BeTrue();
        result.Errors.Should().ContainSingle(x => x.GetType() == typeof(SitecoreLayoutServiceClientException));
        result.Errors.First().Data[StatusCodeKey].Should().Be((int)System.Net.HttpStatusCode.Continue);
    }

    [Theory]
    [AutoNSubstituteData]
    public async Task Request_With30xStatusCode_PopulatesErrors(
        HttpLayoutRequestHandlerWrapper stub,
        SitecoreLayoutRequest request)
    {
        // Arrange
        stub.Responses.Push(new HttpResponseMessage
        {
            StatusCode = System.Net.HttpStatusCode.PermanentRedirect
        });

        // Act
        SitecoreLayoutResponse result = await stub.Request(request, string.Empty);

        // Assert
        result.HasErrors.Should().BeTrue();
        result.Errors.Should().ContainSingle(x => x.GetType() == typeof(SitecoreLayoutServiceClientException));
        result.Errors.First().Data[StatusCodeKey].Should().Be((int)System.Net.HttpStatusCode.PermanentRedirect);
    }

    [Theory]
    [AutoNSubstituteData]
    public async Task Request_With404StatusCode_ReturnsContentAndPopulatesError(
        HttpLayoutRequestHandlerWrapper stub,
        ISitecoreLayoutSerializer serializer,
        SitecoreLayoutRequest request)
    {
        // Arrange
        stub.Responses.Push(new HttpResponseMessage
        {
            StatusCode = System.Net.HttpStatusCode.NotFound,
            Content = new StringContent("JSON")
        });

        // Act
        SitecoreLayoutResponse result = await stub.Request(request, string.Empty);

        // Assert
        serializer.Received(1).Deserialize("JSON");
        result.Content.Should().BeEquivalentTo(CannedResponses.Simple);
        result.HasErrors.Should().BeTrue();
        result.Errors.Should().ContainSingle(x => x.GetType() == typeof(ItemNotFoundSitecoreLayoutServiceClientException));
        result.Errors.First().Data[StatusCodeKey].Should().Be((int)System.Net.HttpStatusCode.NotFound);
    }

    [Theory]
    [AutoNSubstituteData]
    public async Task Request_With40xStatusCode_PopulatesErrors(
        HttpLayoutRequestHandlerWrapper stub,
        SitecoreLayoutRequest request)
    {
        // Arrange
        stub.Responses.Push(new HttpResponseMessage
        {
            StatusCode = System.Net.HttpStatusCode.BadRequest
        });

        // Act
        SitecoreLayoutResponse result = await stub.Request(request, string.Empty);

        // Assert
        result.HasErrors.Should().BeTrue();
        result.Errors.Should().ContainSingle(x => x.GetType() == typeof(InvalidRequestSitecoreLayoutServiceClientException));
        result.Errors.First().Data[StatusCodeKey].Should().Be((int)System.Net.HttpStatusCode.BadRequest);
    }

    [Theory]
    [AutoNSubstituteData]
    public async Task Request_With50xStatusCode_PopulatesErrors(
        HttpLayoutRequestHandlerWrapper stub,
        SitecoreLayoutRequest request)
    {
        // Arrange
        stub.Responses.Push(new HttpResponseMessage
        {
            StatusCode = System.Net.HttpStatusCode.InternalServerError
        });

        // Act
        SitecoreLayoutResponse result = await stub.Request(request, string.Empty);

        // Assert
        result.HasErrors.Should().BeTrue();
        result.Errors.Should().ContainSingle(x => x.GetType() == typeof(InvalidResponseSitecoreLayoutServiceClientException));
        result.Errors.ToList().First().InnerException.Should().NotBeNull();
        result.Errors.ToList().First().InnerException.Should().BeOfType(typeof(SitecoreLayoutServiceServerException));
        result.Errors.First().Data[StatusCodeKey].Should().Be((int)System.Net.HttpStatusCode.InternalServerError);
    }

    [Theory]
    [AutoNSubstituteData]
    public async Task Request_WithDeserializationError_PopulatesErrors(
        HttpLayoutRequestHandlerWrapper stub,
        ISitecoreLayoutSerializer serializer,
        SitecoreLayoutRequest request)
    {
        // Arrange
        serializer.Deserialize(Arg.Any<string>()).Throws(new Exception());

        // Act
        SitecoreLayoutResponse result = await stub.Request(request, string.Empty);

        // Assert
        result.HasErrors.Should().BeTrue();
        result.Errors.Should().ContainSingle(x => x.GetType() == typeof(InvalidResponseSitecoreLayoutServiceClientException));
        result.Errors.First().Data[StatusCodeKey].Should().Be((int)System.Net.HttpStatusCode.OK);
    }

    [Theory]
    [AutoNSubstituteData]
    public async Task Request_WithInvalidHttpHeaders_DoNotPopulatesErrors(
        HttpLayoutRequestHandlerWrapper stub,
        SitecoreLayoutRequest request)
    {
        // Arrange
        HttpResponseMessageWrapper? httpResponse = new(System.Net.HttpStatusCode.OK)
        {
            Headers = null
        };

        stub.Responses.Push(httpResponse);

        // Act
        SitecoreLayoutResponse result = await stub.Request(request, string.Empty);

        // Assert
        result.HasErrors.Should().BeFalse();
        result.Errors.Should().BeEmpty();
        result.Content.Should().NotBeNull();
    }
}