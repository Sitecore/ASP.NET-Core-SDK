using AwesomeAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Sitecore.AspNetCore.SDK.RenderingEngine.Binding.Extensions;
using Sitecore.AspNetCore.SDK.RenderingEngine.Binding.Providers;
using Sitecore.AspNetCore.SDK.RenderingEngine.Tests.Mocks;
using Xunit;

namespace Sitecore.AspNetCore.SDK.RenderingEngine.Tests.Binding.Extensions;

public class BindingExtensionsFixture
{
    [Fact]
    public void GetModelBinder_WithTSourceAndTType_IsGuarded()
    {
        // Arrange
        Func<BinderTypeModelBinder?> act =
            () => BindingExtensions.GetModelBinder<TestSitecoreLayoutBindingSource, object>(null!);

        // Act & Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void GetModelBinder_WithTSource_IsGuarded()
    {
        // Arrange
        Func<BinderTypeModelBinder?> act =
            () => BindingExtensions.GetModelBinder<TestSitecoreLayoutBindingSource>(null!);

        // Act & Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void AddSitecoreModelBinderProviders_IsGuarded()
    {
        // Arrange
        Action act =
            () => BindingExtensions.AddSitecoreModelBinderProviders(null!);

        // Act & Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void GetModelBinder_WithValidBindingSource_ReturnsCorrectModelBinder()
    {
        // Arrange
        TestModelMetadata testMetadata = new(typeof(object));
        BindingInfo bindingInfo = new()
        {
            BindingSource = new TestSitecoreLayoutBindingSource()
        };
        TestModelBinderProviderContext context = new(testMetadata, bindingInfo);

        // Act
        BinderTypeModelBinder? result = context.GetModelBinder<TestSitecoreLayoutBindingSource>();

        // Assert
        result.Should().NotBeNull();
    }

    [Fact]
    public void GetModelBinder_WithInvalidBindingSource_ReturnsNull()
    {
        // Arrange
        TestModelMetadata testMetadata = new(typeof(object));
        BindingInfo bindingInfo = new()
        {
            BindingSource = new TestBindingSource()
        };
        TestModelBinderProviderContext context = new(testMetadata, bindingInfo);

        // Act
        BinderTypeModelBinder? result = context.GetModelBinder<TestSitecoreLayoutBindingSource>();

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void GetModelBinder_WithValidModelTypeAndBindingSource_ReturnsCorrectModelBinder()
    {
        // Arrange
        TestModelMetadata testMetadata = new(typeof(string));
        BindingInfo bindingInfo = new()
        {
            BindingSource = new TestSitecoreLayoutBindingSource()
        };
        TestModelBinderProviderContext context = new(testMetadata, bindingInfo);

        // Act
        BinderTypeModelBinder? result = context.GetModelBinder<TestSitecoreLayoutBindingSource, string>();

        // Assert
        result.Should().NotBeNull();
    }

    [Fact]
    public void GetModelBinder_WithInvalidModelTypeAndBindingSource_ReturnsNull()
    {
        // Arrange
        TestModelMetadata testMetadata = new(typeof(object));
        BindingInfo bindingInfo = new()
        {
            BindingSource = new TestBindingSource()
        };
        TestModelBinderProviderContext context = new(testMetadata, bindingInfo);

        // Act
        BinderTypeModelBinder? result = context.GetModelBinder<TestSitecoreLayoutBindingSource, string>();

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void AddSitecoreModelBinderProviders_MvcOptions_Contain_ExpectedProviders()
    {
        // Arrange
        MvcOptions options = new();

        // Act
        options.AddSitecoreModelBinderProviders();

        // Assert
        options.ModelBinderProviders.Should().HaveCount(4);
        options.ModelBinderProviders[0].Should().BeOfType(typeof(SitecoreLayoutRouteModelBinderProvider));
        options.ModelBinderProviders[1].Should().BeOfType(typeof(SitecoreLayoutContextModelBinderProvider));
        options.ModelBinderProviders[2].Should().BeOfType(typeof(SitecoreLayoutComponentModelBinderProvider));
        options.ModelBinderProviders[3].Should().BeOfType(typeof(SitecoreLayoutResponseModelBinderProvider));
    }
}