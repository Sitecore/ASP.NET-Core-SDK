using AutoFixture.Idioms;
using FluentAssertions;
using Sitecore.AspNetCore.SDK.AutoFixture.Attributes;
using Sitecore.AspNetCore.SDK.AutoFixture.Extensions;
using Sitecore.AspNetCore.SDK.ExperienceEditor.Models;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Response;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Serialization;
using Sitecore.AspNetCore.SDK.TestData;
using Xunit;

namespace Sitecore.AspNetCore.SDK.ExperienceEditor.Tests.Models;

public class ExperienceEditorPostModelFixture
{
    private const string RequestExample =
        """{"id":"jssdevex","args":["/?sc_httprenderengineurl=https%3a%2f%2f8eeabadd.ngrok.io","{\"sitecore\":{\"context\":{\"pageEditing\":false,\"site\":{\"name\":\"jssdevex\"},\"pageState\":\"normal\",\"language\":\"en\"},\"route\":{\"name\":\"home\",\"displayName\":\"home\",\"fields\":{\"pageTitle\":{\"value\":\"Welcome to Sitecore JSS\"}},\"databaseName\":\"master\",\"deviceId\":\"fe5d7fdf-89c0-4d99-9aa3-b5fbd009c9f3\",\"itemId\":\"4e8410b0-28c5-52c5-8439-12a1ab247560\",\"itemLanguage\":\"en\",\"itemVersion\":1,\"layoutId\":\"80848506-1859-5f78-8fc6-f692c0c49795\",\"templateId\":\"6c0659f1-c66d-5877-a83b-510b6e0c64a2\",\"templateName\":\"App Route\",\"placeholders\":{\"jss-main\":[{\"uid\":\"2c4a53cc-9da8-5f51-9d79-6ee2fc671b2d\",\"componentName\":\"ContentBlock\",\"dataSource\":\"{695CF95F-3E00-5B9F-A090-EB9C6D666DB5}\",\"params\":{},\"fields\":{\"heading\":{\"value\":\"Welcome to Sitecore JSS\"},\"content\":{\"value\":\"<p>Thanks for using JSS!! Here are some resources to get you started:</p>\\n\\n<h3><a href=\\\"https://jss.sitecore.net\\\" rel=\\\"noopener noreferrer\\\">Documentation</a></h3>\\n<p>The official JSS documentation can help you with any JSS task from getting started to advanced techniques.</p>\\n\\n<h3><a href=\\\"/styleguide\\\">Styleguide</a></h3>\\n<p>The JSS styleguide is a living example of how to use JSS, hosted right in this app.\\nIt demonstrates most of the common patterns that JSS implementations may need to use,\\nas well as useful architectural patterns.</p>\\n\\n<h3><a href=\\\"/graphql\\\">GraphQL</a></h3>\\n<p>JSS features integration with the Sitecore GraphQL API to enable fetching non-route data from Sitecore - or from other internal backends as an API aggregator or proxy.\\nThis route is a living example of how to use an integrate with GraphQL data in a JSS app.</p>\\n\\n<div class=\\\"alert alert-dark\\\">\\n  <h4>This app is a boilerplate</h4>\\n  <p>The JSS samples are a boilerplate, not a library. That means that any code in this app is meant for you to own and customize to your own requirements.</p>\\n  <p>Want to change the lint settings? Do it. Want to read manifest data from a MongoDB database? Go for it. This app is yours.</p>\\n</div>\\n\\n<div class=\\\"alert alert-dark\\\">\\n  <h4>How to start with an empty app</h4>\\n  <p>To remove all of the default sample content (the Styleguide and GraphQL routes) and start out with an empty JSS app:</p>\\n  <ol>\\n    <li>Delete <code>/src/components/Styleguide*</code> and <code>/src/components/GraphQL*</code></li>\\n    <li>Delete <code>/sitecore/definitions/components/Styleguide*</code>, <code>/sitecore/definitions/templates/Styleguide*</code>, and <code>/sitecore/definitions/components/GraphQL*</code></li>\\n    <li>Delete <code>/data/component-content/Styleguide</code></li>\\n    <li>Delete <code>/data/content/Styleguide</code></li>\\n    <li>Delete <code>/data/routes/styleguide</code> and <code>/data/routes/graphql</code></li>\\n    <li>Delete <code>/data/dictionary/*.yml</code></li>\\n  </ol>\\n</div>\\n\"}}}]}}}}","{\"language\":\"en\",\"dictionary\":{\"Documentation\":\"Documentation\",\"GraphQL\":\"GraphQL\",\"Styleguide\":\"Styleguide\",\"styleguide-sample\":\"This is a dictionary entry in English as a demonstration\"},\"httpContext\":{\"request\":{\"url\":\"https://jssdevex.dev.local:443/?sc_httprenderengineurl=https://8eeabadd.ngrok.io\",\"path\":\"/\",\"querystring\":{\"sc_httprenderengineurl\":\"https://8eeabadd.ngrok.io\"},\"userAgent\":\"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/80.0.3987.100 Safari/537.36 Edg/80.0.361.53\"}}}"],"functionName":"renderView","moduleName":"server.bundle"}""";

    [Theory]
    [AutoNSubstituteData]
    public void Ctor_IsGuarded(GuardClauseAssertion guard)
    {
        // Act / Assert
        guard.VerifyConstructors<ExperienceEditorPostModel>();
    }

    [Fact]
    public void Ctor_SetsDefaults()
    {
        // Arrange / Act
        ExperienceEditorPostModel sut = new();

        // Assert
        sut.Id.Should().BeNull();
        sut.FunctionName.Should().BeNull();
        sut.ModuleName.Should().BeNull();
        sut.Args.Should().BeEmpty("Assigned by default to an empty list");
    }

    [Fact]
    public void EeRequestCanBeDeserializedToAnInstance()
    {
        ExperienceEditorPostModel? model = Serializer.Deserialize<ExperienceEditorPostModel>(RequestExample);

        JsonLayoutServiceSerializer serializer = new();
        SitecoreLayoutResponseContent? layout = serializer.Deserialize(model!.Args[1]);

        model.Should().NotBeNull();
        layout.Should().NotBeNull();
    }
}