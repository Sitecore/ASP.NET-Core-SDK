using System.Dynamic;
using System.Text;
using FluentAssertions;
using Microsoft.AspNetCore.TestHost;
using NSubstitute;
using Sitecore.AspNetCore.SDK.ExperienceEditor.Configuration;
using Sitecore.AspNetCore.SDK.ExperienceEditor.Extensions;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Interfaces;
using Sitecore.AspNetCore.SDK.RenderingEngine.Extensions;
using Sitecore.AspNetCore.SDK.TestData;
using Xunit;

namespace Sitecore.AspNetCore.SDK.RenderingEngine.Integration.Tests.Fixtures.ExperienceEditor;

public class ExperienceEditorParsePathByItemIdFixture : IDisposable
{
    private const string PreviewRequest =
        """{"id":"jss-sample-app","args":["/?sc_itemid=%7bcfdd7ba2-e646-5294-87fc-6fad34451a97%7d&sc_ee_fb=false&sc_lang=en&sc_mode=preview&sc_debug=0&sc_trace=0&sc_prof=0&sc_ri=0&sc_rb=0","{\"sitecore\":{\"context\":{\"pageEditing\":false,\"site\":{\"name\":\"jss-sample-app\"},\"pageState\":\"preview\",\"language\":\"en\",\"itemPath\":\"/graphql/sample-1\"},\"route\":{\"name\":\"sample-1\",\"displayName\":\"sample-1\",\"fields\":{\"pageTitle\":{\"value\":\"Sample 1 Page Title\"}},\"databaseName\":\"master\",\"deviceId\":\"fe5d7fdf-89c0-4d99-9aa3-b5fbd009c9f3\",\"itemId\":\"cfdd7ba2-e646-5294-87fc-6fad34451a97\",\"itemLanguage\":\"en\",\"itemVersion\":1,\"layoutId\":\"5179e218-3df6-5af7-8147-d2d4c05da992\",\"templateId\":\"dfe73d70-9835-584e-b0f5-28c58ab064d7\",\"templateName\":\"App Route\",\"placeholders\":{\"jss-main\":[{\"uid\":\"9157cc8c-5760-5114-9f2d-93cbe39b30dc\",\"componentName\":\"ContentBlock\",\"dataSource\":\"{27F322AF-3D30-5F2F-ADA2-DAB630D35EC6}\",\"params\":{},\"fields\":{\"heading\":{\"value\":\"GraphQL Sample 1\"},\"content\":{\"value\":\"<p>A child route here to illustrate the power of GraphQL queries. <a href=\\\"/graphql\\\">Back to GraphQL route</a></p>\\n\"}}}]}}}}","{\"language\":\"en\",\"dictionary\":{\"Documentation\":\"Documentation\",\"GraphQL\":\"GraphQL\",\"Styleguide\":\"Styleguide\",\"styleguide-sample\":\"This is a dictionary entry in English as a demonstration\"},\"httpContext\":{\"request\":{\"url\":\"https://sc100xm1cm:443/?sc_itemid={cfdd7ba2-e646-5294-87fc-6fad34451a97}&sc_ee_fb=false&sc_lang=en&sc_mode=preview&sc_debug=0&sc_trace=0&sc_prof=0&sc_ri=0&sc_rb=0\",\"path\":\"/\",\"querystring\":{\"sc_itemid\":\"{cfdd7ba2-e646-5294-87fc-6fad34451a97}\",\"sc_ee_fb\":\"false\",\"sc_lang\":\"en\",\"sc_mode\":\"preview\",\"sc_debug\":\"0\",\"sc_trace\":\"0\",\"sc_prof\":\"0\",\"sc_ri\":\"0\",\"sc_rb\":\"0\"},\"userAgent\":\"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/84.0.4147.45 Safari/537.36 Edg/84.0.522.20\"}}}"],"functionName":"renderView","moduleName":"server.bundle","jssEditingSecret":"mysecret"}""";

