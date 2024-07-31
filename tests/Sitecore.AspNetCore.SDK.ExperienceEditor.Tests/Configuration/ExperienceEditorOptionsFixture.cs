using AutoFixture.Xunit2;
using FluentAssertions;
using Sitecore.AspNetCore.SDK.ExperienceEditor.Configuration;
using Xunit;

namespace Sitecore.AspNetCore.SDK.ExperienceEditor.Tests.Configuration;

public class ExperienceEditorOptionsFixture
{
    [Theory]
    [AutoData]
    public void Ctor_Assets_SetsDefaultValue([NoAutoProperties] ExperienceEditorOptions sut)
    {
        sut.Endpoint.Should().NotBeNullOrEmpty();
    }
}