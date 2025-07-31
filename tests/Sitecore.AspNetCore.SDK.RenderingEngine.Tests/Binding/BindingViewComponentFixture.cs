using System.Text;
using AutoFixture;
using AwesomeAssertions;
using Microsoft.AspNetCore.Mvc.Rendering;
using NSubstitute;
using Sitecore.AspNetCore.SDK.AutoFixture.Attributes;
using Sitecore.AspNetCore.SDK.RenderingEngine.Binding;
using Sitecore.AspNetCore.SDK.RenderingEngine.Tests.Mocks;
using Xunit;

namespace Sitecore.AspNetCore.SDK.RenderingEngine.Tests.Binding;

public class BindingViewComponentFixture
{
    // ReSharper disable once UnusedMember.Global - Used by testing framework
    public static Action<IFixture> AutoSetup()
    {
        return f =>
        {
            IViewModelBinder? binder = Substitute.For<IViewModelBinder>();

            f.Inject(binder);
        };
    }

    [Fact]
    public void BindingViewComponent_Guarded()
    {
        // Arrange
        Func<BindingViewComponentMock> act =
            () => new BindingViewComponentMock(null!);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Theory]
    [AutoNSubstituteData]
    public void BindingViewComponent_BinderPropertySet(
        IViewModelBinder viewModelBinder)
    {
        // Act
        BindingViewComponentMock sut = new(viewModelBinder);

        // Assert
        sut.BinderAccessor.Should().Be(viewModelBinder);
    }

    [Theory]
    [AutoNSubstituteData]
    public async Task BindView__WithT_BinderCalled(
        IViewModelBinder viewModelBinder,
        IViewModelBinder binder)
    {
        // Arrange
        BindingViewComponentMock sut = new(viewModelBinder);

        // Act
        await sut.BindView<StringBuilder>();

        // Assert
        await binder.Received(1).Bind<StringBuilder>(Arg.Any<ViewContext>());
    }

    [Theory]
    [AutoNSubstituteData]
    public async Task BindView_WithModel_BinderCalled(
        IViewModelBinder viewModelBinder,
        IViewModelBinder binder,
        StringBuilder model)
    {
        // Arrange
        BindingViewComponentMock sut = new(viewModelBinder);

        // Act
        await sut.BindView(model);

        // Assert
        await binder.Received(1).Bind(model, Arg.Any<ViewContext>());
    }

    [Theory]
    [AutoNSubstituteData]
    public async Task BindView_WithViewName_BinderCalled(
        IViewModelBinder viewModelBinder,
        IViewModelBinder binder,
        string viewName)
    {
        // Arrange
        BindingViewComponentMock sut = new(viewModelBinder);

        // Act
        await sut.BindView<StringBuilder>(viewName);

        // Assert
        await binder.Received(1).Bind<StringBuilder>(Arg.Any<ViewContext>());
    }

    [Theory]
    [AutoNSubstituteData]
    public async Task BindView_WithModelAndViewName_BinderCalled(
        IViewModelBinder viewModelBinder,
        IViewModelBinder binder,
        string viewName,
        StringBuilder model)
    {
        // Arrange
        BindingViewComponentMock sut = new(viewModelBinder);

        // Act
        await sut.BindView(viewName, model);

        // Assert
        await binder.Received(1).Bind(model, Arg.Any<ViewContext>());
    }
}