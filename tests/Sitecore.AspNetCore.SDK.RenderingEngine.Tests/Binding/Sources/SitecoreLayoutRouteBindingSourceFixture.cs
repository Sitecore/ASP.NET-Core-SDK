using AutoFixture;
using FluentAssertions;
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

public class SitecoreLayoutRouteBindingSourceFixture
{
    // ReSharper disable once UnusedMember.Global - Used by testing framework
    public static Action<IFixture> AutoSetup => f =>
    {
        SitecoreLayoutResponseContent content = CannedResponses.StyleGuide1WithContext;
        SitecoreRenderingContext context = new()
        {
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
        SitecoreLayoutRouteBindingSource sut,
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
    public void GetModel_WithRouteType_ReturnsModel(
        SitecoreLayoutRouteBindingSource sut,
        IServiceProvider serviceProvider,
        ModelBindingContext modelBindingContext,
        SitecoreRenderingContext renderingContext)
    {
        // Arrange
        ModelMetadata? modelMeta = Substitute.For<ModelMetadata>(ModelMetadataIdentity.ForType(typeof(Route)));
        modelBindingContext.ModelMetadata.Returns(modelMeta);

        // Act
        object? model = sut.GetModel(serviceProvider, modelBindingContext, renderingContext);

        // Assert
        model.Should().Be(renderingContext.Response!.Content!.Sitecore!.Route);
    }

    [Theory]
    [AutoNSubstituteData]
    public void GetModel_WithGenericRouteType_ReturnsModel(
        SitecoreLayoutRouteBindingSource sut,
        IServiceProvider serviceProvider,
        ModelBindingContext modelBindingContext,
        SitecoreRenderingContext renderingContext)
    {
        // Arrange
        ModelMetadata? modelMeta = Substitute.For<ModelMetadata>(ModelMetadataIdentity.ForType(typeof(Route)));
        modelBindingContext.ModelMetadata.Returns(modelMeta);

        // Act
        object? model = sut.GetModel(serviceProvider, modelBindingContext, renderingContext);

        // Assert
        model.Should().BeOfType(typeof(Route));
        model.Should().BeEquivalentTo(renderingContext.Response!.Content!.Sitecore!.Route);
    }

    [Theory]
    [AutoNSubstituteData]
    public void GetModel_WithUnknownType_ReturnsNull(
        SitecoreLayoutRouteBindingSource sut,
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

    [Theory]
    [AutoNSubstituteData]
    public void GetModel_NullRoute_ReturnsNull(
        SitecoreLayoutRouteBindingSource sut,
        IServiceProvider serviceProvider,
        ModelBindingContext modelBindingContext)
    {
        // Arrange
        SitecoreLayoutResponseContent content = CannedResponses.StyleGuide1WithoutRoute;
        SitecoreRenderingContext renderingContext = new()
        {
            Response = new SitecoreLayoutResponse([])
            {
                Content = content
            }
        };

        // Act
        object? model = sut.GetModel(serviceProvider, modelBindingContext, renderingContext);

        // Assert
        model.Should().BeNull();
    }
}