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

public class SitecoreLayoutContextBindingSourceFixture
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
        SitecoreLayoutContextBindingSource sut,
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
    public void GetModel_RenderingContextResponseIsNull_ShouldReturnNull(
        SitecoreLayoutContextBindingSource sut,
        ModelBindingContext modelBindingContext,
        ISitecoreRenderingContext renderingContext,
        IServiceProvider serviceProvider)
    {
        // Arrange
        ModelMetadata? modelMeta = Substitute.For<ModelMetadata>(ModelMetadataIdentity.ForType(typeof(CustomContext)));
        modelBindingContext.ModelMetadata.Returns(modelMeta);
        renderingContext.Response = null;

        // Act
        CustomContext? model = sut.GetModel(serviceProvider, modelBindingContext, renderingContext) as CustomContext;

        // Assert
        model.Should().BeNull();
    }

    [Theory]
    [AutoNSubstituteData]
    public void GetModel_RenderingContextResponseContentIsNull_ShouldReturnNull(
        SitecoreLayoutContextBindingSource sut,
        ModelBindingContext modelBindingContext,
        ISitecoreRenderingContext renderingContext,
        IServiceProvider serviceProvider)
    {
        // Arrange
        ModelMetadata? modelMeta = Substitute.For<ModelMetadata>(ModelMetadataIdentity.ForType(typeof(CustomContext)));
        modelBindingContext.ModelMetadata.Returns(modelMeta);
        renderingContext.Response!.Content = null;

        // Act
        CustomContext? model = sut.GetModel(serviceProvider, modelBindingContext, renderingContext) as CustomContext;

        // Assert
        model.Should().BeNull();
    }

    [Theory]
    [AutoNSubstituteData]
    public void GetModel_RenderingContextRawDataIsNull_ShouldReturnNull(
        SitecoreLayoutContextBindingSource sut,
        ModelBindingContext modelBindingContext,
        ISitecoreRenderingContext renderingContext,
        IServiceProvider serviceProvider)
    {
        // Arrange
        ModelMetadata? modelMeta = Substitute.For<ModelMetadata>(ModelMetadataIdentity.ForType(typeof(CustomContext)));
        modelBindingContext.ModelMetadata.Returns(modelMeta);
        renderingContext.Response!.Content!.ContextRawData = null!;

        // Act
        CustomContext? model = sut.GetModel(serviceProvider, modelBindingContext, renderingContext) as CustomContext;

        // Assert
        model.Should().BeNull();
    }

    [Theory]
    [AutoNSubstituteData]
    public void GetModel_WithSubContextType_ReturnsModel(
        SitecoreLayoutContextBindingSource sut,
        IServiceProvider serviceProvider,
        ModelBindingContext modelBindingContext,
        SitecoreRenderingContext renderingContext)
    {
        // Arrange
        ModelMetadata? modelMeta = Substitute.For<ModelMetadata>(ModelMetadataIdentity.ForType(typeof(CustomContext)));
        modelBindingContext.ModelMetadata.Returns(modelMeta);

        // Act
        CustomContext? model = sut.GetModel(serviceProvider, modelBindingContext, renderingContext) as CustomContext;

        // Assert
        model.Should().NotBeNull();
        model!.TestClass1.Should().NotBeNull();
        model.TestClass2.Should().NotBeNull();
        model.SingleProperty.Should().NotBeNullOrWhiteSpace();
        model.Site.Should().NotBeNull();
        model.PageState.Should().NotBeNull();
        model.Language.Should().NotBeNullOrWhiteSpace();
        model.WrongName.Should().BeNull();
    }

    [Theory]
    [AutoNSubstituteData]
    public void GetModel_WithIndividualType_ReturnsModel(
        SitecoreLayoutContextBindingSource sut,
        IServiceProvider serviceProvider,
        ModelBindingContext modelBindingContext,
        SitecoreRenderingContext renderingContext)
    {
        // Arrange
        ModelMetadata? modelMeta = Substitute.For<ModelMetadata>(ModelMetadataIdentity.ForType(typeof(CustomContextIndividual)));
        modelBindingContext.ModelMetadata.Returns(modelMeta);

        // Act
        CustomContextIndividual? model = sut.GetModel(serviceProvider, modelBindingContext, renderingContext) as CustomContextIndividual;

        // Assert
        model.Should().NotBeNull();
        model!.TestClass1.Should().NotBeNull();
        model.TestClass2.Should().NotBeNull();
        model.SingleProperty.Should().NotBeNullOrWhiteSpace();
        model.WrongName.Should().BeNull();
    }

    [Theory]
    [AutoNSubstituteData]
    public void GetModel_WithIndividualInnerType_ReturnsModel(
        SitecoreLayoutContextBindingSource sut,
        IServiceProvider serviceProvider,
        ModelBindingContext modelBindingContext,
        SitecoreRenderingContext renderingContext)
    {
        // Arrange
        ModelMetadata? modelMeta = Substitute.For<ModelMetadata>(ModelMetadataIdentity.ForType(typeof(TestClass1)));
        modelBindingContext.ModelMetadata.Returns(modelMeta);
        modelBindingContext.FieldName.Returns("TestClass1");

        // Act
        TestClass1? model = sut.GetModel(serviceProvider, modelBindingContext, renderingContext) as TestClass1;

        // Assert
        model.Should().NotBeNull();
        model!.TestString.Should().NotBeNullOrWhiteSpace();
        model.TestInt.Should().NotBe(default);
        model.TestTime.Should().NotBe(default);
    }

    // ReSharper disable UnusedAutoPropertyAccessor.Local - Used by serialization
    // ReSharper disable UnusedMember.Local - Used by serialization
    private class TestClass1
    {
        public string? TestString { get; set; }

        public int? TestInt { get; set; }

        public DateTime? TestTime { get; set; }
    }

    // ReSharper disable once ClassNeverInstantiated.Local - Init by serialization
    private class TestClass2
    {
        public string? TestString { get; set; }

        public int TestInt { get; set; }
    }

    private class CustomContext : Context
    {
        public TestClass1? TestClass1 { get; set; }

        public TestClass2? TestClass2 { get; set; }

        public TestClass1? WrongName { get; set; }

        public string? SingleProperty { get; set; }
    }

    private class CustomContextIndividual
    {
        public TestClass1? TestClass1 { get; set; }

        public TestClass2? TestClass2 { get; set; }

        public TestClass1? WrongName { get; set; }

        public string? SingleProperty { get; set; }
    }

    // ReSharper restore UnusedMember.Local - Used by serialization
    // ReSharper restore UnusedAutoPropertyAccessor.Local - Used by serialization
}