    private const string PreviewRequestWithNullItemPath =
        """{"id":"jss-sample-app","args":["/?sc_itemid=%7bcfdd7ba2-e646-5294-87fc-6fad34451a97%7d&sc_ee_fb=false&sc_lang=en&sc_mode=preview&sc_debug=0&sc_trace=0&sc_prof=0&sc_ri=0&sc_rb=0","{\"sitecore\":{\"context\":{\"pageEditing\":false,\"site\":{\"name\":\"jss-sample-app\"},\"pageState\":\"preview\",\"language\":\"en\",\"itemPath\":\"\"},\"route\":{\"name\":\"sample-1\",\"displayName\":\"sample-1\",\"fields\":{\"pageTitle\":{\"value\":\"Sample 1 Page Title\"}},\"databaseName\":\"master\",\"deviceId\":\"fe5d7fdf-89c0-4d99-9aa3-b5fbd009c9f3\",\"itemId\":\"cfdd7ba2-e646-5294-87fc-6fad34451a97\",\"itemLanguage\":\"en\",\"itemVersion\":1,\"layoutId\":\"5179e218-3df6-5af7-8147-d2d4c05da992\",\"templateId\":\"dfe73d70-9835-584e-b0f5-28c58ab064d7\",\"templateName\":\"App Route\",\"placeholders\":{\"jss-main\":[{\"uid\":\"9157cc8c-5760-5114-9f2d-93cbe39b30dc\",\"componentName\":\"ContentBlock\",\"dataSource\":\"{27F322AF-3D30-5F2F-ADA2-DAB630D35EC6}\",\"params\":{},\"fields\":{\"heading\":{\"value\":\"GraphQL Sample 1\"},\"content\":{\"value\":\"<p>A child route here to illustrate the power of GraphQL queries. <a href=\\\"/graphql\\\">Back to GraphQL route</a></p>\\n\"}}}]}}}}","{\"language\":\"en\",\"dictionary\":{\"Documentation\":\"Documentation\",\"GraphQL\":\"GraphQL\",\"Styleguide\":\"Styleguide\",\"styleguide-sample\":\"This is a dictionary entry in English as a demonstration\"},\"httpContext\":{\"request\":{\"url\":\"https://sc100xm1cm:443/?sc_itemid={cfdd7ba2-e646-5294-87fc-6fad34451a97}&sc_ee_fb=false&sc_lang=en&sc_mode=preview&sc_debug=0&sc_trace=0&sc_prof=0&sc_ri=0&sc_rb=0\",\"path\":\"/\",\"querystring\":{\"sc_itemid\":\"{cfdd7ba2-e646-5294-87fc-6fad34451a97}\",\"sc_ee_fb\":\"false\",\"sc_lang\":\"en\",\"sc_mode\":\"preview\",\"sc_debug\":\"0\",\"sc_trace\":\"0\",\"sc_prof\":\"0\",\"sc_ri\":\"0\",\"sc_rb\":\"0\"},\"userAgent\":\"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/84.0.4147.45 Safari/537.36 Edg/84.0.522.20\"}}}"],"functionName":"renderView","moduleName":"server.bundle","jssEditingSecret":"mysecret"}""";

    private const string PreviewRequestWithoutItemPath =
        """{"id":"jss-sample-app","args":["/?sc_itemid=%7bcfdd7ba2-e646-5294-87fc-6fad34451a97%7d&sc_ee_fb=false&sc_lang=en&sc_mode=preview&sc_debug=0&sc_trace=0&sc_prof=0&sc_ri=0&sc_rb=0","{\"sitecore\":{\"context\":{\"pageEditing\":false,\"site\":{\"name\":\"jss-sample-app\"},\"pageState\":\"preview\",\"language\":\"en\"},\"route\":{\"name\":\"sample-1\",\"displayName\":\"sample-1\",\"fields\":{\"pageTitle\":{\"value\":\"Sample 1 Page Title\"}},\"databaseName\":\"master\",\"deviceId\":\"fe5d7fdf-89c0-4d99-9aa3-b5fbd009c9f3\",\"itemId\":\"cfdd7ba2-e646-5294-87fc-6fad34451a97\",\"itemLanguage\":\"en\",\"itemVersion\":1,\"layoutId\":\"5179e218-3df6-5af7-8147-d2d4c05da992\",\"templateId\":\"dfe73d70-9835-584e-b0f5-28c58ab064d7\",\"templateName\":\"App Route\",\"placeholders\":{\"jss-main\":[{\"uid\":\"9157cc8c-5760-5114-9f2d-93cbe39b30dc\",\"componentName\":\"ContentBlock\",\"dataSource\":\"{27F322AF-3D30-5F2F-ADA2-DAB630D35EC6}\",\"params\":{},\"fields\":{\"heading\":{\"value\":\"GraphQL Sample 1\"},\"content\":{\"value\":\"<p>A child route here to illustrate the power of GraphQL queries. <a href=\\\"/graphql\\\">Back to GraphQL route</a></p>\\n\"}}}]}}}}","{\"language\":\"en\",\"dictionary\":{\"Documentation\":\"Documentation\",\"GraphQL\":\"GraphQL\",\"Styleguide\":\"Styleguide\",\"styleguide-sample\":\"This is a dictionary entry in English as a demonstration\"},\"httpContext\":{\"request\":{\"url\":\"https://sc100xm1cm:443/?sc_itemid={cfdd7ba2-e646-5294-87fc-6fad34451a97}&sc_ee_fb=false&sc_lang=en&sc_mode=preview&sc_debug=0&sc_trace=0&sc_prof=0&sc_ri=0&sc_rb=0\",\"path\":\"/\",\"querystring\":{\"sc_itemid\":\"{cfdd7ba2-e646-5294-87fc-6fad34451a97}\",\"sc_ee_fb\":\"false\",\"sc_lang\":\"en\",\"sc_mode\":\"preview\",\"sc_debug\":\"0\",\"sc_trace\":\"0\",\"sc_prof\":\"0\",\"sc_ri\":\"0\",\"sc_rb\":\"0\"},\"userAgent\":\"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/84.0.4147.45 Safari/537.36 Edg/84.0.522.20\"}}}"],"functionName":"renderView","moduleName":"server.bundle","jssEditingSecret":"mysecret"}""";

