using AutoFixture;
using AwesomeAssertions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Sitecore.AspNetCore.SDK.AutoFixture.Attributes;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Response;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Response.Model;
using Sitecore.AspNetCore.SDK.RenderingEngine.Binding.Sources;
using Sitecore.AspNetCore.SDK.RenderingEngine.Interfaces;
using Sitecore.AspNetCore.SDK.RenderingEngine.Rendering;
using Sitecore.AspNetCore.SDK.TestData;
using Xunit;

namespace Sitecore.AspNetCore.SDK.RenderingEngine.Tests.Binding.Sources;

public class SitecoreLayoutComponentParameterBindingSourceFixture
{
    // ReSharper disable once UnusedMember.Global - Used by testing framework
    public static Action<IFixture> AutoSetup => f =>
    {
        SitecoreLayoutResponseContent content = CannedResponses.StyleGuideWithComponentParameters;
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
        SitecoreLayoutComponentParameterBindingSource sut,
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
        SitecoreLayoutComponentParameterBindingSource sut,
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
    public void GetModel_NullParameters_ReturnsNull(
        SitecoreLayoutComponentParameterBindingSource sut,
        IServiceProvider serviceProvider,
        ModelBindingContext modelBindingContext)
    {
        // Arrange
        SitecoreRenderingContext renderingContext = new()
        {
            Component = new Component
            {
                Parameters = null!
            }
        };

        // Act
        object? model = sut.GetModel(serviceProvider, modelBindingContext, renderingContext);

        // Assert
        model.Should().BeNull();
    }

    [Theory]
    [AutoNSubstituteData]
    public void GetModel_EmptyParameters_ReturnsNull(
        SitecoreLayoutComponentParameterBindingSource sut,
        IServiceProvider serviceProvider,
        ModelBindingContext modelBindingContext)
    {
        // Arrange
        SitecoreRenderingContext renderingContext = new()
        {
            Component = new Component
            {
                Parameters = []
            }
        };

        // Act
        object? model = sut.GetModel(serviceProvider, modelBindingContext, renderingContext);

        // Assert
        model.Should().BeNull();
    }

    [Theory]
    [AutoNSubstituteData]
    public void GetModel_NullPropertyName_ReturnsNull(
        SitecoreLayoutComponentParameterBindingSource sut,
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
        SitecoreLayoutComponentParameterBindingSource sut,
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
        SitecoreLayoutComponentParameterBindingSource sut,
        IServiceProvider serviceProvider,
        ModelBindingContext modelBindingContext,
        SitecoreRenderingContext renderingContext)
    {
        // Arrange
        sut.Name = null;
        modelBindingContext.FieldName = "CmpParam1";

        // Act
        object? model = sut.GetModel(serviceProvider, modelBindingContext, renderingContext);

        // Assert
        model.Should().NotBeNull();
        model.Should().Be("Value1");
    }

    [Theory]
    [InlineAutoNSubstituteData("CmpParam1")]
    [InlineAutoNSubstituteData("cmpParam1")]
    [InlineAutoNSubstituteData("cmpparam1")]
    [InlineAutoNSubstituteData("CMPPARAM1")]
    public void GetModel_PropertyNameIsSet_PropertyNameIsUsedForBinding(
        string name,
        SitecoreLayoutComponentParameterBindingSource sut,
        IServiceProvider serviceProvider,
        ModelBindingContext modelBindingContext,
        SitecoreRenderingContext renderingContext)
    {
        // Arrange
        sut.Name = name;
        modelBindingContext.FieldName = "componentName";

        // Act
        object? model = sut.GetModel(serviceProvider, modelBindingContext, renderingContext);

        // Assert
        model.Should().NotBeNull();
        model.Should().Be("Value1");
    }

    [Theory]
    [AutoNSubstituteData]
    public void GetModel_UnknownPropertyName_ReturnsNull(
        SitecoreLayoutComponentParameterBindingSource sut,
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
        SitecoreLayoutComponentParameterBindingSource sut,
        IServiceProvider serviceProvider,
        ModelBindingContext modelBindingContext,
        SitecoreRenderingContext renderingContext)
    {
        // Arrange
        sut.Name = "CmpParam1";

        // Act
        object? model = sut.GetModel(serviceProvider, modelBindingContext, renderingContext);

        // Assert
        model.Should().NotBeNull();
        model.Should().Be("Value1");
    }
}