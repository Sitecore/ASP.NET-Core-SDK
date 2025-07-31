using AutoFixture;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using AwesomeAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using NSubstitute;
using Sitecore.AspNetCore.SDK.AutoFixture.Attributes;
using Sitecore.AspNetCore.SDK.AutoFixture.Extensions;
using Sitecore.AspNetCore.SDK.ExperienceEditor.Configuration;
using Sitecore.AspNetCore.SDK.ExperienceEditor.Extensions;
using Sitecore.AspNetCore.SDK.ExperienceEditor.Mappers;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Response;
using Xunit;

namespace Sitecore.AspNetCore.SDK.ExperienceEditor.Tests.Mappers;

public class SitecoreLayoutResponseMapperFixture
{
    // ReSharper disable once UnusedMember.Global - Used by testing framework
    public static Action<IFixture> AutoSetup => f =>
    {
        f.Register<string, PathString>(s => new PathString("/" + s));
        IOptions<ExperienceEditorOptions>? optionSub = f.Freeze<IOptions<ExperienceEditorOptions>>();
        ExperienceEditorOptions options = new();
        optionSub.Value.Returns(options);
    };

    [Theory]
    [AutoNSubstituteData]
    public void Ctor_InvalidArgs_Throws(GuardClauseAssertion guard)
    {
        guard.VerifyConstructors<SitecoreLayoutResponseMapper>();
    }

    [Theory]
    [AutoNSubstituteData]
    public void Map_NullRequest_ThrowsException(IOptions<ExperienceEditorOptions> options, SitecoreLayoutResponseContent content)
    {
        // Arrange
        SitecoreLayoutResponseMapper sut = new(options);

        // Arrange
        Action action = () => sut.MapRoute(content, "/", null!);

        // Act / Assert
        action.Should().Throw<ArgumentNullException>().WithParameterName("request");
    }

    [Theory]
    [AutoNSubstituteData]
    public void Map_WithRequest_ReturnsMappedRequest(
        IOptions<ExperienceEditorOptions> options,
        [Frozen] HttpRequest request,
        [Frozen] SitecoreLayoutResponseContent content)
    {
        // Arrange
        content.Sitecore!.Route!.DatabaseName = "master";
        options.Value.MapToRequest((sitecoreResponse, scPath, httpRequest) =>
            httpRequest.Path = scPath + sitecoreResponse.Sitecore?.Route?.DatabaseName);
        SitecoreLayoutResponseMapper sut = new(options);

        // Act
        string? result = sut.MapRoute(content, "/testString/", request);

        // Assert
        result.Should().Be("/testString/master");
    }
}