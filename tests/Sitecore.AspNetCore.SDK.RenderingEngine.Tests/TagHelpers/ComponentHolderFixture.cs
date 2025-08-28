using AwesomeAssertions;
using NSubstitute;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Response.Model;
using Sitecore.AspNetCore.SDK.RenderingEngine.Interfaces;
using Sitecore.AspNetCore.SDK.RenderingEngine.TagHelpers;
using Xunit;

namespace Sitecore.AspNetCore.SDK.RenderingEngine.Tests.TagHelpers;

public class ComponentHolderFixture
{
    [Fact]
    public void ChangeComponentInCtor()
    {
        // Arrange
        Component testComponent = new();
        ISitecoreRenderingContext? context = Substitute.For<ISitecoreRenderingContext>();
        context.Component.Returns(new Component());

        // Act
        _ = new ComponentHolder(context, testComponent);

        // Arrange
        context.Component.Should().Be(testComponent);
    }

    [Fact]
    public void ReturnComponentInDispose()
    {
        // Arrange
        Component testComponent = new();
        ISitecoreRenderingContext? context = Substitute.For<ISitecoreRenderingContext>();
        context.Component.Returns(testComponent);

        // Act
        ComponentHolder holder = new(context, new Component());
        holder.Dispose();

        // Arrange
        context.Component.Should().Be(testComponent);
    }
}