    private readonly TestServer _server;

    public ExperienceEditorParsePathByItemIdFixture()
    {
        _server = CreateTestServer(options =>
        {
            options.Endpoint = TestConstants.EEMiddlewarePostEndpoint;
            options.JssEditingSecret = TestConstants.JssEditingSecret;
        });
    }

    [Fact]
    public async Task EE_SendPathWhenItemLoadedByItemID()
    {
        // Arrange
        const string eeUrl = TestConstants.EEMiddlewarePostEndpoint;

        // Act
        string response = await SendPost(_server, eeUrl, PreviewRequest);
        dynamic? json = Serializer.Deserialize<ExpandoObject>(response);
        string? actualPath = json?.html.ToString();

        // Asserts
        actualPath.Should().Be("/graphql/sample-1");
    }

    [Fact]
    public async Task EE_SendPathWhenItemLoadedByItemIDIsEmpty()
    {
        // Arrange
        const string eeUrl = TestConstants.EEMiddlewarePostEndpoint;

        // Act
        string response = await SendPost(_server, eeUrl, PreviewRequestWithNullItemPath);
        dynamic? json = Serializer.Deserialize<ExpandoObject>(response);
        string? actualPath = json?.html.ToString();

        // Asserts
        actualPath.Should().Be("/", "Expecting it to return Path value instead of itemPath.");
    }

    [Fact]
    public async Task EE_SendPathWhenItemLoadedWithoutItemID()
    {
        // Arrange
        const string eeUrl = TestConstants.EEMiddlewarePostEndpoint;

        // Act
        string response = await SendPost(_server, eeUrl, PreviewRequestWithoutItemPath);
        dynamic? json = Serializer.Deserialize<ExpandoObject>(response);
        string? actualPath = json?.html.ToString();

        // Asserts
        actualPath.Should().Be("/", "Expecting it to return Path value instead of itemPath.");
    }

    public void Dispose()
    {
        _server.Dispose();
        GC.SuppressFinalize(this);
    }

    private static async Task<string> SendPost(TestServer server, string eeUrl, string requestBody)
    {
        // Act
        HttpClient client = server.CreateClient();
        HttpResponseMessage response = await client
            .PostAsync(eeUrl, new StringContent(requestBody))
            .ConfigureAwait(false);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
    }

    private static TestServer CreateTestServer(Action<ExperienceEditorOptions> eeOptions) =>
        new TestServerBuilder()
            .ConfigureServices(builder =>
            {
                builder.AddSingleton(Substitute.For<ISitecoreLayoutClient>());
                builder.AddSitecoreRenderingEngine(options =>
                    {
                        options.AddDefaultComponentRenderer();
                    })
                    .WithExperienceEditor(eeOptions);
                builder.AddSingleton<TestMiddleware>();
            })
            .Configure(app =>
            {
                app.UseSitecoreExperienceEditor();
                app.UseMiddleware<TestMiddleware>();
                app.UseRouting();
                app.UseSitecoreRenderingEngine();
                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapFallbackToController("Default", "Home");
                });
            })
            .BuildServer(new Uri("http://localhost"));

#pragma warning disable CS9113
#pragma warning disable CA1822
    private class TestMiddleware(RequestDelegate next)
    {
        public async Task Invoke(HttpContext httpContext)
        {
            PathString path = httpContext.Request.Path;
            await httpContext.Response.Body.WriteAsync(Encoding.UTF8.GetBytes(path));
        }
    }
#pragma warning restore CA1822
#pragma warning restore CS9113
}