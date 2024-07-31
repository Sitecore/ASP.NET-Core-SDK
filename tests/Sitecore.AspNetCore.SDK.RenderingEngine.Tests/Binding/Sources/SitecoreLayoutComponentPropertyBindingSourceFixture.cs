using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using NSubstitute;
using Sitecore.AspNetCore.SDK.AutoFixture.Attributes;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Response;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Response.Model;
using Sitecore.AspNetCore.SDK.RenderingEngine.Binding.Sources;
using Sitecore.AspNetCore.SDK.RenderingEngine.Interfaces;
using Sitecore.AspNetCore.SDK.RenderingEngine.Rendering;
using Sitecore.AspNetCore.SDK.TestData;
using Xunit;

namespace Sitecore.AspNetCore.SDK.RenderingEngine.Tests.Binding.Sources;

public class SitecoreLayoutComponentPropertyBindingSourceFixture
{
    // ReSharper disable once UnusedMember.Global - Used by testing framework
    public static Action<IFixture> AutoSetup => f =>
    {
        SitecoreLayoutResponseContent content = CannedResponses.StyleGuide1WithContext;
        Placeholder? jssPlaceholder = content.Sitecore?.Route?.Placeholders["jss-main"];
        SitecoreRenderingContext context = new()
        {
            Component = jssPlaceholder != null && jssPlaceholder.Any() ? jssPlaceholder.ComponentAt(0) : null,
            Response = new SitecoreLayoutResponse([])
            {
                Content = content
            }
        };

        f.Inject(context);
    };

    [Theory]
    [AutoNSubstituteData]
    public void GetModel_IsGuarded(
        SitecoreLayoutComponentPropertyBindingSource sut,
        ModelBindingContext modelBindingContext,
        ISitecoreRenderingContext renderingContext,
        IServiceProvider serviceProvider)
    {
        // Arrange
        Func<object?> allNull =
            () => sut.GetModel(null!, null!, null!);
        Func<object?> serviceProviderNull =
            () => sut.GetModel(null!, modelBindingContext, renderingContext);
        Func<object?> firstAndSecondArgsNull =
            () => sut.GetModel(null!, null!, renderingContext);
        Func<object?> firstAndThirdArgsNull =
            () => sut.GetModel(null!, modelBindingContext, null!);
        Func<object?> secondAndThirdArgsNull =
            () => sut.GetModel(serviceProvider, null!, null!);
        Func<object?> bindingContextNull =
            () => sut.GetModel(serviceProvider, null!, renderingContext);
        Func<object?> contextNull =
            () => sut.GetModel(serviceProvider, modelBindingContext, null!);

        // Act & Assert
        allNull.Should().Throw<ArgumentNullException>();
        serviceProviderNull.Should().Throw<ArgumentNullException>();
        firstAndSecondArgsNull.Should().Throw<ArgumentNullException>();
        firstAndThirdArgsNull.Should().Throw<ArgumentNullException>();
        secondAndThirdArgsNull.Should().Throw<ArgumentNullException>();
        bindingContextNull.Should().Throw<ArgumentNullException>();
        contextNull.Should().Throw<ArgumentNullException>();
    }

    [Theory]
    [AutoNSubstituteData]
    public void GetModel_NullComponent_ReturnsNull(
        SitecoreLayoutComponentPropertyBindingSource sut,
        IServiceProvider serviceProvider,
        ModelBindingContext modelBindingContext)
    {
        // Arrange
        SitecoreRenderingContext renderingContext = new()
        {
            Component = null
        };

        // Act
        object? model = sut.GetModel(serviceProvider, modelBindingContext, renderingContext);

        // Assert
        model.Should().BeNull();
    }

    [Theory]
    [AutoNSubstituteData]
    public void GetModel_NullPropertyName_ReturnsNull(
        SitecoreLayoutComponentPropertyBindingSource sut,
        IServiceProvider serviceProvider,
        ModelBindingContext modelBindingContext,
        SitecoreRenderingContext renderingContext)
    {
        // Arrange
        sut.Name = null;
        modelBindingContext.FieldName = null!;

        // Act
        object? model = sut.GetModel(serviceProvider, modelBindingContext, renderingContext);

        // Assert
        model.Should().BeNull();
    }

