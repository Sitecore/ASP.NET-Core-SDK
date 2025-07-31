using System.Net;
using System.Text;
using AwesomeAssertions;
using Microsoft.AspNetCore.TestHost;
using NSubstitute;
using Sitecore.AspNetCore.SDK.ExperienceEditor.Extensions;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Interfaces;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Request;
using Sitecore.AspNetCore.SDK.RenderingEngine.Extensions;
using Sitecore.AspNetCore.SDK.TestData;
using Xunit;

// ReSharper disable StringLiteralTypo
namespace Sitecore.AspNetCore.SDK.RenderingEngine.Integration.Tests.Fixtures.ExperienceEditor;

public class ExperienceEditorFixture : IDisposable
{
    private readonly TestServer _server;

    public ExperienceEditorFixture()
    {
        TestServerBuilder testHostBuilder = new();
        _ = testHostBuilder
            .ConfigureServices(builder =>
            {
                builder.AddSingleton(Substitute.For<ISitecoreLayoutClient>());
                builder.AddSitecoreRenderingEngine(options =>
                {
                    options.AddDefaultComponentRenderer();
                }).WithExperienceEditor(options =>
                {
                    options.Endpoint = TestConstants.EEMiddlewarePostEndpoint;
                    options.JssEditingSecret = TestConstants.JssEditingSecret;
                });
            })
            .Configure(app =>
            {
                app.UseSitecoreExperienceEditor();
                app.UseRouting();
                app.UseSitecoreRenderingEngine();
                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapFallbackToController("Default", "Home");
                });
            });

        _server = testHostBuilder.BuildServer(new Uri("http://localhost"));
    }

    [Fact]
    public async Task EEEndpoint_SendsNonWrappedResponse_WhenGetRequest()
    {
        // Arrange
        HttpClient client = _server.CreateClient();

        // Act
        HttpResponseMessage response = await client.GetAsync(TestConstants.EEMiddlewarePostEndpoint);
        string responseString = await response.Content.ReadAsStringAsync();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        responseString.Should().NotContain("{\"html\":\"{");
    }

    [Fact]
    public async Task EEEndpoint_SendsNonWrappedResponse_WhenGetRequestInSampleEndPoint()
    {
        // Arrange
        HttpClient client = _server.CreateClient();

        // Act
        HttpResponseMessage response = await client.GetAsync(TestConstants.SampleEndPoint);
        string responseString = await response.Content.ReadAsStringAsync();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        responseString.Should().NotContain("{\"html\":\"{");
    }

    [Fact]
    public async Task EEEndpoint_Sends400ErrorCodeInResponse_WhenEmptyStringSentInRequestBody()
    {
        // Arrange
        HttpClient client = _server.CreateClient();
        StringContent content = new(string.Empty, Encoding.UTF8, "application/json");

        // Act
        HttpResponseMessage response = await client
            .PostAsync(TestConstants.EEMiddlewarePostEndpoint, content);
        string responseString = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        responseString.Should().NotContain("{\"html\":\"{");
    }

    [Fact]
    public async Task EEEndpoint_Sends400ErrorCodeInResponse_WhenInvalidRequestBody()
    {
        // Arrange
        HttpClient client = _server.CreateClient();
        StringContent content = new("abcnnmkksmvkdmfvkdkvmkdmv");

        HttpResponseMessage response = await client
            .PostAsync(TestConstants.EEMiddlewarePostEndpoint, content);
        string responseString = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        responseString.Should().NotContain("{\"html\":\"{");
    }

    [Fact]
    public async Task EEEndpoint_SendsNonWrappedResponse_WhenPostedToNonEEEndpoint()
    {
        // Arrange
        HttpClient client = _server.CreateClient();
        StringContent content = new(TestConstants.EESampleRequest);
        ISitecoreLayoutClient layoutClient = _server.Services.GetRequiredService<ISitecoreLayoutClient>();

        // Act
        HttpResponseMessage response = await client
                .PostAsync(TestConstants.SampleEndPoint, content)
            ;
        response.EnsureSuccessStatusCode();
        string responseString = await response.Content.ReadAsStringAsync();

        // Asserts
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        responseString.Should().Contain("null");
        responseString.Should().NotContain("{\"html\":\"{");
        await layoutClient.Received().Request(Arg.Is<SitecoreLayoutRequest>(x => x.Any()));
        await layoutClient.Received().Request(Arg.Is<SitecoreLayoutRequest>(x => x.Path() == TestConstants.SampleEndPoint));
    }

    [Fact]
    public async Task EEEndpoint_SendsCorrectResponse_WhenCorrectDataSentInRequestBody()
    {
        // Arrange
        HttpClient client = _server.CreateClient();
        StringContent content = new(TestConstants.EESampleRequest);

        // Act
        HttpResponseMessage response = await client
            .PostAsync(TestConstants.EEMiddlewarePostEndpoint, content);
        response.EnsureSuccessStatusCode();
        string responseString = await response.Content.ReadAsStringAsync();

        // Asserts
        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.ReasonPhrase.Should().Be("OK");
        response.Content.Headers.ContentType!.MediaType = "application/json";
        response.Content.Headers.ContentType.CharSet = "utf-8";
        responseString.Should().Contain("{\"html\":\"{");
        responseString.Should().EndWith("}");
        responseString.Should().Be(TestConstants.SampleResponseForEE);
    }

    [Fact]
    public async Task EEEndpoint_SendsCorrectResponse_WhenLargeDataSentInRequestBody()
    {
        // Arrange
        HttpClient client = _server.CreateClient();
        StringContent content = new(TestConstants.EELargeRequest);

        // Act
        HttpResponseMessage response = await client
            .PostAsync(TestConstants.EEMiddlewarePostEndpoint, content);
        response.EnsureSuccessStatusCode();
        string responseString = await response.Content.ReadAsStringAsync();

        // Asserts
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.ReasonPhrase.Should().Be("OK");
        responseString.Should().StartWith("{\"html\":\"{");
        responseString.Should().EndWith("}");
        responseString.Should().Be(TestConstants.SampleResponseForEE);
    }

    [Fact]
    public async Task EEEndpoint_ByPassesLayoutServiceRequest_WhenCorrectDataSentInRequestBody()
    {
        // Arrange
        HttpClient client = _server.CreateClient();
        StringContent content = new(TestConstants.EESampleRequest);
        ISitecoreLayoutClient layoutClient = _server.Services.GetRequiredService<ISitecoreLayoutClient>();

        // Act
        HttpResponseMessage response = await client
            .PostAsync(TestConstants.EEMiddlewarePostEndpoint, content);
        response.EnsureSuccessStatusCode();

        // Asserts
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        await layoutClient.DidNotReceive().Request(Arg.Is<SitecoreLayoutRequest>(x => x.Any()));
    }

    [Fact]
    public async Task EEEndpoint_MakeLayoutServiceRequest_WhenGetRequest()
    {
        // Arrange
        HttpClient client = _server.CreateClient();
        ISitecoreLayoutClient layoutClient = _server.Services.GetRequiredService<ISitecoreLayoutClient>();

        // Act
        HttpResponseMessage response = await client
            .GetAsync(TestConstants.EEMiddlewarePostEndpoint);
        response.EnsureSuccessStatusCode();

        // Asserts
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        await layoutClient.Received().Request(Arg.Is<SitecoreLayoutRequest>(x => x.Any()));
        await layoutClient.Received().Request(Arg.Is<SitecoreLayoutRequest>(x => x.Path() == TestConstants.EEMiddlewarePostEndpoint));
    }

    [Fact]
    public async Task EEEndpoint_MakesLayoutServiceRequest_WhenInvalidRequest_SendToSampleEEndPoint()
    {
        // Arrange
        HttpClient client = _server.CreateClient();
        StringContent content = new("invalidPostRequest");
        ISitecoreLayoutClient layoutClient = _server.Services.GetRequiredService<ISitecoreLayoutClient>();

        // Act
        HttpResponseMessage response = await client
            .PostAsync(TestConstants.SampleEndPoint, content);

        // Asserts
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        await layoutClient.Received().Request(Arg.Is<SitecoreLayoutRequest>(x => x.Any()));
    }

    [Fact]
    public async Task EEEndpoint_MakesLayoutServiceRequest_WhenPostedToNonEEEndpoint()
    {
        // Arrange
        HttpClient client = _server.CreateClient();
        ISitecoreLayoutClient layoutClient = _server.Services.GetRequiredService<ISitecoreLayoutClient>();

        // Act
        HttpResponseMessage response = await client.PostAsync(TestConstants.SampleEndPoint, new StringContent(string.Empty, Encoding.UTF8, "application/json"));

        // Asserts
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        await layoutClient.Received().Request(Arg.Is<SitecoreLayoutRequest>(x => x.Any()));
    }

    [Fact]
    public async Task EEEndpoint_MakesLayoutServiceRequest_WhenValidRequest_SendToSampleEndPoint()
    {
        // Arrange
        HttpClient client = _server.CreateClient();
        StringContent content = new(TestConstants.EESampleRequest);

        ISitecoreLayoutClient layoutClient = _server.Services.GetRequiredService<ISitecoreLayoutClient>();

        // Act
        HttpResponseMessage response = await client.PostAsync(TestConstants.SampleEndPoint, content);

        // Asserts
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        await layoutClient.Received().Request(Arg.Is<SitecoreLayoutRequest>(x => x.Any()));
    }

    [Fact]
    public async Task EEEndpoint_ChangePostToGet_WhenPostedToEEEndpoint()
    {
        // Arrange
        HttpClient client = _server.CreateClient();
        StringContent content = new(TestConstants.EESampleRoutingRequest);

        // Act
        HttpResponseMessage response = await client
            .PostAsync(TestConstants.EEMiddlewarePostEndpoint, content);
        response.EnsureSuccessStatusCode();
        string responseString = await response.Content.ReadAsStringAsync();

        // Asserts
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        responseString.Should().Be("""{"html":"\u0022GET\u0022"}""");
    }

    [Fact]
    public async Task EEEndpoint_NotChangePostToGet_WhenPostedToNonEEEndpoint()
    {
        // Arrange
        HttpClient client = _server.CreateClient();
        StringContent content = new(TestConstants.EESampleRoutingRequest);

        // Act
        HttpResponseMessage response = await client
            .PostAsync(TestConstants.SamplePostRouteEndPoint, content);
        response.EnsureSuccessStatusCode();
        string responseString = await response.Content.ReadAsStringAsync();

        // Asserts
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        responseString.Should().NotContain("GET");
        responseString.Should().Contain("POST");
    }

    [Theory]
    [InlineData("/jss-render/test")]
    [InlineData("/jss-render_test")]
    [InlineData("/jss-rendertest")]
    [InlineData("/jss-render test")]
    public async Task EEEndpoint_SendsInvalidResponse_WhenPostedToNonEEEndpoint(string invalidEndPoint)
    {
        // Arrange
        HttpClient client = _server.CreateClient();
        StringContent content = new(TestConstants.EESampleRequest);

        // Act
        HttpResponseMessage response = await client
            .PostAsync(invalidEndPoint, content);
        response.EnsureSuccessStatusCode();
        string responseString = await response.Content.ReadAsStringAsync();

        // Asserts
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        responseString.Should().NotContain("{\"html\":\"{");
    }

    [Theory]
    [InlineData("/Jss-Render")]
    [InlineData("/jss-RenDer")]
    [InlineData("/JSS-RENDER")]
    public async Task EEEndpoint_SendsCorrectResponse_withoutCaseSensitiveEndPoint(string validEndPoint)
    {
        // Arrange
        HttpClient client = _server.CreateClient();
        StringContent content = new(TestConstants.EESampleRequest);

        // Act
        HttpResponseMessage response = await client
            .PostAsync(validEndPoint, content);
        response.EnsureSuccessStatusCode();
        string responseString = await response.Content.ReadAsStringAsync();

        // Asserts
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        responseString.Should().Contain("{\"html\":\"{");
        responseString.Should().Be(TestConstants.SampleResponseForEE);
    }

    [Fact]
    public async Task EEEndpoint_ReturnSuccess_WhenRequestContainEmptyPath()
    {
        // Arrange
        HttpClient client = _server.CreateClient();
        StringContent content = new(TestConstants.EEEmptyRoutingRequest);

        // Act
        HttpResponseMessage response = await client
            .PostAsync(TestConstants.EEMiddlewarePostEndpoint, content);
        response.EnsureSuccessStatusCode();
        string responseString = await response.Content.ReadAsStringAsync();

        // Asserts
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        responseString.Should().Be("""{"html":"\u0022success\u0022"}""");
    }

    [Fact]
    public async Task EEEndpoint_NotChangeToGET_WhenRequestSendToDefaultRouting_ToSampleRoutingPoint()
    {
        // Arrange
        HttpClient client = _server.CreateClient();
        StringContent content = new(TestConstants.EEDefaultRoutingRequest);

        // Act
        HttpResponseMessage response = await client
            .PostAsync(TestConstants.SamplePostRouteEndPoint, content);
        response.EnsureSuccessStatusCode();
        string responseString = await response.Content.ReadAsStringAsync();

        // Asserts
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        responseString.Should().Contain("POST");
    }

    [Fact]
    public async Task EEEndpoint_NotChangeToGet_whenInvalidRequest_SendToSampleRoutingEndPoint()
    {
        // Arrange
        HttpClient client = _server.CreateClient();
        StringContent content = new("InvalidPost");

        // Act
        HttpResponseMessage response = await client
            .PostAsync(TestConstants.SamplePostRouteEndPoint, content);
        response.EnsureSuccessStatusCode();
        string responseString = await response.Content.ReadAsStringAsync();

        // Asserts
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        responseString.Should().Contain("POST");
    }

    [Fact]
    public async Task ReturnSuccess_whenRequestSendWithoutPath_ToSampleRoutingEndPoint()
    {
        // Arrange
        HttpClient client = _server.CreateClient();
        StringContent content = new(TestConstants.EmptyPathRequest);

        // Act
        HttpResponseMessage response = await client
            .PostAsync(TestConstants.EESampleRoutingRequest, content);
        response.EnsureSuccessStatusCode();
        string responseString = await response.Content.ReadAsStringAsync();

        // Asserts
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        responseString.Should().Contain("success");
    }

    [Fact]
    public async Task EEEndpoint_Send400ErrorCode_WhenIncompleteRequest_EEEndPoint()
    {
        // Arrange
        HttpClient client = _server.CreateClient();
        StringContent content = new(TestConstants.EEIncompleteRequest);

        // Act
        HttpResponseMessage response = await client
            .PostAsync(TestConstants.EEMiddlewarePostEndpoint, content);
        await response.Content.ReadAsStringAsync();

        // Asserts
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        response.ReasonPhrase.Should().Be("Bad Request");
    }

    [Fact]
    public async Task EEndpoint_ReturnSuccess_WhenRequestContainDefaultPath()
    {
        // Arrange
        HttpClient client = _server.CreateClient();
        StringContent content = new(TestConstants.EEDefaultRoutingRequest);

        // Act
        HttpResponseMessage response = await client
            .PostAsync(TestConstants.EEMiddlewarePostEndpoint, content);
        string responseString = await response.Content.ReadAsStringAsync();

        // Asserts
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        responseString.Should().Be("""{"html":"\u0022success\u0022"}""");
    }

    [Fact]
    public async Task EEEndpoint_SendsWrappedResponse_WhenRequestContainLongPath()
    {
        // Arrange
        HttpClient client = _server.CreateClient();
        StringContent content = new(TestConstants.EELongPathRequest);

        // Act
        HttpResponseMessage response = await client
            .PostAsync(TestConstants.EEMiddlewarePostEndpoint, content);
        string responseString = await response.Content.ReadAsStringAsync();

        // Asserts
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        responseString.Should().Be("""{"html":"\u0022success\u0022"}""");
    }

    [Fact]
    public async Task EEEndpoint_Sends400Error_WhenRequestDoesNotHaveItemPath()
    {
        // Arrange
        HttpClient client = _server.CreateClient();
        StringContent content = new(TestConstants.EmptyPathRequest);

        // Act
        HttpResponseMessage response = await client
            .PostAsync(TestConstants.EEMiddlewarePostEndpoint, content);
        await response.Content.ReadAsStringAsync();

        // Asserts
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        response.ReasonPhrase.Should().Be("Bad Request");
    }

    [Fact]
    public async Task EEEndpoint_ChangePostToGet_WhenRequestHaveCaseSensitivePath()
    {
        // Arrange
        HttpClient client = _server.CreateClient();
        StringContent content = new(TestConstants.CaseSensitiveItemPathRequest);

        // Act
        HttpResponseMessage response = await client
            .PostAsync(TestConstants.EEMiddlewarePostEndpoint, content);
        string responseString = await response.Content.ReadAsStringAsync();

        // Asserts
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        responseString.Should().Be("""{"html":"\u0022GET\u0022"}""");
    }

    [Fact]
    public async Task NotChangeToGet_WhenRequestContainCaseSensitivePath_PostToNonEEEndPoint()
    {
        // Arrange
        HttpClient client = _server.CreateClient();
        StringContent content = new(TestConstants.CaseSensitiveItemPathRequest);

        // Act
        HttpResponseMessage response = await client
            .PostAsync(TestConstants.SamplePostRouteEndPoint, content);
        string responseString = await response.Content.ReadAsStringAsync();

        // Asserts
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        responseString.Should().Contain("POST");
    }

    [Fact]
    public async Task EEEndpoint_SendsUnauthorizedErrorCodeInResponse_WhenWrongSecret()
    {
        // Arrange
        HttpClient client = _server.CreateClient();
        StringContent content = new(TestConstants.EESampleRequestWithWrongRequestedSecret, Encoding.UTF8, "application/json");

        // Act
        HttpResponseMessage response = await client
            .PostAsync(TestConstants.EEMiddlewarePostEndpoint, content);
        string responseString = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        responseString.Should().NotContain("{\"html\":\"{");
    }

    [Fact]
    public async Task EEEndpoint_SendsUnauthorizedErrorCodeInResponse_WhenNoSecret()
    {
        // Arrange
        HttpClient client = _server.CreateClient();
        StringContent content = new(TestConstants.EESampleRequestWithNoSecret, Encoding.UTF8, "application/json");

        // Act
        HttpResponseMessage response = await client
            .PostAsync(TestConstants.EEMiddlewarePostEndpoint, content);
        string responseString = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        responseString.Should().NotContain("{\"html\":\"{");
    }

    public void Dispose()
    {
        _server.Dispose();
        GC.SuppressFinalize(this);
    }
}