using AutoFixture;
using AutoFixture.Idioms;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using NSubstitute;
using Sitecore.AspNetCore.SDK.AutoFixture.Attributes;
using Sitecore.AspNetCore.SDK.AutoFixture.Extensions;
using Sitecore.AspNetCore.SDK.LayoutService.Client.Response.Model;
using Sitecore.AspNetCore.SDK.RenderingEngine.Interfaces;
using Sitecore.AspNetCore.SDK.RenderingEngine.Rendering;
using Sitecore.AspNetCore.SDK.RenderingEngine.Tests.Mocks;
using Xunit;

namespace Sitecore.AspNetCore.SDK.RenderingEngine.Tests.Rendering;

public class ModelBoundViewComponentComponentRendererFixture
{
    private const string Locator = "testLocator";
    private const string ViewName = "testView";

    // ReSharper disable once UnusedMember.Global - Used by testing framework
    public static Action<IFixture> AutoSetup => f =>
    {
        f.Behaviors.Add(new OmitOnRecursionBehavior());

        IHtmlHelper? htmlHelper = Substitute.For<IHtmlHelper>();
        f.Inject(htmlHelper);

        f.Inject(new ViewContext());
        ICustomViewComponentHelper? viewComponentHelper = f.Create<ICustomViewComponentHelper>();
        f.Inject<IViewComponentHelper>(viewComponentHelper);
        f.Inject<IViewContextAware>(viewComponentHelper);

        IServiceProvider? serviceProvider = f.Freeze<IServiceProvider>();
        serviceProvider.GetService(typeof(IViewComponentHelper)).Returns(viewComponentHelper);

        f.Freeze<ISitecoreRenderingContext>();

        ModelBoundViewComponentComponentRenderer<HeaderBlock> viewComponentRenderer = new(Locator, ViewName);
        f.Inject(viewComponentRenderer);
    };

    [Theory]
    [AutoNSubstituteData]
    public void Render_IsGuarded(GuardClauseAssertion guard)
    {
        guard.VerifyMethod<ViewComponentComponentRenderer>("Render");
    }

    [Theory]
    [AutoNSubstituteData]
    public void Constructor_IsGuarded(GuardClauseAssertion guard)
    {
        guard.VerifyConstructors<ViewComponentComponentRenderer>();
    }

    [Theory]
    [AutoNSubstituteData]
    public void Render_RenderingContextComponentIsNull_ViewComponentHelperIsInvokedWithLocatorAndCorrectModelType(
        IViewComponentHelper viewComponentHelper,
        ISitecoreRenderingContext renderingContext,
        ViewContext viewContext,
        ModelBoundViewComponentComponentRenderer<HeaderBlock> sut)
    {
        // Arrange
        renderingContext.Component = null;

        // Act
        _ = sut.Render(renderingContext, viewContext);

        // Assert
        Received.InOrder(() =>
            _ = viewComponentHelper.InvokeAsync(
                Locator,
                Arg.Is<object>(m => ConvertToModelType(m) == typeof(HeaderBlock))));
    }

    [Theory]
    [AutoNSubstituteData]
    public void Render_RenderingContextComponentIsNotNull_ViewComponentHelperIsInvokedWithLocatorAndCorrectModelType(
        IViewComponentHelper viewComponentHelper,
        ISitecoreRenderingContext renderingContext,
        ViewContext viewContext,
        ModelBoundViewComponentComponentRenderer<HeaderBlock> sut)
    {
        // Arrange
        renderingContext.Component = new Component();

        // Act
        _ = sut.Render(renderingContext, viewContext);

        // assert
        Received.InOrder(() =>
        {
            _ = viewComponentHelper.InvokeAsync(
                Locator,
                Arg.Is<object>(m =>
                    GetViewName(m) == ViewName &&
                    ConvertToModelType(m) == typeof(HeaderBlock)));
        });
    }

    [Theory]
    [AutoNSubstituteData]
    public void Render_RenderingContextComponentHasFieldsMatchingModelProperties_ViewComponentHelperIsInvokedWithLocatorAndCorrectModelType(
        IViewComponentHelper viewComponentHelper,
        ISitecoreRenderingContext renderingContext,
        ViewContext viewContext,
        ModelBoundViewComponentComponentRenderer<HeaderBlock> sut)
    {
        // Arrange
        renderingContext.Component = new Component()
        {
            Fields = new Dictionary<string, IFieldReader>()
            {
                ["Heading1"] = new Field<string>() { Value = "Test Heading 1" }
            }
        };

        // Act
        _ = sut.Render(renderingContext, viewContext);

        // assert
        Received.InOrder(() =>
        {
            _ = viewComponentHelper.InvokeAsync(
                Locator,
                Arg.Is<object>(m =>
                    GetViewName(m) == ViewName &&
                    ConvertToModelType(m) == typeof(HeaderBlock)));
        });
    }

    private static string? GetViewName(object obj)
    {
        return obj.GetType().GetProperty("viewName")?.GetValue(obj, null)?.ToString();
    }

    private static Type? ConvertToModelType(object obj)
    {
        object? modelType = obj.GetType().GetProperty("modelType")?.GetValue(obj, null);

        return modelType as Type;
    }
}