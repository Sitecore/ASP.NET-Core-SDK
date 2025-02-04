using AutoFixture.Idioms;
using FluentAssertions;
using GraphQL.Client.Http;
using Sitecore.AspNetCore.SDK.AutoFixture.Attributes;
using Sitecore.AspNetCore.SDK.AutoFixture.Extensions;
using Sitecore.AspNetCore.SDK.Pages.GraphQL;
using Xunit;

namespace Sitecore.AspNetCore.SDK.Pages.Tests.GraphQL
{
    public class GraphQLClientFactoryFixture
    {
        [Theory]
        [AutoNSubstituteData]
        public void Ctor_InvalidArgs_Throws(GuardClauseAssertion guard)
        {
            guard.VerifyConstructors<GraphQLClientFactory>();
        }

        [Theory]
        [AutoNSubstituteData]
        public void GenerateClient_NullUriDefaultsToEdgePlatformUri(GraphQLClientFactory sut)
        {
            // Act
            var result = sut.GenerateClient(null, string.Empty, false);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<GraphQLHttpClient>();
            result.As<GraphQLHttpClient>().Options.EndPoint?.AbsoluteUri.Should().Contain("https://edge-platform.sitecorecloud.io/v1/content/api/graphql/v1");
        }

        [Theory]
        [AutoNSubstituteData]
        public void GenerateClient_OverriddenUriUsedInClient(GraphQLClientFactory sut)
        {
            // Act
            var result = sut.GenerateClient(new Uri("https://some.domain.com"), string.Empty, false);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<GraphQLHttpClient>();
            result.As<GraphQLHttpClient>().Options.EndPoint?.AbsoluteUri.Should().Contain("https://some.domain.com");
        }

        [Theory]
        [AutoNSubstituteData]
        public void GenerateClient_ContextIdIsAppendedToClientUri(GraphQLClientFactory sut)
        {
            // Arrange
            sut = new GraphQLClientFactory("1234");

            // Act
            var result = sut.GenerateClient(null, string.Empty, false);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<GraphQLHttpClient>();
            result.As<GraphQLHttpClient>().Options.EndPoint?.AbsoluteUri.Should().Contain("sitecoreContextId=1234");
        }

        [Theory]
        [AutoNSubstituteData]
        public void GenerateClient_CorrectParamsAreSetWhenGeneratingClient(GraphQLClientFactory sut)
        {
            // Arrange
            sut = new GraphQLClientFactory("1234");

            // Act
            var result = sut.GenerateClient(null, "layout_kid_1234", true);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<GraphQLHttpClient>();
            result.As<GraphQLHttpClient>().HttpClient.DefaultRequestHeaders.GetValues("sc_layoutKind").Should().Equal(["layout_kid_1234"]);
            result.As<GraphQLHttpClient>().HttpClient.DefaultRequestHeaders.GetValues("sc_editmode").Should().Equal([true.ToString()]);
        }
    }
}