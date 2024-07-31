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

public class SitecoreLayoutContextModelBinderProviderFixture
{
    public static Action<IFixture> SitecoreLayoutContextBindingSource => f =>
    {
        SitecoreLayoutContextBindingSource? bindingSource = Substitute.For<SitecoreLayoutContextBindingSource>();

        bindingSource.CanAcceptDataFrom(Arg.Any<BindingSource>()).Returns(true);

        BindingInfo bindingInfo = new()
        {
            BindingSource = bindingSource
        };

        f.Inject(bindingInfo);
    };

    public static Action<IFixture> SitecoreLayoutContextPropertyBindingSource => f =>
    {
        SitecoreLayoutContextPropertyBindingSource? bindingSource = Substitute.For<SitecoreLayoutContextPropertyBindingSource>("test");

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
    public void GetBinder_NullContext_Throws(SitecoreLayoutContextModelBinderProvider sut)
    {
        Func<IModelBinder?> act =
            () => sut.GetBinder(null!);

        act.Should().Throw<ArgumentNullException>();
    }

    [Theory]
    [AutoNSubstituteData(nameof(SitecoreLayoutContextBindingSource))]
    public void GetBinder_WithSitecoreLayoutContextBindingSource_ReturnsBinder(
        SitecoreLayoutContextModelBinderProvider sut,
        ModelBinderProviderContext modelBinderProviderContext)
    {
        // Act
        IModelBinder? binder = sut.GetBinder(modelBinderProviderContext);

        // Assert
        binder.Should().NotBeNull();
    }

    [Theory]
    [AutoNSubstituteData(nameof(SitecoreLayoutContextPropertyBindingSource))]
    public void GetBinder_WithSitecoreLayoutContextPropertyBindingSource_ReturnsBinder(
        SitecoreLayoutContextModelBinderProvider sut,
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
        SitecoreLayoutContextModelBinderProvider sut,
        ModelBinderProviderContext modelBinderProviderContext)
    {
        // Act
        IModelBinder? binder = sut.GetBinder(modelBinderProviderContext);

        // Assert
        binder.Should().BeNull();
    }

    [Theory]
    [AutoNSubstituteData]
    public void GetBinder_WithContextModelType_ReturnsBinder(
        SitecoreLayoutContextModelBinderProvider sut)
    {
        // Arrange
        TestModelMetadata modelMetadata = new(typeof(Context));
        TestModelBinderProviderContext modelBinderProviderContext = new(modelMetadata);

        // Act
        IModelBinder? binder = sut.GetBinder(modelBinderProviderContext);

        // Assert
        binder.Should().NotBeNull();
    }
}