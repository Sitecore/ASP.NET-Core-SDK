using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using NSubstitute;
using Sitecore.AspNetCore.SDK.AutoFixture.Attributes;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Response.Model;
using Sitecore.AspNetCore.SDK.RenderingEngine.Binding.Providers;
using Sitecore.AspNetCore.SDK.RenderingEngine.Binding.Sources;
using Sitecore.AspNetCore.SDK.RenderingEngine.Tests.Mocks;
using Xunit;

namespace Sitecore.AspNetCore.SDK.RenderingEngine.Tests.Binding.Providers;

public class SitecoreLayoutRouteModelBinderProviderFixture
{
    public static Action<IFixture> SitecoreLayoutRouteBindingSource => f =>
    {
        SitecoreLayoutRouteBindingSource? bindingSource = Substitute.For<SitecoreLayoutRouteBindingSource>();

        bindingSource.CanAcceptDataFrom(Arg.Any<BindingSource>()).Returns(true);

        BindingInfo bindingInfo = new()
        {
            BindingSource = bindingSource
        };

        f.Inject(bindingInfo);
    };

    public static Action<IFixture> SitecoreLayoutRouteFieldBindingSource => f =>
    {
        SitecoreLayoutRouteFieldBindingSource? bindingSource = Substitute.For<SitecoreLayoutRouteFieldBindingSource>("test");

        bindingSource.CanAcceptDataFrom(Arg.Any<BindingSource>()).Returns(true);

        BindingInfo bindingInfo = new()
        {
            BindingSource = bindingSource
        };

        f.Inject(bindingInfo);
    };

    public static Action<IFixture> SitecoreLayoutRoutePropertyBindingSource => f =>
    {
        SitecoreLayoutRoutePropertyBindingSource? bindingSource = Substitute.For<SitecoreLayoutRoutePropertyBindingSource>("test");

        bindingSource.CanAcceptDataFrom(Arg.Any<BindingSource>()).Returns(true);

        BindingInfo bindingInfo = new()
        {
            BindingSource = bindingSource
        };

        f.Inject(bindingInfo);
    };

    public static Action<IFixture> UnknownBindingSource => f =>
    {
        BindingSource? bindingSource = Substitute.For<BindingSource>("test", "test", true, true);

        bindingSource.CanAcceptDataFrom(Arg.Any<BindingSource>()).Returns(true);

        BindingInfo bindingInfo = new()
        {
            BindingSource = bindingSource
        };

        f.Inject(bindingInfo);
    };

    [Theory]
    [AutoNSubstituteData]
    public void GetBinder_NullContext_Throws(SitecoreLayoutRouteModelBinderProvider sut)
    {
        // Arrange
        Func<IModelBinder?> act =
            () => sut.GetBinder(null!);

        // Act & Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Theory]
    [AutoNSubstituteData(nameof(SitecoreLayoutRouteBindingSource))]
    public void GetBinder_WithSitecoreLayoutRouteBindingSource_ReturnsBinder(
        SitecoreLayoutRouteModelBinderProvider sut,
        ModelBinderProviderContext modelBinderProviderContext)
    {
        // Act
        IModelBinder? binder = sut.GetBinder(modelBinderProviderContext);

        // Assert
        binder.Should().NotBeNull();
    }

    [Theory]
    [AutoNSubstituteData(nameof(SitecoreLayoutRouteFieldBindingSource))]
    public void GetBinder_WithSitecoreLayoutRouteFieldBindingSource_ReturnsBinder(
        SitecoreLayoutRouteModelBinderProvider sut,
        ModelBinderProviderContext modelBinderProviderContext)
    {
        // Act
        IModelBinder? binder = sut.GetBinder(modelBinderProviderContext);

        // Assert
        binder.Should().NotBeNull();
    }

    [Theory]
    [AutoNSubstituteData(nameof(SitecoreLayoutRoutePropertyBindingSource))]
    public void GetBinder_WithSitecoreLayoutRoutePropertyBindingSource_ReturnsBinder(
        SitecoreLayoutRouteModelBinderProvider sut,
        ModelBinderProviderContext modelBinderProviderContext)
    {
        // Act
        IModelBinder? binder = sut.GetBinder(modelBinderProviderContext);

        // Assert
        binder.Should().NotBeNull();
    }

    [Theory]
    [AutoNSubstituteData(nameof(UnknownBindingSource))]
    public void GetBinder_WithUnknownBindingSource_ReturnsNull(
        SitecoreLayoutRouteModelBinderProvider sut,
        ModelBinderProviderContext modelBinderProviderContext)
    {
        // Act
        IModelBinder? binder = sut.GetBinder(modelBinderProviderContext);

        // Assert
        binder.Should().BeNull();
    }

    [Theory]
    [AutoNSubstituteData]
    public void GetBinder_WithRouteModelType_ReturnsBinder(
        SitecoreLayoutRouteModelBinderProvider sut)
    {
        // Arrange
        TestModelMetadata modelMetadata = new(typeof(Route));
        TestModelBinderProviderContext modelBinderProviderContext = new(modelMetadata);

        // Act
        IModelBinder? binder = sut.GetBinder(modelBinderProviderContext);

        // Assert
        binder.Should().NotBeNull();
    }

    [Theory]
    [AutoNSubstituteData]
    public void GetBinder_WithRouteOfTModelType_ReturnsBinder(
        SitecoreLayoutRouteModelBinderProvider sut)
    {
        // Arrange
        TestModelMetadata modelMetadata = new(typeof(Route));
        TestModelBinderProviderContext modelBinderProviderContext = new(modelMetadata);

        // Act
        IModelBinder? binder = sut.GetBinder(modelBinderProviderContext);

        // Assert
        binder.Should().NotBeNull();
    }
}