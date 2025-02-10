using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using AutoFixture;
using AutoFixture.Idioms;
using Castle.Core.Logging;
using FluentAssertions;
using GraphQL;
using GraphQL.Client.Abstractions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Sitecore.AspNetCore.SDK.AutoFixture.Attributes;
using Sitecore.AspNetCore.SDK.AutoFixture.Extensions;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Exceptions;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Request;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Request.Handlers.GraphQL;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Response;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Serialization;
using Sitecore.AspNetCore.SDK.Pages.GraphQL;
using Sitecore.AspNetCore.SDK.Pages.Request.Handlers.GraphQL;
using Xunit;

namespace Sitecore.AspNetCore.SDK.Pages.Tests.Request.Handlers.GraphQL
{
    public class GraphQLEditingServiceHandlerFixture
    {
        [ExcludeFromCodeCoverage]
        public static Action<IFixture> AutoSetup => f =>
        {
            IGraphQLClient client = Substitute.For<IGraphQLClient>();
            IGraphQLClientFactory clientFactory = Substitute.For<IGraphQLClientFactory>();
            clientFactory.GenerateClient(Arg.Any<string>(), Arg.Any<bool>()).Returns(client);
            client.SendQueryAsync<EditingLayoutQueryResponse>(Arg.Any<GraphQLRequest>()).Returns(new GraphQLResponse<EditingLayoutQueryResponse>
            {
                Data = new EditingLayoutQueryResponse
                {
                    Item = new ItemModel
                    {
                        Rendered = JsonDocument.Parse("{\"test\":\"value\"}").RootElement
                    },
                    Site = new Site
                    {
                        SiteInfo = new SiteInfo
                        {
                            Dictionary = new SiteInfoDictionary
                            {
                                PageInfo = new PageInfo
                                {
                                    HasNext = false
                                }
                            }
                        }
                    }
                }
            });

            f.Inject(clientFactory);
        };

        [Theory]
        [AutoNSubstituteData]
        public void Ctor_InvalidArgs_Throws(GuardClauseAssertion guard)
        {
            guard.VerifyConstructors<GraphQLEditingServiceHandler>();
        }

        [Theory]
        [AutoNSubstituteData]
        public async Task Request_RequestParamIsNull_ErrorThrown(GraphQLEditingServiceHandler sut)
        {
            // Act
            Func<Task> act = async () => { await sut.Request(null!, string.Empty); };

            // Assert
            await act.Should().ThrowAsync<ArgumentNullException>();
        }

        [Theory]
        [AutoNSubstituteData]
        public async Task Request_HandlerNameIsNull_ErrorThrown(GraphQLEditingServiceHandler sut)
        {
            // Act
            Func<Task> act = async () => { await sut.Request([], null!); };

            // Assert
            await act.Should().ThrowAsync<ArgumentException>();
        }

        [Theory]
        [AutoNSubstituteData]
        public async Task Request_HandlerNameIsEmptyString_ErrorThrown(GraphQLEditingServiceHandler sut)
        {
            // Act
            Func<Task> act = async () => { await sut.Request([], string.Empty); };

            // Assert
            await act.Should().ThrowAsync<ArgumentException>();
        }

        [Theory]
        [AutoNSubstituteData]
        public async Task Request_NotValidEditingRequest_ErrorThrown(GraphQLEditingServiceHandler sut)
        {
            // Arrange
            SitecoreLayoutRequest request = [];

            // Act
            Func<Task> act = async () => { await sut.Request(request, "editingHandler"); };

            // Assert
            await act.Should().ThrowAsync<ArgumentException>().WithMessage("GraphQLEditingServiceHandler: Error attempting to process non-editing request");
        }

        [Theory]
        [AutoNSubstituteData]
        public async Task Request_NoLanguageSet_ErrorThrown(GraphQLEditingServiceHandler sut)
        {
            // Arrange
            SitecoreLayoutRequest request = new SitecoreLayoutRequest
            {
                {
                    "sc_request_headers_key" , new Dictionary<string, string[]>()
                    {
                        { "mode", ["edit"] }
                    }
                }
            };

            // Act
            SitecoreLayoutResponse result = await sut.Request(request, "editingHandler");

            // Assert
            result.Errors.Should().ContainItemsAssignableTo<ItemNotFoundSitecoreLayoutServiceClientException>();
        }

        [Theory]
        [AutoNSubstituteData]
        public async Task Request_ValidRequest_NoErrorsThrown(IGraphQLClientFactory clientFactory)
        {
            // Arrange
            GraphQLEditingServiceHandler sut = new(clientFactory, Substitute.For<ISitecoreLayoutSerializer>(), Substitute.For<ILogger<GraphQLEditingServiceHandler>>());
            SitecoreLayoutRequest request = new()
            {
                {
                    "sc_request_headers_key" , new Dictionary<string, string[]>()
                    {
                        { "mode", ["edit"] },
                        { "language", ["en"] },
                        { "sc_layoutKind", ["Final"] },
                        { "sc_itemid", ["item_1234"] },
                        { "sc_version", ["version_1234"] },
                        { "sc_site", ["site_1234"] }
                    }
                },
                {
                    "sc_lang", "en"
                }
            };

            // Act
            SitecoreLayoutResponse result = await sut.Request(request, "editingHandler");

            // Assert
            result.Errors.Should().BeEmpty();
        }
    }
}
