using System.Net;
using FluentAssertions;
using GraphQL;
using NSubstitute;
using Sitecore.AspNetCore.SDK.GraphQL.Request;
using Sitecore.AspNetCore.SDK.Pages.Request.Handlers.GraphQL;
using Sitecore.AspNetCore.SDK.TestData;
using Xunit;

namespace Sitecore.AspNetCore.SDK.RenderingEngine.Integration.Tests.Fixtures.Pages;

public class PagesEditingFixture(TestWebApplicationFactory<TestPagesProgram> factory) : IClassFixture<TestWebApplicationFactory<TestPagesProgram>>
{
    private readonly TestWebApplicationFactory<TestPagesProgram> factory = factory;

    [Fact]
    public async Task EditingRequest_ValidRequest_ReturnsChromeDecoratedResponse()
    {
        // Arrange
        factory.MockGraphQLClient.SendQueryAsync<EditingLayoutQueryResponse>(Arg.Any<GraphQLHttpRequestWithHeaders>()).Returns(TestConstants.SimpleEditingLayoutQueryResponse);
        factory.MockGraphQLClient.SendQueryAsync<EditingDictionaryResponse>(Arg.Any<GraphQLRequest>()).Returns(TestConstants.DictionaryResponseWithoutPaging);

        HttpClient client = factory.CreateClient();
        string url = $"/Pages/index?mode=edit&secret={TestConstants.JssEditingSecret}&sc_itemid={TestConstants.TestItemId}&sc_version=1&sc_layoutKind=final";

        // Act
        var response = await client.GetAsync(url);
        var responseBody = await response.Content.ReadAsStringAsync();

        // Assert
        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        responseBody.Should().NotBeNullOrEmpty();
        responseBody.Should().Contain("<code chrometype='placeholder' class='scpm' kind='open' type='text/sitecore' id='headless-main_00000000-0000-0000-0000-000000000000'></code><code chrometype='placeholder' class='scpm' kind='close' type='text/sitecore'></code></div>");
        responseBody.Should().Contain("<code chrometype='placeholder' class='scpm' kind='close' type='text/sitecore'></code></div>");
    }
}