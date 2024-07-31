using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using NSubstitute;
using Sitecore.AspNetCore.SDK.AutoFixture.Attributes;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Response.Model;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Response.Model.Fields;
using Sitecore.AspNetCore.SDK.RenderingEngine.Binding.Providers;
using Sitecore.AspNetCore.SDK.RenderingEngine.Binding.Sources;
using Sitecore.AspNetCore.SDK.RenderingEngine.Tests.Mocks;
using Xunit;

namespace Sitecore.AspNetCore.SDK.RenderingEngine.Tests.Binding.Providers;

public class SitecoreLayoutComponentModelBinderProviderFixture
{
    public static Action<IFixture> ExpectedBindingSource => f =>
    {
        SitecoreLayoutComponentFieldBindingSource? bindingSource = Substitute.For<SitecoreLayoutComponentFieldBindingSource>("test", "test", false, false);

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
    public void GetBinder_NullContext_Throws(SitecoreLayoutComponentModelBinderProvider sut)
    {
        // Arrange
        Func<IModelBinder?> act =
            () => sut.GetBinder(null!);

        // Act & Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Theory]
    [AutoNSubstituteData(nameof(ExpectedBindingSource))]
    public void GetBinder_WithSitecoreLayoutComponentFieldBindingSource_ReturnsBinder(
        SitecoreLayoutComponentModelBinderProvider sut,
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
        SitecoreLayoutComponentModelBinderProvider sut,
        ModelBinderProviderContext modelBinderProviderContext)
    {
        // Act
        IModelBinder? binder = sut.GetBinder(modelBinderProviderContext);

        // Assert
        binder.Should().BeNull();
    }

    [Theory]
    [InlineAutoNSubstituteData(typeof(Field))]
    [InlineAutoNSubstituteData(typeof(Field<string>))]
    [InlineAutoNSubstituteData(typeof(CheckboxField))]
    [InlineAutoNSubstituteData(typeof(ContentListField))]
    [InlineAutoNSubstituteData(typeof(DateField))]
    [InlineAutoNSubstituteData(typeof(FileField))]
    [InlineAutoNSubstituteData(typeof(HyperLinkField))]
    [InlineAutoNSubstituteData(typeof(ImageField))]
    [InlineAutoNSubstituteData(typeof(ItemLinkField))]
    [InlineAutoNSubstituteData(typeof(NumberField))]
    [InlineAutoNSubstituteData(typeof(RichTextField))]
    [InlineAutoNSubstituteData(typeof(TextField))]
    public void GetBinder_WithFieldModelType_ReturnsBinder(
        Type fieldType,
        SitecoreLayoutComponentModelBinderProvider sut)
    {
        // Arrange
        TestModelMetadata modelMetadata = new(fieldType);
        TestModelBinderProviderContext modelBinderProviderContext = new(modelMetadata);

        // Act
        IModelBinder? binder = sut.GetBinder(modelBinderProviderContext);

        // Assert
        binder.Should().NotBeNull();
    }
}