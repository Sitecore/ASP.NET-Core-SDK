using AutoFixture;
using AwesomeAssertions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
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

public class SitecoreLayoutComponentBindingSourceFixture
{
    public static Action<IFixture> AutoSetup => f =>
    {
        SitecoreLayoutResponseContent content = CannedResponses.StyleGuide1;
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
        SitecoreLayoutComponentBindingSource sut,
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
        SitecoreLayoutComponentBindingSource sut,
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
    public void GetModel_WithComponentType_ReturnsModel(
        SitecoreLayoutComponentBindingSource sut,
        IServiceProvider serviceProvider,
        ModelBindingContext modelBindingContext,
        SitecoreRenderingContext renderingContext)
    {
        // Arrange
        ModelMetadata? modelMeta = Substitute.For<ModelMetadata>(ModelMetadataIdentity.ForType(typeof(Component)));
        modelBindingContext.ModelMetadata.Returns(modelMeta);

        // Act
        object? model = sut.GetModel(serviceProvider, modelBindingContext, renderingContext);

        // Assert
        model.Should().Be(renderingContext.Component);
    }

    [Theory]
    [AutoNSubstituteData]
    public void GetModel_WithInvalidModel_ReturnsNull(
        SitecoreLayoutComponentBindingSource sut,
        IServiceProvider serviceProvider,
        ModelBindingContext modelBindingContext,
        SitecoreRenderingContext renderingContext)
    {
        // Arrange
        ModelMetadata? modelMeta = Substitute.For<ModelMetadata>(ModelMetadataIdentity.ForType(typeof(string)));
        modelBindingContext.ModelMetadata.Returns(modelMeta);

        // Act
        object? result = sut.GetModel(serviceProvider, modelBindingContext, renderingContext);

        // Assert
        result.Should().BeNull();
    }
}