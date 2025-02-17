using GraphQL;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Request.Handlers.GraphQL;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Response;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Response.Model;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Serialization.Fields;
using Sitecore.AspNetCore.SDK.Pages.Request.Handlers.GraphQL;
using System.Text.Json;

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
                        },
                        Site = new Pages.Request.Handlers.GraphQL.Site
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
                        },
                        Site = new Pages.Request.Handlers.GraphQL.Site
                        {
                            SiteInfo = new SiteInfo
                            {
                                Dictionary = new SiteInfoDictionary
                                {
                                    PageInfo = new PageInfo
                                    {
                                        HasNext = true,
                                        EndCursor = "cursor_value_1234"
                                    }
                                }
                            }
                        }
                    }
                };
            }
        }

        public static GraphQLResponse<EditingDictionaryResponse> EditingDictionaryResponse
        {
            get
            {
                return new GraphQLResponse<EditingDictionaryResponse>
                {
                    Data = new EditingDictionaryResponse
                    {
                        Site = new Pages.Request.Handlers.GraphQL.Site
                        {
                            SiteInfo = new SiteInfo
                            {
                                Dictionary = new SiteInfoDictionary
                                {
                                    Results = new List<SiteInfoDictionaryItem>
                                    {
                                        new SiteInfoDictionaryItem
                                        {
                                            Key = "key1",
                                            Value = "value1"
                                        },
                                        new SiteInfoDictionaryItem
                                        {
                                            Key = "key2",
                                            Value = "value2"
                                        }
                                    },
                                    PageInfo = new PageInfo
                                    {
                                        HasNext = false
                                    }
                                }
                            }
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
                        },
                        Site = new Pages.Request.Handlers.GraphQL.Site
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
                                    "placeholder_1", new Placeholder()
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
                                    "placeholder_1", new Placeholder
                                    {
                                        new Component
                                        {
                                            Name = "component_1",
                                            Id = "component_1",
                                            Placeholders = new Dictionary<string, Placeholder>
                                            {
                                                {
                                                    "nested_placeholder_1", new Placeholder()
                                                }
                                            }
                                        }
                                    }
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
                                    "placeholder_1", new Placeholder
                                    {
                                        new Component
                                        {
                                            Name = "component_1",
                                            Id = "component_1"
                                        }
                                    }
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
                                    "placeholder_1", new Placeholder
                                    {
                                        new Component
                                        {
                                            Name = "component_1",
                                            Id = "component_1",
                                            Placeholders = new Dictionary<string, Placeholder>
                                            {
                                                {
                                                    "nested_placeholder_1", new Placeholder
                                                    {
                                                        new Component
                                                        {
                                                            Name = "nested_component_2",
                                                            Id = "nested_component_2"
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
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
                                    "placeholder_1", new Placeholder
                                    {
                                        new Component
                                        {
                                            Name = "component_1",
                                            Id = "component_1",
                                            Fields = new Dictionary<string, IFieldReader>
                                            {
                                                { "field_1", new JsonSerializedField(JsonDocument.Parse("{\"metadata\":{\"datasource\":{\"id\":\"datasource_id\",\"language\":\"en\",\"revision\":\"revision_1\",\"version\":1},\"title\":\"Text\",\"fieldId\":\"field_id\",\"fieldType\":\"Text\",\"rawValue\":\"field_raw_value\"},\"value\":\"field_value\"}")) }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                };
            }
        }
    }
}
