using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NSubstitute;
using Sitecore.AspNetCore.SDK.AutoFixture.Attributes;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Response.Model.Fields;
using Sitecore.AspNetCore.SDK.RenderingEngine.Binding;
using Sitecore.AspNetCore.SDK.RenderingEngine.Tests.Mocks;
using Sitecore.AspNetCore.SDK.RenderingEngine.ViewComponents;
using Xunit;

namespace Sitecore.AspNetCore.SDK.RenderingEngine.Tests.ViewComponents;

public class SitecoreComponentViewComponentFixture
{
    // ReSharper disable once UnusedMember.Global - Used by testing framework
    public static Action<IFixture> AutoSetup()
    {
        return f =>
        {
            IViewModelBinder? viewModelBinder = Substitute.For<IViewModelBinder>();
            f.Inject(viewModelBinder);
            f.Inject(new SitecoreComponentViewComponent(viewModelBinder));
        };
    }

    [Fact]
    public void SitecoreComponentViewComponent_Ctor_Guarded()
    {
        Assert.Throws<ArgumentNullException>(() => new SitecoreComponentViewComponent(null!));
    }

    [Theory]
    [AutoNSubstituteData]
    public async Task InvokeAsync_ModelIsNull_ThrowsArgumentNullException(SitecoreComponentViewComponent sut)
    {
        // Arrange

        // Act
        ArgumentNullException exception = await Assert.ThrowsAsync<ArgumentNullException>(() => sut.InvokeAsync(null!, null!));

        // Assert
        exception.Message.Should().Be("Value cannot be null. (Parameter 'modelType')");
    }

    [Theory]
    [AutoNSubstituteData]
    public async Task InvokeAsync_ViewNameIsNull_ThrowsArgumentNullException(SitecoreComponentViewComponent sut)
    {
        // Arrange
        var model = new { PropertyA = "Test Property" };

        // Act
        ArgumentNullException exception = await Assert.ThrowsAsync<ArgumentNullException>(() => sut.InvokeAsync(model.GetType(), null!));

        // Assert
        exception.Message.Should().Be("Value cannot be null. (Parameter 'viewName')");
    }

    [Theory]
    [AutoNSubstituteData]
    public async Task InvokeAsync_ViewNameIsEmptyString_ThrowsArgumentNullException(SitecoreComponentViewComponent sut)
    {
        // Arrange
        var model = new { PropertyA = "Test Property" };
        Func<Task<IViewComponentResult>> act =
            () => sut.InvokeAsync(model.GetType(), string.Empty);

        // Act & Assert
        await act.Should().ThrowAsync<ArgumentException>().WithParameterName("viewName");
    }

    [Theory]
    [AutoNSubstituteData]
    public async Task InvokeAsync_ValidParameters(SitecoreComponentViewComponent sut, IViewModelBinder binder)
    {
        // Arrange
        HeaderBlock model = new() { Heading1 = new TextField("Test Heading 1"), Heading2 = new TextField("Test Heading 2") };

        // Act
        _ = await sut.InvokeAsync(model.GetType(), "TestView");

        // Assert
        await binder.Received(1).Bind(
            Arg.Is(typeof(HeaderBlock)),
            Arg.Any<ViewContext>());
    }
}