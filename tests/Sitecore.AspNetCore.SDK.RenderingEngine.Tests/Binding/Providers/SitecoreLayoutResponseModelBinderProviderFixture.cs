using AutoFixture;
using AwesomeAssertions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using NSubstitute;
using Sitecore.AspNetCore.SDK.AutoFixture.Attributes;
using Sitecore.AspNetCore.SDK.RenderingEngine.Binding.Providers;
using Sitecore.AspNetCore.SDK.RenderingEngine.Binding.Sources;
using Xunit;

namespace Sitecore.AspNetCore.SDK.RenderingEngine.Tests.Binding.Providers;

public class SitecoreLayoutResponseModelBinderProviderFixture
{
    // ReSharper disable once UnusedMember.Global - Used by testing framework
    public static Action<IFixture> AutoSetup => f =>
    {
        SitecoreLayoutResponseBindingSource? bindingSource = Substitute.For<SitecoreLayoutResponseBindingSource>();

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
    public void GetBinder_NullContext_Throws(SitecoreLayoutResponseModelBinderProvider sut)
    {
        // Arrange
        Func<IModelBinder?> act =
            () => sut.GetBinder(null!);

        // Act & Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Theory]
    [AutoNSubstituteData]
    public void GetBinder_WithSitecoreLayoutBindingSource_ReturnsBinder(
        SitecoreLayoutResponseModelBinderProvider sut,
        ModelBinderProviderContext modelBinderProviderContext)
    {
        IModelBinder? binder = sut.GetBinder(modelBinderProviderContext);

        binder.Should().NotBeNull();
    }

    [Theory]
    [AutoNSubstituteData(nameof(UnknownBindingSource))]
    public void GetBinder_WithUnknownBindingSource_ReturnsNull(
        SitecoreLayoutResponseModelBinderProvider sut,
        ModelBinderProviderContext modelBinderProviderContext)
    {
        IModelBinder? binder = sut.GetBinder(modelBinderProviderContext);

        binder.Should().BeNull();
    }
}