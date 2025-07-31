using AutoFixture;
using AwesomeAssertions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using NSubstitute;
using Sitecore.AspNetCore.SDK.AutoFixture.Attributes;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Response;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Response.Model;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Response.Model.Fields;
using Sitecore.AspNetCore.SDK.RenderingEngine.Binding.Sources;
using Sitecore.AspNetCore.SDK.RenderingEngine.Interfaces;
using Sitecore.AspNetCore.SDK.RenderingEngine.Rendering;
using Sitecore.AspNetCore.SDK.TestData;
using Xunit;

namespace Sitecore.AspNetCore.SDK.RenderingEngine.Tests.Binding.Sources;

public class SitecoreLayoutComponentFieldBindingSourceFixture
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
        SitecoreLayoutComponentFieldBindingSource sut,
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
        SitecoreLayoutComponentFieldBindingSource sut,
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
    public void GetModel_NullFieldName_ReturnsNull(
        SitecoreLayoutComponentFieldBindingSource sut,
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
    public void GetModel_EmptyFieldName_ReturnsNull(
        SitecoreLayoutComponentFieldBindingSource sut,
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
    public void GetModel_NullFieldName_BindingContextFieldNameIsUsedForBinding(
        SitecoreLayoutComponentFieldBindingSource sut,
        IServiceProvider serviceProvider,
        ModelBindingContext modelBindingContext,
        SitecoreRenderingContext renderingContext)
    {
        // Arrange
        ModelMetadata? modelMeta = Substitute.For<ModelMetadata>(ModelMetadataIdentity.ForType(typeof(TextField)));
        modelBindingContext.ModelMetadata.Returns(modelMeta);

        sut.Name = null;
        modelBindingContext.FieldName = "heading1";

        // Act
        object? model = sut.GetModel(serviceProvider, modelBindingContext, renderingContext);

        // Assert
        model.Should().NotBeNull();
        model.Should().BeOfType(typeof(TextField));
        TextField? fieldModel = model as TextField;
        fieldModel!.Value.Should().Be("HeaderBlock - This is heading1");
    }

    [Theory]
    [AutoNSubstituteData]
    public void GetModel_FieldNameIsSet_PropertyNameIsUsedForBinding(
        SitecoreLayoutComponentFieldBindingSource sut,
        IServiceProvider serviceProvider,
        ModelBindingContext modelBindingContext)
    {
        // Arrange
        ModelMetadata? modelMeta = Substitute.For<ModelMetadata>(ModelMetadataIdentity.ForType(typeof(TextField)));
        modelBindingContext.ModelMetadata.Returns(modelMeta);

        sut.Name = "heading1";
        modelBindingContext.FieldName = "someParam";

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

        // Act
        object? model = sut.GetModel(serviceProvider, modelBindingContext, context);

        // Assert
        model.Should().NotBeNull();
        model.Should().BeOfType(typeof(TextField));
        TextField? fieldModel = model as TextField;
        fieldModel!.Value.Should().Be("HeaderBlock - This is heading1");
    }

    [Theory]
    [AutoNSubstituteData]
    public void GetModel_UnknownFieldName_ReturnsNull(
        SitecoreLayoutComponentFieldBindingSource sut,
        IServiceProvider serviceProvider,
        ModelBindingContext modelBindingContext,
        SitecoreRenderingContext renderingContext)
    {
        // Arrange
        sut.Name = "InvalidFieldName";
        modelBindingContext.FieldName = "invalidFieldName";

        // Act
        object? model = sut.GetModel(serviceProvider, modelBindingContext, renderingContext);

        // Assert
        model.Should().BeNull();
    }

    [Theory]
    [AutoNSubstituteData]
    public void GetModel_WithValidFieldName_ReturnsModel(
        SitecoreLayoutComponentFieldBindingSource sut,
        IServiceProvider serviceProvider,
        ModelBindingContext modelBindingContext,
        SitecoreRenderingContext renderingContext)
    {
        // Arrange
        ModelMetadata? modelMeta = Substitute.For<ModelMetadata>(ModelMetadataIdentity.ForType(typeof(TextField)));
        modelBindingContext.ModelMetadata.Returns(modelMeta);

        sut.Name = "heading1";

        // Act
        object? model = sut.GetModel(serviceProvider, modelBindingContext, renderingContext);

        // Assert
        model.Should().NotBeNull();
        model.Should().BeOfType(typeof(TextField));
        TextField? fieldModel = model as TextField;
        fieldModel!.Value.Should().Be("HeaderBlock - This is heading1");
    }

    [Theory]
    [AutoNSubstituteData]
    public void GetModel_WithInvalidType_ReturnsNull(
        SitecoreLayoutComponentFieldBindingSource sut,
        IServiceProvider serviceProvider,
        ModelBindingContext modelBindingContext,
        SitecoreRenderingContext renderingContext)
    {
        // Arrange
        ModelMetadata? modelMeta = Substitute.For<ModelMetadata>(ModelMetadataIdentity.ForType(typeof(string)));
        modelBindingContext.ModelMetadata.Returns(modelMeta);

        sut.Name = "heading1";

        // Act
        object? model = sut.GetModel(serviceProvider, modelBindingContext, renderingContext);

        // Assert
        model.Should().BeNull();
    }

    [Theory]
    [AutoNSubstituteData]
    public void GetModel_ComponentWithoutDatasource_ReturnsContextItemFieldValue_IfFieldsMatch_WithContextItem(
        SitecoreLayoutComponentFieldBindingSource sut,
        IServiceProvider serviceProvider,
        ModelBindingContext modelBindingContext,
        SitecoreRenderingContext renderingContext)
    {
        // Arrange
        ModelMetadata? modelMeta = Substitute.For<ModelMetadata>(ModelMetadataIdentity.ForType(typeof(TextField)));
        modelBindingContext.ModelMetadata.Returns(modelMeta);
        renderingContext.Component!.DataSource = null!;
        renderingContext.Component.Fields = [];
        sut.Name = "pageTitle";

        // Act
        object? model = sut.GetModel(serviceProvider, modelBindingContext, renderingContext);

        // Assert
        model.Should().NotBeNull();
        model.Should().BeOfType(typeof(TextField));
        TextField? fieldModel = model as TextField;
        fieldModel!.Value.Should().Be("Styleguide | Sitecore JSS");
    }

    [Theory]
    [AutoNSubstituteData]
    public void GetModel_ComponentWithoutDatasource_ReturnsNull_IfFieldsTypesDoNotMatch_WithContextItem(
        SitecoreLayoutComponentFieldBindingSource sut,
        IServiceProvider serviceProvider,
        ModelBindingContext modelBindingContext,
        SitecoreRenderingContext renderingContext)
    {
        // Arrange
        ModelMetadata? modelMeta = Substitute.For<ModelMetadata>(ModelMetadataIdentity.ForType(typeof(NumberField)));
        modelBindingContext.ModelMetadata.Returns(modelMeta);
        renderingContext.Component!.DataSource = null!;
        renderingContext.Component.Fields = [];
        sut.Name = "pageTitle";

        // Act
        object? model = sut.GetModel(serviceProvider, modelBindingContext, renderingContext);

        // Assert
        model.Should().BeNull();
    }

    [Theory]
    [AutoNSubstituteData]
    public void GetModel_ComponentWithoutDatasource_ReturnsNull_IfFieldsNamesDoNotMatch_WithContextItem(
        SitecoreLayoutComponentFieldBindingSource sut,
        IServiceProvider serviceProvider,
        ModelBindingContext modelBindingContext,
        SitecoreRenderingContext renderingContext)
    {
        // Arrange
        ModelMetadata? modelMeta = Substitute.For<ModelMetadata>(ModelMetadataIdentity.ForType(typeof(TextField)));
        modelBindingContext.ModelMetadata.Returns(modelMeta);
        renderingContext.Component!.DataSource = null!;
        renderingContext.Component.Fields = [];

        // Act
        object? model = sut.GetModel(serviceProvider, modelBindingContext, renderingContext);

        // Assert
        model.Should().BeNull();
    }
}