    [Theory]
    [AutoNSubstituteData]
    public void GetModel_EmptyPropertyName_ReturnsNull(
        SitecoreLayoutComponentPropertyBindingSource sut,
        IServiceProvider serviceProvider,
        ModelBindingContext modelBindingContext,
        SitecoreRenderingContext renderingContext)
    {
        // Arrange
        sut.Name = string.Empty;
        modelBindingContext.FieldName = string.Empty;

        // Act
        object? model = sut.GetModel(serviceProvider, modelBindingContext, renderingContext);

        // Assert
        model.Should().BeNull();
    }

    [Theory]
    [AutoNSubstituteData]
    public void GetModel_NullPropertyName_FieldNameIsUsedForBinding(
        SitecoreLayoutComponentPropertyBindingSource sut,
        IServiceProvider serviceProvider,
        ModelBindingContext modelBindingContext,
        SitecoreRenderingContext renderingContext)
    {
        // Arrange
        sut.Name = null;
        modelBindingContext.FieldName = "Name";

        // Act
        object? model = sut.GetModel(serviceProvider, modelBindingContext, renderingContext);

        // Assert
        model.Should().NotBeNull();
        model.Should().Be("HeaderBlock");
    }

    [Theory]
    [AutoNSubstituteData]
    public void GetModel_PropertyNameIsSet_PropertyNameIsUsedForBinding(
        SitecoreLayoutComponentPropertyBindingSource sut,
        IServiceProvider serviceProvider,
        ModelBindingContext modelBindingContext,
        SitecoreRenderingContext renderingContext)
    {
        // Arrange
        sut.Name = "Name";
        modelBindingContext.FieldName = "componentName";

        // Act
        object? model = sut.GetModel(serviceProvider, modelBindingContext, renderingContext);

        // Assert
        model.Should().NotBeNull();
        model.Should().Be("HeaderBlock");
    }

    [Theory]
    [AutoNSubstituteData]
    public void GetModel_UnknownPropertyName_ReturnsNull(
        SitecoreLayoutComponentPropertyBindingSource sut,
        IServiceProvider serviceProvider,
        ModelBindingContext modelBindingContext,
        SitecoreRenderingContext renderingContext)
    {
        // Arrange
        sut.Name = "InvalidName";
        modelBindingContext.FieldName = "componentInvalidName";

        // Act
        object? model = sut.GetModel(serviceProvider, modelBindingContext, renderingContext);

        // Assert
        model.Should().BeNull();
    }

    [Theory]
    [AutoNSubstituteData]
    public void GetModel_WithValidPropertyName_ReturnsModel(
        SitecoreLayoutComponentPropertyBindingSource sut,
        IServiceProvider serviceProvider,
        ModelBindingContext modelBindingContext,
        SitecoreRenderingContext renderingContext)
    {
        // Arrange
        sut.Name = "Name";

        // Act
        object? model = sut.GetModel(serviceProvider, modelBindingContext, renderingContext);

        // Assert
        model.Should().NotBeNull();
        model.Should().Be("HeaderBlock");
    }

    [Theory]
    [AutoNSubstituteData]
    public void GetModel_WithWithRightModelFieldType_ReturnsPropertyValue(
        SitecoreLayoutComponentPropertyBindingSource sut,
        IServiceProvider serviceProvider,
        ModelBindingContext modelBindingContext,
        SitecoreRenderingContext renderingContext)
    {
        modelBindingContext.ModelType.ReturnsForAnyArgs(typeof(string));

        // Arrange
        sut.Name = "Name";
        modelBindingContext.FieldName = "componentName";

        // Act
        object? model = sut.GetModel(serviceProvider, modelBindingContext, renderingContext);

        // Assert
        model.Should().NotBeNull();
        model.Should().Be("HeaderBlock");
    }

    [Theory]
    [AutoNSubstituteData]
    public void GetModel_WithWithWrongModelFieldType_ReturnsNull(
        SitecoreLayoutComponentPropertyBindingSource sut,
        IServiceProvider serviceProvider,
        ModelBindingContext modelBindingContext,
        SitecoreRenderingContext renderingContext)
    {
        modelBindingContext.ModelType.ReturnsForAnyArgs(typeof(int));

        // Arrange
        sut.Name = "Name";
        modelBindingContext.FieldName = "componentName";

        // Act
        object? model = sut.GetModel(serviceProvider, modelBindingContext, renderingContext);

        // Assert
        model.Should().BeNull();
    }
}