using System.Text;
using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using NSubstitute;
using Sitecore.AspNetCore.SDK.AutoFixture.Attributes;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Response;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Response.Model;
using Sitecore.AspNetCore.SDK.RenderingEngine.Binding;
using Sitecore.AspNetCore.SDK.RenderingEngine.Rendering;
using Sitecore.AspNetCore.SDK.RenderingEngine.Tests.Mocks;
using Sitecore.AspNetCore.SDK.TestData;
using Xunit;

namespace Sitecore.AspNetCore.SDK.RenderingEngine.Tests.Binding;

public class SitecoreViewModelBinderFixture
{
    // ReSharper disable once UnusedMember.Global - Used by testing framework
    public static Action<IFixture> AutoSetup => f =>
    {
        ControllerBase? controller = Substitute.For<ControllerBase>();
        SitecoreRenderingContext renderingContext = new()
        {
            Response = new SitecoreLayoutResponse([]) { Content = CannedResponses.StyleGuide1 },
            Component = new Component(),
            Controller = controller
        };

        FeatureCollection featureCollection = new();
        featureCollection.Set(renderingContext);

        ActionContext actionContext = new(
            new DefaultHttpContext(featureCollection),
            new Microsoft.AspNetCore.Routing.RouteData(),
            new Microsoft.AspNetCore.Mvc.Abstractions.ActionDescriptor());

        BindingSource? bindingSource = Substitute.For<BindingSource>("dds", "sds", true, true);

        bindingSource.CanAcceptDataFrom(Arg.Any<BindingSource>()).Returns(true);

        BindingInfo bindingInfo = new()
        {
            BindingSource = bindingSource
        };

        f.Inject(bindingInfo);
        f.Inject(actionContext);
        f.Inject(renderingContext);
        f.Inject(controller);
    };

    [Theory]
    [AutoNSubstituteData]
    internal async Task Bind_NullContext_Throws(SitecoreViewModelBinder sut)
    {
        Func<Task<SitecoreViewModelBinder>> act =
            () => sut.Bind<SitecoreViewModelBinder>(null!);

        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Theory]
    [AutoNSubstituteData]
    internal void Bind_WithViewContext_ReturnsModel(
        SitecoreViewModelBinder sut,
        ViewContext viewContext)
    {
        Task<StringBuilder> model = sut.Bind<StringBuilder>(viewContext);

        model.Should().NotBeNull();
    }

    [Theory]
    [AutoNSubstituteData]
    internal async Task Bind_NullModel_Throws(SitecoreViewModelBinder sut)
    {
        Func<Task> act =
            () => sut.Bind<SitecoreViewModelBinder>(null!, null!);

        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Theory]
    [AutoNSubstituteData]
    internal async Task Bind_ModelWithNullViewContext_Throws(SitecoreViewModelBinder sut)
    {
        Func<Task> act =
            () => sut.Bind(new SitecoreViewModelBinder(), null!);

        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Theory]
    [AutoNSubstituteData]
    internal async Task Bind_WithModelAndViewContext_ReturnsModel(
        SitecoreViewModelBinder sut,
        ViewContext viewContext,
        string model,
        ControllerBase controller)
    {
        await sut.Bind(model, viewContext);

        await controller.Received(1).TryUpdateModelAsync(Arg.Is(model));
    }

    [Theory]
    [AutoNSubstituteData]
    internal async Task BindUsingType_NullModelType_Throws(SitecoreViewModelBinder sut)
    {
        // Arrange
        Func<Task<object>> act =
            () => sut.Bind(null!, null!);

        // Act & Assert
        await act.Should().ThrowAsync<ArgumentNullException>().WithMessage("Value cannot be null. (Parameter 'modelType')");
    }

    [Theory]
    [AutoNSubstituteData]
    internal async Task BindUsingType_NullViewContext_Throws(SitecoreViewModelBinder sut)
    {
        // Arrange
        Func<Task<object>> act =
            () => sut.Bind(typeof(StringBuilder), null!);

        // Act & Assert
        await act.Should().ThrowAsync<ArgumentNullException>().WithMessage("Value cannot be null. (Parameter 'viewContext')");
    }

    [Theory]
    [AutoNSubstituteData]
    internal async Task BindUsingType_WithModelAndViewContext_ReturnsModel(
        SitecoreViewModelBinder sut,
        ViewContext viewContext,
        ControllerBase controller)
    {
        await sut.Bind(typeof(ContentBlock), viewContext);

        await controller.Received(1)
            .TryUpdateModelAsync(
                Arg.Is<object>(o => o.GetType() == typeof(ContentBlock)),
                Arg.Is(typeof(ContentBlock)),
                Arg.Is(string.Empty));
    }
}