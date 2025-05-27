using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using AutoFixture;
using AutoFixture.Idioms;
using FluentAssertions;
using GraphQL;
using GraphQL.Client.Abstractions;
using Microsoft.Extensions.Options;
using NSubstitute;
using Sitecore.AspNetCore.SDK.AutoFixture.Attributes;
using Sitecore.AspNetCore.SDK.AutoFixture.Extensions;
using Sitecore.AspNetCore.SDK.Pages.Configuration;
using Sitecore.AspNetCore.SDK.Pages.Request.Handlers.GraphQL;
using Sitecore.AspNetCore.SDK.Pages.Services;
using Xunit;

namespace Sitecore.AspNetCore.SDK.Pages.Tests.Services;

public class DictionaryServiceFixture
{
    [ExcludeFromCodeCoverage]
    public static Action<IFixture> AutoSetup => f =>
    {
        IOptions<PagesOptions> pagesOptions = Substitute.For<IOptions<PagesOptions>>();
        pagesOptions.Value.Returns(new PagesOptions());
        f.Inject(pagesOptions);
    };

    [Theory]
    [AutoNSubstituteData]
    public void Ctor_InvalidArgs_Throws(GuardClauseAssertion guard)
    {
        guard.VerifyConstructors<DictionaryService>();
    }

    [Theory]
    [AutoNSubstituteData]
    public async Task GetSiteDictionary_SiteNameIsNull_ErrorThrown(DictionaryService sut)
    {
        // Act
        Func<Task> act = async () => { await sut.GetSiteDictionary(string.Empty, null!, null!); };

        // Assert
        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Theory]
    [AutoNSubstituteData]
    public async Task GetSiteDictionary_LanguageIsNull_ErrorThrown(DictionaryService sut)
    {
        // Act
        Func<Task> act = async () => { await sut.GetSiteDictionary("valid_site", null!, null!); };

        // Assert
        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Theory]
    [AutoNSubstituteData]
    public async Task GetSiteDictionary_ClientIsNull_ErrorThrown(DictionaryService sut)
    {
        // Act
        Func<Task> act = async () => { await sut.GetSiteDictionary("valid_site", "valid_language", null!); };

        // Assert
        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Theory]
    [AutoNSubstituteData]
    public async Task GetSiteDictionary_SinglePageResults_ReturnsCorrectCollection(IOptions<PagesOptions> pageOptions)
    {
        // Arrange
        DictionaryService sut = new(pageOptions);
        IGraphQLClient graphQLClient = Substitute.For<IGraphQLClient>();
        graphQLClient.SendQueryAsync<EditingDictionaryResponse>(Arg.Any<GraphQLRequest>()).Returns(Constants.DictionaryResponseWithoutPaging);

        // Act
        List<SiteInfoDictionaryItem> result = await sut.GetSiteDictionary("valid_site", "valid_language", graphQLClient);

        // Assert
        result.Should().HaveCount(1);
    }

    [Theory]
    [AutoNSubstituteData]
    public async Task GetSiteDictionary_MultiplePageResult_ReturnsCorrectCollection(IOptions<PagesOptions> pageOptions)
    {
        // Arrange
        DictionaryService sut = new(pageOptions);
        IGraphQLClient graphQLClient = Substitute.For<IGraphQLClient>();
        graphQLClient.SendQueryAsync<EditingDictionaryResponse>(Arg.Is<GraphQLRequest>(x => GraphQLQueryHasAfterVariableWithValue(x, string.Empty))).Returns(Constants.DictionaryResponseWithPaging);
        graphQLClient.SendQueryAsync<EditingDictionaryResponse>(Arg.Is<GraphQLRequest>(x => GraphQLQueryHasAfterVariableWithValue(x, "abcd1234"))).Returns(Constants.DictionaryResponseWithoutPaging);

        // Act
        List<SiteInfoDictionaryItem> result = await sut.GetSiteDictionary("valid_site", "valid_language", graphQLClient);

        // Assert
        result.Should().HaveCount(2);
    }

    public bool GraphQLQueryHasAfterVariableWithValue(GraphQLRequest graphQlRequst, string expectedAfterValue)
    {
        if (!graphQlRequst.ContainsKey("variables"))
        {
            return false;
        }

        Type afterVariable = graphQlRequst["variables"].GetType();
        PropertyInfo? afterProperty = afterVariable.GetProperty("after");
        if (afterProperty == null)
        {
            return false;
        }

        object? afterVariableValue = afterProperty.GetValue(graphQlRequst["variables"]);
        if (afterVariableValue == null)
        {
            return false;
        }

        return afterVariableValue.ToString() == expectedAfterValue;
    }
}
