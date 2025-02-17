using System.Diagnostics.CodeAnalysis;
using AutoFixture;
using AutoFixture.Idioms;
using FluentAssertions;
using GraphQL;
using GraphQL.Client.Abstractions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Sitecore.AspNetCore.SDK.AutoFixture.Attributes;
using Sitecore.AspNetCore.SDK.AutoFixture.Extensions;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Exceptions;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Request;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Response;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Response.Model;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Serialization;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Serialization.Fields;
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
            f.Inject(client);

            IGraphQLClientFactory clientFactory = Substitute.For<IGraphQLClientFactory>();
            f.Inject(clientFactory);

            ISitecoreLayoutSerializer mockSerializer = Substitute.For<ISitecoreLayoutSerializer>();
            f.Inject(mockSerializer);

            SitecoreLayoutRequest request = new()
            {
                {
                    "sc_request_headers_key" , new Dictionary<string, string[]>()
                    {
                        { "mode", ["edit"] },
                        { "language", ["en"] },
                        { "sc_layoutKind", ["Final"] },
                        { "sc_itemid", ["item_1234"] },
                        { "sc_version", ["version_1234"] }
                    }
                },
                {
                    "sc_lang", "en"
                },
                {
                    "sc_site", "site_1234"
                }
            };
            f.Inject(request);
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
        public async Task Request_ValidRequest_NoErrorsThrown(IGraphQLClientFactory clientFactory, IGraphQLClient client, SitecoreLayoutRequest request)
        {
            // Arrange
            clientFactory.GenerateClient(Arg.Any<string>(), Arg.Any<bool>()).Returns(client);
            client.SendQueryAsync<EditingLayoutQueryResponse>(Arg.Any<GraphQLRequest>()).Returns(Constants.SimpleEditingLayoutQueryResponse);
            GraphQLEditingServiceHandler sut = new(clientFactory, Substitute.For<ISitecoreLayoutSerializer>(), Substitute.For<ILogger<GraphQLEditingServiceHandler>>());

            // Act
            SitecoreLayoutResponse result = await sut.Request(request, "editingHandler");

            // Assert
            result.Errors.Should().BeEmpty();
            await client.Received(1).SendQueryAsync<EditingLayoutQueryResponse>(Arg.Any<GraphQLRequest>());
        }

        [Theory]
        [AutoNSubstituteData]
        public async Task Request_ValidRequest_DictionaryRequestMade(IGraphQLClientFactory clientFactory, IGraphQLClient client, SitecoreLayoutRequest request)
        {
            // Arrange
            clientFactory.GenerateClient(Arg.Any<string>(), Arg.Any<bool>()).Returns(client);
            client.SendQueryAsync<EditingLayoutQueryResponse>(Arg.Any<GraphQLRequest>()).Returns(Constants.EditingLayoutQueryResponseWithDictionaryPaging);
            client.SendQueryAsync<EditingDictionaryResponse>(Arg.Any<GraphQLRequest>()).Returns(Constants.EditingDictionaryResponse);
            GraphQLEditingServiceHandler sut = new(clientFactory, Substitute.For<ISitecoreLayoutSerializer>(), Substitute.For<ILogger<GraphQLEditingServiceHandler>>());

            // Act
            SitecoreLayoutResponse result = await sut.Request(request, "editingHandler");

            // Asset
            result.Errors.Should().BeEmpty();
            await client.Received(1).SendQueryAsync<EditingLayoutQueryResponse>(Arg.Any<GraphQLRequest>());
            await client.Received(1).SendQueryAsync<EditingDictionaryResponse>(Arg.Any<GraphQLRequest>());
        }

        [Theory]
        [AutoNSubstituteData]
        public async Task Request_ValidRequest_PlaceholderChromesAreAdded(IGraphQLClientFactory clientFactory, IGraphQLClient client, ISitecoreLayoutSerializer mockSerializer, SitecoreLayoutRequest request)
        {
            // Arrange
            clientFactory.GenerateClient(Arg.Any<string>(), Arg.Any<bool>()).Returns(client);
            client.SendQueryAsync<EditingLayoutQueryResponse>(Arg.Any<GraphQLRequest>()).Returns(Constants.MockEditingLayoutQueryResponse);
            mockSerializer.Deserialize(Arg.Any<string>()).Returns(Constants.MockLayoutResponse_Placeholder);
            GraphQLEditingServiceHandler sut = new(clientFactory, mockSerializer, Substitute.For<ILogger<GraphQLEditingServiceHandler>>());

            // Act
            SitecoreLayoutResponse result = await sut.Request(request, "editingHandler");

            // Assert
            result.Errors.Should().BeEmpty();
            result?.Content?.Sitecore?.Route?.Placeholders["placeholder_1"].Count.Should().Be(2);
            result?.Content?.Sitecore?.Route?.Placeholders["placeholder_1"][0].Should().BeOfType<EditableChrome>();
            result?.Content?.Sitecore?.Route?.Placeholders["placeholder_1"][0].As<EditableChrome>().Attributes["chrometype"].Should().Be("placeholder");
            result?.Content?.Sitecore?.Route?.Placeholders["placeholder_1"][0].As<EditableChrome>().Attributes["kind"].Should().Be("open");
            result?.Content?.Sitecore?.Route?.Placeholders["placeholder_1"][0].As<EditableChrome>().Attributes["id"].Should().Be($"placeholder_1_{Guid.Empty.ToString()}");
            result?.Content?.Sitecore?.Route?.Placeholders["placeholder_1"][1].Should().BeOfType<EditableChrome>();
            result?.Content?.Sitecore?.Route?.Placeholders["placeholder_1"][1].As<EditableChrome>().Attributes["chrometype"].Should().Be("placeholder");
            result?.Content?.Sitecore?.Route?.Placeholders["placeholder_1"][1].As<EditableChrome>().Attributes["kind"].Should().Be("close");
        }

        [Theory]
        [AutoNSubstituteData]
        public async Task Request_ValidRequest_NestedPlaceholderChromesAreAdded(IGraphQLClientFactory clientFactory, IGraphQLClient client, ISitecoreLayoutSerializer mockSerializer, SitecoreLayoutRequest request)
        {
            // Arrange
            clientFactory.GenerateClient(Arg.Any<string>(), Arg.Any<bool>()).Returns(client);
            client.SendQueryAsync<EditingLayoutQueryResponse>(Arg.Any<GraphQLRequest>()).Returns(Constants.MockEditingLayoutQueryResponse);
            mockSerializer.Deserialize(Arg.Any<string>()).Returns(Constants.MockLayoutResponse_NestedPlaceholder);
            GraphQLEditingServiceHandler sut = new(clientFactory, mockSerializer, Substitute.For<ILogger<GraphQLEditingServiceHandler>>());

            // Act
            SitecoreLayoutResponse result = await sut.Request(request, "editingHandler");

            // Assert
            result.Errors.Should().BeEmpty();
            result?.Content?.Sitecore?.Route?.Placeholders["placeholder_1"][2].As<Component>().Placeholders["nested_placeholder_1"][0].Should().BeOfType<EditableChrome>();
            result?.Content?.Sitecore?.Route?.Placeholders["placeholder_1"][2].As<Component>().Placeholders["nested_placeholder_1"][0].As<EditableChrome>().Attributes["chrometype"].Should().Be("placeholder");
            result?.Content?.Sitecore?.Route?.Placeholders["placeholder_1"][2].As<Component>().Placeholders["nested_placeholder_1"][0].As<EditableChrome>().Attributes["kind"].Should().Be("open");
            result?.Content?.Sitecore?.Route?.Placeholders["placeholder_1"][2].As<Component>().Placeholders["nested_placeholder_1"][0].As<EditableChrome>().Attributes["id"].Should().Be("container-{*}_component_1");
            result?.Content?.Sitecore?.Route?.Placeholders["placeholder_1"][2].As<Component>().Placeholders["nested_placeholder_1"][1].Should().BeOfType<EditableChrome>();
            result?.Content?.Sitecore?.Route?.Placeholders["placeholder_1"][2].As<Component>().Placeholders["nested_placeholder_1"][1].As<EditableChrome>().Attributes["chrometype"].Should().Be("placeholder");
            result?.Content?.Sitecore?.Route?.Placeholders["placeholder_1"][2].As<Component>().Placeholders["nested_placeholder_1"][1].As<EditableChrome>().Attributes["kind"].Should().Be("close");
        }

        [Theory]
        [AutoNSubstituteData]
        public async Task Request_ValidRequest_RenderingChromesAreAdded(IGraphQLClientFactory clientFactory, IGraphQLClient client, ISitecoreLayoutSerializer mockSerializer, SitecoreLayoutRequest request)
        {
            // Arrange
            clientFactory.GenerateClient(Arg.Any<string>(), Arg.Any<bool>()).Returns(client);
            client.SendQueryAsync<EditingLayoutQueryResponse>(Arg.Any<GraphQLRequest>()).Returns(Constants.MockEditingLayoutQueryResponse);
            mockSerializer.Deserialize(Arg.Any<string>()).Returns(Constants.MockLayoutResponse_WithComponentInPlaceholder);
            GraphQLEditingServiceHandler sut = new(clientFactory, mockSerializer, Substitute.For<ILogger<GraphQLEditingServiceHandler>>());

            // Act
            SitecoreLayoutResponse result = await sut.Request(request, "editingHandler");

            // Assert
            result.Errors.Should().BeEmpty();
            result?.Content?.Sitecore?.Route?.Placeholders["placeholder_1"].Count.Should().Be(5);
            result?.Content?.Sitecore?.Route?.Placeholders["placeholder_1"][1].Should().BeOfType<EditableChrome>();
            result?.Content?.Sitecore?.Route?.Placeholders["placeholder_1"][1].As<EditableChrome>().Attributes["chrometype"].Should().Be("rendering");
            result?.Content?.Sitecore?.Route?.Placeholders["placeholder_1"][1].As<EditableChrome>().Attributes["kind"].Should().Be("open");
            result?.Content?.Sitecore?.Route?.Placeholders["placeholder_1"][1].As<EditableChrome>().Attributes["id"].Should().Be($"component_1");
            result?.Content?.Sitecore?.Route?.Placeholders["placeholder_1"][3].Should().BeOfType<EditableChrome>();
            result?.Content?.Sitecore?.Route?.Placeholders["placeholder_1"][3].As<EditableChrome>().Attributes["chrometype"].Should().Be("rendering");
            result?.Content?.Sitecore?.Route?.Placeholders["placeholder_1"][3].As<EditableChrome>().Attributes["kind"].Should().Be("close");
        }

        [Theory]
        [AutoNSubstituteData]
        public async Task Request_ValidRequest_RenderingInNestedPlaceholderChromesAreAdded(IGraphQLClientFactory clientFactory, IGraphQLClient client, ISitecoreLayoutSerializer mockSerializer, SitecoreLayoutRequest request)
        {
            // Arrange
            clientFactory.GenerateClient(Arg.Any<string>(), Arg.Any<bool>()).Returns(client);
            client.SendQueryAsync<EditingLayoutQueryResponse>(Arg.Any<GraphQLRequest>()).Returns(Constants.MockEditingLayoutQueryResponse);
            mockSerializer.Deserialize(Arg.Any<string>()).Returns(Constants.MockLayoutResponse_ComponentInNestedPlaceholder);
            GraphQLEditingServiceHandler sut = new(clientFactory, mockSerializer, Substitute.For<ILogger<GraphQLEditingServiceHandler>>());

            // Act
            SitecoreLayoutResponse result = await sut.Request(request, "editingHandler");

            // Assert
            result.Errors.Should().BeEmpty();
            result?.Content?.Sitecore?.Route?.Placeholders["placeholder_1"].Count.Should().Be(5);
            result?.Content?.Sitecore?.Route?.Placeholders["placeholder_1"][2].As<Component>().Placeholders["nested_placeholder_1"][1].Should().BeOfType<EditableChrome>();
            result?.Content?.Sitecore?.Route?.Placeholders["placeholder_1"][2].As<Component>().Placeholders["nested_placeholder_1"][1].As<EditableChrome>().Attributes["chrometype"].Should().Be("rendering");
            result?.Content?.Sitecore?.Route?.Placeholders["placeholder_1"][2].As<Component>().Placeholders["nested_placeholder_1"][1].As<EditableChrome>().Attributes["kind"].Should().Be("open");
            result?.Content?.Sitecore?.Route?.Placeholders["placeholder_1"][2].As<Component>().Placeholders["nested_placeholder_1"][1].As<EditableChrome>().Attributes["id"].Should().Be($"nested_component_2");
            result?.Content?.Sitecore?.Route?.Placeholders["placeholder_1"][2].As<Component>().Placeholders["nested_placeholder_1"][3].Should().BeOfType<EditableChrome>();
            result?.Content?.Sitecore?.Route?.Placeholders["placeholder_1"][2].As<Component>().Placeholders["nested_placeholder_1"][3].As<EditableChrome>().Attributes["chrometype"].Should().Be("rendering");
            result?.Content?.Sitecore?.Route?.Placeholders["placeholder_1"][2].As<Component>().Placeholders["nested_placeholder_1"][3].As<EditableChrome>().Attributes["kind"].Should().Be("close");
        }

        [Theory]
        [AutoNSubstituteData]
        public async Task Request_ValidRequest_FieldRenderingChromesAreAdded(IGraphQLClientFactory clientFactory, IGraphQLClient client, ISitecoreLayoutSerializer mockSerializer, SitecoreLayoutRequest request)
        {
            // Arrange
            clientFactory.GenerateClient(Arg.Any<string>(), Arg.Any<bool>()).Returns(client);
            client.SendQueryAsync<EditingLayoutQueryResponse>(Arg.Any<GraphQLRequest>()).Returns(Constants.MockEditingLayoutQueryResponse);
            mockSerializer.Deserialize(Arg.Any<string>()).Returns(Constants.MockLayoutResponse_ComponentWithField);
            GraphQLEditingServiceHandler sut = new(clientFactory, mockSerializer, Substitute.For<ILogger<GraphQLEditingServiceHandler>>());

            // Act
            SitecoreLayoutResponse result = await sut.Request(request, "editingHandler");

            // Assert
            result.Errors.Should().BeEmpty();
            result?.Content?.Sitecore?.Route?.Placeholders["placeholder_1"].Count.Should().Be(5);
            result?.Content?.Sitecore?.Route?.Placeholders["placeholder_1"][2].As<Component>().Fields.Values.Count.Should().Be(1);
            result?.Content?.Sitecore?.Route?.Placeholders["placeholder_1"][2].As<Component>().Fields["field_1"].Should().BeOfType<JsonSerializedField>();
            JsonSerializedField? jsonSerialisedField = result?.Content?.Sitecore?.Route?.Placeholders["placeholder_1"][2].As<Component>().Fields["field_1"] as JsonSerializedField;
            jsonSerialisedField.Should().NotBeNull();
            EditableField<object>? editableField = jsonSerialisedField?.Read<EditableField<object>>();
            editableField.Should().NotBeNull();
            editableField?.OpeningChrome.Should().NotBeNull();
            editableField?.OpeningChrome?.Attributes["chrometype"].Should().Be("field");
            editableField?.OpeningChrome?.Attributes["kind"].Should().Be("open");
            editableField?.OpeningChrome?.Content.Should().Be(@"{""datasource"":{""id"":""datasource_id"",""language"":""en"",""revision"":""revision_1"",""version"":1},""title"":""Text"",""fieldId"":""field_id"",""fieldType"":""Text"",""rawValue"":""field_raw_value""}");
            editableField?.ClosingChrome.Should().NotBeNull();
            editableField?.ClosingChrome?.Attributes["chrometype"].Should().Be("field");
            editableField?.ClosingChrome?.Attributes["kind"].Should().Be("close");
        }
    }
}