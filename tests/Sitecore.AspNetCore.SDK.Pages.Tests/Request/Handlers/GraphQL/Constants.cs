using System.Text.Json;
using GraphQL;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Request.Handlers.GraphQL;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Response;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Response.Model;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Serialization.Fields;
using Sitecore.AspNetCore.SDK.Pages.Request.Handlers.GraphQL;

namespace Sitecore.AspNetCore.SDK.Pages.Tests.Request.Handlers.GraphQL
{
    public static class Constants
    {
        public static GraphQLResponse<EditingLayoutQueryResponse> SimpleEditingLayoutQueryResponse
        {
            get
            {
                return new GraphQLResponse<EditingLayoutQueryResponse>
                {
                    Data = new EditingLayoutQueryResponse
                    {
                        Item = new ItemModel
                        {
                            Rendered = JsonDocument.Parse("{\"test\":\"value\"}").RootElement
                        }
                    }
                };
            }
        }

        public static GraphQLResponse<EditingLayoutQueryResponse> EditingLayoutQueryResponseWithDictionaryPaging
        {
            get
            {
                return new GraphQLResponse<EditingLayoutQueryResponse>
                {
                    Data = new EditingLayoutQueryResponse
                    {
                        Item = new ItemModel
                        {
                            Rendered = JsonDocument.Parse("{\"test\":\"value\"}").RootElement
                        }
                    }
                };
            }
        }

        public static GraphQLResponse<EditingLayoutQueryResponse> MockEditingLayoutQueryResponse
        {
            get
            {
                return new GraphQLResponse<EditingLayoutQueryResponse>
                {
                    Data = new EditingLayoutQueryResponse
                    {
                        Item = new ItemModel
                        {
                            Rendered = JsonDocument.Parse(@"{ ""sitecore"" : {}}").RootElement
                        }
                    }
                };
            }
        }

        public static SitecoreLayoutResponseContent MockLayoutResponse_Placeholder
        {
            get
            {
                 return new SitecoreLayoutResponseContent
                 {
                     Sitecore = new SitecoreData
                     {
                         Route = new Route
                         {
                             Placeholders = new Dictionary<string, Placeholder>
                            {
                                {
                                    "placeholder_1", []
                                }
                            }
                         }
                     }
                 };
            }
        }

        public static SitecoreLayoutResponseContent MockLayoutResponse_NestedPlaceholder
        {
            get
            {
                return new SitecoreLayoutResponseContent
                {
                    Sitecore = new SitecoreData
                    {
                        Route = new Route
                        {
                            Placeholders = new Dictionary<string, Placeholder>
                            {
                                {
                                    "placeholder_1", [
                                        new Component
                                        {
                                            Name = "component_1",
                                            Id = "component_1",
                                            Placeholders = new Dictionary<string, Placeholder>
                                            {
                                                {
                                                    "nested_placeholder_1", []
                                                }
                                            }
                                        }
                                    ]
                                }
                            }
                        }
                    }
                };
            }
        }

        public static SitecoreLayoutResponseContent MockLayoutResponse_WithComponentInPlaceholder
        {
            get
            {
                return new SitecoreLayoutResponseContent
                {
                    Sitecore = new SitecoreData
                    {
                        Route = new Route
                        {
                            Placeholders = new Dictionary<string, Placeholder>
                            {
                                {
                                    "placeholder_1", [
                                        new Component
                                        {
                                            Name = "component_1",
                                            Id = "component_1"
                                        }
                                    ]
                                }
                            }
                        }
                    }
                };
            }
        }

        public static SitecoreLayoutResponseContent MockLayoutResponse_ComponentInNestedPlaceholder
        {
            get
            {
                return new SitecoreLayoutResponseContent
                {
                    Sitecore = new SitecoreData
                    {
                        Route = new Route
                        {
                            Placeholders = new Dictionary<string, Placeholder>
                            {
                                {
                                    "placeholder_1", [
                                        new Component
                                        {
                                            Name = "component_1",
                                            Id = "component_1",
                                            Placeholders = new Dictionary<string, Placeholder>
                                            {
                                                {
                                                    "nested_placeholder_1", [
                                                        new Component
                                                        {
                                                            Name = "nested_component_2",
                                                            Id = "nested_component_2"
                                                        }
                                                    ]
                                                }
                                            }
                                        }
                                    ]
                                }
                            }
                        }
                    }
                };
            }
        }

        public static SitecoreLayoutResponseContent MockLayoutResponse_ComponentWithField
        {
            get
            {
                return new SitecoreLayoutResponseContent
                {
                    Sitecore = new SitecoreData
                    {
                        Route = new Route
                        {
                            Placeholders = new Dictionary<string, Placeholder>
                            {
                                {
                                    "placeholder_1", [
                                        new Component
                                        {
                                            Name = "component_1",
                                            Id = "component_1",
                                            Fields = new Dictionary<string, IFieldReader>
                                            {
                                                {
                                                    "field_1",
                                                    new JsonSerializedField(JsonDocument.Parse(
                                                        "{\"metadata\":{\"datasource\":{\"id\":\"datasource_id\",\"language\":\"en\",\"revision\":\"revision_1\",\"version\":1},\"title\":\"Text\",\"fieldId\":\"field_id\",\"fieldType\":\"Text\",\"rawValue\":\"field_raw_value\"},\"value\":\"field_value\"}"))
                                                }
                                            }
                                        }
                                    ]
                                }
                            }
                        }
                    }
                };
            }
        }
    }
}
