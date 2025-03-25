using GraphQL;
using Sitecore.AspNetCore.SDK.Pages.Request.Handlers.GraphQL;

namespace Sitecore.AspNetCore.SDK.Pages.Tests.Services
{
    public static class Constants
    {
        public static GraphQLResponse<EditingDictionaryResponse> DictionaryResponseWithoutPaging
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
                                        }
                                    },
                                    PageInfo = new PageInfo
                                    {
                                        HasNext = false,
                                        EndCursor = string.Empty
                                    }
                                }
                            }
                        }
                    }
                };
            }
        }

        public static GraphQLResponse<EditingDictionaryResponse> DictionaryResponseWithPaging
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
                                            Key = "page1",
                                            Value = "page1"
                                        }
                                    },
                                    PageInfo = new PageInfo
                                    {
                                        HasNext = true,
                                        EndCursor = "abcd1234"
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
