using AwesomeAssertions;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Response.Model.Presentation;
using Xunit;

namespace Sitecore.AspNetCore.SDK.LayoutService.Client.Tests.Response.Model.Presentation;

public class PersonalizationFixture
{
    [Fact]
    public void Ctor_SetsDefaults()
    {
        // Arrange & act
        Personalization sut = new();

        // Assert
        sut.Rules.Should().BeNull();
        sut.Conditions.Should().BeNull();
        sut.MultiVariateTestId.Should().BeNull();
        sut.PersonalizationTest.Should().BeNull();
    }
}